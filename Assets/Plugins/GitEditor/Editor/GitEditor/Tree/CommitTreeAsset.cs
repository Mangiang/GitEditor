using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The namespace used for commit tree relative classes.
/// </summary>
namespace GitEditor.Tree
{
    /// <summary>
    /// The CommitTreeAsset
    /// </summary>
    [CreateAssetMenu(fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
    public class CommitTreeAsset : ScriptableObject
    {
        /// <summary>
        /// The element list
        /// </summary>
        [SerializeField]
        private List<CommitTreeElement> treeElements = new List<CommitTreeElement>();

        /// <summary>
        /// Getter/setters
        /// </summary>
        internal List<CommitTreeElement> TreeElements
        {
            get { return treeElements; }
            set { treeElements = value; }
        }

        void Awake()
        {
            if (treeElements.Count == 0)
                treeElements = CommitTreeParser.GenerateCommitTree();
        }

        /// <summary>
        /// Reload the tree
        /// </summary>
        public void Reload()
        {
            treeElements = CommitTreeParser.GenerateCommitTree();
        }
    } 
}