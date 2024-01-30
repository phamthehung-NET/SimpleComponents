namespace SimpleComponents.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewModel<T>
    {
        /// <summary>
        /// Id of item on the tree
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ParentId of item on the tree.
        /// Should be equal 0 if level equal 1
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Display content of item.
        /// No need to provide if ContentTemplate has been assigned
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Level of item
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Background color for individual item
        /// </summary>
        public string BackGroundColor { get; set; } = "white";

        /// <summary>
        /// Text color for individual item
        /// </summary>
        public string Color { get; set; } = "#369";

        /// <summary>
        /// Data of item
        /// </summary>
        public T Data { get; set; }
    }
}
