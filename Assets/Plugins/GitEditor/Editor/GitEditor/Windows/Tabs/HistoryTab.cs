using GitEditor.DataAccess;
using GitEditor.Model;
using LibGit2Sharp;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The namespace used for windows tabs.
/// </summary>
namespace GitEditor.Windows.Tabs
{
    /// <summary>
    /// Manages the display of the GitEditor History tab.
    /// </summary>
    public class HistoryTab : DisplayableTab
    {
        /// <summary>
        /// The list of HistoryCommit.
        /// </summary>
        [SerializeField] private List<HistoryCommit> historyCommits;
        /// <summary>
        /// Is the current local HEAD ahead of remote ?
        /// </summary>
        [SerializeField] private string aheadBy;
        /// <summary>
        /// Is the current local HEAD behind remote ?
        /// </summary>
        [SerializeField] private string behindBy;
        /// <summary>
        /// The history scroll position.
        /// </summary>
        [SerializeField] private Vector2 historyScrollPos;
        /// <summary>
        /// The current history page.
        /// </summary>
        [SerializeField] private int currentPage;
        /// <summary>
        /// The maximum history page.
        /// </summary>
        [SerializeField] private int maxPage;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="name">The tab name</param>
        /// <param name="editorWindow">The editorWindow</param>
        public HistoryTab(string name, GitEditor editorWindow) : base(name, editorWindow)
        {
            historyCommits = new List<HistoryCommit>();
            this.aheadBy = "";
            this.behindBy = "";
            historyScrollPos = new Vector2();
            RefreshHistory();

            #region //////// Ahead and behind labels initialization ///////
            int? aheadBy = RepositoryManager.GetRepositoryAhead();
            int? behindBy = RepositoryManager.GetRepositoryBehind();

            if (aheadBy != null)
            {
                this.aheadBy = string.Format("(Ahead by {0} commits)", aheadBy);
            }
            if (behindBy != null)
            {
                this.behindBy = string.Format("(Behind by {0} commits)", behindBy);
            }
            #endregion
        }

        public override void Display()
        {
            GUILayout.Label("GIT HISTORY", EditorStyles.boldLabel);

            #region ///// Header
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                RefreshHistory();
            }
            if (GUILayout.Button(string.Format("{0} Pull", behindBy)))
            {
                editorWindow.Close();
                editorWindow.passwordWindow.callback = (password) =>
                {
                    SettingsTab settingsTab = editorWindow.GetSettingsTab();
                    RepositoryManager.Pull(settingsTab.GetUserUsername(), settingsTab.GetUserEmail(), password);
                    GitEditor.GetWindow().Show();
                };
                editorWindow.passwordWindow.Show(true);
            }
            if (GUILayout.Button("Checkout current branch"))
            {
                RepositoryManager.CheckoutBranch(editorWindow.GetBranchesTab().GetCurrentBranchName());
            }
            GUILayout.EndHorizontal();
            #endregion


            /// Compute the total height of the scroll view
            float height = 0;
            historyCommits.ForEach((elt) =>
            {
                // GUI Button, rule, space heght + status, id, message, changes height
                height += 70 + (elt.changes.Count + 2 + (elt.onlyLocal || elt.onlyRemote ? 1 : 0)) * EditorGUIUtility.singleLineHeight * 1.5f;
            });

