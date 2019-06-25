using UnityEngine;

namespace Assets.Scripts.Common.UI
{
    public class NguiCRLabel : NguiCRComponent
    {
        public UILabel Label;

        protected override void OnFreshCRCView(ItemData itemData)
        {
            var content = itemData.Value;
            if (content == null) return;
            Label.text = content;
            UpdateWidget(itemData.Width, itemData.Height);
        }

        public override Vector2 UpdateWidget(int width = 0, int height = 0)
        {
            var size = base.UpdateWidget(width, height);
            return size;
        }
    }
}
