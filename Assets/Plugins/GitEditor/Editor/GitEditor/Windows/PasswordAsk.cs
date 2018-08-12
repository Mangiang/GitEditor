using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The namespace used for windows.
/// </summary>
namespace GitEditor.Windows
{
    /// <summary>
    /// Window used to ask the user his password.
    /// </summary>
    public class PasswordAsk : EditorWindow
    {
        /// <summary>
        /// The user password.
        /// </summary>
        [SerializeField] public string password;
        /// <summary>
        /// The callback to execute after pushing the Push button.
        /// </summary>
        public Action<string> callback = null;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Password : ");
            password = EditorGUILayout.PasswordField(password);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Push"))
            {
                if (callback != null)
                {
                    callback(password);
                    Close();
                }
            }
        }
    } 
}