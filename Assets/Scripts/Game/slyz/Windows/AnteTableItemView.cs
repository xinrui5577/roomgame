using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.slyz.Windows
{
    public class AnteTableItemView : YxView
    {
        public YxBaseLabelAdapter NameLabel;
        public YxBaseLabelAdapter DescribeLabel;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<AnteTableItemDate>();
            if (data == null) return;
            SetNameLabe(data.Name);
            SetDescribeLabel(data.Describe);
        }

        public void SetDescribeLabel(string describe)
        {
            if (DescribeLabel == null) return;
            DescribeLabel.Text(describe);
        }

        public void SetNameLabe(string field)
        {
            if (NameLabel == null) return;
            NameLabel.Text(field);
        }

        public class AnteTableItemDate
        {
            public string Name;
            public string Describe;
        }
    }

}
