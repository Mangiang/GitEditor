using System.Collections.Generic;

/// <summary>
/// The namespace used for models.
/// </summary>
namespace GitEditor.Model
{
    /// <summary>
    /// Class holding a folder informations.
    /// Used by CommitTreeParser to generate the tree.
    /// </summary>
    public class Folder : File
    {
        /// <summary>
        /// The folder file children.
        /// </summary>
        public List<File> filesChildren = new List<File>();

        /// <summary>
        /// The folder folder childre.
        /// </summary>
        public List<Folder> folderChildren = new List<Folder>();
    } 
}