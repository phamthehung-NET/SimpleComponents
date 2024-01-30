using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SimpleComponents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimpleComponents.Common.ComponentConstants;

namespace SimpleComponents
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class FolderView<T>
    {
        private CalendarJsInterop jsInterop;

        /// <summary>
        /// List of folders will be display on the tree
        /// </summary>
        [Parameter]
        [EditorRequired]
        public List<FolderModel<T>> Folders { get; set; }

        /// <summary>
        /// Method to handle when a folder is clicked
        /// </summary>
        [Parameter]
        public EventCallback<FolderModel<T>> OnFolderClick { get; set; }

        /// <summary>
        /// Handle show root folder (...).
        /// Affected to the drag and drop folder level 1.
        /// Default is true.
        /// </summary>
        [Parameter]
        public bool ShowRootFolder { get; set; } = true;

        /// <summary>
        /// Icon type of folder.
        /// Following the given set of icons.
        /// Need not to provide if FolderTemplate has been provided
        /// </summary>
        [Parameter]
        public FolderIconType IconType { get; set; } = FolderIconType.Folder;

        /// <summary>
        /// Render Html for name and icon of folders
        /// </summary>
        [Parameter]
        public RenderFragment<T> FolderTemplate { get; set; }

        /// <summary>
        /// Content of empty folder
        /// </summary>
        [Parameter]
        public string EmptyText { get; set; } = "(empty)";

        /// <summary>
        /// Method to handle drag and drop folder to other folder.
        /// The returned object is the result after handling the level of the dragged folder and the children of it
        /// </summary>
        [Parameter]
        public EventCallback<DragFolderModel<FolderModel<T>>> OnDragAndDropFolder { get; set; }

        /// <summary>
        /// Model to save the folder after drag and drop
        /// </summary>
        private DragFolderModel<FolderModel<T>> DragModel = new();

        /// <summary>
        /// Get folders by current level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private IEnumerable<FolderModel<T>> GetFoldersByLevel(int level)
        {
            return Folders.Where(x => x.Level == level);
        }

        /// <summary>
        /// Get child folders at next level of current folder
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private IEnumerable<FolderModel<T>> GetFoldersAtNextLevel(int parentId, int level)
        {
            return Folders.Where(x => x.Level == level + 1 && x.ParentId == parentId);
        }

        /// <summary>
        /// Find all child folders by recursive method
        /// </summary>
        /// <param name="parents"></param>
        /// <returns></returns>
        private List<FolderModel<T>> FindChildFolders(IEnumerable<FolderModel<T>> parents)
        {
            List<FolderModel<T>> folders = new();
            foreach (var item in parents)
            {
                var data = GetFoldersAtNextLevel(item.Id, item.Level);
                folders.AddRange(data);
                folders.AddRange(FindChildFolders(data));
            }
            return folders;
        }

        /// <summary>
        /// Handle a folder is clicked
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task OnFolderClicked(FolderModel<T> item)
        {
            if (OnFolderClick.HasDelegate)
            {
                await OnFolderClick.InvokeAsync(item);
            }
            item.IsExpanded = !item.IsExpanded;
        }

        /// <summary>
        /// Handle on a folder is dragged
        /// </summary>
        /// <param name="e"></param>
        /// <param name="item"></param>
        private void OnFolderDragged(DragEventArgs e, FolderModel<T> item)
        {
            DragModel = new();
            DragModel.DraggedFolder = item;
            var data = FindChildFolders(new List<FolderModel<T>>() { item });
            DragModel.FolderChildren.AddRange(data);
        }

        /// <summary>
        /// Handle drop to other folder
        /// </summary>
        /// <param name="e"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task OnDropFolder(DragEventArgs e, FolderModel<T> item)
        {
            DragModel.DestinationFolder = item;
            if (DragModel.DraggedFolder != null && DragModel.DraggedFolder.Id != item.Id && !DragModel.FolderChildren.Select(x => x.Id).Contains(item.Id))
            {
                var levelChanged = item.Level + 1 - DragModel.DraggedFolder.Level;
                DragModel.DraggedFolder.Level = item.Level + 1;
                DragModel.DraggedFolder.ParentId = item.Id;

                foreach (var child in DragModel.FolderChildren)
                {
                    child.Level += levelChanged;
                }
            }
            if (OnDragAndDropFolder.HasDelegate)
            {
                await OnDragAndDropFolder.InvokeAsync(DragModel);
            }
        }

        /// <summary>
        /// Handle when folder at lower level is dragged
        /// </summary>
        /// <param name="item"></param>
        private void OnLowerLevelDrag(FolderModel<T> item)
        {
            DragModel = new();
            DragModel.DraggedFolder = item;
            var data = FindChildFolders(new List<FolderModel<T>>() { item });
            DragModel.FolderChildren.AddRange(data);
        }
    }
}
