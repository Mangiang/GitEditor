using GitEditor.Model;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The namespace used for Data Access classes.
/// </summary>
namespace GitEditor.DataAccess
{
    /// <summary>
    /// The Repository Manager.
    /// This class handles all Repository relative operations.
    /// </summary>
    public class RepositoryManager
    {
        private static string repoPath; /*!< The path to the repository relative to the Unity project's root. */

        /// <summary>
        /// Initializes the RepositoryManager
        /// </summary>
        /// <param name="path">The path to the repository relative to the Unity porject's root.</param>
        public static void Init(string path)
        { repoPath = path; }

        #region /////////////// Utility methods ///////////////

        /// <summary>
        /// Get the list of changes since the last commit.
        /// </summary>
        /// <returns>The changes.</returns>
        public static TreeChanges GetChanges()
        {
            using (var repo = new Repository(repoPath))
            {
                return repo.Diff.Compare<TreeChanges>(repo.Head.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory);
            }
        }

        /// <summary>
        /// Get the list of changes between two commits.
        /// </summary>
        /// <param name="firstTree">The first commit.</param>
        /// <param name="secondTree">The second commit.</param>
        /// <returns>The changes.</returns>
        public static TreeChanges GetChanges(LibGit2Sharp.Tree firstTree, LibGit2Sharp.Tree secondTree)
        {
            using (var repo = new Repository(repoPath))
            {
                return repo.Diff.Compare<TreeChanges>(firstTree, secondTree);
            }
        }

        /// <summary>
        /// Get the Patch for a list of paths.
        /// </summary>
        /// <param name="pathList">The list of paths.</param>
        /// <returns>The Patch</returns>
        public static Patch GetPatch(List<string> pathList)
        {
            using (var repo = new Repository(repoPath))
            {
                return repo.Diff.Compare<Patch>(pathList);
            }
        }

        /// <summary>
        /// Get the repository origin url.
        /// </summary>
        /// <returns>The url</returns>
        public static string GetRepositoryUrl()
        {

            using (Repository repo = new Repository(repoPath))
            {
                Remote remote = repo.Network.Remotes["origin"];
                return remote.Url;
            }
        }

        /// <summary>
        /// Get the last commit SHA id.
        /// </summary>
        /// <returns>The SHA id.</returns>
        public static string GetCurrentCommitSha()
        {
            using (Repository repo = new Repository(repoPath))
            {
                return repo.Head.Tip.Sha;
            }
        }

        /// <summary>
        /// Get the number of commits the local repository is ahead.
        /// </summary>
        /// <returns>The number of commits. Can be null</returns>
        public static int? GetRepositoryAhead()
        {
            using (Repository repo = new Repository(repoPath))
            {
                return repo.Head.TrackingDetails.AheadBy;
            }
        }

        /// <summary>
        /// Get the number of commits the local repository is behind.
        /// </summary>
        /// <returns>The number of commits. Can be null</returns>
        public static int? GetRepositoryBehind()
        {
            using (Repository repo = new Repository(repoPath))
            {
                return repo.Head.TrackingDetails.BehindBy;
            }
        }

