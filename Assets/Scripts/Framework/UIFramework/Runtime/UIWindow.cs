using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;

namespace Framework.UIFramework
{
    public abstract class UIWindow : IUIBase
    {
        #region property member
        public GameObject uiGo
        {
            get => _uiGo;
            set => _uiGo = value;
        }
        public Transform uiTran
        {
            get => _uiGo.transform;
        }
        private GameObject _uiGo;

        public RectTransform uiRectTran
        {
            get;
            private set;
        }
        
        public bool IsActive => uiGo.activeSelf;

        private List<IUIBase> _childWidget = new List<IUIBase>(10);
#if UNITY_EDITOR
        public int ChildWidgetCount => _childWidget.Count;
        public long ShowTimestamp;
        public long ShowCount;
#endif    

        public CancellationTokenSource WindowCancellation { get; private set; }
        public CancellationToken Token => WindowCancellation.Token;
        #endregion

        //因为通过反射创建
        //在IL2CPP时候可能被裁剪，这里需要预留一下
        [Preserve]
        protected UIWindow(GameObject uiNode)
        {
            OnBind(uiNode);
        }

        #region IUIBase Interface
        void IUIBase.OnCreate()
        {
            OnCreate();
        }
        
        void IUIBase.OnDestroy()
        {
            OnDestroy();
        }

        void IUIBase.OnShow(UIOpenParam openParam)
        {
            OnShow(openParam);
        }

        void IUIBase.OnHide()
        {
            OnHide();
        }

        void IUIBase.OnTick(float deltaTime)
        {
            OnTick(deltaTime);
        }
        #endregion
        
        protected virtual void OnBind(GameObject go)
        {
            uiRectTran = go.GetComponent<RectTransform>();
        }
        
        protected virtual void OnCreate()
        {           
            
        }

        protected virtual void OnDestroy()
        {
            //todo 清理子widget
            foreach (var widget in _childWidget)
            {
                widget.OnDestroy();
            }
            WindowCancellation?.Cancel();
            WindowCancellation?.Dispose();
            GameObject.Destroy(uiGo);
        }
        protected virtual void OnShow(UIOpenParam openParam)
        {
            uiGo.SetActive(true);
            if (WindowCancellation != null)
            {
                WindowCancellation.Cancel();
                WindowCancellation.Dispose();
                WindowCancellation = null;
            }
            WindowCancellation = new CancellationTokenSource();
        }
        protected virtual void OnHide()
        {
            uiGo.SetActive(false);
            for (int i = 0; i < _childWidget.Count; i++)
            {
                _childWidget[i]?.OnHide();
            }

            if (WindowCancellation != null)
            {
                WindowCancellation.Cancel();
                WindowCancellation.Dispose();
                WindowCancellation = null;    
            }
        }
        protected virtual void OnTick(float deltaTime)
        {
            
        }
        
        internal void AddChildWidget(UIWidget uiElement)
        {
            if (uiElement != null)
            {
                _childWidget.Add(uiElement);    
            }
        }

        internal void RemoveChildWidget(UIWidget uiElement)
        {
            if (uiElement != null)
            {
                _childWidget.Remove(uiElement);    
            }
        }
    }
}