using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
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
        private CalendarJsInterop _jsInterop;

        /// <summary>
        /// Events of calendar
        /// </summary>
        [Parameter]
        [EditorRequired]
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
        /// Template html Title on the top of calendar.
        /// Including change month button group
        /// </summary>
        [Parameter]
        public RenderFragment TitleTemplate { get; set; }

        /// <summary>
        /// Start day of pth-week.
        /// Only Monday or Sunday
        /// </summary>
        [Parameter]
        public DayOfWeek StartDayOfWeek { get; set; } = DayOfWeek.Sunday;

        /// <summary>
        /// Method to handle user moves the mouse from start date to end date.
        /// Return date range with start date and end date
        /// </summary>
        [Parameter]
        public EventCallback<DateRange> OnSelectDateRange { get; set; }

        /// <summary>
        /// List day of the pth-week
        /// </summary>
        List<DayOfWeek> _daysOfWeek = new()
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
        Dictionary<DateTime, int> _showingDate = new();

        /// <summary>
        /// List event show on the calendar grid after handled
        /// </summary>
        List<CalendarEvent<T>> _showingEvents = new();

        /// <summary>
        /// Selected range when user drag and drop
        /// </summary>
        DateRange _selectedRange = new();

        /// <summary>
        /// Initializes the component and sets up the initial state of the calendar.
        /// </summary>
        /// <remarks>
        /// This method is invoked by the framework during the component's initialization phase.
        /// It configures the calendar's starting day of the week and initializes the JavaScript interop
        /// for handling calendar-related operations.
        /// </remarks>
        protected override void OnInitialized()
        {
            if (StartDayOfWeek == DayOfWeek.Monday)
            {
                _daysOfWeek = new()
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
                _daysOfWeek = new()
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

            _jsInterop = new(Js);

            InitCalendar();
        }

        /// <summary>
        /// Invoked when the component's parameters are set or updated.
        /// </summary>
        /// <remarks>
        /// This method is called by the framework whenever the component's parameters are assigned new values.
        /// It ensures that the component's state is updated accordingly.
        /// </remarks>
        protected override void OnParametersSet()
        {
            HandleShrinkEvents(Events);
        }

        /// <summary>
        /// Executes logic after the component has been rendered.
        /// </summary>
        /// <param name="firstRender">
        /// A boolean value indicating whether this is the first time the component has been rendered.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await _jsInterop.ResizeCalendarEvents();
        }
        /// <summary>
        /// Handles the click event for the "Next" or "Previous" buttons in the calendar.
        /// </summary>
        /// <param name="month">
        /// The number of months to move the calendar. A positive value moves the calendar forward,
        /// while a negative value moves it backward.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation of updating the calendar's displayed date
        /// and invoking the <see cref="OnNextPreviousBtnClick"/> callback.
        /// </returns>
        /// <remarks>
        /// This method updates the <see cref="ShowedDate"/> property by adding the specified number of months,
        /// reinitialize the calendar grid, adjusts the events displayed, and triggers the callback to notify
        /// about the date change.
        /// </remarks>
        private async Task HandleNextPreviousBtnClick(int month)
        {
            ShowedDate = ShowedDate.AddMonths(month);
            InitCalendar();
            HandleShrinkEvents(Events);
            await OnNextPreviousBtnClick.InvokeAsync(ShowedDate);
        }

        /// <summary>
        /// Init calendar grid UI
        /// Handle dates will be displayed on the grid
        /// </summary>
        private void InitCalendar()
        {
            _showingDate = new();

            var firstDayOfMonth = ShowedDate;
            var firstDayInDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            var lastDayOfMonth = LastDayOfMonth(ShowedDate);
            var lastDayInDayOfWeek = (int)lastDayOfMonth.DayOfWeek;

            // row of calendar grid
            int row = 1;

            if (StartDayOfWeek == DayOfWeek.Monday)
            {
                switch (firstDayInDayOfWeek)
                {
                    case > (int)DayOfWeek.Monday:
                        {
                            for (DateTime date = firstDayOfMonth.AddDays(-(firstDayInDayOfWeek - 1)); date < firstDayOfMonth; date = date.AddDays(1))
                            {
                                _showingDate.Add(date, row);
                            }

                            break;
                        }
                    case (int)DayOfWeek.Sunday:
                        {
                            for (DateTime date = firstDayOfMonth.AddDays(-6); date < firstDayOfMonth; date = date.AddDays(1))
                            {
                                _showingDate.Add(date, row);

                                if (_showingDate.Count % 7 == 0)
                                {
                                    row++;
                                }
                            }

                            break;
                        }
                }

                for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
                {
                    _showingDate.Add(date, row);

                    if (_showingDate.Count % 7 == 0)
                    {
                        row++;
                    }
                }

                if (lastDayInDayOfWeek <= (int)DayOfWeek.Sunday || lastDayInDayOfWeek > (int)DayOfWeek.Saturday) return;

                for (DateTime date = lastDayOfMonth.AddDays(1); date <= lastDayOfMonth.AddDays(7 - lastDayInDayOfWeek); date = date.AddDays(1))
                {
                    _showingDate.Add(date, row);
                }
            }
            else
            {

                for (DateTime date = firstDayOfMonth.AddDays(-firstDayInDayOfWeek); date <= lastDayOfMonth.AddDays(6 - lastDayInDayOfWeek); date = date.AddDays(1))
                {
                    _showingDate.Add(date, row);

                    if (_showingDate.Count % 7 == 0)
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
            var startDate = _showingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.StartDate.Date) == 0);
            var endDate = _showingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.EndDate.Date) == 0);

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
            return _showingDate.Where(x => x.Value == row).OrderBy(x => x.Key).First().Key;
        }

        /// <summary>
        /// Get the last date of row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private DateTime LastDateOfRow(int row)
        {
            return _showingDate.Where(x => x.Value == row).OrderByDescending(x => x.Key).First().Key;
        }

        /// <summary>
        /// Shrink events after Events parameter set
        /// </summary>
        private void HandleShrinkEvents(List<CalendarEvent<T>> events)
        {
            try
            {
                List<CalendarEvent<T>> calendars = new();
                foreach (var item in events.OrderBy(x => x.StartDate))
                {
                    if (!IsStartDateAndEndDateStaySameRow(item))
                    {
                        ProcessMultiRowEvent(item, calendars);
                    }
                    else
                    {
                        ProcessSingleRowEvent(item, calendars);
                    }
                }
                _showingEvents = calendars;
                if (ShowCreateBtn)
                {
                    HandleShowCreateBtn();
                }
            }
            catch { }
        }

        /// <summary>
        /// Processes a calendar event that spans multiple rows in the calendar view.
        /// </summary>
        /// <param name="item">The calendar event to be processed, containing details such as start date, end date, and associated data.</param>
        /// <param name="calendars">The list of calendar events to which the processed event will be added.</param>
        private void ProcessMultiRowEvent(CalendarEvent<T> item, List<CalendarEvent<T>> calendars)
        {
            var startDateRow = _showingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.StartDate.Date) == 0).Value;
            var endDateRow = _showingDate.FirstOrDefault(x => DateTime.Compare(x.Key.Date, item.EndDate.Date) == 0).Value;
            if (startDateRow > 0)
            {
                ProcessRows(item, calendars, startDateRow, endDateRow > 0 ? endDateRow : _showingDate.MaxBy(x => x.Value).Value);
            }
            else if (startDateRow == 0 && endDateRow > 0)
            {
                ProcessRows(item, calendars, 1, endDateRow);
            }
        }

        /// <summary>
        /// Processes calendar events and distributes them across the specified rows.
        /// </summary>
        /// <param name="item">The calendar event to be processed.</param>
        /// <param name="calendars">The list of calendar events to which the processed event will be added.</param>
        /// <param name="startRow">The starting row index for the event.</param>
        /// <param name="endRow">The ending row index for the event.</param>
        private void ProcessRows(CalendarEvent<T> item, List<CalendarEvent<T>> calendars, int startRow, int endRow)
        {
            for (int i = startRow; i <= endRow; i++)
            {
                var calendarEvent = CreateCalendarEvent(item, i, startRow, endRow);
                SetEventOrder(calendarEvent, calendars);
                calendars.Add(calendarEvent);
            }
        }

        /// <summary>
        /// Creates a new calendar event for a specific row in the calendar.
        /// </summary>
        /// <param name="item">The original calendar event to be processed.</param>
        /// <param name="currentRow">The current row in the calendar where the event will be placed.</param>
        /// <param name="startRow">The starting row of the event in the calendar.</param>
        /// <param name="endRow">The ending row of the event in the calendar.</param>
        /// <returns>A new <see cref="CalendarEvent{T}"/> instance representing the event for the specified row.</returns>
        private CalendarEvent<T> CreateCalendarEvent(CalendarEvent<T> item, int currentRow, int startRow, int endRow)
        {
            return new CalendarEvent<T>
            {
                Id = item.Id,
                StartDate = currentRow == startRow ? item.StartDate : FirstDateOfRow(currentRow),
                EndDate = currentRow == endRow ? item.EndDate : LastDateOfRow(currentRow),
                Content = item.Content,
                BackGroundColor = GetBackgroundColor(item),
                IsStartEvent = currentRow == startRow,
                Data = item.Data,
                Duration = ((currentRow == endRow ? item.EndDate : LastDateOfRow(currentRow)).AddDays(1).Date
                            - (currentRow == startRow ? item.StartDate : FirstDateOfRow(currentRow)).Date).TotalDays
            };
        }

        /// <summary>
        /// Processes a single-row calendar event by calculating its duration, 
        /// setting its order, and adding it to the provided list of calendar events.
        /// </summary>
        /// <param name="item">
        /// The <see cref="CalendarEvent{T}"/> instance representing the event to be processed.
        /// </param>
        /// <param name="calendars">
        /// A list of <see cref="CalendarEvent{T}"/> instances representing the existing calendar events.
        /// </param>
        private void ProcessSingleRowEvent(CalendarEvent<T> item, List<CalendarEvent<T>> calendars)
        {
            item.Duration = (item.EndDate.AddDays(1).Date - item.StartDate.Date).TotalDays;
            SetEventOrder(item, calendars);
            calendars.Add(item);
        }

        /// <summary>
        /// Sets the order of the specified calendar event within the provided list of calendar events.
        /// </summary>
        /// <param name="calendarEvent">
        /// The <see cref="CalendarEvent{T}"/> instance for which the order is to be set.
        /// </param>
        /// <param name="calendars">
        /// A list of <see cref="CalendarEvent{T}"/> instances representing the existing calendar events.
        /// </param>
        private void SetEventOrder(CalendarEvent<T> calendarEvent, List<CalendarEvent<T>> calendars)
        {
            var prevEvents = calendars.Where(x => DateTime.Compare(x.StartDate.Date, calendarEvent.StartDate.Date) <= 0
                                                  && DateTime.Compare(x.EndDate, calendarEvent.StartDate.Date) >= 0);
            if (prevEvents.Any())
            {
                calendarEvent.Order = prevEvents.MaxBy(x => x.Order).Order + 1;
            }
        }

        /// <summary>
        /// Determines the background color for a given calendar event.
        /// </summary>
        /// <param name="item">The calendar event for which the background color is to be determined.</param>
        /// <returns>
        /// A string representing the background color. If the event occurs today and 
        /// <see cref="EventTodayBackGroundColor"/> is set, it returns that color; otherwise, it defaults to "#BAF3FF".
        /// </returns>
        private string GetBackgroundColor(CalendarEvent<T> item)
        {
            return string.IsNullOrEmpty(EventTodayBackGroundColor) ||
                   (DateTime.Compare(item.StartDate.Date, DateTime.Now.Date) <= 0 &&
                    DateTime.Compare(item.EndDate.Date, DateTime.Now.Date) >= 0)
                ? EventTodayBackGroundColor
                : "#BAF3FF";
        }

        /// <summary>
        /// Retrieves a list of dates corresponding to a specific row in the calendar.
        /// </summary>
        /// <param name="row">The row number for which to retrieve the dates.</param>
        /// <returns>A list of <see cref="DateTime"/> objects representing the dates in the specified row.</returns>
        List<DateTime> GetDatesByRow(int row)
        {
            return _showingDate.Where(x => x.Value == row).Select(x => x.Key).ToList();
        }

        /// <summary>
        /// Handle a day on the grid clicked
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Handle a calendar event is clicked
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task HandleEventClick(CalendarEvent<T> e)
        {
            if (OnEventClick.HasDelegate)
            {
                await OnEventClick.InvokeAsync(e.Data);
            }
        }

        /// <summary>
        /// Handle show the create button to the empty day
        /// </summary>
        private void HandleShowCreateBtn()
        {
            var eventsDate = _showingEvents.Select(x => new { StartDate = x.StartDate.Date, EndDate = x.EndDate.Date });
            var emptyDate = _showingDate.Where(x => !eventsDate.Any(y => DateTime.Compare(x.Key.Date, y.StartDate) >= 0
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

                _showingEvents.Add(emptyEvent);
            }

            _showingEvents = _showingEvents.OrderBy(x => x.StartDate).ToList();
        }

        /// <summary>
        /// Handle when user drag a day on the grid.
        /// The dragged day is start date
        /// </summary>
        /// <param name="e"></param>
        /// <param name="startDate"></param>
        private void OnDragDay(DragEventArgs e, DateTime startDate)
        {
            if (OnSelectDateRange.HasDelegate)
            {
                _selectedRange.StartDate = startDate;
            }
        }

        /// <summary>
        /// Handle when user drop the day to another day.
        /// Get the end date
        /// </summary>
        /// <param name="e"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task OnDropDay(DragEventArgs e, DateTime endDate)
        {
            if (OnSelectDateRange.HasDelegate && _selectedRange.StartDate.Date != DateTime.MinValue)
            {
                if (DateTime.Compare(_selectedRange.StartDate.Date, endDate.Date) > 0)
                {
                    _selectedRange.EndDate = _selectedRange.StartDate;
                    _selectedRange.StartDate = endDate;
                }
                else
                {
                    _selectedRange.EndDate = endDate;
                }

                await OnSelectDateRange.InvokeAsync(_selectedRange);
                _selectedRange = new();
            }
        }
    }
}
