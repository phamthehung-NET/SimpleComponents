// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function resizeCalendarEvents() {
    let heights = [];

    removeDragEventListener();

    registDragEventListener();

    for (let i = 1; i <= 6; i++) {
        // get the highest order of entry row on the grid
        const maxOrder = Math.max.apply(null, $(`.calendar-event[data-row=${i}]`).map(function () {
            return $(this).attr("data-order");
        }).get());

        for (let order = 1; order <= maxOrder; order++) {
            if (order > 1) {
                // get the previous event to get height and top css attribute
                const calendarItem = $(`.calendar-event[data-row=${i}][data-order=${order}]`);
                const prevCalendarItem = $(`.calendar-event[data-row=${i}][data-order=${order - 1}]`);
                const height = prevCalendarItem.outerHeight();
                //substring 'px'
                const top = parseInt(prevCalendarItem.css("top").substring(0, prevCalendarItem.css("top").length - 2))
                calendarItem.css("top", `${height + top + 10}px`)
            }
        }

        // get the highest height in rows on the grid to apply to the rest rows
        const maxHeight = Math.max.apply(null, $(`.calendar-event[data-row=${i}][data-order=${maxOrder}]`).map(function () {
            let top = parseInt($(this).css("top").substring(0, $(this).css("top").length - 2))
            return $(this).outerHeight() + top;
        }).get());

        const dayNumber = $(`.calendar-event[data-row=${i}]`).parents().find("span")[0]

        if (!isNaN(maxHeight + $(dayNumber).outerHeight())) {
            heights.push(maxHeight + $(dayNumber).outerHeight());
        }
    }

    $("div.pth-days.d-flex").css("height", `${Math.max.apply(null, heights)}px`);
}

export function removeDragEventListener() {
    $("body").off("dragstart", ".pth-day")
}

export function registDragEventListener() {
    $(".pth-day").on("dragstart", function (e) {
        e.originalEvent.dataTransfer.setDragImage(new Image(), 0, 0);
    })
}