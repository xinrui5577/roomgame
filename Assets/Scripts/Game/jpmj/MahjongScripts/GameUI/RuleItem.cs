using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class RuleItem : MonoBehaviour
    {

        public Text Tital;

        public Text Content;

        public Image TitalBg;

        public RectTransform Rect;

        public float ItemLeft;
        public float ItemRight;
        public float BgWithTital;
        public float BgWithContent;

        public Vector2 Size
        {
            get { return Rect.rect.size; }

        }

        void Start()
        {
        }

        public virtual void SetTital(string tital)
        {
            Tital.text = tital;
            var titalW = Tital.preferredWidth;
            var titalBgSize = new Vector2(titalW + BgWithTital + BgWithTital, TitalBg.rectTransform.sizeDelta.y); 
            TitalBg.rectTransform.sizeDelta = titalBgSize;
            var pos = new Vector3(ItemLeft, TitalBg.rectTransform.localPosition.y, TitalBg.rectTransform.localPosition.z);
            TitalBg.rectTransform.localPosition = pos;

        }

        public virtual void SetContent(string content)
        {
            var titalBgSize = TitalBg.rectTransform.sizeDelta;
            var contentWidth = Size.x - titalBgSize.x - ItemLeft - ItemRight - BgWithContent;
            Content.rectTransform.sizeDelta = new Vector2(contentWidth, Content.rectTransform.sizeDelta.y);
            Content.text = content;
            var contentHight = Content.preferredHeight;

            if (contentHight > Size.y)
            {
                Rect.sizeDelta = new Vector2(Rect.sizeDelta.x,contentHight+40);
            }
            var contentPosX = ItemLeft + titalBgSize.x + BgWithContent;
            Content.rectTransform.localPosition = new Vector3(contentPosX, Content.rectTransform.localPosition.y, Content.rectTransform.localPosition.z);
        }
    }
}
