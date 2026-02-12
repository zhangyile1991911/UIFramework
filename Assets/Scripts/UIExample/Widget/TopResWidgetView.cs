using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using Framework.UIFramework;
//using SuperScrollView;


namespace UIExample
{
    /// <summary>
    /// Auto Generate Class!!!
    /// </summary>
    [UI("Assets/Prefab/Widgets/TopResWidget.prefab")]
    public partial class TopResWidget : UIWidget
    {
		public Image Img_PLv;
		public Transform Ins_Turn;
		public Transform Ins_Money;

    
    	protected override void OnBind(GameObject go)
    	{
    	    uiGo = go;
    	    
			Img_PLv = go.transform.Find("Img_PLv").GetComponent<Image>();
			Ins_Turn = go.transform.Find("Ins_Turn").GetComponent<Transform>();
			Ins_Money = go.transform.Find("Ins_Money").GetComponent<Transform>();

            base.OnBind(go);
    	}
    }
}