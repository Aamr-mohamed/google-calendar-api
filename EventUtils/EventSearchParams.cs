namespace GoogleCalenderApi.EventUtils
{
    public class EventSearchParams
    {
        public string? name { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
        public int? pageSize { get; set; }
        public int pageNumber { get; set; } = 1;
        public string? token { get; set; }
    }
}
