using System;

namespace Framework.UIFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIAttribute : Attribute
    {
        public string ResPath{get;private set;}
        public UIAttribute(string path)
        {
            ResPath = path;
        }
    }   

    public enum UILifeTimeDefine
    {
        Transient,
        Permanent,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UILifeTime : Attribute
    {
        private UILifeTimeDefine windowLifeTimeDefine;
        public bool IsPermanent => windowLifeTimeDefine == UILifeTimeDefine.Permanent;
        public UILifeTime(UILifeTimeDefine inDefine)
        {
            windowLifeTimeDefine = inDefine;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class UIElementChecker : Attribute
    {
        
    }
}