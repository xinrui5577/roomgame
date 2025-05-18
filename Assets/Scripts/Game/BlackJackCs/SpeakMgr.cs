using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs
{
    /// <summary>
    /// 玩家说话管理类 说话:德州扑克轮到本玩家决定跟牌与否的简称
    /// </summary>
    public class SpeakMgr : MonoBehaviour
    {

        /// <summary>
        /// 按键数组
        /// </summary>
        public GameObject[] Buttons;
       
        /// <summary>
        /// 其他标签页 加注,说话,结束选项,自动选项
        /// </summary>
        public GameObject[] Pages;
     
        public GameObject DoubleBtn;

        public GameObject InsuranceBtn;

        public GameObject BetView;

        public GameObject SpeakView;
   
        /// <summary>
        /// 设置是否显示说话界面
        /// </summary>
        /// <param name="showSpeakView">是否显示说话界面</param>
        public void SpeakViewActive(bool showSpeakView)
        {
            SpeakView.SetActive(showSpeakView);
        }

        /// <summary>
        /// 设置是否显示下注界面
        /// </summary>
        /// <param name="active">是否显示下注界面</param>
        public virtual void BetViewActive(bool active)
        {
            if (BetView == null) return;
            BetView.SetActive(active);
        }


        public void DoubleBtnEnable(bool enable)
        {
            DoubleBtn.GetComponent<BoxCollider>().enabled = enable;
            if (enable)
                DoubleBtn.GetComponent<UIButton>().state = UIButtonColor.State.Normal;
            else
                DoubleBtn.GetComponent<UIButton>().state = UIButtonColor.State.Disabled;
        }


        public void InsuranceBtnEnable(bool enable)
        {
            InsuranceBtn.GetComponent<BoxCollider>().enabled = enable;
            if (enable)
                InsuranceBtn.GetComponent<UIButton>().state = UIButtonColor.State.Normal;
            else
                InsuranceBtn.GetComponent<UIButton>().state = UIButtonColor.State.Disabled;
        }

        /// <summary>
        /// 不显示按键
        /// </summary>
        public void ShowNothing()
        {
            BetViewActive(false);
            SpeakViewActive(false);
        }
        
        /// <summary>
        /// 重置信息
        /// </summary>
        public void Reset()
        {
            DoubleBtnEnable(true);
            DoubleBtnEnable(true);
            ShowNothing();
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum SpeakButton
        {
            BetBtn,
            FoldBtn,
            FollowBtn,
            KickBtn,
            NotKickBtn,
            ShowPokerBtn,
            BackKickBtn,
            CancelBetBtn,
        }
    }
}
