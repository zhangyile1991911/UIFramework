using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Framework.UIFramework
{
    public class UIManager
    {
        private LRUCache<Type, IUIBase> _uiCachedDic;

        private Transform _bottom;

        private Transform _center;

        private Transform _top;
        
        private Transform _banner;
        
        private Transform _popup;

        private Transform _guide;

        private IResourceManager _resManager;
        
        public CanvasScaler RootCanvasScaler => _rootCanvasScaler;
        private CanvasScaler _rootCanvasScaler;

        public Canvas RootCanvas => _rootCanvas;
        private Canvas _rootCanvas;
        
        public Vector2 ScreenSize => _rootCanvas.GetComponent<RectTransform>().sizeDelta;
       
        public int Capacity { get; private set; }
        
        //IUISystem interface begin
        public UIWindow Get(Type uiName)
        {
            IUIBase ui = null;
            if (!_uiCachedDic.TryGetValue(uiName, out ui))
            {
                return null;
            }

            return ui as UIWindow;
        }

        public T Get<T>()where T : UIWindow
        {
            return Get(typeof(T)) as T;
        }
        
        //IUISystem interface end

        private void OnOpenUI(IUIBase ui,Action<UIWindow> onComplete,UIOpenParam openParam,UILayer layer)
        {
            Transform parentNode = GetParentNode(layer);
            if (ui.uiTran.parent != parentNode)
            {
                ui.uiTran.parent = parentNode;
            }

            ui.uiTran.SetSiblingIndex(parentNode.childCount);
            ui.OnShow(openParam);
            onComplete?.Invoke(ui as UIWindow);
        }
        
        public void OpenUI<T>(Action<UIWindow> onComplete = null,UIOpenParam openParam = null,UILayer layer = UILayer.Bottom)where T : UIWindow
        {
            IUIBase ui = null;
            Type uiType = typeof(T);
            if (_uiCachedDic.TryGetValue(uiType, out ui))
            {
                OnOpenUI(ui, onComplete, openParam,layer);
            }
            else
            {
                UILifeTime uiLifeTime = uiType.GetCustomAttribute<UILifeTime>();
                LoadUIAsync(uiType, loadUi=>
                {
                    OnOpenUI(loadUi,onComplete,openParam,layer);
                },layer,uiLifeTime.IsPermanent).Forget();
            }
        }
        
        public UniTask<UIWindow> OpenUIAsync<T>(UIOpenParam openParam = null,UILayer layer = UILayer.Bottom)where T : UIWindow
        {
            IUIBase ui = null;
            Type uiType = typeof(T);
            if (_uiCachedDic.TryGetValue(uiType, out ui))
            {
                OnOpenUI(ui, null, openParam, layer);
                return new UniTask<UIWindow>(ui as UIWindow);
            }
            UILifeTime uiLifeTime = uiType.GetCustomAttribute<UILifeTime>();
            return LoadUIAsync(uiType, (loadUi) =>
            {
                OnOpenUI(loadUi, null, openParam, layer);
            }, layer, uiLifeTime.IsPermanent);
        }

        public void CloseUI(Type uiName)
        {
            IUIBase ui = null;
            if (_uiCachedDic.TryGetValue(uiName, out ui))
            {
                ui.OnHide();
            }
        }

        public void CloseWindow(UIWindow uiWindow)
        {
            CloseUI(uiWindow.GetType());
        }

        private void DestroyUI(Type uiName)
        {
            _uiCachedDic.Remove(uiName);
        }

        private Transform GetParentNode(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Bottom:
                    return _bottom;
                case UILayer.Center:
                    return _center;
                case UILayer.Top:
                    return _top;
                case UILayer.Banner:
                    return _banner;
                case UILayer.Popup:
                    return _popup;
                case UILayer.Guide:
                    return _guide;
            }

            return null;
        }
        
        private void LoadUI(Type uiType,Action<IUIBase> onComplete,UILayer layer = UILayer.Bottom,bool isPermanent=false)
        {
            IUIBase ui = null;
            if (!_uiCachedDic.TryGetValue(uiType, out ui))
            {
                LoadUIAsync(uiType,(loadUi)=>
                {
                    onComplete?.Invoke(loadUi);
                },layer,isPermanent).Forget();
            }
        }
        
  
        [Inject]
        private IObjectResolver _objectResolver;
        GameObject InstantiatePrefab(GameObject prefab,Transform parent = null)
        {
            GameObject uiGameObject = _objectResolver.Instantiate(prefab,parent);
            uiGameObject.transform.localScale = Vector3.one;
            uiGameObject.transform.SetParent(parent,false);
            return uiGameObject;
        }
        IUIBase CreateUIBaseInstance(Type uiType,GameObject uiGameObject)
        {
            IUIBase ui = Activator.CreateInstance(
                uiType,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new object[] { uiGameObject },
                null
            ) as IUIBase;
            //依頼性注入
            _objectResolver.Inject(ui);
            return ui;
        }

        private async UniTask<UIWindow> LoadUIAsync(Type uiType, Action<UIWindow> onComplete,UILayer layer,bool isPermanent)
        {
            //获取属性 クラスのプロパティーを取得する
            var attributes = uiType.GetCustomAttribute<UIAttribute>(false);
            
            //读取资源 リソースを読み込む
            var uiPrefab = await _resManager.LoadAssetAsync<GameObject>(attributes.ResPath);
            var parentNode = GetParentNode(layer);
            
            GameObject uiGameObject = InstantiatePrefab(uiPrefab,parentNode);
            
            //生成类实例 インスタンスを生成する
            IUIBase ui = CreateUIBaseInstance(uiType,uiGameObject);
            
            _uiCachedDic.Add(uiType,ui,isPermanent);
            ui.OnCreate();
            onComplete?.Invoke(ui as UIWindow);
            return ui as UIWindow;
        }
        
        [Inject]
        private UIManager(IResourceManager resManager)
        {
            _resManager = resManager;
            OnCreate();
            CreateHierarchyAgent();
        }


        private GameObject _uiModule;
        private void OnCreate()
        {
            _uiCachedDic = new LRUCache<Type, IUIBase>(10);
            _uiCachedDic.OnRemove += (ui) =>
            {
                ui.OnHide();
                ui.OnDestroy();
            };
            //UIModuleノードを取得する
            _uiModule = GameObject.Find("UIModule");

            var root = _uiModule.transform.Find("UIRoot");
            _rootCanvasScaler = root.GetComponent<CanvasScaler>();
            _rootCanvas = root.GetComponent<Canvas>();
            
            _bottom = _uiModule.transform.Find("UIRoot/Bottom");
            _center = _uiModule.transform.Find("UIRoot/Center");
            _top = _uiModule.transform.Find("UIRoot/Top");
            _banner = _uiModule.transform.Find("UIRoot/Banner");
            _popup = _uiModule.transform.Find("UIRoot/Popup");
            _guide = _uiModule.transform.Find("UIRoot/Guide");
            GameObject.DontDestroyOnLoad(_uiModule);
        }
        
        public void HideAllUI()
        {
            _uiModule.gameObject.SetActive(false);
        }

        public void ShowAllUI()
        {
            _uiModule.gameObject.SetActive(true);
        }
        [Conditional("UNITY_EDITOR")]
        private void CreateHierarchyAgent()
        {
            var uiModule = GameObject.Find("UIModule");
            UIManagerAgent agent = uiModule.AddComponent<UIManagerAgent>();
            agent.UIManagerInstance = this;
        }
#if UNITY_EDITOR
        public List<Node<Type, UIWindow>> GetAllCache()
        {
            var nodes = _uiCachedDic.GetAllNodesByOrder();
            var result = new List<Node<Type,UIWindow>>();
            return result;
        }
#endif
    }
}
