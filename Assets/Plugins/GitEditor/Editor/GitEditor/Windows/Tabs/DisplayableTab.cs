using UnityEngine;

/// <summary>
/// The namespace used for windows tabs.
/// </summary>
namespace GitEditor.Windows.Tabs
{
    /// <summary>
    /// The class used to make displayable tabs for the main GitEditor window.
    /// </summary>
    public abstract class DisplayableTab
    {

        /// <summary>
        /// The tab name
        /// </summary>
        [SerializeField] protected readonly string name;
        /// <summary>
        /// The GitEditor window.
        /// </summary>
        [SerializeField]
        protected GitEditor editorWindow;
        
        /// <summary>
        /// Used to display the tab.
        /// </summary>
        public abstract void Display();

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="name">The tab name</param>
        /// <param name="editorWindow">The editorWindow</param>
        protected DisplayableTab(string name, GitEditor editorWindow)
        {
            this.name = name;
            this.editorWindow = editorWindow;
        }
        
        /// <summary>
        /// Get the tab name.
        /// </summary>
        /// <returns>The tab name</returns>
        public string GetName()
        {
            return name;
        }
    }
}
