using Microsoft.JSInterop;

namespace SimpleComponents
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class CalendarJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly DotNetObjectReference<CalendarJsInterop> dotNetObjectReference;
        private IJSRuntime Js;

        public CalendarJsInterop(IJSRuntime jsRuntime)
        {
            dotNetObjectReference = DotNetObjectReference.Create(this);
            Js = jsRuntime;
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/SimpleComponents/calendarJsInterop.js").AsTask());
        }

        public async Task ResizeCalendarEvents()
        {
            if(moduleTask == null)
            {
                return;
            }
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync("resizeCalendarEvents");
        }

        public async ValueTask<string> Prompt(string message)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                dotNetObjectReference.Dispose();
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
