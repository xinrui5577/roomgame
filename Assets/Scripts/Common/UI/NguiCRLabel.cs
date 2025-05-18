using Assets.Scripts.Common.Models.CreateRoomRules;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// label
    /// </summary>
    public class NguiCRLabel : NguiCRComponent
    {
        public UILabel Label;
        
        protected override void OnFreshCRCView(ItemData itemData)
        {
            var content = itemData.Name;
            Label.text = string.IsNullOrEmpty(content)?itemData.Value : string.Format(content, itemData.Value);
            UpdateWidget(itemData.Width, itemData.Height);
        }
    }
}
