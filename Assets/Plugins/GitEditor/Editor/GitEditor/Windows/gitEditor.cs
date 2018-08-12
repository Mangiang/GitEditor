using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using GitEditor.DataAccess;
using GitEditor.Tree;
using GitEditor.Windows.Tabs;

/// <summary>
/// The namespace used for windows.
/// </summary>
namespace GitEditor.Windows
{
    /// <summary>
    /// The main window.
    /// </summary>
    public class GitEditor : EditorWindow
    {
        [MenuItem("Window/Git Editor")]
        public static GitEditor GetWindow()
        {
            var window = GetWindow<GitEditor>();
            window.titleContent = new GUIContent("Git Editor");
            window.Focus();
            window.Repaint();
            return window;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            CommitTreeAsset newTreeAsset = EditorUtility.InstanceIDToObject(instanceID) as CommitTreeAsset;
            if (newTreeAsset != null)
            {
                var window = GetWindow();
                window.GetCommitTab().SetNewTreeAsset(newTreeAsset);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is the window initialized ?
        /// </summary>
        [SerializeField] bool isInitialized;

        /// <summary>
        /// The password asking window.
        /// </summary>
        public PasswordAsk passwordWindow;
        /// <summary>
        /// The credentials asking window.
        /// </summary>
        public CredentialsAsk credentialsWindow;

        /// <summary>
        /// The current tab index.
        /// </summary>
        public int toolbarIdx = 0;
        /// <summary>
        /// The tab labels.
        /// </summary>
        public List<string> toolbarStrings;
        /// <summary>
        /// The display methods for each tab.
        /// </summary>
        public List<DisplayableTab> displays = new List<DisplayableTab>();

        #region Tabs
        /// <summary>
        /// The SettingsTab.
        /// </summary>
        private SettingsTab settingsTab;
        /// <summary>
        /// The BranchesTab.
        /// </summary>
        private BranchesTab branchesTab;
        /// <summary>
        /// The HistoryTab.
        /// </summary>
        private HistoryTab historyTab;
        /// <summary>
        /// The CommitTab.
        /// </summary>
        private CommitTab commitTab;
        #endregion

        void OnEnable()
        {
            RepositoryManager.Init(".");

            #region /////// Tabs initialization ///////
            toolbarStrings = new List<string>();
            commitTab = new CommitTab("Commit", this);
            historyTab = new HistoryTab("History", this);
            branchesTab = new BranchesTab("Branches", this);
            settingsTab = new SettingsTab("Settings", this);

            displays.Add(commitTab);
            displays.Add(historyTab);
            displays.Add(branchesTab);
            displays.Add(settingsTab);

            displays.ForEach((DisplayableTab tab) => {
                toolbarStrings.Add(tab.GetName());
            });
            #endregion

            passwordWindow = new PasswordAsk();
            credentialsWindow = new CredentialsAsk();

        }

        void OnGUI()
        {
            toolbarIdx = GUILayout.Toolbar(toolbarIdx, toolbarStrings.ToArray());
            displays[toolbarIdx].Display();
        }

        #region ///// Getters/Setters ////
        public CommitTab GetCommitTab()
        {
            return commitTab;
        }
        public HistoryTab GetHistoryTab()
        {
            return historyTab;
        }
        public BranchesTab GetBranchesTab()
        {
            return branchesTab;
        }
        public SettingsTab GetSettingsTab()
        {
            return settingsTab;
        }
        #endregion

        void OnSelectionChange()
        {
            commitTab.Reload();
        }
    }
}