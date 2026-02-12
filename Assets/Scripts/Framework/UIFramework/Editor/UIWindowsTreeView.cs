using System;
using System.Collections.Generic;
using System.Linq;
using Framework.UIFramework;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
    public class UIWindowsTreeView : TreeView<int>
    {
        public UIWindowsTreeView(TreeViewState<int> treeViewState) : base(treeViewState, CreateHeader())
        {
            Reload();
        }

        private UIManagerAgent _agent;
        private List<UIWindowTreeViewItem> _windowItems = new();

        public void SetupUIManagerAgent(UIManagerAgent agent)
        {
            _agent = agent;
            RefreshTreeViewItems();
        }

        private void RefreshTreeViewItems()
        {
            _windowItems.Clear();
            var windowList = _agent.UIManagerInstance.GetAllCache();
            for (int i = 0; i < windowList.Count; i++)
            {
                var window = windowList[i];
                UIWindow instance = window.Value as UIWindow;
                if (instance == null) continue;

                UIWindowTreeViewItem oneItem = new();
                oneItem.Rank = i;
                oneItem.WindowName = instance.GetType().ToString().Split('.').Last();
                oneItem.WidgetNum = instance.ChildWidgetCount;
                oneItem.ParentNodeName = instance.uiRectTran.parent.name;
                oneItem.IsActive = instance.IsActive;
                oneItem.UsedCount = instance.ShowCount;
                oneItem.UsedTimestamp = instance.ShowTimestamp;
                oneItem.IsPermanent = window.Permanent;
                _windowItems.Add(oneItem);
            }
        }

        protected override TreeViewItem<int> BuildRoot()
        {
            var root = new TreeViewItem<int> { id = 0, depth = -1, displayName = "Root" };
            root.children ??= new List<TreeViewItem<int>>();

            foreach (var window in _windowItems)
            {
                root.AddChild(window);
            }

            return root;
        }

        private static MultiColumnHeader CreateHeader()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("順位"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("window名"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Widget数"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("階層所属"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("活性化"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("使い回数"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("前回表示"),
                    headerTextAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("常駐"),
                    headerTextAlignment = TextAlignment.Center
                }
            };
            var state = new MultiColumnHeaderState(columns);
            return new MultiColumnHeader(state);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            UIWindowTreeViewItem uiItem = (UIWindowTreeViewItem)args.item;
            if (uiItem == null) return;
            var centerStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter
            };
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                var rect = args.GetCellRect(i);
                switch (args.GetColumn(i))
                {
                    case 0:
                        EditorGUI.LabelField(rect, uiItem.Rank.ToString(), centerStyle);
                        break;
                    case 1:
                        EditorGUI.LabelField(rect, uiItem.WindowName, centerStyle);
                        break;
                    case 2:
                        EditorGUI.LabelField(rect, uiItem.WidgetNum.ToString(), centerStyle);
                        break;
                    case 3:
                        EditorGUI.LabelField(rect, uiItem.ParentNodeName, centerStyle);
                        break;
                    case 4:
                        EditorGUI.LabelField(rect, uiItem.IsActive.ToString(), centerStyle);
                        break;
                    case 5:
                        EditorGUI.LabelField(rect, uiItem.UsedCount.ToString(), centerStyle);
                        break;
                    case 6:
                        EditorGUI.LabelField(rect, DateTimeOffset
                            .FromUnixTimeSeconds(uiItem.UsedTimestamp)
                            .ToLocalTime()
                            .ToString("HH:mm:ss"), centerStyle);
                        break;
                    case 7:
                        EditorGUI.LabelField(rect, uiItem.IsPermanent.ToString(), centerStyle);
                        break;
                }
            }
        }
    }
}