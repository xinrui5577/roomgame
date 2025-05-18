using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum TextType
    {
        HuType,
        HupaiScore,
        GangScore,
        PiaoScore,
        NiaoSocre,
        TotalSocre,
        ZimoNum,
        JiepaoNum,
        DianpaoNum,
        MinggangNum,
        AngangNum,
        PuScore,
        ChBao,
        MoBao,
        GangKaiNum,
    }

    public class TextItem : MonoBehaviour
    {
        public TextType Type;
        public Text Score;

        public string Txt
        {
            get { return Score.text; }
            set { Score.text = value; }
        }
    }

    [System.Serializable]
    public class TextItemContainer
    {
        public TextItem[] Array;

        public TextItem GetItem(TextType type)
        {
            TextItem item;
            for (int i = 0; i < Array.Length; i++)
            {
                item = Array[i];
                if (item != null)
                {
                    if (type == item.Type)
                    {
                        return Array[i];
                    }
                }
            }
            return null;
        }

        public void SetItem(TextType type, string value)
        {
            var item = GetItem(type);
            if (null != item)
            {
                item.Txt = value;
            }
        }

    }
}