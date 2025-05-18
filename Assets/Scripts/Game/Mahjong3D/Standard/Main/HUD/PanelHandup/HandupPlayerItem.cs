using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HandupPlayerItem : MonoBehaviour
    {
        public Text Name;
        public Image Agree;
        public RawImage Icon;

        //请求解散
        public void SetDismissInfo(string name, Texture icon)
        {
            Name.text = name;
            Icon.texture = icon;
            Agree.gameObject.SetActive(false);
        }

        //解散
        public void SetDismissSelect(Sprite sprite)
        {
            Agree.gameObject.SetActive(true);
            Agree.sprite = sprite;
            Agree.SetNativeSize();
        }
    }
}
