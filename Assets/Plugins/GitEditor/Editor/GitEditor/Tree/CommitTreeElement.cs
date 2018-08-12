using System;
using UnityEditor.TreeViewExamples;

/// <summary>
/// The namespace used for commit tree relative classes.
/// </summary>
namespace GitEditor.Tree
{
    /// <summary>
    /// The TreeElement that holds the datas.
    /// </summary>
    [Serializable]
    public class CommitTreeElement : TreeElement
    {
        /// <summary>
        /// Is the element enabled ? (checked)
        /// </summary>
        public bool enabled = true;
        /// <summary>
        /// Are the element children mixed ?
        /// </summary>
        public bool isMixed = false;
        /// <summary>
        /// Have the values been programatically set ?
        /// </summary>
        public bool valueSet = false;
        /// <summary>
        /// The full path to the file or folder represented by the element.
        /// </summary>
        public string fullPath;
        /// <summary>
        /// Lines added since last commit in case of a file
        /// </summary>
        public int addedLines = 0;
        /// <summary>
        /// Lines removed since last commit in case of a file
        /// </summary>
        public int removedLines = 0;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="depth">The depth of the element.</param>
        /// <param name="id">The id of the element.</param>
        /// <param name="fullPath">The full path of the element.</param>
        public CommitTreeElement(string name, int depth, int id, string fullPath = "") : base(name, depth, id)
        {
            this.name = name;
            enabled = true;
            this.fullPath = fullPath;
        }

        /// <summary>
        /// Set the element enable or disable and propagate it to its children.
        /// </summary>
        /// <param name="value">True for enable, False for disable.</param>
        public void SetEnable(bool value)
        {
            valueSet = true;

            if (isMixed)
            {
                enabled = true;
            }
            else
            {
                enabled = value;
            }

            if (hasChildren)
            {
                foreach (CommitTreeElement child in children)
                {
                    child.SetEnableTopDown(value);
                }
            }
        }

        /// <summary>
        /// The recursive helper of SetEnable
        /// </summary>
        /// <param name="value">True for enable, False for disable.</param>
        public void SetEnableTopDown(bool value)
        {
            valueSet = true;
            enabled = value;
            if (hasChildren)
            {
                foreach (CommitTreeElement child in children)
                {
                    child.SetEnableTopDown(value);
                }
            }
        }

        /// <summary>
        /// Check if children are enabled or disabled.
        /// </summary>
        /// <returns>True is the element must have a mixed state, False otherwise.</returns>
        public bool checkChildrenState()
        {
            if (hasChildren && children.Count == 1)
            {
                if (children.Count == 1)
                {
                    CommitTreeElement elt = ((CommitTreeElement)children[0]);
                    if (!elt.enabled)
                    {
                        enabled = false;
                        return false;
                    }
                    else
                    {
                        enabled = true;
                    }

                    if (elt.isMixed)
                    {
                        enabled = true;
                        return true;
                    }
                    else
                        return false;
                }
            }
            else if (!hasChildren)
                return false;

            int childCount = children.Count;
            int enabledCount = children.FindAll(x => ((CommitTreeElement)x).enabled).Count;
            int disabledCount = children.FindAll(x => !((CommitTreeElement)x).enabled).Count;

            if (enabledCount == childCount)
            {
                enabled = true;
                return false;
            }

            if (disabledCount == childCount)
            {
                enabled = false;
                return false;
            }

            if (enabledCount > 0)
            {
                enabled = true;
                return true;
            }
            else
                return false;
        }
    }

}