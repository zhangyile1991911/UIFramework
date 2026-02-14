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
    [UI("Assets/Resources/Prefab/Widgets/CommuCenterWidget.prefab")]
    public partial class CommuCenterWidget : UIWidget
    {
		public Button Btn_Star;
		public Button Btn_Idol;
		public Button Btn_Support;
		public Button Btn_Event;

    
    	protected override void OnBind(GameObject go)
    	{
    	    uiGo = go;
    	    
			Btn_Star = go.transform.Find("Anim_ButtonLayer/Btn_Star").GetComponent<Button>();
			Btn_Idol = go.transform.Find("Anim_ButtonLayer/Btn_Idol").GetComponent<Button>();
			Btn_Support = go.transform.Find("Anim_ButtonLayer/Btn_Support").GetComponent<Button>();
			Btn_Event = go.transform.Find("Anim_ButtonLayer/Btn_Event").GetComponent<Button>();

            base.OnBind(go);
    	}
    }
}