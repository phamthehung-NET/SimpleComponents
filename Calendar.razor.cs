using Microsoft.AspNetCore.Components;
using SimpleComponents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleComponents
{
    public partial class Calendar<T>
    {
        private CalendarJsInterop jsInterop;

        /// <summary>
        /// Events of calendar
        /// </summary>
        [Parameter]
        public List<CalendarEvent<T>> Events { get; set; } = new();

        /// <summary>
        /// Date to show on calendar; default is current month
        /// </summary>
        [Parameter]
        public DateTime ShowedDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        /// <summary>
        /// Min date that user allowed to click
        /// </summary>
        [Parameter]
        public DateTime? MinDate { get; set; }

        /// <summary>
        /// High light today on calendar
        /// </summary>
        [Parameter]
        public bool HighLightToday { get; set; } = false;

        /// <summary>
        /// Method to handle after the next and previous month button are clicked
        /// </summary>
        [Parameter]
        public EventCallback<DateTime> OnNextPreviousBtnClick { get; set; }

        /// <summary>
        /// Method to handle an event on calendar is clicked
        /// </summary>
        [Parameter]
        public EventCallback<T> OnEventClick { get; set; }

        /// <summary>
        /// Method to handle an empty date is clicked
        /// </summary>
        [Parameter]
        public EventCallback<DateTime> OnEmptyDayClick { get; set; }

        /// <summary>
        /// Method to handle a date contains events is clicked
        /// </summary>
        [Parameter]
        public EventCallback<EventDTO<T>> OnEventDayClick { get; set; }

        /// <summary>
        /// Background color for today event
        /// </summary>
        [Parameter]
        public string EventTodayBackGroundColor { get; set; } = "#FEFFBA";

        /// <summary>
        /// Show create button for empty day
        /// </summary>
        [Parameter]
        public bool ShowCreateBtn { get; set; } = false;

        /// <summary>
        /// Content html template of event to display to grid
        /// </summary>
        [Parameter]
        public RenderFragment<T> EventContentTemplate { get; set; }

        /// <summary>
        /// Template html for Create button
        /// </summary>
        [Parameter]
        public RenderFragment CreateBtnTemplate { get; set; }

        /// <summary>
        /// Start day of week.
        /// Only Monday or Sunday
        /// </summary>
        [Parameter]
        public DayOfWeek StartDayOfWeek { get; set; } = DayOfWeek.Sunday;

        /// <summary>
        /// Template html Title on the top of calendar.
        /// Including change month button group
        /// </summary>
        [Parameter]
        public RenderFragment TitleTemplate { get; set; }

        /// <summary>
        /// List day of the week
        /// </summary>
        private List<DayOfWeek> DaysOfWeek = new()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday,
        };

        /// <summary>
        /// Date time of month after split to rows and show on calendar
        /// </summary>
        private Dictionary<DateTime, int> ShowingDate = new();

        /// <summary>
        /// List event show on the calendar grid after handled
        /// </summary>
        private List<CalendarEvent<T>> ShowingEvents = new();

        protected override void OnInitialized()
        {
            if (StartDayOfWeek == DayOfWeek.Monday)
            {
                DaysOfWeek = new()
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday,
                    DayOfWeek.Sunday,
                };
            }
            else
            {
                DaysOfWeek = new()
                {
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday,
                };
            }

            jsInterop = new(Js);

            InitCalendar();
        }

        protected override void OnParametersSet()
        {
            HandleShrinkEvents();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await jsInterop.ResizeCalendarEvents();
        }

        private async Task HandleNextPreviousBtnClick(int month)
        {
            ShowedDate = ShowedDate.AddMonths(month);
            InitCalendar();
            await OnNextPreviousBtnClick.InvokeAsync(ShowedDate);
        }

        /// <summary>
        /// Init calendar grid UI
        /// Handle dates will be displayed on the grid
        /// </summary>
        private void InitCalendar()
        {
            ShowingDate = new();

            var firstDayOfMonth = ShowedDate;
            var firstDayInDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            var lastDayOfMonth = LastDayOfMonth(ShowedDate);
            var lastDayInDayOfWeek = (int)lastDayOfMonth.DayOfWeek;

            // row of calendar grid
            int row = 1;

            if (StartDayOfWeek == DayOfWeek.Monday)
            {
                if (firstDayInDayOfWeek > (int)DayOfWeek.Monday)
                {
                    for (DateTime date = firstDayOfMonth.AddDays(-(firstDayInDayOfWeek - 1)); date < firstDayOfMonth; date = date.AddDays(1))
                    {
                        ShowingDate.Add(date, row);
                    }
                }
                if (firstDayInDayOfWeek == (int)DayOfWeek.Sunday)
                {
                    for (DateTime date = firstDayOfMonth.AddDays(-6); date < firstDayOfMonth; date = date.AddDays(1))
                    {
                        ShowingDate.Add(date, row);

                        if (ShowingDate.Count % 7 == 0)
                        {
                            row++;
                        }
                    }
                }

                for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
                {
                    ShowingDate.Add(date, row);

                    if (ShowingDate.Count % 7 == 0)
                    {
                        row++;
                    }
                }

                if (lastDayInDayOfWeek > (int)DayOfWeek.Sunday && lastDayInDayOfWeek <= (int)DayOfWeek.Saturday)
                {
                    for (DateTime date = lastDayOfMonth.AddDays(1); date <= lastDayOfMonth.AddDays(7 - lastDayInDayOfWeek); date = date.AddDays(1))
                    {
                        ShowingDate.Add(date, row);
                    }
                }
            }
            else
            {

                for (DateTime date = firstDayOfMonth.AddDays(-firstDayInDayOfWeek); date <= lastDayOfMonth.AddDays(6 - lastDayInDayOfWeek); date = date.AddDays(1))
                {
                    ShowingDate.Add(date, row);

                    if (ShowingDate.Count % 7 == 0)
                    {
                        row++;
                    }
                }
            }
        }

        /// <summary>
        /// Check the start date and end date of event are located in same row on calendar grid
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsStartDateAndEndDateStaySameRow(CalendarEvent<T> item)
        {
            var startDate = ShowingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.StartDate.Date) == 0);
            var endDate = ShowingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.EndDate.Date) == 0);

            if (startDate.Key == DateTime.MinValue)
            {
                return false;
            }
            if (endDate.Key == DateTime.MinValue)
            {
                return false;
            }
            return startDate.Value == endDate.Value;
        }

        /// <summary>
        /// Get the last day of month
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private DateTime FirstDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Get the last day of month
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private DateTime LastDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        /// <summary>
        /// Get the first date of row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private DateTime FirstDateOfRow(int row)
        {
            return ShowingDate.Where(x => x.Value == row).OrderBy(x => x.Key).First().Key;
        }

        /// <summary>
        /// Get the last date of row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private DateTime LastDateOfRow(int row)
        {
            return ShowingDate.Where(x => x.Value == row).OrderByDescending(x => x.Key).First().Key;
        }

        /// <summary>
        /// Shrink events after Events parameter set
        /// </summary>
        private void HandleShrinkEvents()
        {
            try
            {
                List<CalendarEvent<T>> calendars = new();

                foreach (var item in Events.OrderBy(x => x.StartDate))
                {
                    if (!IsStartDateAndEndDateStaySameRow(item))
                    {
                        var startDateRow = ShowingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.StartDate.Date) == 0).Value;
                        var endDateRow = ShowingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.EndDate.Date) == 0).Value;

                        if (startDateRow > 0)
                        {
                            if (endDateRow > 0)
                            {
                                for (int i = startDateRow; i <= endDateRow; i++)
                                {
                                    CalendarEvent<T> calendarEvent = new()
                                    {
                                        Id = item.Id,
                                        StartDate = i == startDateRow ? item.StartDate : FirstDateOfRow(i),
                                        EndDate = i == endDateRow ? item.EndDate : LastDateOfRow(i),
                                        Content = item.Content,
                                        BackGroundColor = string.IsNullOrEmpty(EventTodayBackGroundColor) || DateTime.Compare(item.StartDate.Date, DateTime.Now.Date) <= 0 && DateTime.Compare(item.EndDate.Date, DateTime.Now.Date) >= 0 ? EventTodayBackGroundColor : "#BAF3FF",
                                        IsStartEvent = i == startDateRow,
                                        Data = item.Data
                                    };

                                    calendarEvent.Duration = (calendarEvent.EndDate.AddDays(1).Date - calendarEvent.StartDate.Date).TotalDays;

                                    var prevEvents = calendars.Where(x => DateTime.Compare(x.StartDate.Date, calendarEvent.StartDate.Date) <= 0
                                        && DateTime.Compare(x.EndDate, calendarEvent.StartDate.Date) >= 0);

                                    if (prevEvents.Any())
                                    {
                                        calendarEvent.Order = prevEvents.MaxBy(x => x.Order).Order + 1;
                                    }

                                    calendars.Add(calendarEvent);
                                }
                            }
                            else
                            {
                                for (int i = startDateRow; i <= ShowingDate.MaxBy(x => x.Value).Value; i++)
                                {
                                    CalendarEvent<T> calendarEvent = new()
                                    {
                                        Id = item.Id,
                                        StartDate = i == startDateRow ? item.StartDate : FirstDateOfRow(i),
                                        EndDate = i == endDateRow ? item.EndDate : LastDateOfRow(i),
                                        Content = item.Content,
                                        BackGroundColor = string.IsNullOrEmpty(EventTodayBackGroundColor) || DateTime.Compare(item.StartDate.Date, DateTime.Now.Date) <= 0 && DateTime.Compare(item.EndDate.Date, DateTime.Now.Date) >= 0 ? EventTodayBackGroundColor : "#BAF3FF",
                                        IsStartEvent = i == startDateRow,
                                        Data = item.Data
                                    };

                                    calendarEvent.Duration = (calendarEvent.EndDate.AddDays(1).Date - calendarEvent.StartDate.Date).TotalDays;

                                    var prevEvents = calendars.Where(x => DateTime.Compare(x.StartDate.Date, calendarEvent.StartDate.Date) <= 0
                                        && DateTime.Compare(x.EndDate, calendarEvent.StartDate.Date) >= 0);

                                    if (prevEvents.Any())
                                    {
                                        calendarEvent.Order = prevEvents.MaxBy(x => x.Order).Order + 1;
                                    }

                                    calendars.Add(calendarEvent);
                                }
                            }
                        }
                        else if (startDateRow == 0 && endDateRow > 0)
                        {
                            for (int i = 1; i <= endDateRow; i++)
                            {
                                CalendarEvent<T> calendarEvent = new()
                                {
                                    Id = item.Id,
                                    StartDate = i == startDateRow ? item.StartDate : FirstDateOfRow(i),
                                    EndDate = i == endDateRow ? item.EndDate : LastDateOfRow(i),
                                    Content = item.Content,
                                    BackGroundColor = string.IsNullOrEmpty(EventTodayBackGroundColor) || DateTime.Compare(item.StartDate.Date, DateTime.Now.Date) <= 0 && DateTime.Compare(item.EndDate.Date, DateTime.Now.Date) >= 0 ? EventTodayBackGroundColor : "#BAF3FF",
                                    IsStartEvent = i == startDateRow,
                                    Data = item.Data
                                };

                                calendarEvent.Duration = (calendarEvent.EndDate.AddDays(1).Date - calendarEvent.StartDate.Date).TotalDays;

                                var prevEvents = calendars.Where(x => DateTime.Compare(x.StartDate.Date, calendarEvent.StartDate.Date) <= 0
                                    && DateTime.Compare(x.EndDate, calendarEvent.StartDate.Date) >= 0);

                                if (prevEvents.Any())
                                {
                                    calendarEvent.Order = prevEvents.MaxBy(x => x.Order).Order + 1;
                                }

                                calendars.Add(calendarEvent);
                            }
                        }
                    }
                    else
                    {
                        item.Duration = (item.EndDate.AddDays(1).Date - item.StartDate.Date).TotalDays;

                        var prevEvents = calendars.Where(x => DateTime.Compare(x.StartDate.Date, item.StartDate.Date) <= 0
                            && DateTime.Compare(x.EndDate, item.StartDate.Date) >= 0);

                        if (prevEvents.Any())
                        {
                            item.Order = prevEvents.MaxBy(x => x.Order).Order + 1;
                        }

                        calendars.Add(item);
                    }
                }

                ShowingEvents = calendars;

                if (ShowCreateBtn)
                {
                    HandleShowCreateBtn();
                }
            }
            catch { }
        }

        private List<DateTime> GetDatesByRow(int row)
        {
            return ShowingDate.Where(x => x.Value == row).Select(x => x.Key).ToList();
        }

        private async Task HandleDayClick(DateTime date)
        {
            if (!ShowCreateBtn)
            {
                if (date.Month != ShowedDate.Month)
                {
                    await HandleNextPreviousBtnClick(date.Month - ShowedDate.Month);
                }
                else
                {
                    if (MinDate.HasValue && DateTime.Compare(date.Date, MinDate.Value.Date) < 0)
                    {
                        return;
                    }
                    if (!Events.Any(x => DateTime.Compare(date.Date, x.StartDate.Date) >= 0
                                    && DateTime.Compare(date.Date, x.EndDate.Date) <= 0) && OnEmptyDayClick.HasDelegate)
                    {
                        await OnEmptyDayClick.InvokeAsync(date);
                    }
                    else
                    {
                        if (OnEventDayClick.HasDelegate)
                        {
                            EventDTO<T> events = new()
                            {
                                Events = Events.Where(x => DateTime.Compare(date.Date, x.StartDate.Date) >= 0 && DateTime.Compare(date.Date, x.EndDate.Date) <= 0).ToList(),
                                SelectedDate = date
                            };
                            await OnEventDayClick.InvokeAsync(events);
                        }
                    }
                }
            }
            else
            {
                if (MinDate.HasValue && DateTime.Compare(date.Date, MinDate.Value.Date) < 0)
                {
                    return;
                }
                if (!Events.Any(x => DateTime.Compare(date.Date, x.StartDate.Date) >= 0
                                && DateTime.Compare(date.Date, x.EndDate.Date) <= 0) && OnEmptyDayClick.HasDelegate)
                {
                    await OnEmptyDayClick.InvokeAsync(date);
                }
            }
        }

        private async Task HandleEventClick(CalendarEvent<T> e)
        {
            if (OnEventClick.HasDelegate)
            {
                await OnEventClick.InvokeAsync(e.Data);
            }
        }

        private void HandleShowCreateBtn()
        {
            var eventsDate = ShowingEvents.Select(x => new { StartDate = x.StartDate.Date, EndDate = x.EndDate.Date });
            var emptyDate = ShowingDate.Where(x => !eventsDate.Any(y => DateTime.Compare(x.Key.Date, y.StartDate) >= 0
                                && DateTime.Compare(x.Key.Date, y.EndDate) <= 0)).Select(x => x.Key);

            foreach (var item in emptyDate)
            {
                CalendarEvent<T> emptyEvent = new()
                {
                    Id = -1,
                    BackGroundColor = "#01D1FF",
                    Color = "white",
                    Duration = 1,
                    Order = 2,
                    StartDate = item.Date,
                    EndDate = item.Date,
                };

                ShowingEvents.Add(emptyEvent);
            }

            ShowingEvents = ShowingEvents.OrderBy(x => x.StartDate).ToList();
        }
    }
}
