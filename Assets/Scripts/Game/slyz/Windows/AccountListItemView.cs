using YxFramwork.Framework;

namespace Assets.Scripts.Game.slyz.Windows
{
    public class AccountListItemView : YxView
    {
        public UILabel NameLabel;
        public UILabel ValueLabel;

        protected override void OnFreshView()
        {
            var data = GetData<CardStatistics>();
            if (data == null) { return; }
            NameLabel.text = data.TypeName;
            ValueLabel.text = data.TypeCount.ToString();
        }
    }
}
