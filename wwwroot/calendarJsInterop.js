// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}

export function resizeCalendarEvents() {
    let heights = [];

    for (let i = 1; i <= 6; i++) {
        const maxOrder = Math.max.apply(null, $(`.calendar-event[data-row=${i}]`).map(function () {
            return $(this).attr("data-order");
        }).get());

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