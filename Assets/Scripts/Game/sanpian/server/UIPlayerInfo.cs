using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.sanpian.CommonCode;
using Assets.Scripts.Game.sanpian.DataStore;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sanpian.server
{
    public class UIPlayerInfo : MonoBehaviour
    {
        public UILabel NameLabel;
        public UILabel GoldLabel;
        public GameObject IsOnLineSprite;
        public Transform CardsArea;
        public Transform OutCardsArea;
        public GameObject ReadyIcon;
        public GameObject OutCardsIcon;
        public GameObject PassFont;
        public GameObject MyFriendIcon;
        public UILabel textFour;
        public UILabel textA;
        public GameObject YaoIcon;
        public GameObject SpeakIcon;
        public UISprite BiaoQing;
        public TalkBubble TalkBubble;
        public GameObject GpsLine;
        public UILabel GpsLabel;
        public UILabel AddressLabel;
        public NguiTextureAdapter HeadTexture;
        public UISprite TouYou;
        public UILabel GameScore;
        public UILabel FlowerScore;
        [Tooltip("飞字预设")]
        public FlyLabel Prefab;
        [Tooltip("飞字父级物体")]
        public GameObject FlyParent;

        public void FriendIconAni()
        {
            MyFriendIcon.SetActive(true);
            MyFriendIcon.transform.localScale = Vector3.one * 3;
            iTween.ScaleTo(MyFriendIcon, Vector3.one, 1.5f);
        }

        public void AddFlowerScore(int score)
        {
            FlowerScore.text = int.Parse(FlowerScore.text) + score + "";
        }

        public void AddPianScore(int score)
        {
            if (Prefab && FlyParent)
            {
                var obj = FlyParent.AddChild(Prefab.gameObject);
                obj.SetActive(true);
                var flyLabel = obj.GetComponent<FlyLabel>();
                flyLabel.SetLabel(YxUtiles.ReduceNumber(score));
            }
        }

        public void AddGameScore(int score)
        {
            GameScore.text = score + "";
        }

        public void Reset()
        {
            if (MyFriendIcon != null)
            {
                MyFriendIcon.SetActive(false);
            }
            if (PassFont != null)
            {
                PassFont.SetActive(false);
            }
            if (OutCardsIcon != null)
            {
                OutCardsIcon.SetActive(false);
            }
            if (ReadyIcon != null)
            {
                ReadyIcon.SetActive(false);
            }
            if (YaoIcon != null)
            {
                YaoIcon.SetActive(false);
            }
            if (TouYou!=null)
            {
                TouYou.gameObject.SetActive(false);
            }
            FlowerScore.text = "0";
            GameScore.text = "0";
            SpeakIcon.SetActive(false);
        }

        public void ShowYao(int four,int A)
        {
            YaoIcon.SetActive(true);
            textFour.text = four + "";
            textA.text = A + "";
        }

        public void HideYao()
        {
            YaoIcon.SetActive(false);
        }

        public void ShowAddressInfo(UserInfo info)
        {
            AddressLabel.gameObject.SetActive(true);
            AddressLabel.text = string.Format("IP:{0}\n所在地:{1}", info.Ip, info.IsHasGpsInfo ? info.Country : "未提供位置信息\n请开启位置服务,并给予应用相应权限");
        }
    }
}
