
namespace RaceDay.Web.ViewModels
{
    public class EventViewModel
    {
        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public string Location { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }

        public string EventType { get; set; } = string.Empty;

        public int MaximumParticipants { get; set; }

        public string Status { get; set; } = string.Empty;

        public int OrganiserID { get; set; }

        public string OrganiserName { get; set; } = string.Empty;
    }
}
