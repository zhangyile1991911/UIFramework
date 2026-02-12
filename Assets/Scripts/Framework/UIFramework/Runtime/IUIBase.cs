using UnityEngine;

namespace Framework.UIFramework
{
    public enum UILayer
    {
        Bottom,
        Center,
        Top,
        Banner,
        Popup,
        Guide,
    }

    internal interface IUIBase
    {
        GameObject uiGo { get; set; }
        Transform uiTran { get; }
        RectTransform uiRectTran { get; }

        bool IsActive { get; }

        void OnCreate();

        void OnShow(UIOpenParam openParam);

        void OnHide();
    
        void OnTick(float deltaTime);

         void OnDestroy();
    }  
}

