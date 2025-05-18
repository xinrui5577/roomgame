using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TotalResultItem : MonoBehaviour
    {
        public TextItemContainer Container;
        public Image BestPaoshouImg;
        public Image BigWinnerImg;
        public Image MasterImg;
        public Text TresultText;
        public Text UserNameTxt;
        public Text UserIdTxt;
        public RawImage HeadImg;
        //玩家位置
        [HideInInspector]
        public int Chair = -1;

        public void Reset()
        {
            Chair = -1;
            MasterImg.gameObject.SetActive(false);
            BigWinnerImg.gameObject.SetActive(false);
            BestPaoshouImg.gameObject.SetActive(false);
        }

        public string UserId { set { UserIdTxt.text = value; } }
        public string UserName { set { UserNameTxt.text = value; } }
        public bool IsFangZhu { set { MasterImg.gameObject.SetActive(value); } }
        public bool IsBestPao { set { BestPaoshouImg.gameObject.SetActive(value); } }
        public bool IsBigWinner { set { BigWinnerImg.gameObject.SetActive(value); } }

        public virtual int TotalScore
        {
            set { TresultText.text = YxFramwork.Tool.YxUtiles.GetShowNumber(value).ToString(); }
        }

        public bool SetVisible
        {
            set { gameObject.SetActive(value); }
        }

        public void SetHeadImg(Texture define)
        {
            if (define != null)
            {
                HeadImg.texture = define;
            }
        }
    }
}
