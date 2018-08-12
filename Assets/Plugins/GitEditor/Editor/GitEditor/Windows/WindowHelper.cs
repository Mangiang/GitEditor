using GitEditor.Tree;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The namespace used for windows.
/// </summary>
namespace GitEditor.Windows
{
    /// <summary>
    /// Class holding helper methods.
    /// </summary>
    public class WindowHelper
    {
        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">The line color.</param>
        /// <param name="thickness">The line thickness.</param>
        /// <param name="padding">The line padding.</param>
        /// <param name="width">The line width.</param>
        /// <param name="xOffset">The line left offset.</param>
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10, float width = 10, int xOffset = 5)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness), GUILayout.Width(width));
            r.height = thickness;
            r.y += padding / 2;
            r.x += xOffset - 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        /// <summary>
        /// Get current unstaged and stagged changes.
        /// </summary>
        /// <param name="treeAsset">The tree asset.</param>
        /// <returns>The list of changes.</returns>
        public static IList<CommitTreeElement> GetData(ref CommitTreeAsset treeAsset)
        {
            if (treeAsset != null && treeAsset.TreeElements != null && treeAsset.TreeElements.Count > 0)
                return treeAsset.TreeElements;

            // generate some test data
            return CommitTreeParser.GenerateCommitTree();
        }

        /// <summary>
        /// Recurcively get the elements full path list.
        /// Used to commit from the tree elements.
        /// </summary>
        /// <param name="list">The list of full paths.</param>
        /// <param name="root">The tree root</param>
        public static void GetElements(ref List<string> list, CommitTreeElement root)
        {
            if (!root.hasChildren)
            {
                if (!String.IsNullOrEmpty(root.fullPath) && root.enabled)
                    list.Add(root.fullPath);
                return;
            }

            foreach (CommitTreeElement elt in root.children)
            {
                if (elt.enabled)
                    GetElements(ref list, elt);
            }
        }
    }
}
