using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels;

public class EventFormViewModel
{
    [Required, MaxLength(150)] public string EventName { get; set; } = "";
    [MaxLength(500)] public string? Description { get; set; }
    [Required, DataType(DataType.Date)] public DateTime EventDate { get; set; } = DateTime.Today.AddDays(14);
    [Required] public TimeSpan StartTime { get; set; } = new(7, 0, 0);
    [Required, MaxLength(200)] public string Location { get; set; } = "";
    [Range(0.1, 1000)] public decimal DistanceKm { get; set; } = 5;
    [Required] public string EventType { get; set; } = "Run";
    [Range(1, 100000)] public int MaximumParticipants { get; set; } = 100;
    [Required] public string Status { get; set; } = "Open";
}

public class CategoryFormViewModel
{
    public int EventID { get; set; }
    [Required, MaxLength(100)] public string CategoryName { get; set; } = "";
    [Required] public string CategoryType { get; set; } = "Distance";
    [Range(0, 120)] public int? MinimumAge { get; set; }
    [Range(0, 120)] public int? MaximumAge { get; set; }
    [Range(0.1, 1000)] public decimal? DistanceKm { get; set; }
    [MaxLength(300)] public string? Description { get; set; }
}

public class OrganiserEnrollmentViewModel
{
    public int EnrollmentID { get; set; }
    public int ParticipantID { get; set; }
    public string ParticipantName { get; set; } = "";
    public string ParticipantEmail { get; set; } = "";
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = "";
    public DateTime EnrollmentDate { get; set; }
    public string EnrollmentStatus { get; set; } = "";
}

public class RaceResultViewModel
{
    public int ResultID { get; set; }
    public int EnrollmentID { get; set; }
    public int EventID { get; set; }
    public string EventName { get; set; } = "";
    public string ParticipantName { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public TimeSpan FinishTime { get; set; }
    public int OverallPosition { get; set; }
    public int CategoryPosition { get; set; }
}

public class EventResultsPageViewModel
{
    public int EventID { get; set; }
    public string EventName { get; set; } = "";
    public List<OrganiserEnrollmentViewModel> Enrollments { get; set; } = new();
    public List<RaceResultViewModel> Results { get; set; } = new();
}

public class RecordResultFormViewModel
{
    public int EventID { get; set; }
    public int EnrollmentID { get; set; }
    [Required] public TimeSpan FinishTime { get; set; }
    [Range(1, int.MaxValue)] public int OverallPosition { get; set; } = 1;
    [Range(1, int.MaxValue)] public int CategoryPosition { get; set; } = 1;
}
