using System;
using System.Globalization;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.ImgPress;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    /// <summary>
    /// 总成绩预设 ，控制脚本
    /// </summary>
    public class TtResultRecord : MonoBehaviour
    {
        /// <summary>
        /// 玩家组
        /// </summary>
        public UiTTrtPlayerInfo[] Players;

        public Text DateText;
        public Text RoomID;
        public Text RoundCnt;

        public CompressImg Img;

        [HideInInspector]
        public Image Round;
        public Sprite JuImg;
        public Sprite QuanImg;
        public Text Time;     

        protected virtual void OnEnable()
        {
            if (null != Time)
            {
                Time.text = DateTime.Now.ToString("yyyy年MM月dd日   HH:mm:ss");
            }

            if (DateText != null) DateText.text = DateTime.Now.ToShortDateString() + "   " + DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture) + ":" + DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture)
                              + ":" + DateTime.Now.Second.ToString(CultureInfo.InvariantCulture);
        }

        public virtual void Show(TableData tableData, Texture[] defineArray)
        {
            gameObject.SetActive(true);  

            if (RoomID != null) RoomID.text = tableData.RoomInfo.RoomID + "";
            if (Round != null) Round.sprite = tableData.RoomInfo.GameLoopType == EnGameLoopType.round ? JuImg : QuanImg;
            if (RoundCnt != null) RoundCnt.text = tableData.RoomInfo.CurrRound + "/" + tableData.RoomInfo.MaxRound;

            foreach (UiTTrtPlayerInfo uiTTrtPlayerInfo in Players)
            {
                uiTTrtPlayerInfo.SetVisible = false;
            }

            var data = tableData.TotalResult;
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                Players[i].Clear();
                Players[i].SetVisible = true;
                Players[i].AnGangTime = data.AnGang[i];
                Players[i].UserName = data.Name[i];
                Players[i].UserId = data.Id[i].ToString(CultureInfo.InvariantCulture);
                Players[i].ZimoTime = data.Zimo[i];
                Players[i].JiePaoTime = data.Hu[i];
                Players[i].DianPaoTime = data.Pao[i];
                Players[i].MingGangTime = data.Gang[i];
                Players[i].TotalScore = data.Glod[i];

                Players[i].IsFangZhu = i == tableData.OwnerSeat;
                Players[i].IsBestPao = i == data.BeatPaoSeat;
                Players[i].IsBigWinner = i == data.BigWinnerSeat;

                int sex = tableData.UserDatas[i].Sex;
                sex = sex >= 0 ? sex : 0;
                Players[i].SetHeadImg(tableData.UserDatas[i].HeadImage, defineArray[sex % 2]);
            }
        }
        /// <summary>
        /// 点击回退按钮
        /// </summary>
        public void OnBackBtnClick()
        {
            gameObject.SetActive(false);
            App.QuitGame();
        }  

        /// <summary>
        /// 总战绩截图分享
        /// </summary>
        public void OnshareTtResultScreen()
        {
#if UNITY_ANDROID || UNITY_IPHONE

            Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    imageUrl = "file://" + imageUrl;
                }

                EventDispatch.Dispatch((int)ShareEventID.OnWeiChatShareGameResult, new EventData(imageUrl));
            });
#endif
        }
    }
}
