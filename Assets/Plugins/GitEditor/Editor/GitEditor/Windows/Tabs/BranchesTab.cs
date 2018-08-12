using GitEditor.DataAccess;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The namespace used for windows tabs.
/// </summary>
namespace GitEditor.Windows.Tabs
{
    /// <summary>
    /// Manages the display of the GitEditor Branches tab.
    /// </summary>
    public class BranchesTab : DisplayableTab
    {
        /// <summary>
        /// The list of all branches.
        /// </summary>
        [SerializeField]
        private List<string> branches;
        /// <summary>
        /// The name of the current branch.
        /// </summary>
        [SerializeField]
        private string currentBranch;
        /// <summary>
        /// The current branch index.
        /// </summary>
        [SerializeField]
        private int currentBranchIndex;
        /// <summary>
        /// The merge from branch index.
        /// </summary>
        [SerializeField]
        private int fromBranchIndex;
        /// <summary>
        /// The merge to branch index.
        /// </summary>
        [SerializeField]
        private int toBranchIndex;
        /// <summary>
        /// The branch list without the from branch.
        /// </summary>
        [SerializeField]
        List<string> toBranchList;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="name">The tab name</param>
        /// <param name="editorWindow">The editorWindow</param>
        public BranchesTab(string name, GitEditor editorWindow) : base(name, editorWindow)
        {
            branches = new List<string>();
            currentBranch = "";
            currentBranchIndex = 0;
            RepositoryManager.GetBranches(ref branches, ref currentBranch, ref currentBranchIndex);
            toBranchList = new List<string>(branches);
            toBranchList.Remove(branches[fromBranchIndex]);
        }

        public override void Display()
        {
            GUILayout.Label("GIT BRANCH", EditorStyles.boldLabel);

            int newBranchIndex = EditorGUILayout.Popup(currentBranchIndex, branches.ToArray());
            if (newBranchIndex != currentBranchIndex)
            {
                RepositoryManager.CheckoutBranch(branches[newBranchIndex]);
                currentBranchIndex = newBranchIndex;
            }

            WindowHelper.DrawUILine(Color.black, width: editorWindow.position.width - 15);

            #region //// Merge
            EditorGUILayout.LabelField("Merge");
            GUILayout.BeginHorizontal();
            int newFromBranchIndex = EditorGUILayout.Popup(fromBranchIndex, branches.ToArray());
            if (newFromBranchIndex != fromBranchIndex)
            {
                toBranchList = new List<string>(branches); 
                toBranchList.Remove(branches[newFromBranchIndex]);
                fromBranchIndex = newFromBranchIndex;
            }

            EditorGUILayout.LabelField(" => ", GUILayout.Width(60));
            int newToBranchIndex = EditorGUILayout.Popup(toBranchIndex, toBranchList.ToArray());

            if (newToBranchIndex < toBranchList.Count)
            {
                toBranchIndex = newToBranchIndex;
            }
            else
            {
                toBranchIndex = toBranchList.Count - 1;
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Merge", GUILayout.Width(editorWindow.position.width - 15)))
            {
                SettingsTab settingsTab = editorWindow.GetSettingsTab();
                RepositoryManager.CheckoutBranch(toBranchList[toBranchIndex]);
                RepositoryManager.Merge(settingsTab.GetUserUsername(), settingsTab.GetUserEmail(), branches[fromBranchIndex]);
                editorWindow.GetHistoryTab().RefreshHistory();
            }
            #endregion
        }

        #region ///// Getters/Setters ////
        /// <summary>
        /// Get the current branch name.
        /// </summary>
        /// <returns>The current branch name</returns>
        public string GetCurrentBranchName()
        {
            return currentBranch;
        }
        #endregion
    }
}
