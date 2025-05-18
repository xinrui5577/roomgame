using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.mdx
{
    /// <summary>
    /// 庄家相关处理 2016年4月18日16:29:13 创建人：王乔
    /// </summary>
    public class BankerCtrl : MonoBehaviour
    { 
        /// <summary>
        /// 庄家按键
        /// </summary>
        public GameObject BankerBtn;
        /// <summary>
        /// 按键图
        /// </summary>
        public GameObject BtnImage;

  

        //刷新庄家按键
        public void RefreshBankerBtn()
        {
            SetApplyBankerBtnActive(true);
        }

    
        /// <summary>
        /// 隐藏庄家按键
        /// </summary>
        public void SetApplyBankerBtnActive(bool active)
        {
            BankerBtn.SetActive(active);
        }

        

        /// <summary>
        /// 最大赌金
        /// </summary>
        public GameObject Congri;
        public int MaxBet;
        /// <summary>
        /// 当前庄状态
        /// </summary>
        public BankerType CurBankerType;
        /// <summary>
        /// 设置庄状态
        /// </summary>
        /// <param name="bt"></param>
        public void SetBankerType(BankerType bt)
        {
            CurBankerType = bt;
            switch (CurBankerType)
            {
                case BankerType.StayedBanker:
                    SetApplyBankerBtnActive(false);
                    break;
                case BankerType.CanBecomeBanker:
                case BankerType.CanNotBeBanker:
                    SetApplyBankerBtnActive(true);
                    break;
            }
        }

        public void BankerBtnOnClick()
        {
            var gserver = App.GetRServer<MdxGameServer>();
            switch (CurBankerType)
            {
                case BankerType.StayedBanker:
                    gserver.ApplyQuit();
                    break;
                case BankerType.CanBecomeBanker:
                    gserver.ApplyBanker();
                    break;
                case BankerType.CanNotBeBanker:
                    //YxMessageTip.Show(string.Format("金！币！大于{0}可以申请成为庄家...", YxUtiles.GetShowNumberForm(MinApplyBanker)));
                    break;
            }
        }
    }

    /// <summary>
    /// 庄家状态
    /// </summary>
    public enum BankerType
    {
        StayedBanker, //正在坐庄
        CanBecomeBanker, //可以申请上庄
        CanNotBeBanker, //无法申请上庄
    }
}