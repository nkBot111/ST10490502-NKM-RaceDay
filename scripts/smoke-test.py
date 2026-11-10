#!/usr/bin/env python3
"""End-to-end API and MVC smoke test for the local RaceDay demo.
Run after run-api.sh and run-web.sh in separate terminals.
Creates unique test accounts and one sample event in the development database.
"""
import http.cookiejar
import json
import re
import sys
import time
import urllib.error
import urllib.parse
import urllib.request
import uuid
from datetime import datetime, timedelta

API = "http://127.0.0.1:5081"
WEB = "http://127.0.0.1:5080"

def client():
    return urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))

def request(opener, method, url, payload=None):
    data = json.dumps(payload).encode() if payload is not None else None
    req = urllib.request.Request(url, method=method, data=data)
    if data is not None:
        req.add_header("Content-Type", "application/json")
    try:
        with opener.open(req, timeout=20) as resp:
            body = resp.read().decode(errors="replace")
            try:
                content = json.loads(body)
            except json.JSONDecodeError:
                content = body
            return resp.status, content
    except urllib.error.HTTPError as e:
        body = e.read().decode(errors="replace")
        raise AssertionError(f"{method} {url} -> HTTP {e.code}: {body[:350]}") from e

def check(status, expected, what):
    if status != expected:
        raise AssertionError(f"{what}: got HTTP {status}, expected {expected}")
    print(f"PASS: {what}")

def mvc_login(email, password, expected_page, expected_fragment):
    opener = client()
    status, html = request(opener, "GET", WEB + "/Account/Login")
    check(status, 200, "MVC login page")
    token = re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)"', html)
    if not token:
        raise AssertionError("MVC login antiforgery token not found in HTML")
    data = urllib.parse.urlencode({"Email": email, "Password": password, "__RequestVerificationToken": token.group(1)}).encode()
    req = urllib.request.Request(WEB + "/Account/Login", data=data, method="POST", headers={"Content-Type": "application/x-www-form-urlencoded"})
    with opener.open(req, timeout=20) as r:
        body = r.read().decode(errors="replace")
        if r.status != 200 or "Log Out" not in body:
            raise AssertionError("MVC login did not establish a logged-in session. Page: " + body[:350])
    status, body = request(opener, "GET", WEB + expected_page)
    check(status, 200, expected_page)
    if expected_fragment not in body:
        raise AssertionError(expected_page + " did not display expected event: " + expected_fragment)
    print("PASS: MVC signed-in page displays the test event")


def main():
    print("RaceDay NKM API + MVC smoke test")
    time.sleep(1)
    check(request(client(), "GET", API + "/health")[0], 200, "API health")
    check(request(client(), "GET", WEB + "/")[0], 200, "MVC homepage")
    suffix = uuid.uuid4().hex[:12]
    password = "RaceDay!" + suffix + "A9"
    organiser = f"org-{suffix}@example.com"
    participant = f"runner-{suffix}@example.com"
    o = client()
    p = client()
    for opener, email, role in ((o, organiser, "Organiser"), (p, participant, "Participant")):
        status, _ = request(opener, "POST", API + "/api/auth/register", {
            "firstName": "Demo", "lastName": role, "email": email, "password": password, "role": role})
        check(status, 201, f"{role} registration")
        status, data = request(opener, "POST", API + "/api/auth/login", {"email": email, "password": password})
        check(status, 200, f"{role} API login")
        if data.get("role") != role:
            raise AssertionError(f"Wrong role in {role} login response")
    name = f"Codespaces Test Run {suffix}"
    event_date = (datetime.now() + timedelta(days=21)).date().isoformat()
    status, event = request(o, "POST", API + "/api/events", {
        "eventName": name,
        "description": "Automatic smoke-test event",
        "eventDate": event_date + "T00:00:00",
        "startTime": "07:00:00",
        "location": "Cape Town",
        "distanceKm": 5,
        "eventType": "Run",
        "maximumParticipants": 30,
        "status": "Open"
    })
    check(status, 201, "Organiser creates an event")
    event_id = event.get("eventID")
    if not event_id:
        raise AssertionError("Create event response is missing eventID: " + str(event))
    status, category = request(o, "POST", API + f"/api/events/{event_id}/categories", {
        "categoryName": "5KM Fun Run", "categoryType": "Distance", "distanceKm": 5,
        "description": "Distance category"})
    check(status, 201, "Organiser creates a category")
    category_id = category.get("categoryID")
    if not category_id:
        raise AssertionError("Create category response is missing categoryID: " + str(category))
    status, enrollment = request(p, "POST", API + f"/api/events/{event_id}/enrollments", {"categoryID": category_id})
    check(status, 201, "Participant enrols in event")
    enrollment_id = enrollment.get("enrollmentID")
    if not enrollment_id:
        raise AssertionError("Enrol response missing enrollmentID: " + str(enrollment))
    status, _ = request(o, "POST", API + f"/api/enrollments/{enrollment_id}/result", {
        "finishTime": "00:32:15", "overallPosition": 1, "categoryPosition": 1})
    check(status, 201, "Organiser records a race result")
    status, results = request(p, "GET", API + "/api/results/me")
    check(status, 200, "Participant reads own results")
    if not any(r.get("eventID") == event_id for r in results):
        raise AssertionError("Recorded race result not found for participant")
    print("PASS: Recorded result visible to participant")
    mvc_login(participant, password, "/Events/Index", name)
    mvc_login(organiser, password, "/Organiser/Index", name)
    print("\nSUCCESS: API + MVC registration, login, event, category, enrolment and results checks passed")

if __name__ == "__main__":
    try:
        main()
    except (AssertionError, urllib.error.URLError, TimeoutError, ValueError) as exc:
        print("FAIL:", exc)
        sys.exit(1)
