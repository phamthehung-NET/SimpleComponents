namespace SimpleComponents.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventDTO<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CalendarEvent<T>> Events {  get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime SelectedDate { get; set; }
    }
}
