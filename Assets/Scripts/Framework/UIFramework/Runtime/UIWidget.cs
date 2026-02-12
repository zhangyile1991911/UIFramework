using System.Threading;
using UnityEngine;

namespace Framework.UIFramework
{
    public abstract class UIWidget : IUIBase
    {
        public GameObject uiGo
        {
            get => _uiGo;
            set
            {
                uiRectTran = value.GetComponent<RectTransform>();
                _uiGo = value;
            }
        }
    
        public Transform uiTran
        {
            get => _uiGo.transform;
        }
    
        public RectTransform uiRectTran
        {
            get;
            private set;
        }
    
        public bool IsActive => uiGo.activeSelf;
        
        public UIWindow ParentWindow
        {
            get => _parentWindow;
        }
        private GameObject _uiGo;
        private UIWindow _parentWindow;
        protected CancellationTokenSource _linkedToParentCTS { get;private set; }
        
        public CancellationToken token => _linkedToParentCTS.Token;
        
        protected UIWidget(GameObject go,UIWindow parent)
        {
            _uiGo = go;
            _parentWindow = parent;
            _parentWindow?.AddChildWidget(this);
            OnBind(go);
        }

        #region IUIBase Interface

        void IUIBase.OnCreate()
        {
            
        }

        void IUIBase.OnShow(UIOpenParam openParam)
        {
            
        }
        void IUIBase.OnHide()
        {
            
        }
        void IUIBase.OnDestroy()
        {
            
        }
        void IUIBase.OnTick(float deltaTime)
        {
            
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
            _linkedToParentCTS?.Cancel();
            _linkedToParentCTS?.Dispose();
            _parentWindow.RemoveChildWidget(this);
            GameObject.Destroy(uiGo);
        }
    
        protected virtual void OnShow(UIOpenParam openParam)
        {
            uiGo.SetActive(true);
            if (_linkedToParentCTS != null)
            {
                _linkedToParentCTS.Cancel();
                _linkedToParentCTS.Dispose();
                _linkedToParentCTS = null;    
            }
            _linkedToParentCTS = CancellationTokenSource.CreateLinkedTokenSource(_parentWindow.WindowCancellation.Token);
        }
    
        protected virtual void OnHide()
        {
            uiGo.SetActive(false);
            if (_linkedToParentCTS != null)
            {
                _linkedToParentCTS.Cancel();
                _linkedToParentCTS.Dispose();
                _linkedToParentCTS = null;    
            }
        }
    
        protected virtual void OnTick(float deltaTime)
        {
        }
    }
}