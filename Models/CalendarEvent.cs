namespace SimpleComponents.Models
{
    public class CalendarEvent<T>
    {
        /// <summary>
        /// Id of event
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Start date of event
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of event
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Display content as HTml
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStartEvent { get; set; } = true;
        
        /// <summary>
        /// Event duration
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Order { get; set; } = 1;

        /// <summary>
        /// Background color of event
        /// </summary>
        public string BackGroundColor { get; set; } = "#BAF3FF";

        /// <summary>
        /// Text color
        /// </summary>
        public string Color { get; set; } = "black";

        /// <summary>
        /// Data of event
        /// </summary>
        public T Data { get; set; }
    }
}
