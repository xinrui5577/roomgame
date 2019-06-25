using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Adapters
{
    [RequireComponent(typeof(Text))] 
    public class UguiLabelAdapter : YxBaseLabelAdapter
    {
        private Text _label;
        protected void Awake()
        {
        }
        protected override void OnText(string content)
        {
            if (Label == null) return;
            _label.text = content;
        }

        protected Text Label
        {
            get{return _label ?? (_label = GetComponent<Text>());}
        }

        public override void Font(Font font)
        {
            Mfont = font;
            Label.font = font;
        }

        public override int GetTextWidth(string content)
        {
            return (int)_label.flexibleWidth;//todo
        }

        public override int Width {
            get
            {
                return (int)Label.preferredWidth;
            }
            set { }
        }
        public override int Height {
            get
            {
                return (int)Label.preferredHeight;
            }
            set { }
        }

        public override Color Color {
            get { return _label.color; }
            set { _label.color = value; }
        }
    }
}
