using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Framework.UIFramework;

public class CreateWidgetHelper
{
    [Inject]
    IResourceManager resourceManager;
    [Inject]
    IObjectResolver container;

    private GameObject CreateGameObject(GameObject prefab,Transform node)
    {
        return container.Instantiate(prefab,node);
    }

    private T CreateInstance<T>( GameObject uiInstance,UIWindow uiWindow)where T : UIWidget
    {
        T uiWidget = Activator.CreateInstance(
            typeof(T),
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null,
            new object[]{uiInstance,uiWindow},
            null) as T;
        if (uiWidget == null)
        {
            Debug.LogError($"{typeof(T)}が生成されない");
            return null;
        }
        
        container.Inject(uiWidget);
        
        return uiWidget;
    }


    public T CreateUIWidget<T>(UIWindow uiWindow,Transform parentNode)where T : UIWidget
    {
        //クラスのプロパティーを取得する
        Type widgetType = typeof(T);
        if (widgetType == null)
        {
            Debug.LogError($"CreateUIWidget {widgetType} 該当widgetTypeがありません");
            return null;
        }
            
        var attribute = widgetType.GetCustomAttribute<UIAttribute>(false);
        var uiPath = attribute.ResPath;

        //プレハブを読み込む
        GameObject uiPrefab = resourceManager.LoadAsset<GameObject>(uiPath);
        if (uiPrefab == null)
        {
            Debug.LogError($"CreateUIWidget {uiPath} 該当プレハブがありません");
            return null;
        }
        var uiGameObject = CreateGameObject(uiPrefab,parentNode);
        uiGameObject.transform.localPosition = Vector3.zero;
        uiGameObject.transform.localScale = Vector3.one;
        T uiWidget = CreateInstance<T>(uiGameObject,uiWindow);
        if (uiWidget == null)
        {
            Debug.LogError($"{widgetType}が生成されない");
            return null;
        }
        
        return uiWidget;
    }

    public async UniTask<T> CreateUIWidgetAsync<T>(UIWindow uiWindow,Transform parentNode)where T : UIWidget
    {
        var widgetType = typeof(T);
        
        var attribute = widgetType.GetCustomAttribute<UIAttribute>(false);
        var uiPath = attribute.ResPath;

        //プレハブを読み込む
        GameObject uiPrefab = await resourceManager.LoadAssetAsync<GameObject>(uiPath);
        if (uiPrefab == null)
        {
            Debug.LogError($"CreateUIWidget {uiPath} 該当プレハブがありません");
            return null;
        }
        var uiGameObject = CreateGameObject(uiPrefab,parentNode);
        uiGameObject.transform.localPosition = Vector3.zero;
        uiGameObject.transform.localScale = Vector3.one;
        T uiWidget = CreateInstance<T>(uiGameObject,uiWindow);
        if (uiWidget == null)
        {
            Debug.LogError($"{widgetType}が生成されない\n[{widgetType}无法生成]");
            return null;
        }
        return uiWidget;
    }

    public T CreateWidgetController<T>(UIWindow uiWindow, GameObject uiGameObject) where T : UIWidget
    {
        T uiWidget = CreateInstance<T>(uiGameObject,uiWindow);
        if (uiWidget == null)
        {
            Debug.LogError($"{typeof(T)}が生成されない\n[{typeof(T)}无法生成]");
            return null;
        }

        //uiGameObjectがUnityより生成されたので、ここに手動で注入する必要がある
        container.InjectGameObject(uiGameObject);
        container.Inject(uiWidget);

        return uiWidget;
    }

    public T CreateWidgetController<T>(UIWidget uiWidget,GameObject uiGameObject) where T : UIWidget
    {
        T widgetController = CreateInstance<T>(uiGameObject,uiWidget.ParentWindow);
        if (widgetController == null)
        {
            Debug.LogError($"{typeof(T)}が生成されない");
            return null;
        }

        //uiGameObjectがUnityより生成されたので、ここに手動で注入する必要がある
        container.InjectGameObject(uiGameObject);
        container.Inject(widgetController);

        return widgetController;
    }

}