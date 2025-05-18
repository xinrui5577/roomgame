using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

/*===================================================
 *文件名称:     TrendNormalItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-07-06
 *描述:         常用走势Item(大眼仔，小路，曱甴路公用)
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class TrendNormalItem :YxView
    {
        #region UI Param
        [Tooltip("结果类型（庄闲）")]
        public UISprite ResultType;
        [Tooltip("图片格式")]
        public string SpriteFormat = "Big_{0}";
        [Tooltip("宽度")]
        public float ItemWidth = 33.9f;
        [Tooltip("高度")]
        public float ItemHeigth = 22;
        #endregion

        #region Data Param
        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        #endregion

        #region Function

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data==null)
            {
                return;
            }
            OnFreshData();
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        protected virtual void OnFreshData()
        {
            var node = Data as RoadNode;
            if (node != null)
            {
                var data = node;
                ShowItem(data);
            }
        }

        protected virtual void ShowItem(RoadNode data)
        {
            var isRed = data.IsRed ? ConstantData.KeyBetBanker : ConstantData.KeyBetLeisure;
            ResultType.TrySetComponentValue(string.Format(SpriteFormat, isRed));
            SetItemPos(new Vector2(data.X - 1, data.Y - 1));
        }

        /// <summary>
        /// 设置当前Item位置
        /// </summary>
        /// <param name="vec"></param>
        protected void SetItemPos(Vector2 vec)
        {
            var posX = vec.x * ItemWidth;
            var posY = vec.y * ItemHeigth;
            transform.localPosition = new Vector3(posX, -posY);
        }
        #endregion
    }
}
