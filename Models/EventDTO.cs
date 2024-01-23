namespace SimpleComponents.Models
{
    public class EventDTO<T>
    {
        public List<CalendarEvent<T>> Events {  get; set; }
        
        public DateTime SelectedDate { get; set; }
    }
}
