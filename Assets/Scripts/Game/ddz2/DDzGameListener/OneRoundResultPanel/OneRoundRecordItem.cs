using System.Globalization;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.OneRoundResultPanel
{
    public class OneRoundRecordItem : MonoBehaviour
    {

        public GameObject Dizhusp;
        public GameObject Winsp;
        public GameObject BackGround;

        /// <summary>
        /// 昵称
        /// </summary>
        public UILabel NiChengLabel;
        /// <summary>
        /// 加倍
        /// </summary>
        public UILabel JiaBeiLabe;
        /// <summary>
        /// 成绩
        /// </summary>
        public UILabel RecorLabel;


        public void SetOneRoundRecordItemData(OneRoundResultListener.UserRecord userData,int landSeat)
        {

            var userSeat = userData.Seat;
            //设置如果是玩家自己则设置字体为黄色,并设置相应成绩
            if (userSeat == App.GetGameData<DdzGameData>().SelfSeat)
            {
                NiChengLabel.color = Color.yellow;
                JiaBeiLabe.color = Color.yellow;
                RecorLabel.color = Color.yellow;
            }

            //地主图标
            SetActive(Dizhusp, userSeat == landSeat);

            //胜利图标设置
            SetActive(Winsp, userData.Gold > 0);

            SetActive(BackGround, userData.Index%2 == 0);

            //昵称
            SetLabelData(NiChengLabel, userData.PlayerName);

            int rate = Mathf.Abs(userData.Nm)/userData.Ante;  
            //加倍
            SetLabelData(JiaBeiLabe, string.Format("x{0}", rate));

            //成绩
            SetLabelData(RecorLabel, YxUtiles.GetShowNumber(userData.Gold).ToString(CultureInfo.InvariantCulture));

           
        }


        protected void SetActive(GameObject go, bool active)
        {
            if (go == null) return;
            go.SetActive(active);
        }

        protected void SetLabelData(UILabel label, string txt)
        {
            if (label == null) return;
            label.text = txt;
        }

    }
}