            historyScrollPos = GUILayout.BeginScrollView(historyScrollPos, false, true);
            #region ////// Display the scroll view
            GUI.BeginGroup(new Rect(0, 5, editorWindow.position.width - 15, height));
            string currentCommitID = editorWindow.GetCommitTab().GetCurrentCommitID();
            int historyCommitsCount = historyCommits.Count;
            for (int commitIdx = currentPage*10; commitIdx < (currentPage +1)*10 && commitIdx < historyCommitsCount; commitIdx++)
            {
                HistoryCommit com = historyCommits[commitIdx];
                if (com.onlyLocal)
                {
                    GUI.backgroundColor = Color.green;
                }
                else if (com.onlyRemote)
                {
                    GUI.backgroundColor = Color.red;
                }
                else if (com.id == currentCommitID)
                {
                    GUI.backgroundColor = Color.blue;
                }

                #region ////// Display Box
                GUILayout.BeginVertical(new GUIStyle("Box"));

                #region ////// Display status

                if (com.onlyLocal || com.onlyRemote)
                {
                    string status = "";
                    if (com.onlyLocal)
                    {
                        status = "Ready to be pushed";
                    }
                    else if (com.onlyRemote)
                    {
                        status = "Ready to be pulled";
                    }
                    else if (com.id == currentCommitID)
                    {
                        status = "Current deteched head";
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Status : ", EditorStyles.boldLabel, GUILayout.Width(60));
                    GUILayout.Label(status, EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                }
                #endregion

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("ID : ", GUILayout.Width(60));
                GUILayout.Label(com.id);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Author : ", GUILayout.Width(60));
                GUILayout.Label(com.author);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Message : ", GUILayout.Width(60));
                GUILayout.Label(com.message);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                foreach (string change in com.changes)
                    GUILayout.Label(change);

                if (!com.onlyLocal && !com.onlyRemote)
                {
                    if (GUILayout.Button("Revert to", GUILayout.Width(editorWindow.position.width - 30)))
                    {
                        RepositoryManager.Revert(com.id);
                    }
                }

                GUILayout.EndVertical();
                #endregion

                GUI.backgroundColor = Color.white;
            }
            GUI.EndGroup();
            #endregion
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Prev page"))
            {
                if (currentPage > 0)
                    currentPage--;
            }
            GUILayout.Label(string.Format("{0}/{1}", currentPage + 1, maxPage));
            if (GUILayout.Button("Next page"))
            {
                if (currentPage < maxPage - 1)
                    currentPage++;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Refresh the history list
        /// </summary>
        public void RefreshHistory()
        {
            historyCommits.Clear();

            int? AheadByTmp = RepositoryManager.GetRepositoryAhead();
            int? BehindByTmp = RepositoryManager.GetRepositoryBehind();

            if (AheadByTmp != null)
            {
                aheadBy = string.Format("(Ahead by {0} commits)", AheadByTmp);
            }
            if (BehindByTmp != null)
            {
                behindBy = string.Format("(Behind by {0} commits)", BehindByTmp);
            }

            List<CommitInfos> commitInfos = new List<CommitInfos>();
            RepositoryManager.GetCommits(ref commitInfos);

            foreach (CommitInfos info in commitInfos)
            {
                HistoryCommit histoCommit = new HistoryCommit
                {
                    id = info.sha,
                    message = info.messageShort
                };

                foreach (TreeEntryChanges change in info.changesFromParent)
                {
                    histoCommit.changes.Add(string.Format("{0} : {1}", change.Status, change.Path));
                }
                histoCommit.author = string.Format("{0} : {1}", info.authorName, info.authorEmail);

                if (AheadByTmp != null && AheadByTmp > 0)
                {
                    histoCommit.onlyLocal = true;
                    AheadByTmp--;
                }
                else if (BehindByTmp != null && BehindByTmp > 0)
                {
                    histoCommit.onlyRemote = true;
                    BehindByTmp--;
                }

                historyCommits.Add(histoCommit);
            }

            currentPage = 0;
            maxPage = Mathf.CeilToInt(historyCommits.Count / 10);
        }

        #region ///// Getters/Setters ////
        /// <summary>
        /// Get the string aheadBy
        /// </summary>
        /// <returns>The string aheadBy</returns>
        public string GetAheadBy()
        {
            return aheadBy;
        }
        /// <summary>
        /// Get the string behindBy
        /// </summary>
        /// <returns>The string behindBy</returns>
        public string GetBehindBy()
        {
            return behindBy;
        }
        #endregion
    }
}