using GitEditor.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

/// <summary>
/// The namespace used for commit tree relative classes.
/// </summary>
namespace GitEditor.Tree
{
    /// <summary>
    /// The tree that displays the commits
    /// </summary>
    class CommitTreeView : TreeViewWithTreeModel<CommitTreeElement>
    {
        /// <summary>
        /// The row height.
        /// </summary>
        const float rowHeights = 20f;
        /// <summary>
        /// The toggle width
        /// </summary>
        const float toggleWidth = 18f;

        enum CommitColumns
        {
            Name,
            Add,
            Remove
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="state">The TreeViewState.</param>
        /// <param name="multicolumnHeader">The MultiColumnHeader.</param>
        /// <param name="model">The TreeModel.</param>
        public CommitTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<CommitTreeElement> model) : base(state, multicolumnHeader, model)
        {
            // Custom setup
            rowHeight = rowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (rowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = toggleWidth;

            Reload();
        }

        /// <summary>
        /// Build the rows from the root.
        /// </summary>
        /// <param name="root">The root TreeViewItem.</param>
        /// <returns></returns>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        /// <summary>
        /// Display the rows.
        /// </summary>
        /// <param name="args">The row.</param>
        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<CommitTreeElement>)args.item;
            int visibleColumnsCount = args.GetNumVisibleColumns();

            for (int i = 0; i < visibleColumnsCount; ++i)
            {
                CellGUI(args.GetCellRect(i), item, (CommitColumns)args.GetColumn(i), ref args);
            }
        }

        /// <summary>
        /// Display the cells.
        /// </summary>
        /// <param name="cellRect">The cell Rect.</param>
        /// <param name="item">The TreeViewItem to be displayed.</param>
        /// <param name="column">The column.</param>
        /// <param name="args">The row.</param>
        void CellGUI(Rect cellRect, TreeViewItem<CommitTreeElement> item, CommitColumns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case CommitColumns.Add:
                    {
                        if (!item.hasChildren)
                        {
                            GUI.Label(cellRect, string.Format("+++++ {0}", item.data.addedLines));
                        }
                    }
                    break;
                case CommitColumns.Remove:
                    {
                        if (!item.hasChildren)
                        {
                            GUI.Label(cellRect, string.Format("----- {0}", item.data.removedLines));
                        }
                    }
                    break;
                case CommitColumns.Name:
                    {
                        // Do toggle
                        Rect toggleRect = cellRect;
                        toggleRect.x += GetContentIndent(item);
                        toggleRect.width = toggleWidth;

                        // If children elements are in mixed configuration.
                        if (item.data.checkChildrenState())
                            item.data.isMixed = true;
                        else
                            item.data.isMixed = false;

                        if (item.data.isMixed)
                            EditorGUI.showMixedValue = true;

                        if (toggleRect.xMax < cellRect.xMax)
                        {
                            bool newItemValue = EditorGUI.Toggle(toggleRect, item.data.enabled);
                            // Apply user check value if not mixed and different than the previous frame.
                            if (!item.data.isMixed && newItemValue != item.data.enabled && !item.data.valueSet)
                            {
                                item.data.SetEnable(newItemValue);
                            }
                            else
                            {
                                item.data.valueSet = false;
                            }
                        }

                        if (item.data.isMixed)
                        {
                            EditorGUI.showMixedValue = false;
                        }

                        // Default icon and label
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                    }
                    break;
            }
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Add"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Remove"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 150,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                }
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(CommitColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            return state;
        }

        /// <summary>
        /// Called on item right click.
        /// </summary>
        /// <param name="id">The item id.</param>
        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);

            CommitTreeElement elt = treeModel.Find(id);

            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Ignore element"), false, () => IgnoreFile(elt.fullPath));
            menu.AddItem(new GUIContent("Revert element"), false, () => RevertFile(elt.fullPath));
            menu.ShowAsContext();
        }

        /// <summary>
        /// Ignores the file or folder by removing from the tracked files list and adding it to the gitignore.
        /// </summary>
        /// <param name="path">The file or folder path.</param>
        void IgnoreFile(string path)
        {
            string gitIgnore = "";
            using (StreamReader readtext = new StreamReader(".gitignore"))
            {
                gitIgnore = readtext.ReadToEnd();
            }
            var regex = string.Format("^{0}$", path);
            var match = Regex.Match(gitIgnore, regex, RegexOptions.IgnoreCase);

            if (!match.Success) // Check if the path is not yet in the gitignore
            {
                using (StreamWriter writetext = new StreamWriter(".gitignore", append: true))
                {
                    writetext.WriteLine(path);
                }
            }

            RepositoryManager.Remove(path);

            treeModel.SetData(CommitTreeParser.GenerateCommitTree());
            Reload();
        }

        /// <summary>
        /// Revert a file or folder.
        /// </summary>
        /// <param name="path">The path to the file or folder.</param>
        void RevertFile(string path)
        {
            RepositoryManager.Checkout(path);
            treeModel.SetData(CommitTreeParser.GenerateCommitTree());
            Reload();
        }
    }
}