using GitEditor.DataAccess;
using GitEditor.Tree;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

/// <summary>
/// The namespace used for windows tabs.
/// </summary>
namespace GitEditor.Windows.Tabs
{
    /// <summary>
    /// Manages the display of the GitEditor Commit tab.
    /// </summary>
    public class CommitTab : DisplayableTab
    {
        /// <summary>
        /// The TreeViewState.
        /// </summary>
        [SerializeField]
        private TreeViewState treeViewState;
        /// <summary>
        /// The MultiColumnHeaderState.
        /// </summary>
        [SerializeField]
        private MultiColumnHeaderState multiColumnHeaderState;
        /// <summary>
        /// The current commit message.
        /// </summary>
        [SerializeField]
        private string currentCommitMessage;
        /// <summary>
        /// The SHA id of the current commit.
        /// </summary>
        [SerializeField]
        private string currentCommitID = "";
        /// <summary>
        /// The CommitTreeView.
        /// </summary>
        private CommitTreeView commitTreeView;
        /// <summary>
        /// The CommitTreeAsset.
        /// </summary>
        private CommitTreeAsset treeAsset;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="name">The tab name</param>
        /// <param name="editorWindow">The editorWindow</param>
        public CommitTab(string name, GitEditor editorWindow) : base(name, editorWindow)
        {
            #region /////// Commit tree initialization ////////
            if (treeViewState == null)
                treeViewState = new TreeViewState();

            bool firstInit = multiColumnHeaderState == null;
            var headerState = CommitTreeView.CreateDefaultMultiColumnHeaderState(editorWindow.position.width - 5);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(multiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(multiColumnHeaderState, headerState);
            multiColumnHeaderState = headerState;

            var multiColumnHeader = new MultiColumnHeader(headerState);
            if (firstInit)
                multiColumnHeader.ResizeToFit();

            var treeModel = new TreeModel<CommitTreeElement>(WindowHelper.GetData(ref treeAsset));
            #endregion

            commitTreeView = new CommitTreeView(treeViewState, multiColumnHeader, treeModel);
            currentCommitID = RepositoryManager.GetCurrentCommitSha();
        }

        public override void Display()
        {
            GUILayout.Label("GIT COMMIT", EditorStyles.boldLabel);

            #region ///// Header
            float groupWidth = editorWindow.position.width - 5;
            GUI.BeginGroup(new Rect(5, 45, groupWidth, 20));
            if (GUI.Button(new Rect(0, 0, groupWidth / 3, 20), "Refresh"))
            {
                ReloadTree();
            }
            if (GUI.Button(new Rect(groupWidth / 3, 0, groupWidth / 3, 20), "Expand All"))
            {
                commitTreeView.ExpandAll();
            }
            if (GUI.Button(new Rect(groupWidth * 2 / 3, 0, groupWidth / 3, 20), "Collapse All"))
            {
                commitTreeView.CollapseAll();
            }
            GUI.EndGroup();
            #endregion

            commitTreeView.OnGUI(new Rect(5, 70, editorWindow.position.width - 10, 300));

            currentCommitMessage = EditorGUI.TextArea(
                new Rect(5, 375, editorWindow.position.width - 5, 90),
                currentCommitMessage);

            if (GUI.Button(new Rect(5, 475, editorWindow.position.width - 10, 20), "Commit"))
            {
                SettingsTab settingsTab = editorWindow.GetSettingsTab();
                if (String.IsNullOrEmpty(settingsTab.GetUserUsername()) || String.IsNullOrEmpty(settingsTab.GetUserEmail()))
                {
                    editorWindow.Close();
                    editorWindow.credentialsWindow.callback = (username, email) =>
                    {
                        settingsTab.SaveNewCredentials(username, email);
                        Commit();
                        GitEditor.GetWindow().Show();
                    };
                    editorWindow.credentialsWindow.Show(true);
                }
                else
                {
                    Commit();
                }

            }

            if (GUI.Button(new Rect(5, 500, editorWindow.position.width - 10, 20), string.Format("{0} Push", editorWindow.GetHistoryTab().GetAheadBy())))
            {
                editorWindow.Close();
                editorWindow.passwordWindow.callback = (password) =>
                {
                    RepositoryManager.Push(editorWindow.GetSettingsTab().GetUserUsername(), password, editorWindow.GetBranchesTab().GetCurrentBranchName());
                    GitEditor.GetWindow().Show();
                };
                editorWindow.passwordWindow.Show(true);
            }
        }

        /// <summary>
        /// Commit the stagged files.
        /// </summary>
        private void Commit()
        {
            CommitTreeElement root = commitTreeView.treeModel.root;

            List<string> fileList = new List<string>();

            // Get all the checked files
            WindowHelper.GetElements(ref fileList, root);

            foreach (string file in fileList)
                RepositoryManager.Stage(file);

            SettingsTab settingsTab = editorWindow.GetSettingsTab();

            RepositoryManager.Commit(settingsTab.GetUserUsername(), settingsTab.GetUserEmail(), currentCommitMessage);

            editorWindow.GetHistoryTab().RefreshHistory();
            currentCommitMessage = "";
            ReloadTree();
        }
        
        /// <summary>
        /// Reload the tree.
        /// </summary>
        public void ReloadTree()
        {
            commitTreeView.treeModel.SetData(WindowHelper.GetData(ref treeAsset));
            commitTreeView.Reload();
            commitTreeView.ExpandAll();
        }

        /// <summary>
        /// Reload the tree.
        /// </summary>
        public void Reload()
        {
            var myTreeAsset = Selection.activeObject as CommitTreeAsset;
            if (myTreeAsset != null && myTreeAsset != treeAsset)
            {
                treeAsset = myTreeAsset;
                commitTreeView.treeModel.SetData(WindowHelper.GetData(ref treeAsset));
                commitTreeView.Reload();
                commitTreeView.ExpandAll();
            }
        }

        #region ///// Getters/Setters ////
        /// <summary>
        /// Get the current commit SHA id.
        /// </summary>
        /// <returns>The current commit id.</returns>
        public string GetCurrentCommitID()
        {
            return currentCommitID;
        }

        /// <summary>
        /// Set the new tree asset.
        /// Used on GitEditor OnOpenAsset. 
        /// </summary>
        /// <param name="newTreeAsset">The new CommitTreeAsset.</param>
        public void SetNewTreeAsset(CommitTreeAsset newTreeAsset)
        {
            treeAsset = newTreeAsset;
        }
        #endregion
    }
}