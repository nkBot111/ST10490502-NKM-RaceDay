
namespace RaceDay.Web.ViewModels
{
    public class EnrollmentViewModel
    {
        public int EnrollmentID { get; set; }

        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; }

        public string EnrollmentStatus { get; set; } = string.Empty;
    }
}
