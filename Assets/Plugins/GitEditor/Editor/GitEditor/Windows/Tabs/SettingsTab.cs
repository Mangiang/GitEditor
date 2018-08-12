using GitEditor.DataAccess;
using GitEditor.Model;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The namespace used for windows tabs.
/// </summary>
namespace GitEditor.Windows.Tabs
{
    /// <summary>
    /// Manages the display of the GitEditor Settings tab.
    /// </summary>
    public class SettingsTab : DisplayableTab
    {
        /// <summary>
        /// The settings xml file path.
        /// </summary>
        [SerializeField]
        private const string settingsFilePath = "./Assets/Plugins/GitEditor/.gitEditorSettings.xml";
        /// <summary>
        /// The Settings.
        /// </summary>
        [SerializeField]
        private Settings settings;
        /// <summary>
        /// The origin remote url.
        /// </summary>
        [SerializeField]
        private string originUrl;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="name">The tab name</param>
        /// <param name="editorWindow">The editorWindow</param>
        public SettingsTab(string name, GitEditor editorWindow) : base(name, editorWindow)
        {
            originUrl = RepositoryManager.GetRepositoryUrl();
            settings = XMLManager.ReadXML<Settings>(settingsFilePath);
        }

        public override void Display()
        {
            GUILayout.Label("GIT SETTINGS", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Username : ", GUILayout.Width(100));
            settings.credentials.username = EditorGUILayout.TextField(settings.credentials.username, GUILayout.Width(500));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Email : ", GUILayout.Width(100));
            settings.credentials.email = EditorGUILayout.TextField(settings.credentials.email, GUILayout.Width(500));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Save credentials"))
            {
                XMLManager.WriteXML(settingsFilePath, settings);
            }

            WindowHelper.DrawUILine(Color.black, width: editorWindow.position.width - 15);

            GUILayout.BeginHorizontal();
            GUILayout.Label("origin url : ", GUILayout.Width(100));
            originUrl = EditorGUILayout.TextField(originUrl, GUILayout.Width(500));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Save url"))
            {
                RepositoryManager.SetRemoteUrl(originUrl, "origin");
            }
        }

        /// <summary>
        /// Save the new credentials.
        /// Used by the CredentialsAsk window.
        /// </summary>
        /// <param name="newUsername">The new username.</param>
        /// <param name="newEmail">The new email.</param>
        public void SaveNewCredentials(string newUsername, string newEmail)
        {
            settings.credentials.username = newUsername;
            settings.credentials.email = newEmail;
            XMLManager.WriteXML(settingsFilePath, settings);
        }

        #region /////// Getters/Setters ////////
        /// <summary>
        /// Get the settings file path.
        /// </summary>
        /// <returns>The file path.</returns>
        public string GetSettingsFilePath()
        {
            return settingsFilePath;
        }
        /// <summary>
        /// Get the user username.
        /// </summary>
        /// <returns>The user username</returns>
        public string GetUserUsername()
        {
            return settings.credentials.username;
        }
        /// <summary>
        /// Get the user email.
        /// </summary>
        /// <returns>The user email.</returns>
        public string GetUserEmail()
        {
            return settings.credentials.email;
        }
        #endregion
    }
}
