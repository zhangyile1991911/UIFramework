using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Framework.UIFramework;
using Framework.UIFramework.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class UIAutoGenerateEditorWindow : EditorWindow
    {
        private GameObject uiRootGo;
        private Vector2 scrollPos;

        private static List<string> UIWidgetBase = new List<string>();
        private static string[] WindowBaseArray;
        private static int UIWidgetBaseIndex = 0;
        private static List<string> UIWindowBase = new List<string>();
        private static string[] WidgetBaseArray;
        private static int UIWindowBaseIndex = 0;

        [MenuItem("Custom Tools/UI生成/UIコード生成ツール", false, 10)]
        static void ShowEditor()
        {
            UIAutoGenerateEditorWindow window = GetWindow<UIAutoGenerateEditorWindow>();
            window.minSize = new Vector2(420, 300);
            window.titleContent.text = "UIコード生成ツール";

            UIWidgetBaseIndex = 0;
            UIWindowBaseIndex = 0;

            CollectionUIBaseAssembly();
        }

        static void CollectionUIBaseAssembly()
        {
            UIWidgetBase.Clear();
            UIWidgetBase.Add(nameof(UIWidget));
            
            UIWindowBase.Clear();
            UIWindowBase.Add(nameof(UIWindow));
            Assembly UIFramework = typeof(UIWindow).Assembly;
            foreach (var one in UIFramework.GetTypes())
            {
                bool isUIWidget = one.IsSubclassOf(typeof(UIWidget));
                if (isUIWidget)
                {
                    UIWidgetBase.Add(one.Name);
                }
                bool isUIWindow = one.IsSubclassOf(typeof(UIWindow));
                if (isUIWindow)
                {
                    UIWindowBase.Add(one.Name);
                }
            }

            WindowBaseArray = UIWindowBase.ToArray();
            WidgetBaseArray = UIWidgetBase.ToArray();
        }

        private void OnGUI()
        {
            if (WindowBaseArray == null || WidgetBaseArray == null)
            {
                CollectionUIBaseAssembly();
            }

            UIWindowBaseIndex = Mathf.Clamp(UIWindowBaseIndex, 0, Mathf.Max(0, UIWindowBase.Count - 1));
            UIWidgetBaseIndex = Mathf.Clamp(UIWidgetBaseIndex, 0, Mathf.Max(0, UIWidgetBase.Count - 1));

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("UIコード生成ツール", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                EditorGUILayout.HelpBox(
                    "注意事項:\nUIの命名規則については\nAssets/Settings/UIAutoCreateInfoConfig ファイルを参照してください",
                    MessageType.Info);

                EditorGUILayout.Space(8);

                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("コード生成", EditorStyles.boldLabel);
                    EditorGUILayout.Space(6);

                    EditorGUI.BeginChangeCheck();
                    var newuiRootGo = (GameObject)EditorGUILayout.ObjectField("Prefabノード", uiRootGo, typeof(GameObject), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        uiRootGo = newuiRootGo;
                        UpdateParentClassChoice();
                    }

                    EditorGUILayout.Space(6);
                    UIWindowBaseIndex = EditorGUILayout.Popup("Window 親クラス", UIWindowBaseIndex, WindowBaseArray);
                    UIWidgetBaseIndex = EditorGUILayout.Popup("Widget 親クラス", UIWidgetBaseIndex, WidgetBaseArray);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(8);

                EditorGUILayout.BeginHorizontal();
                {
                    using (new EditorGUI.DisabledScope(uiRootGo == null))
                    {
                        if (GUILayout.Button("ウィンドウを生成する", GUILayout.Height(32)))
                        {
                            GeneratorWindow(uiRootGo);
                        }

                        if (GUILayout.Button("コンポーネントを生成する", GUILayout.Height(32)))
                        {
                            GeneratorWidget(uiRootGo);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private Type GetParentClassType()
        {
            if(uiRootGo == null)return null;
            string className = uiRootGo.name;
            var config = AssetDatabase.LoadAssetAtPath<UIAutoGenerateInfoConfig>("Assets/Settings/UIConfig/UIAutoCreateInfoConfig.asset");
            
            var uiCodeAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(s => s.GetName().Name == config.CustomAssembly);
            foreach (var one in uiCodeAssembly.GetTypes())
            {
                if (one.Name == className)
                {
                    return one.BaseType;
                }
            }
            return null;
        }
        
        private void UpdateParentClassChoice()
        {
            var existedParentType = GetParentClassType();
            UIWindowBaseIndex = 0;
            UIWidgetBaseIndex = 0;
            if(existedParentType != null)
            {
                if(existedParentType.IsSubclassOf(typeof(UIWindow)))
                {
                    UIWindowBaseIndex = Array.IndexOf(WindowBaseArray,existedParentType.Name);
                }
                else if(existedParentType.IsSubclassOf(typeof(UIWidget)))
                {
                    UIWidgetBaseIndex = Array.IndexOf(WidgetBaseArray,existedParentType.Name);
                }
            }
        }
        
        private void GeneratorWindow(GameObject go)
        {
            var config = AssetDatabase.LoadAssetAtPath<UIAutoGenerateInfoConfig>("Assets/Settings/UIConfig/UIAutoCreateInfoConfig.asset");
            if(CreateWindowUIClass(config))
            {
                CreateWindowPrefab(go,config);
            }
        }
        
        private void CreateWindowPrefab(GameObject gameObject,UIAutoGenerateInfoConfig config)
        {
            //检查目录
            if (!AssetDatabase.IsValidFolder(config.WindowPrefabPath))
            {
                throw new System.Exception($"パス:{config.WindowPrefabPath}が該当しません");
            }

            var uiName = GetUIName();
            var localPath = string.Format("{0}/{1}.prefab", config.WindowPrefabPath, uiName);
            if (File.Exists(localPath))
            {
                Debug.Log($"{uiName}.prefabがすでに存在しています");
                return;
            }
            //确保prefab唯一
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            //创建一个prefab 并且输出日志
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction, out prefabSuccess);
            if (prefabSuccess)
                Debug.Log($"{uiName}.prefabが生成されました");
            else
                Debug.Log($"{uiName}.prefabが生成されません");
            AssetDatabase.Refresh();
        }
        
        private bool CreateWindowUIClass(UIAutoGenerateInfoConfig config)
        {
            if (uiRootGo == null) throw new System.Exception("生成するノードを入れてください");
            string uiName = GetUIName();
            string parentClassName = GetUIWindowParentName();
            var targetPath = config.WindowScriptPath;
            CheckTargetPath(targetPath);

            var creator = new UIClassAutoCreate();
            if(creator.RunRequirementCheck(parentClassName,uiRootGo))
            {
                creator.CreateWindow(uiName,parentClassName,uiRootGo,config);
                return true;
            }
            return false;
        }
        
        private void GeneratorWidget(GameObject go)
        {
            var config = AssetDatabase.LoadAssetAtPath<UIAutoGenerateInfoConfig>("Assets/Settings/UIConfig/UIAutoCreateInfoConfig.asset");
            if(CreateWidgetClass(config))
            {
                CreateWidgetPrefab(go,config);    
            }
            
        }

        private bool CreateWidgetClass(UIAutoGenerateInfoConfig config)
        {
            if (uiRootGo == null) throw new System.Exception("プレハブのルートノードをドラッグ＆ドロップしてください。");
            string uiName = GetUIName();
            
            var parentClassName = GetUIWidgetParentName();
            var targetPath = config.WindowScriptPath;
            CheckTargetPath(targetPath);
            var creator = new UIClassAutoCreate();
            if(creator.RunRequirementCheck(parentClassName,uiRootGo))
            {
                creator.CreateWidget(uiName,parentClassName,uiRootGo,config);
                return true;
            }
            return false;
        }

        private void CreateWidgetPrefab(GameObject gameObject,UIAutoGenerateInfoConfig config)
        {
            //フォルダを確認する
            if (!Directory.Exists(config.WidgetPrefabPath))
            {
                throw new System.Exception($"{config.WidgetPrefabPath}パスは該当しません");
            }

            var uiName = GetUIName();
            var localPath = string.Format("{0}/{1}.prefab", config.WidgetPrefabPath, uiName);
            if (File.Exists(localPath))
            {
                Debug.Log($"{uiName}.prefabは既存です");
                return;
            }
            //确保prefab唯一
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            //创建一个prefab 并且输出日志
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction, out prefabSuccess);
            if (prefabSuccess)
                Debug.Log($"{uiName}.prefabが生成されました");
            else
                Debug.Log($"{uiName}.prefabが生成されません");
            AssetDatabase.Refresh();
        }
        
        private string GetUIName()
        {
            string uiName = uiRootGo.name.Replace("UI", "");
            return uiName;
        }

        private string GetUIWindowParentName()
        {
            return UIWindowBase[UIWindowBaseIndex];
        }

        private string GetUIWidgetParentName()
        {
            return UIWidgetBase[UIWidgetBaseIndex];
        }
        
        private void CheckTargetPath(string targetPath)
        {
            string[] road = targetPath.Split('/');
            string findPath = road[0] + "/" + road[1];
            for (int i = 2; i < road.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(findPath+"/"+road[i]))
                {
                    AssetDatabase.CreateFolder(findPath,road[i]);
                }

                findPath = findPath + "/" + road[i];
            }
        }
    }
}
