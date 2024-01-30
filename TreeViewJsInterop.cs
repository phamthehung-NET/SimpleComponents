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
    public class TreeViewJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly DotNetObjectReference<TreeViewJsInterop> dotNetObjectReference;
        private IJSRuntime Js;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        public TreeViewJsInterop(IJSRuntime jsRuntime)
        {
            dotNetObjectReference = DotNetObjectReference.Create(this);
            Js = jsRuntime;
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/SimpleComponents/treeViewJsInterop.js").AsTask());
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
