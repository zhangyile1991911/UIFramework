using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using Framework.UIFramework;
using VContainer;

namespace UIExample
{
    /// <summary>
    /// Auto Generate Class!!!
    /// </summary>
    [UILifeTime(UILifeTimeDefine.Transient)]
    public partial class HomeWindow : UIWindow
    {
        [Inject]
        CreateWidgetHelper WidgetHelper;

        TopResWidget topResWidget;

        [UnityEngine.Scripting.Preserve]
        protected HomeWindow(GameObject uiNode):base(uiNode)
        {
            
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            topResWidget = WidgetHelper.CreateWidgetController<TopResWidget>(this,Ins_TopResWidget.gameObject);
        }
        
        protected override void OnShow(UIOpenParam openParam)
        {
            base.OnShow(openParam);
        }
    
        protected override void OnHide()
        {
            base.OnHide();
        }
    
        protected override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}