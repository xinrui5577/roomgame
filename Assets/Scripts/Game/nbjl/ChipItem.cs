using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     ChipItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-07-03
 *描述:        	筹码Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class ChipItem : YxView 
    {
        #region UI Param
        [Tooltip("筹码值")]
        public YxBaseLabelAdapter ValueLabel;
        [Tooltip("背景")]
        public YxBaseSpriteAdapter Background;
        #endregion

        #region Data Param
        [Tooltip("背景数量")]
        public int BackgroundCount=6;
        [Tooltip("背景格式")]
        public string BackgroundFormat= "chip_{0}";
        /// <summary>
        /// 筹码类型
        /// </summary>
        public int Type { get; private set; }

        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            var data = GetData<ChipData>();
            if (data == null) return;
            if (BackgroundCount > 0)
            {
                var bgName = string.Format(BackgroundFormat, data.Type% BackgroundCount);
                Background.SetSpriteName(bgName);
            }
            Type = data.Type;
            if (ValueLabel != null)
            {
                ValueLabel.Text(data.Value);
            }
        }

        #endregion

        #region Function

        /// <summary>
        /// 设置层级
        /// </summary>
        public void SetDepth(ChipItem item)
        {
            int id = int.Parse(Id);
            ValueLabel.Depth += id*2;
            Background.Depth += id * 2;
        }

        #endregion
    }

    /// <summary>
    /// 筹码数据
    /// </summary>
    public class ChipData:IRecycleData
    {
        /// <summary>
        /// 筹码类型
        /// </summary>
        public int Type;

        /// <summary>
        /// 筹码值
        /// </summary>
        public int Value;
    }
}
