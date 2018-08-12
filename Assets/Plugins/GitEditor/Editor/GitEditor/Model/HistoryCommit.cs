using System.Collections.Generic;

/// <summary>
/// The namespace used for models.
/// </summary>
namespace GitEditor.Model
{
    /// <summary>
    /// Class holds a commit informations.
    /// Used by GitEditor to generate the view.
    /// </summary>
    public class HistoryCommit
    {
        /// <summary>
        /// The SHA identifier.
        /// </summary>
        public string id = "";

        /// <summary>
        /// The short message.
        /// </summary>
        public string message = "";

        /// <summary>
        /// The list of changes.
        /// </summary>
        public List<string> changes = new List<string>();

        /// <summary>
        /// Is the commit only on remote ?
        /// </summary>
        public bool onlyRemote = false;

        /// <summary>
        /// Is the commit only on local ?
        /// </summary>
        public bool onlyLocal = false;

        /// <summary>
        /// The commit author.
        /// </summary>
        public string author = "";
    }
}
