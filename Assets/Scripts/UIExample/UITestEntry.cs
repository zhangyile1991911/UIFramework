using Cysharp.Threading.Tasks;
using Framework.UIFramework;
using UIExample;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class UITestEntry : IStartable
{
    [Inject]
    UIManager uiManager;

    public void Start()
    {
        uiManager.OpenUIAsync<HomeWindow>().Forget();
    }
}
