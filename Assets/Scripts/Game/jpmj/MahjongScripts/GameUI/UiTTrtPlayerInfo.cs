using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    /// <summary>
    /// 单个玩家的总成绩信息
    /// </summary>
    public class UiTTrtPlayerInfo : MonoBehaviour
    {
        public Image FangzhuImg;
        public Image BestPaoshouImg;
        public Image BigWinnerImg;

        public Text UserNameTxt;
        public Text UserIdTxt;

        public Text ZimoTxt;
        public Text JiePaoTxt;

        public Text DianPaoTxt;

        public Text AnGangTxt;
        public Text MingGangTxt;

        public Text TresultText;
        /// <summary>
        /// 头像
        /// </summary>
        public RawImage HeadImg;

        //玩家位置
        [HideInInspector]
        public int Chair = -1;

        public virtual void Clear()
        {
            Chair = -1;
            FangzhuImg.gameObject.SetActive(false);
            BestPaoshouImg.gameObject.SetActive(false);
            BigWinnerImg.gameObject.SetActive(false);

            UserNameTxt.text = "";
            UserIdTxt.text = "";

            ZimoTxt.text = "";
            JiePaoTxt.text = "";
            DianPaoTxt.text = "";
            AnGangTxt.text = "";
            MingGangTxt.text = "";
            TresultText.text = "";
        }

        public string UserName
        {
            set { UserNameTxt.text = value; }
        }

        public string UserId
        {
            set { UserIdTxt.text = value; }
        }


        public bool IsFangZhu
        {
            set {FangzhuImg.gameObject.SetActive(value); }
        }

        public bool IsBestPao
        {
            set { BestPaoshouImg.gameObject.SetActive(value);}
        }

        public bool IsBigWinner
        {
            set { BigWinnerImg.gameObject.SetActive(value);}
        }

        public int ZimoTime
        {
            set { ZimoTxt.text = value + ""; }
        }

        public int JiePaoTime
        {
            set { JiePaoTxt.text = value + ""; }
        }

        public int DianPaoTime
        {
            set { DianPaoTxt.text = value + ""; }
        }
        public int AnGangTime
        {
            set { AnGangTxt.text = value + ""; }
        }
        public int MingGangTime
        {
            set { MingGangTxt.text = value + ""; }
        }

        public virtual int TotalScore
        {
            set { TresultText.text = value / UtilData.ShowGoldRate + ""; }
        }

        public bool SetVisible
        {
            set{gameObject.SetActive(value);}
        }

        public void SetHeadImg(string url, Texture define)
        {
            AsyncImage.GetInstance().SetTextureWithAsyncImage(url, HeadImg,define);
        }
    }
}
