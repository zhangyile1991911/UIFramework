using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using Framework.UIFramework;

namespace UIExample
{
    /// <summary>
    /// Auto Generate Class!!!
    /// </summary>
    [UI("Assets/Resources/Prefab/Windows/HomeWindow.prefab")]
    public partial class HomeWindow : UIWindow
    {
		public Transform Ins_BG;
		public RectTransform RT_Center;
		public Transform Ins_CommuCenterWidget;
		public Transform Ins_TopResWidget;

        protected override void OnBind(GameObject go)
        {
            uiGo = go;
			Ins_BG = go.transform.Find("Ins_BG").GetComponent<Transform>();
			RT_Center = go.transform.Find("RT_Center").GetComponent<RectTransform>();
			Ins_CommuCenterWidget = go.transform.Find("RT_Center/Ins_CommuCenterWidget").GetComponent<Transform>();
			Ins_TopResWidget = go.transform.Find("Ins_TopResWidget").GetComponent<Transform>();

            base.OnBind(go);
        }
    }
}
