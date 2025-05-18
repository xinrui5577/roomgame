using System;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.slyz
{
    public class UnitLabelAdapter : YxBaseLabelAdapter
    {
        public string NullString = "";
        /// <summary>
        /// 
        /// </summary>
        public YxBaseLabelAdapter[] Labels;


        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        {
            throw new System.NotImplementedException();
        }

        public override int Width { get; set; }
        public override int Height { get; set; }
        public override int Depth { get; set; }
        public override Color Color { get; set; }
        public override YxEPivot Pivot { get; set; }
        protected override void OnText(string content)
        {
            var contentCount = content.Length;
            var labelCount = Labels.Length;
            var count = Mathf.Min(labelCount, contentCount);
            var contentLastIndex = contentCount - 1;
            var i = 0;
            for (; i < count; i++)
            {
                var index = contentLastIndex - i;
                var label = Labels[i];
                var c = content[index].ToString();
                label.Text(c);
            }
            for (; i < labelCount; i++)
            {
                var label = Labels[i];
                label.Text(NullString);
            }
        }

        public override void Font(Font font)
        {
        }

        public override void SetStyle(YxLabelStyle style)
        {
        }

        public override void SetAlignment(YxEAlignment alignment)
        {
        }

        public override int GetTextWidth(string content)
        {
            return 0;
        }

        public override void FreshStyle(YxBaseLabelAdapter labelGo)
        {
        }

        private string _value;
        public override string Value
        {
            get { return _value; }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }
    }
}