        /// <summary>
        /// Get all commits on current branch
        /// </summary>
        /// <param name="commits">The list of commits to populate</param>
        public static void GetCommits(ref List<CommitInfos> commits)
        {
            using (var repo = new Repository(repoPath))
            {
                List<Commit> commitList = repo.Commits.ToList();
                foreach (Commit commit in commitList)
                {
                    foreach (var parent in commit.Parents)
                    {
                        commits.Add(new CommitInfos()
                        {
                            sha = commit.Sha,
                            messageShort = commit.MessageShort,
                            authorEmail = commit.Author.Email,
                            authorName = commit.Author.Name,
                            changesFromParent = GetChanges(parent.Tree, commit.Tree)
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Get the list of all branches. 
        /// </summary>
        /// <param name="branches">The list of branches</param>
        /// <param name="currentBranch">The currentBranch name.</param>
        /// <param name="currentBranchIndex">The current branch index.</param>
        public static void GetBranches(ref List<string> branches, ref string currentBranch, ref int currentBranchIndex)
        {
            using (var repo = new Repository(repoPath))
            {
                foreach (Branch b in repo.Branches.Where(b => !b.IsRemote))
                {
                    branches.Add(b.FriendlyName);
                    if (b.IsCurrentRepositoryHead)
                    {
                        currentBranch = b.FriendlyName;
                    }
                    currentBranchIndex = branches.IndexOf(currentBranch);
                }
            }
        }
        #endregion

        #region /////////////// Checkout methods ///////////////

        /// <summary>
        /// Checkout a file or folder. 
        /// </summary>
        /// <param name="path">The path to the file or folder.</param>
        public static void Checkout(string path)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    CheckoutOptions options = new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force };
                    repo.CheckoutPaths(repo.Head.Tip.Sha, new string[] { path }, options);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on checkout : " + e.Message);
            }
        }

        /// <summary>
        /// Revert to a specific commit.
        /// </summary>
        /// <param name="commitId">The commit SHA id.</param>
        public static void Revert(string commitId)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    {
                        Commands.Checkout(repo, commitId);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on revert commit : " + e.Message);
            }
        }

        /// <summary>
        /// Checkout a branch.
        /// </summary>
        /// <param name="currentBranch">The branch to checkout.</param>
        public static void CheckoutBranch(string currentBranch)
        {
            try
            {
                using (var repo = new Repository("."))
                {
                    CheckoutOptions options = new CheckoutOptions();
                    options.CheckoutModifiers = CheckoutModifiers.Force;
                    Commands.Checkout(repo, currentBranch, options);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on checkout branch : " + e.Message);
            }
        }
        #endregion

        /// <summary>
        /// Stage a file or folder.
        /// </summary>
        /// <param name="path">The path to the file or folder</param>
        public static void Stage(string path)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    Commands.Stage(repo, path);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on stage : " + e.Message);
            }
        }

        /// <summary>
        /// Commit the stagged files.
        /// </summary>
        /// <param name="username">The user username.</param>
        /// <param name="email">The user email.</param>
        /// <param name="commitMessage">The commit message</param>
        public static void Commit(string username, string email, string commitMessage)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    Signature author = new Signature(username, email, DateTime.Now);
                    Signature committer = author;

                    Commit commit = repo.Commit(commitMessage, author, committer);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on commit : " + e.Message);
            }
        }

        /// <summary>
        /// Push the commits to the server.
        /// </summary>
        /// <param name="username">The user username.</param>
        /// <param name="password">The user password.</param>
        /// <param name="currentBranch">The current branch.</param>
        public static void Push(string username, string password, string currentBranch)
        {
            using (var repo = new Repository(repoPath))
            {
                try
                {
                    var options = new PushOptions();
                    options.CredentialsProvider = (_url, _user, _cred) =>
                        new UsernamePasswordCredentials { Username = username, Password = password };
                    repo.Network.Push(repo.Branches[currentBranch], options);
                }
                catch (Exception e)
                {
                    Debug.Log("Error on push : " + e.Message);
                }
            }
        }

        /// <summary>
        /// Pull commits from the server.
        /// </summary>
        /// <param name="username">The user username.</param>
        /// <param name="email">The user email.</param>
        /// <param name="password">The user password.</param>
        public static void Pull(string username, string email, string password)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    PullOptions options = new PullOptions();
                    options.FetchOptions = new FetchOptions();
                    options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials()
                            {
                                Username = username,
                                Password = password
                            });
                    Commands.Pull(repo, new Signature(username, email, new DateTimeOffset(DateTime.Now)), options);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on pull : " + e.Message);
            }
        }

        /// <summary>
        /// Merge branch into current branch.
        /// </summary>
        /// <param name="username">The user username.</param>
        /// <param name="email">The user email.</param>
        /// <param name="branchName">The branch to merge from.</param>
        public static void Merge(string username, string email, string branchName)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    repo.Merge(repo.Branches[branchName], new Signature(username, email, new DateTimeOffset(DateTime.Now)));
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on pull : " + e.Message);
            }
        }

        /// <summary>
        /// Removes a file or folder from the repository.
        /// </summary>
        /// <param name="path">Path to the file or folder.</param>
        public static void Remove(string path)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    Commands.Remove(repo, path, removeFromWorkingDirectory: false);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error on remove : " + e.Message);
            }
        }

        /// <summary>
        /// Set the remote url.
        /// </summary>
        /// <param name="repositoryUrl">The new url.</param>
        /// <param name="remoteName">The remote name.</param>
        public static void SetRemoteUrl(string repositoryUrl, string remoteName)
        {
            using (var repo = new Repository(repoPath))
            {
                var newUrl = repositoryUrl;
                Remote remote = repo.Network.Remotes[remoteName];
                repo.Network.Remotes.Update(remote.Name, r => r.Url = newUrl);
            }
        }
    }
}
