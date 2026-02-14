using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Framework.UIFramework;
using Framework.UIFramework.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class UIClassAutoCreate
    {
        private GameObject uiRootGo;

        private class UIDeclaration
        {
            public string DeclarationCode;
            public string InitFindCode;
        }
        private Dictionary<string,UIDeclaration> allNodeInfos = new Dictionary<string, UIDeclaration>();

        //他のprefabを参考する
        private string IgnoreCommonName = "Ins_";
    
        private UIAutoGenerateInfoConfig infoConfig;
        
        public void CreateWindow(string uiClassName,string uiParentClass,GameObject uiRootGo,UIAutoGenerateInfoConfig config, bool isForceUpdate = false)
        {
            this.uiRootGo = uiRootGo;
            allNodeInfos.Clear();
            
            infoConfig = config;
            
            FindGoChild(uiRootGo.transform,true);
            
            if (allNodeInfos.Count <= 0)
            {
                Debug.Log("<color=#ff0000>ノードの数がゼロなので、もう一度ノードの名前を確認してください！</color>");
            }
            
            var allDeclaration = new StringBuilder();
            var allFindCode = new StringBuilder();

            foreach (var node in allNodeInfos)
            {
                allDeclaration.Append(node.Value.DeclarationCode);
                allFindCode.Append(node.Value.InitFindCode);
            }
            
            //找到生成UI类模板文件
            var templateAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(config.UIWindowViewTemplatePath);
            var viewTemplateFile = templateAsset.text; 
            
            //替换类名 クラス名を入れ替える
            viewTemplateFile = viewTemplateFile.Replace("{0}",uiClassName);
            //替换成员变量声明　変数を定義
            viewTemplateFile = viewTemplateFile.Replace("{1}",allDeclaration.ToString());
            //替换find代码　ノード紐付けコードを生成
            viewTemplateFile = viewTemplateFile.Replace("{2}",allFindCode.ToString());
            //替换命名空间　ネーミングスペースを入れ替える
            viewTemplateFile = viewTemplateFile.Replace("{3}",config.CustomWindowScope);
            //替换资源路径　リソースパスを入れ替える
            var prefabResPath = $"{config.WindowPrefabPath}/{uiClassName}.prefab";
            viewTemplateFile = viewTemplateFile.Replace("{4}",prefabResPath);
            // templateFile = templateFile.Replace("{5}",enumClassName);
            viewTemplateFile = viewTemplateFile.Replace("{6}",uiParentClass);
            string uiVIewFilePath = string.Format("{0}/{1}View.cs", config.WindowScriptPath,uiClassName);
            if (!isForceUpdate && File.Exists(uiVIewFilePath))
            {
                if (EditorUtility.DisplayDialog("警告", "既存スクリプトを上書きますか", "はい","いいえ"))
                {
                    SaveFile(viewTemplateFile,uiVIewFilePath);
                }
            }
            else
            {
                SaveFile(viewTemplateFile,uiVIewFilePath);
            }
            
            //生成控制代码
            //コードを生成する
            var controlTemplateFile = AssetDatabase.LoadAssetAtPath<TextAsset>(config.UIWindowControlTemplatePath).text;
            string uiControllerFilePath = string.Format("{0}/{1}Control.cs", config.WindowScriptPath, uiClassName);
            controlTemplateFile = controlTemplateFile.Replace("{0}", uiClassName);
            if (!File.Exists(uiControllerFilePath))
            {
                controlTemplateFile = controlTemplateFile.Replace("{1}", uiParentClass);
                controlTemplateFile = controlTemplateFile.Replace("{2}", config.CustomWindowScope);
                SaveFile(controlTemplateFile,uiControllerFilePath);
            }
            else
            {//親クラスが一致かを判断する
                Assembly UIFrame = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(s => s.GetName().Name == infoConfig.CustomWindowScope);
                Type uiClassType = UIFrame.GetTypes().FirstOrDefault(x => x.Name == uiClassName);
                var oldParentClassName = uiClassType.BaseType.Name;
                if (!oldParentClassName.Equals(uiParentClass))
                {//違った場合、元親クラスを書き換えする
                    ReplaceParentClass(uiControllerFilePath,oldParentClassName,uiParentClass);
                }
            }
        }
        
        void SaveFile(string content,string filePath)
        {
            if(File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(content);
                }
            }
            Debug.Log(filePath+"が生成されました");
            AssetDatabase.Refresh();
        }

        private void FindGoChild(Transform ts,bool isRoot)
        {
            if (!isRoot)
            {
                CheckUINode(ts);
                if (ts.name.StartsWith(IgnoreCommonName)) return;
            }

            for (int i = 0; i < ts.childCount; i++)
            {
                FindGoChild(ts.GetChild(i),false);
            }
        }

        private void CheckUINode(Transform child)
        {
            var names = child.name.Split("_");
            if (names == null||names.Length < 1)
            {
                return;
            }

            //2 UI节点全路径
            var path = GetFullNodePath(child);
            
            foreach (var oneName in names)
            {
                //1 确定成员 类型名字
                var fieldTypeInfo = DetermineExportType(oneName+"_");
                if (fieldTypeInfo == null) continue;

                string classFieldName = $"{oneName}_{names[^1]}";
        
                var DefineNodeCode = string.Format("\t\tpublic {0} {1};\n", fieldTypeInfo.typeName, classFieldName);
                //3 生成查找语句
                var findNodeCode = string.Format("\t\t\t{0} = go.transform.Find(\"{1}\").GetComponent<{2}>();\n",
                    classFieldName, path, fieldTypeInfo.typeName);

                if (allNodeInfos.ContainsKey(classFieldName))
                {
                    throw new Exception("名前が重複しました!"+path);
                }

                var one = new UIDeclaration()
                {
                    DeclarationCode = DefineNodeCode,
                    InitFindCode = findNodeCode
                };
                allNodeInfos.Add(classFieldName,one);
            }
        }

        private UIFieldRule DetermineExportType(string transformName)
        { 
            return infoConfig.uiNameRules.
                Where(one => transformName.Contains(one.prefixName)).
                Select(one=>one).
                FirstOrDefault();
        }

        private string GetFullNodePath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != uiRootGo.transform)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }
        
        public void CreateWidget(string uiWidgetName,string uiParentName, GameObject uiRootGo,UIAutoGenerateInfoConfig config)
        {
            this.uiRootGo = uiRootGo;
            allNodeInfos.Clear();
            
            infoConfig = config;
            
            FindGoChild(uiRootGo.transform,true);
            
            if (allNodeInfos.Count <= 0)
            {
                Debug.Log("<color=#ff0000>ノードの数がゼロなので，もう一度ノード名前を確認してください！</color>");
            }
            
            var allDeclaration = new StringBuilder();
            var allFindCode = new StringBuilder();

            foreach (var node in allNodeInfos)
            {
                allDeclaration.Append(node.Value.DeclarationCode);
                allFindCode.Append(node.Value.InitFindCode);
            }
            
            //找到生成UI类模板文件
            var templateAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(config.UIWidgetViewTemplatePath);
            var viewTemplateFile = templateAsset.text; 
            
            //替换类名
            viewTemplateFile = viewTemplateFile.Replace("{0}",uiWidgetName);
            //替换成员变量声明
            viewTemplateFile = viewTemplateFile.Replace("{1}",allDeclaration.ToString());
            //替换find代码
            viewTemplateFile = viewTemplateFile.Replace("{2}",allFindCode.ToString());
            //替换资源路径
            var prefabResPath = $"{config.WidgetPrefabPath}/{uiWidgetName}.prefab";
            viewTemplateFile = viewTemplateFile.Replace("{3}",prefabResPath);

            viewTemplateFile = viewTemplateFile.Replace("{4}",uiParentName);

            viewTemplateFile = viewTemplateFile.Replace("{5}",infoConfig.CustomWidgetScope);

            string uiVIewFilePath = string.Format("{0}/{1}View.cs", config.WidgetScriptPath,uiWidgetName);
            if (File.Exists(uiVIewFilePath))
            {
                if (EditorUtility.DisplayDialog("警告", "既存スクリプトを上書きますか", "はい","いいえ"))
                {
                    SaveFile(viewTemplateFile,uiVIewFilePath);
                }
            }
            else
            {
                SaveFile(viewTemplateFile,uiVIewFilePath);
            }

            //生成控制代码
            var controlTemplateFile = AssetDatabase.LoadAssetAtPath<TextAsset>(config.UIWidgetControlTemplatePath).text;
            string uiControllerFilePath = string.Format("{0}/{1}Control.cs", config.WidgetScriptPath, uiWidgetName);
            controlTemplateFile = controlTemplateFile.Replace("{0}", uiWidgetName);
            controlTemplateFile = controlTemplateFile.Replace("{1}", uiWidgetName);
            controlTemplateFile = controlTemplateFile.Replace("{2}",uiParentName);
            controlTemplateFile = controlTemplateFile.Replace("{3}",config.CustomWidgetScope);
            if (!File.Exists(uiControllerFilePath))
            {
                SaveFile(controlTemplateFile,uiControllerFilePath);
            }
            else
            {//親クラスが一致かを判断する
                Assembly uiCodeAssembley = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(s => s.GetName().Name == config.CustomWidgetScope);
                Type uiClassType = uiCodeAssembley.GetTypes().FirstOrDefault(x => x.Name == uiWidgetName);
                var oldParentClassName = uiClassType.BaseType.Name;
                if (!oldParentClassName.Equals(uiParentName))
                {//一致しない場合、元親クラスを書き換えする
                    ReplaceParentClass(uiControllerFilePath,oldParentClassName,uiParentName);
                }
            }
        }

        void ReplaceParentClass(string uiClassFilePath,string oldParentClassName,string newParentClassName)
        {
            var newFilePath = uiClassFilePath + ".bak";
            var firstFound = false;
            using(var readFs = new FileStream(uiClassFilePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(readFs))
            using(var writeFs = new FileStream(newFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(writeFs))
            {
                while (reader.ReadLine() is { } line)
                {
                    if (!firstFound)
                    {
                        var isContain = line.Contains(oldParentClassName);
                        if (isContain)
                        {//マッチしたら,書き換えします
                            line = line.Replace(oldParentClassName,newParentClassName);
                            firstFound = true;    
                        }
                    }
                    writer.WriteLine(line);
                }
            }
            File.Replace(newFilePath,uiClassFilePath,null);
        }

        public bool RunRequirementCheck(string parentClassName, GameObject uiRootGo)
        {
            if(parentClassName == nameof(UIWindow) || parentClassName == nameof(UIWidget))
            {
                Debug.Log("Default Parent Class has not Requirement Checker method");
                return true;
            }

            var UIFrameAssembly = typeof(UIWindow).Assembly;
            var parentType = UIFrameAssembly.GetTypes().First(one=>parentClassName == one.Name);

            var RequirementChecker = parentType.GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.GetCustomAttribute<UIRequirement>() != null);

            var paramArr = RequirementChecker.GetParameters();
            if(paramArr.Length != 1)
            {
                Debug.Log("Paramater Count dont match");
                return true;    
            }
            
            if(paramArr[0].ParameterType != typeof(GameObject))
            {
                Debug.Log("Paramater Type dont match");
                return true;    
            }
            if(RequirementChecker.ReturnType != typeof(bool))
            {
                Debug.Log("Paramater ReturnType dont match");
                return true;
            }

            var result = RequirementChecker.Invoke(null,new object[]{uiRootGo});
            return (bool)result;
        } 
    }
}
