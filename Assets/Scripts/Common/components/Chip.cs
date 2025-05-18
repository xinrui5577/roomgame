using com.yxixia.utile.Utiles;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.components
{
    /// <inheritdoc />
    /// <summary>
    /// 筹码通用类
    /// </summary>
    public class Chip : YxView
    {
        public int BackgroundCount;
        public string BackgroundPrefix;
        /// <summary>
        /// 值
        /// </summary>
        public YxBaseLabelAdapter ValueLabel;
        /// <summary>
        /// 背景
        /// </summary>
        public YxBaseSpriteAdapter Background;
        /// <summary>
        /// 单位
        /// </summary>
        public bool IsPicUnit = true;
        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            ValueLabel.LabelType = YxBaseLabelAdapter.YxELabelType.Normal;

        }

        protected override void OnFreshView()
        {
            var data = GetData<ChipData>();
            if (data == null) return;
            if (BackgroundCount > 0)
            {
                var bgName = string.Format("{0}{1}", BackgroundPrefix, data.BgId % BackgroundCount);
                Background.SetSpriteName(bgName);
            }
            var value = YxUtiles.ReduceNumber(data.Value, 2, IsPicUnit);
            ValueLabel.Text(value);
            SetDepth(data.Depth);
        }

        /// <summary>
        /// 获取chip背景名字
        /// </summary>
        /// <returns></returns>
        public string GetChipName()
        {
            var data = GetData<ChipData>();
            return string.Format("{0}{1}", BackgroundPrefix, data.BgId % BackgroundCount);
        }

        private void SetDepth(int depth)
        {
            ValueLabel.Depth = depth+1;
            Background.Depth = depth;
        }
    }

    public class ChipData
    {
        /// <summary>
        /// 背景名称
        /// </summary>
        public int BgId;
        /// <summary>
        /// 值
        /// </summary>
        public int Value;

        public int Depth;
    }
}
