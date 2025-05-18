using UnityEngine;
using YxFramwork.Tool;
using YxFramwork.Common;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.brnn.brnn_skin02
{
    /// <summary>
    /// 动画牌型的显示以及处理
    /// </summary>
    public class CardPoint02 : MonoBehaviour
    {

        public GameObject NoScoreMark;

        public UISprite CardsType;

        public UILabel ScoreLabel;

        public GameObject[] Animations;

        private int _selfScore = 0;



        public void ShowScore(int gold)
        {
            _selfScore = gold;
            Invoke("ShowScoreLabel", 1f);
        }

        void ShowScoreLabel()
        {
            if (App.GetGameData<BrnnGameData>().IsBanker || _selfScore == 0)
            {
                NoScoreMark.SetActive(true);
                ScoreLabel.gameObject.SetActive(false);
                return;
            }

            NoScoreMark.SetActive(false);
            ScoreLabel.gameObject.SetActive(true);
            ScoreLabel.text = YxUtiles.ReduceNumber(_selfScore);
        }

        public void ShowCardType(ISFSObject data,int[] cards)
        {
            HideTypes();
            Reset();
            int niu = data.GetInt("niu");
            int type = data.GetInt("type");
            int index = 0;
            string[] daxie = { "n0", "n1", "n2", "n3", "n4", "n5", "n6", "n7", "n8", "n9", "n10" };
            string spriName = string.Empty;
            string soundName = string.Empty;
            switch (type)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    index = 1;
                    spriName = daxie[niu];
                    soundName = string.Format("bull{0}", type);
                    break;
                case 7:
                case 8:
                case 9:
                    index = 2;
                    spriName = daxie[niu];
                    soundName = string.Format("bull{0}", type);
                    break;
                case 10:        //牛牛
                    index = 3;
                    spriName = GetNiuName(cards);
                    soundName = string.Format("bull{0}", type);
                    break;
                case 12:        //四花牛是银牛 & 牛牛
                    index = 4;
                    spriName = "yinniu";
                    soundName = "bull10";
                    break;
                case 13:        //五小牛是金牛
                    index = 4;
                    spriName = "jinniu";
                    soundName = "bull10";
                    break;
                //case 14:      //同花牛
                //    index = 6;
                //    break;
                //case 15:      //葫芦
                //    break;
                case 16:        //炸弹牛
                    index = 5;
                    break;
                default:
                    index = 0;
                    spriName = daxie[niu];
                    soundName = string.Format("bull{0}", type);
                    break;
            }
            var anim = Animations[index];
            var typeCtrl = anim.GetComponentInChildren<CardTypeAnimCtrl02>();
            if(typeCtrl != null)
            {
                typeCtrl.SetSpriteName(spriName);
                typeCtrl.SetSoundName(soundName);
            }
            anim.SetActive(true);
        }

        private void HideTypes()
        {
            foreach (var anim in Animations)
            {
                anim.SetActive(false);
            }
        }

        string GetNiuName(int[] cards)
        {
            string name = "n10";
            foreach (var val in cards)
            {
                if(val == 0x5f || val == 0x61 )
                {
                    name = "daniu";
                   
                    break;
                }
                if(val == 0x51 || val == 0x5e)
                {
                    name = "xiaoniu";
                   
                }
            }

            return name;
        }


        public void Reset()
        {
            NoScoreMark.SetActive(false);
            ScoreLabel.gameObject.SetActive(false);
            HideTypes();
            CancelInvoke();
        }

        

    }
}