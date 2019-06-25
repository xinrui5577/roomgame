using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.BackPackWindows
{
    public class BackPackItemView : YxView
    {
        public UILabel GoodsCountLabel;
        /// <summary>
        /// 物品名
        /// </summary>
        public UILabel GoodsNameLabel;
        /// <summary>
        /// 物品描述
        /// </summary>
        public UILabel GoodsdescLabel;
        /// <summary>
        /// 物品图标
        /// </summary>
        public UITexture GoodsIcon;

        protected override void OnFreshView()
        {
            base.OnFreshView();
        }
    }
}
