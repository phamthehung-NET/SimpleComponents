using Microsoft.AspNetCore.Components;
using SimpleComponents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleComponents
{
    public partial class TreeViewItem<T>
    {
        private CalendarJsInterop jsInterop;

        /// <summary>
        /// List item
        /// </summary>
        [Parameter]
        public List<TreeViewModel<T>> TreeItems { get; set; } = new();

        /// <summary>
        /// ParentId of showing items
        /// </summary>
        [Parameter]
        public int ParentId { get; set; }

        /// <summary>
        /// Event when click to an item on tree
        /// </summary>
        [CascadingParameter]
        public EventCallback<TreeViewModel<T>> OnItemClick { get; set; }

        /// <summary>
        /// Template of content apply for all items on the tree
        /// </summary>
        [CascadingParameter]
        public RenderFragment<T> ContentTemplate { get; set; }

        /// <summary>
        /// Current level of showing items
        /// </summary>
        private int CurrentLevel = 0;

        /// <summary>
        /// Data is showing on tree
        /// </summary>
        private List<TreeViewModel<T>> ShowingData = new();

        /// <summary>
        /// 
        /// </summary>
        protected override void OnParametersSet()
        {
            CurrentLevel = TreeItems.Any() ? TreeItems.MinBy(x => x.Level).Level : 0;
            ShowingData = GetItemsByLevel(CurrentLevel);
        }

        /// <summary>
        /// Get items by current level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private List<TreeViewModel<T>> GetItemsByLevel(int level)
        {
            return TreeItems.Where(x => x.Level == level && x.ParentId == ParentId).ToList();
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
