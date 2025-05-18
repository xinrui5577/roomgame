using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Fishing.entitys
{
    public class FlyCoinLabel : YxView
    {
        public YxBaseLabelAdapter Label;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null) return;
            var coinValue = (int) Data;
            SetValue(coinValue);
        }


        public void SetValue(int value)
        {
            Label.Text(value);
        }

    }
}
