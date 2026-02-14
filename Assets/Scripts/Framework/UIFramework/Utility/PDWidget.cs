

using Framework.UIFramework;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework.UIFramework
{
   public class PDWidget : UIWidget
    {
        protected PlayableDirector PD_root;
        protected Animator AN_root;
        protected CanvasGroup CG_root;

        protected PDWidget(GameObject go, UIWindow parent):base(go, parent)
        {
            PD_root = go.GetComponent<PlayableDirector>();
            CG_root = go.GetComponent<CanvasGroup>();
            AN_root = go.GetComponent<Animator>();
        }

        #if UNITY_EDITOR
        [UIRequirement]
        public static bool RequirementChecker(GameObject gameObject)
        {
            var pd = gameObject.GetComponent<PlayableDirector>();
            if(pd == null)
            {
                Debug.LogError($"PlayableDirector not found!");
                return false;
            }
            var an = gameObject.GetComponent<Animator>();
            if(an == null)
            {
                Debug.LogError($"Animator not found!");
                return false;
            }
            var cg = gameObject.GetComponent<CanvasGroup>();
            if(cg == null)
            {
                Debug.LogError($"CanvasGroup not found!");
                return false;
            }
            return true;
        }
        #endif
    } 
}
