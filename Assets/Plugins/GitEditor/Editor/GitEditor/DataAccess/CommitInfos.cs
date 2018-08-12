using LibGit2Sharp;

/// <summary>
/// The namespace used for Data Access classes.
/// </summary>
namespace GitEditor.DataAccess
{
    /// <summary>
    /// The temporary commit infos host.
    /// Used to pass commits infos from RepositoryManager to GitEditor.
    /// </summary>
    public class CommitInfos
    {
        /// <summary>
        /// The commit SHA identifier.
        /// </summary>
        public string sha;

        /// <summary>
        /// The commit short message.
        /// </summary>
        public string messageShort;

        /// <summary>
        /// The commit author name.
        /// </summary>
        public string authorName;

        /// <summary>
        /// The commit author email.
        /// </summary>
        public string authorEmail;

        /// <summary>
        /// The changes of the commit.
        /// </summary>
        public TreeChanges changesFromParent;
    }
}
