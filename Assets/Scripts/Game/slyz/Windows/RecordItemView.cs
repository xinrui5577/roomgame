using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.slyz.Windows
{
    public class RecordItemView : YxView
    {
        public YxBaseLabelAdapter NameLabel;
        public YxBaseLabelAdapter TimeLabel;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var cardRecord = GetData<CardRecord>();
            SetNameLabel(cardRecord.TypeName);
            SetTimeLabel(cardRecord.TypeTime);
        }

        public void SetNameLabel(string cname)
        {
            if (NameLabel == null) return;
            NameLabel.Text(cname);
        }

        public void SetTimeLabel(string time)
        {
            if (TimeLabel == null) return;
            TimeLabel.Text(time);
        }
    }
}
