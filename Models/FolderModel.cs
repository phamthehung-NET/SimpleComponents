namespace SimpleComponents.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FolderModel<T>
    {
        /// <summary>
        /// Id of folder
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Parent id of folder.
        /// Should equal 0 if level equal 1
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Content of each folder
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Level of folder
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Expanded state of folder
        /// </summary>
        public bool IsExpanded { get; set; } = false;

        /// <summary>
        /// Text color of folder content
        /// </summary>
        public string Color { get; set; } = "#369";

        /// <summary>
        /// Data of folder
        /// </summary>
        public T Data { get; set; }
    }
}
