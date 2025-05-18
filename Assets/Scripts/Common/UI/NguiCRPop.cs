using Assets.Scripts.Common.Models.CreateRoomRules;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class NguiCRPop : NguiCRComponent
    {
        public UILabel NameLabel;
        public UIPopupList PopList;
        protected override void OnStart()
        {
            base.OnStart();
            PopList.onChange.Add(new EventDelegate(ChangeValue));
        }
        protected override void OnFreshCRCView(ItemData itemData)
        {
            NameLabel.text = itemData.Name;
            PopList.Clear();
            var arr = itemData.Options.Split(',');
            var arrLen = arr.Length;
            int index;
            var selecteItem = "";
            int.TryParse(itemData.Value,out index);
            for (var i = 0; i < arrLen; i++)
            {
                var option = arr[i];
                if (i == index)
                {
                    selecteItem = option;
                }
                PopList.AddItem(option);
            }
            PopList.value = selecteItem;
            UpdateWidget(itemData.Width, itemData.Height);
        }

        public void ChangeValue()
        {
            var data = GetData<ItemData>();
            if (data == null) { return;}
            var value = PopList.value;
            var items = PopList.items;
            if (items == null) { return;}
            var index = items.IndexOf(value).ToString();
            data.Value = index;
            var info = data.Parent;
            CreateRoomRuleInfo.SaveItemValue(info.CurTabId, data.Id, info.GameKey, index);
        }
    }
}
