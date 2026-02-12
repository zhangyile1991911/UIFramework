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
    [UI("Assets/Resources/Prefab/Widgets/HomeCenterWidget.prefab")]
    public partial class HomeCenterWidget : UIWidget
    {
		public Button Btn_Produce;
		public Button Btn_Event;
		public Button Btn_Shop;

    
    	protected override void OnBind(GameObject go)
    	{
    	    uiGo = go;
    	    
			Btn_Produce = go.transform.Find("Btn_Produce").GetComponent<Button>();
			Btn_Event = go.transform.Find("Btn_Event").GetComponent<Button>();
			Btn_Shop = go.transform.Find("Btn_Shop").GetComponent<Button>();

            base.OnBind(go);
    	}
    }
}