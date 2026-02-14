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
    [UI("Assets/Resources/Prefab/Widgets/ContestCenterWidget.prefab")]
    public partial class ContestCenterWidget : UIWidget
    {
		public Button Btn_Course;
		public Button Btn_Contest;

    
    	protected override void OnBind(GameObject go)
    	{
    	    uiGo = go;
    	    
			Btn_Course = go.transform.Find("Btn_Course").GetComponent<Button>();
			Btn_Contest = go.transform.Find("Btn_Contest").GetComponent<Button>();

            base.OnBind(go);
    	}
    }
}