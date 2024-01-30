using Microsoft.AspNetCore.Components;
using SimpleComponents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleComponents
{
    public partial class TreeView<T>
    {
        private CalendarJsInterop jsInterop;

        /// <summary>
        /// Data of the tree
        /// </summary>
        [Parameter]
        public List<TreeViewModel<T>> TreeItems { get; set; } = new();

        /// <summary>
        /// Handle an item on the tree is clicked
        /// </summary>
        [Parameter]
        public EventCallback<TreeViewModel<T>> OnItemClick { get; set; }

        /// <summary>
        /// Html template of data content
        /// </summary>
        [Parameter]
        public RenderFragment<T> ContentTemplate { get; set; }

        /// <summary>
        /// Get items by current level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private IEnumerable<TreeViewModel<T>> GetItemsByLevel(int level)
        {
            return TreeItems.Where(x => x.Level == level);
        }

        /// <summary>
        /// Get items at next level by current item Id
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private IEnumerable<TreeViewModel<T>> GetItemsAtNextLevel(int parentId, int level)
        {
            return TreeItems.Where(x => x.Level == level + 1 && x.ParentId == parentId);
        }
    }
}
