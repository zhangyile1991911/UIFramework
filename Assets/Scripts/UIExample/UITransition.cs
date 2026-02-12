using Cysharp.Threading.Tasks;
using Framework.UIFramework;
using UnityEngine;
using VContainer;

namespace UIExample
{
    public class UITransition
    {
        [Inject]
        UIManager _uiManager;

        public async UniTask<UIWindow> CutIn<T>(UIWindow wnd)where T : UIWindow
        {
            var nextWindow = await _uiManager.OpenUIAsync<T>();
            _uiManager.CloseWindow(wnd);
            return nextWindow;
        }

        public async UniTask<UIWindow> TransitionWithLoading<T>() where T : UIWindow
        {
            var loadingWindow = await _uiManager.OpenUIAsync<LoadingWindow>();

            var nextWindow = await _uiManager.OpenUIAsync<T>();
            _uiManager.CloseWindow(loadingWindow);
            
            return nextWindow;
        }

        public void TransitToFadeInOut<T>(UIWindow wnd)
        {
            
        }

        public void FadeOut(UIWindow wnd)
        {
            
        }

        public void FadeIn()
        {
            
        }
    }

}
