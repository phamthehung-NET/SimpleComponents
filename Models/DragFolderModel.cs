namespace SimpleComponents.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DragFolderModel<T>
    {
        /// <summary>
        /// Folder is dragged
        /// </summary>
        public T DraggedFolder {  get; set; }

        /// <summary>
        /// Destination folder
        /// </summary>
        public T DestinationFolder { get; set; }

        /// <summary>
        /// Children of dragged folder
        /// </summary>
        public List<T> FolderChildren { get; set; } = new();
    }
}
