using UnityEngine;

namespace Assets.Scripts.Game.Ttzkf
{
    public class RuleCtrl : MonoBehaviour
    {
        /// <summary>
        /// 黑色背景
        /// </summary>
        public GameObject Bg;
        protected void Start()
        {
            if (Bg.activeSelf)
            {
                Bg.SetActive(false);
            }
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void Close()
        {
            Bg.SetActive(false);
        }

        /// <summary>
        /// 点击游戏规则按钮
        /// </summary>
        public void OnClickRuleBtn()
        {
            Bg.SetActive(true);
        }
    }
}
