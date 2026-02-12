using System;
using Framework.UIFramework;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
    public class UIManagerTrackerWindow : EditorWindow
    {
       [SerializeField]TreeViewState<int> _windowTreeState;

       private UIWindowsTreeView _windowsTreeView;
       
       private UIManagerAgent _uiManagerAgent;

       [MenuItem("Window/UI Manager Tracker")]
       public static void ShowWindow()
       {
           var window = GetWindow(typeof(UIManagerTrackerWindow));
           window.titleContent = new GUIContent("UI Manager Tracker");
           window.Show();
       }

       private void OnEnable()
       {
           if (_windowTreeState == null)
           {
               _windowTreeState = new TreeViewState<int>();    
           }

           if (_windowsTreeView == null)
           {
               _windowsTreeView = new UIWindowsTreeView(_windowTreeState);    
           }

           EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
           if (EditorApplication.isPlaying)
           {
               FindUIManagerInHierarchy();
           }
       }

       private void OnGUI()
       {
           GUILayout.BeginHorizontal();
           GUILayout.Space(2);
           string capacity;
           if (_uiManagerAgent == null)
           {
               capacity = "容量:0";
           }
           else
           {
               capacity = $"容量:{_uiManagerAgent.UIManagerInstance.Capacity}";
           }
           GUILayout.Label(capacity,GUILayout.Width(80),GUILayout.Height(30));
           if (GUILayout.Button("Reload", GUILayout.Width(80),GUILayout.Height(30)))
           {
               if (EditorApplication.isPlaying)
               {
                   if (_uiManagerAgent == null)
                   {
                       FindUIManagerInHierarchy();
                   }
                   else
                   {
                       _windowsTreeView.SetupUIManagerAgent(_uiManagerAgent);
                       _windowsTreeView.Reload();    
                   }
               }
           }
           GUILayout.Space(5);
           GUILayout.EndHorizontal();
           
           Rect total = new Rect(0, 30, position.width, position.height);
           _windowsTreeView.OnGUI(total);
           _windowsTreeView.multiColumnHeader.ResizeToFit();
       }

       void UpdateWindowTreeView()
       {
           
       }

       private void OnPlayModeStateChanged(PlayModeStateChange state)
       {
           if (state == PlayModeStateChange.ExitingEditMode)
           {
               
           }

           if (state == PlayModeStateChange.EnteredEditMode)
           {
               FindUIManagerInHierarchy();
           }
       }
       
       private void FindUIManagerInHierarchy()
       {
           _uiManagerAgent = FindFirstObjectByType<UIManagerAgent>();
           if (_uiManagerAgent != null)
           {
               _windowsTreeView.SetupUIManagerAgent(_uiManagerAgent);
               _windowsTreeView.Reload();
           }
       }
    }
}