using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.bjlb
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


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(int min)
        {
            MinApplyBanker = min;
        }

        //刷新庄家按键
        public void RefreshBanker()
        {
            BankerBtn.SetActive(true);
        }

    
        /// <summary>
        /// 隐藏庄家按键
        /// </summary>
        public void SetApplyBankerBtnActive(bool active)
        {
            BankerBtn.SetActive(active);
        }

        /// <summary>
        /// 上庄限制金额
        /// </summary>
        public int MinApplyBanker;

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
                    BtnImage.GetComponent<UISprite>().spriteName = "60";
                    BankerBtn.GetComponent<UIButton>().normalSprite = "60";
                    break;
                case BankerType.CanBecomeBanker:
                    BtnImage.GetComponent<UISprite>().spriteName = "4";
                    BankerBtn.GetComponent<UIButton>().normalSprite = "4";
                    break;
                case BankerType.CanNotBeBanker:
                    BtnImage.GetComponent<UISprite>().spriteName = "2"; //允许玩家点击按钮
                    BankerBtn.GetComponent<UIButton>().normalSprite = "2";
                    break;
            }
        }

        public void BankerBtnOnClick()
        {
            var gserver = App.GetRServer<BjlGameServer>();
            switch (CurBankerType)
            {
                case BankerType.StayedBanker:
                    gserver.ApplyQuit();
                    break;
                case BankerType.CanBecomeBanker:
                    gserver.ApplyBanker();
                    break;
                case BankerType.CanNotBeBanker:
                    YxMessageTip.Show(string.Format("金！币！大于{0}可以申请成为庄家...", YxUtiles.GetShowNumberForm(MinApplyBanker)));
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