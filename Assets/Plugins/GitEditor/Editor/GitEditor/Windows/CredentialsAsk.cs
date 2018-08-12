using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The namespace used for windows.
/// </summary>
namespace GitEditor.Windows
{
    /// <summary>
    /// Window used to ask the user his credentials informations if no settings were saved.
    /// </summary>
    public class CredentialsAsk : EditorWindow
    {
        /// <summary>
        /// The user username.
        /// </summary>
        [SerializeField] public string username;
        /// <summary>
        /// The user email.
        /// </summary>
        [SerializeField] public string email;
        /// <summary>
        /// The callback to excecute after pushing the Save button.
        /// </summary>
        public Action<string, string> callback = null;

        private void OnEnable()
        {
            minSize = new Vector2(500, 50);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Username : ", GUILayout.Width(100));
            username = EditorGUILayout.TextField(username);
            EditorGUILayout.LabelField("Email : ", GUILayout.Width(100));
            email = EditorGUILayout.TextField(email);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save"))
            {
                if (callback != null)
                {
                    callback(username, email);
                    Close();
                }
            }
        }
    } 
}