using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class JiaPiaoPnl : MonoBehaviour
    {
        public RectTransform BtnsGroup;

        public Sprite[] Sprites;
        public Image[] Images;

        public void OnJiaPiaoClick(int value)
        {
            EventDispatch.Dispatch((int)NetEventId.OnJiaPiaoClick, new EventData(value));
            BtnsGroup.gameObject.SetActive(false);
        }

        public void SetImageByPlace(int place, int index)
        {
            Image image = Images[place];
            image.gameObject.SetActive(true);
            image.sprite = Sprites[index];
            image.SetNativeSize();
        }

        void Awake()
        {
            HideAllImg();
        }

        public void HideAllImg()
        {
            for (int i = 0; i < Images.Length; i++)
                Images[i].gameObject.SetActive(false);   
        }
    }
}