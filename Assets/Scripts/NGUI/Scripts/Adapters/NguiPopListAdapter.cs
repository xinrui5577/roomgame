using System.Collections.Generic;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    /// <summary>
    /// 
    /// </summary>
    public class NguiPopListAdapter : YxBasePopListAdapter
    {
        private UIPopupList _poplist;

        public UIPopupList PopupLIist
        {
            get { return _poplist == null ? _poplist = GetComponent<UIPopupList>() : _poplist; }
        }


        public override void AddItem(string item)
        {
            PopupLIist.AddItem(item);
        }

        public override void RemoveItem(string item)
        {
            PopupLIist.RemoveItem(item);
        }

        public override List<string> GetItemList()
        {
            return PopupLIist.items;
        }

        public override void Set(string value, bool notify = true)
        {
            PopupLIist.Set(value, notify);
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }
    }
}
