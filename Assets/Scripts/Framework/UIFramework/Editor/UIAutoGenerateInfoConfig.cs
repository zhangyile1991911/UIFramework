using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UIFramework.Editor
{
    [Serializable]
    public class UIFieldRule
    {
        public string prefixName;
        public string typeName;
    }

    [Serializable,CreateAssetMenu(menuName = "UIConfig/CreateAutoCreateInfoConfig")]
    public class UIAutoGenerateInfoConfig : ScriptableObject
    {
        public List<UIFieldRule> uiNameRules;

        public string UIWindowViewTemplatePath;
        public string UIWindowControlTemplatePath;
        public string UIWidgetViewTemplatePath;
        public string UIWidgetControlTemplatePath;
        public string WindowScriptPath;
        public string WidgetScriptPath;
        public string WindowPrefabPath;
        public string WidgetPrefabPath;

        public string CustomWindowScope = "UIExample";
        public string CustomWidgetScope = "UIExample";

        public string CustomAssembly = "UIExample";
    }
}