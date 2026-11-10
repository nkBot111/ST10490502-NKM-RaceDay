FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Raceday.API/Raceday.API.csproj Raceday.API/
RUN dotnet restore Raceday.API/Raceday.API.csproj
COPY Raceday.API/ Raceday.API/
RUN dotnet publish Raceday.API/Raceday.API.csproj -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5081
EXPOSE 5081
ENTRYPOINT ["dotnet", "Raceday.API.dll"]
