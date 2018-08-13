using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using GitEditor.DataAccess;
using GitEditor.Tree;
using GitEditor.Windows.Tabs;
using System.Threading;

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
        [SerializeField] float initializationProgress;
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

        Thread CommitInitThread;
        Thread HistoryInitThread;
        Thread BranchesInitThread;
        Thread SettingsInitThread;

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
            isInitialized = false;

            #region /////// Tabs initialization ///////
            toolbarStrings = new List<string>();
            initializationProgress = 0;
            commitTab = new CommitTab("Commit", this);
            displays.Add(commitTab);
            toolbarStrings.Add(commitTab.GetName());
            initializationProgress += 25;

            HistoryInitThread = new Thread(InitHistory);
            BranchesInitThread = new Thread(InitBranches);
            SettingsInitThread = new Thread(InitSettings);

            HistoryInitThread.Start();
            BranchesInitThread.Start();
            SettingsInitThread.Start();
            #endregion

            passwordWindow = new PasswordAsk();
            credentialsWindow = new CredentialsAsk();

        }

        void InitHistory()
        {
            historyTab = new HistoryTab("History", this);
            displays.Add(historyTab);
            toolbarStrings.Add(historyTab.GetName());
            initializationProgress += 25;
        }
        void InitBranches()
        {
            branchesTab = new BranchesTab("Branches", this);
            displays.Add(branchesTab);
            toolbarStrings.Add(branchesTab.GetName());
            initializationProgress += 25;
        }
        void InitSettings()
        {
            settingsTab = new SettingsTab("Settings", this);
            displays.Add(settingsTab);
            toolbarStrings.Add(settingsTab.GetName());
            initializationProgress += 25;
        }

        void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                if (initializationProgress == 100)
                    isInitialized = true;
            }
            if (isInitialized)
            {
                    EditorUtility.ClearProgressBar();
                toolbarIdx = GUILayout.Toolbar(toolbarIdx, toolbarStrings.ToArray());
                displays[toolbarIdx].Display();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Initialization", "Initialization ...", initializationProgress);
            }
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