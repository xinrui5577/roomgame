using Assets.Scripts.Game.BlackJackCs.Tool;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.BlackJackCs
{

    public class ResultMgr : MonoBehaviour
    {
        /// <summary>
        /// 保险
        /// </summary>
        public UILabel InsuranceLabel;

        public GameObject InsurnaceObj;
        /// <summary>
        /// 输赢
        /// </summary>
        public UILabel GoldLabel;

        /// <summary>
        /// 黑杰克图标
        /// </summary>
        public GameObject BlackJackMark;

        /// <summary>
        /// 胜利图标
        /// </summary>
        public UITexture WinMark;

        /// <summary>
        /// 失败图标
        /// </summary>
        public UITexture LostMark;

        public UIGrid Grid;

        public GameObject ResultView;
        public GameObject Bg;

        public GameObject DoubleMark;

        public UITexture HeadImage;
      

        public void InitResultView(ISFSObject data)
        {

            //设置玩家头像
            HeadImage.mainTexture = App.GameData.GetPlayer<BjPlayerPanel>().HeadPortrait.GetTexture(); //blackjackGameManager.GetInstance().SelfPlayer.UserIcon.mainTexture;

            //设置输赢保险
            int insurance = 0;
            if (data.ContainsKey("insurance"))
               insurance = data.GetInt("insurance");
            InsuranceLabel.text = "-" + insurance;
            InsurnaceObj.SetActive(insurance != 0);


            //设置输赢筹码
            if (data.ContainsKey("gold"))
            {
                int gold = data.GetInt("gold");
                GoldLabel.text = YxUtiles.ReduceNumber(gold);
                
                WinMark.gameObject.SetActive(gold >= 0);
                LostMark.gameObject.SetActive(gold < 0);
            }

            Grid.Reposition();
            DoubleMark.SetActive(data.ContainsKey("double"));

            //显示双倍标记
            DoubleMark.SetActive(data.ContainsKey("addRate") && data.GetBool("addRate"));
            BlackJackMark.SetActive(data.ContainsKey("blackJack") && data.GetBool("blackJack"));

        }

        public void ShowResultView()
        {
            Tools.MoveView(ResultView,new Vector3(0,1024,0), Vector3.zero);
            Bg.SetActive(true);
            Invoke("CouldCloseView", 1);
        }

        protected void CouldCloseView()
        {
            Bg.GetComponent<BoxCollider>().enabled = true;
        }

        public void HideResultView()
        {
            Tools.MoveView(ResultView, ResultView.transform.localPosition, new Vector3(0, 1024, 0));
            Bg.GetComponent<BoxCollider>().enabled = false;
            Bg.SetActive(false);
        }
    }
}