using Assets.Scripts.Common.Utils;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class RuleViewItem : MonoBehaviour
    {

        public UILabel TitleLabel;

        public UILabel ContentLabel;

        public string TitleFormat;

        public float Space;

        public float ItemHeight
        {
            get { return ContentLabel.height + Space; }
        }

        public void SetRuleItem(string titel,string content)
        {
            SetTitleLael(titel);
            SetContentLabel(content);
        }

        void SetTitleLael(string titel)
        {
            TitleLabel.TrySetComponentValue(string.Format(TitleFormat, titel));
        }

        void SetContentLabel(string content)
        {
            ContentLabel.TrySetComponentValue(content);
        }
    }
}
