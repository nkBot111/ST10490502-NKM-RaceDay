
namespace RaceDay.Web.ViewModels
{
    public class EventDetailsViewModel : EventViewModel
    {
        public List<EventCategoryViewModel> Categories { get; set; }
            = new List<EventCategoryViewModel>();
    }

    public class EventCategoryViewModel
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string CategoryType { get; set; } = string.Empty;

        public int? MinimumAge { get; set; }

        public int? MaximumAge { get; set; }

        public decimal? DistanceKm { get; set; }

        public string? Description { get; set; }
    }
}
