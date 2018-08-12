/// <summary>
/// The namespace used for models.
/// </summary>
namespace GitEditor.Model
{
    /// <summary>
    /// Class holding files informations.
    /// Used by CommitTreeParser to generate the tree.
    /// </summary>
    public class File
    {
        /// <summary>
        /// The file name.
        /// </summary>
        public string name;

        /// <summary>
        /// The file full path.
        /// </summary>
        public string fullPath;
    } 
}