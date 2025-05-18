using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahCreatItem : MonoBehaviour
    {
        public List<string> IconStateNames;
        public UISprite IconSprite;
        public List<Color> ShowInfoColors;
        public UILabel ShowInfo;

        public void SetItemView(Dictionary<string, object> dic)
        {
            var show = dic.ContainsKey("show") && bool.Parse(dic["show"].ToString());
            gameObject.SetActive(show);

            var text = dic.ContainsKey("text") ? dic["text"].ToString() : "";
            ShowInfo.text = text;
            var clickEnable = dic.ContainsKey("clickEnable") && bool.Parse(dic["clickEnable"].ToString());
            GetComponent<BoxCollider>().enabled = clickEnable;

            var color = dic.ContainsKey("color") ?int.Parse(dic["color"].ToString()) :-1;
            if (color == 0)
            {
                if (clickEnable)
                {
                    IconSprite.spriteName = IconStateNames[1];
                    ShowInfo.color = ShowInfoColors[2];
                }
                else
                {
                    IconSprite.spriteName = IconStateNames[0];
                    ShowInfo.color = ShowInfoColors[0];
                }
            }
            else
            {
                if (clickEnable)
                {
                    IconSprite.spriteName = IconStateNames[3];
                    ShowInfo.color = ShowInfoColors[2];
                }
                else
                {
                    IconSprite.spriteName = IconStateNames[2];
                    ShowInfo.color = ShowInfoColors[1];
                }
            }

        }
    }
}
