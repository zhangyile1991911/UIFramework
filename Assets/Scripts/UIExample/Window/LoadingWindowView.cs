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
    [UI("Assets/Resources/Prefab/Windows/LoadingWindow.prefab")]
    public partial class LoadingWindow : UIWindow
    {
		public Image Img_Bg;

        protected override void OnBind(GameObject go)
        {
            uiGo = go;
			Img_Bg = go.transform.Find("Img_Bg").GetComponent<Image>();

            base.OnBind(go);
        }
    }
}
