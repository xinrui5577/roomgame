using Assets.Scripts.Common.Windows.TabPages; 

namespace Assets.Scripts.Hall.View.ShopWindows
{
    public class YxShopTableItemView : YxTabItem
    { 
        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            InitStateTotal = 1;
        }
    }
}
