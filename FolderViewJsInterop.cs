using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleComponents
{
    /// <summary>
    /// 
    /// </summary>
    public class FolderViewJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly DotNetObjectReference<FolderViewJsInterop> dotNetObjectReference;
        private IJSRuntime Js;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        public FolderViewJsInterop(IJSRuntime jsRuntime)
        {
            dotNetObjectReference = DotNetObjectReference.Create(this);
            Js = jsRuntime;
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/SimpleComponents/folderViewJsInterop.js").AsTask());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ResizeCalendarEvents()
        {
            if (moduleTask == null)
            {
                return;
            }
            var module = await moduleTask.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async ValueTask<string> Prompt(string message)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
