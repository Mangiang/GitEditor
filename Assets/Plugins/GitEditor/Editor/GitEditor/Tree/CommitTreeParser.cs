using GitEditor.DataAccess;
using GitEditor.Model;
using LibGit2Sharp;
using System;
using System.Collections.Generic;

/// <summary>
/// The namespace used for commit tree relative classes.
/// </summary>
namespace GitEditor.Tree
{
    /// <summary>
    /// The tree that displays the commits
    /// </summary>
    public class CommitTreeParser
    {
        /// <summary>
        /// The element id counter.
        /// </summary>
        static int IDCounter;

        /// <summary>
        /// Generates the commit tree.
        /// </summary>
        /// <returns>The list of elements.</returns>
        public static List<CommitTreeElement> GenerateCommitTree()
        {
            IDCounter = 0;
            TreeChanges changes = RepositoryManager.GetChanges();
            Folder rootFolder = new Folder();
            foreach (TreeEntryChanges c in changes)
            {
                #region ////// Build a tree using File and Folder models
                string[] pathSplit = c.Path.Split(new[] { "\\", "/" }, StringSplitOptions.None);
                if (pathSplit.Length == 1)
                {
                    File endFile = new File()
                    {
                        name = pathSplit[0],
                        fullPath = pathSplit[0]
                    };

                    rootFolder.filesChildren.Add(endFile);
                    continue;
                }

                Folder parent = rootFolder;
                for (int i = 0; i < pathSplit.Length; ++i)
                {
                    if (i == pathSplit.Length - 1)
                    {
                        File endFile = new File()
                        {
                            name = pathSplit[i],
                            fullPath = c.Path
                        };

                        parent.filesChildren.Add(endFile);
                        break;
                    }
                    else
                    {
                        Folder currentFolder = parent.folderChildren.Find(x => x.name == pathSplit[i]);
                        if (currentFolder == null)
                        {
                            currentFolder = new Folder()
                            {
                                name = pathSplit[i]
                            };
                            parent.folderChildren.Add(currentFolder);
                        }
                        parent = currentFolder;
                    }
                }
                #endregion
            }

            // Use the File and Folder tree to create the CommitTree
            var root = new CommitTreeElement("Root", -1, IDCounter);
            var treeElements = new List<CommitTreeElement>();
            treeElements.Add(root);
            AddChildren(ref treeElements, 0, rootFolder);
            return treeElements;
        }

        /// <summary>
        /// Add children to CommitTreeElement.
        /// </summary>
        /// <param name="treeElements">The element list.</param>
        /// <param name="depth">The depth of the element.</param>
        /// <param name="parent">The parent</param>
        public static void AddChildren(ref List<CommitTreeElement> treeElements, int depth, Folder parent)
        {
            foreach (File file in parent.filesChildren)
            {
                var child = new CommitTreeElement(file.name, depth, ++IDCounter, file.fullPath);
                Patch patch = RepositoryManager.GetPatch(new List<string>() { file.fullPath });
                child.addedLines = patch.LinesAdded;
                child.addedLines = patch.LinesDeleted;
                treeElements.Add(child);
            }

            foreach (Folder folder in parent.folderChildren)
            {
                var child = new CommitTreeElement(folder.name, depth, ++IDCounter);
                treeElements.Add(child);
                AddChildren(ref treeElements, depth + 1, folder);
            }
        }
    }
}