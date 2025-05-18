using Assets.Scripts.Game.BlackJackCs.Tool;
using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class RuleMgr : MonoBehaviour
    {
        /// <summary>
        /// 规则界面
        /// </summary>
        public GameObject RuleView;

        public GameObject Bg;


        public void ShowRuleView()
        {
            Tools.MoveView(RuleView,new Vector3(0,1024,0),Vector3.zero);
            Bg.SetActive(true);
        }

        public void HideRuleView()
        {
            Tools.MoveView(RuleView,Vector3.zero, new Vector3(0,1024,0));
            Bg.SetActive(false);
        }
    }
}