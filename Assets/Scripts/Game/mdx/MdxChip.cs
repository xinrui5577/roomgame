using Assets.Scripts.Common.components;
using YxFramwork.Common.Adapters;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mdx
{
    public class MdxChip : Chip {

        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            //ValueLabel.LabelType = YxBaseLabelAdapter.YxELabelType.ReduceNumberWithUnit;

        }

        //protected override void OnFreshView()
        //{
        //    var data = GetData<ChipData>();
        //    if (data == null) return;
        //    if (BackgroundCount > 0)
        //    {
        //        var bgName = string.Format("{0}{1}", BackgroundPrefix, data.BgId % BackgroundCount);
        //        Background.SetSpriteName(bgName);
        //    }
        //    var value = YxUtiles.ReduceNumber(data.Value, 2, IsPicUnit);
        //    ValueLabel.Text(value);
        //    SetDepth(data.Depth);
        //}

        //private void SetDepth(int depth)
        //{
        //    ValueLabel.Depth = depth + 1;
        //    Background.Depth = depth;
        //}

        public void HideChip()
        {
            gameObject.SetActive(false);
        }
    }
}
