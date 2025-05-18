using System.Globalization;
using Assets.Scripts.Common.Models.CreateRoomRules;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class NguiCRInput : NguiCRComponent
    {
        public UILabel FileLabel;
        public UIInput TextInput;
     
        protected override void OnStart()
        {
            base.OnStart();
            TextInput.onChange.Add(new EventDelegate(ChangeValue));
        }

        protected float Min = float.NaN;
        protected float Max = float.NaN;

        protected override void OnFreshCRCView(ItemData itemData)
        { 
            var validation = itemData.Validation;
            TextInput.validation = validation;
            FileLabel.text = itemData.Name;
            TextInput.value = itemData.Value;
            var range = itemData.Range;
            if (range != null)
            {
                var len = range.Length;
                if (len > 0) float.TryParse(range[0], out Min);
                if (len > 1) float.TryParse(range[len-1], out Max);
            }
            UpdateWidget(itemData.Width, itemData.Height);
        }

        public string GetValue()
        {
            return TextInput.value;
        }

        public void ChangeValue()
        {
            var data = GetData<ItemData>();
            if (data == null) return;
            var value = GetValid(data);
            data.Value = value;
            var info = data.Parent;
            CreateRoomRuleInfo.SaveItemValue(info.CurTabId,data.Id,info.GameKey,value);
        }

        /// <summary>
        /// 获得有效值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetValid(ItemData data)
        { 
            var validation = data.Validation;
            var value = TextInput.value;
            switch (validation)
            {
                case UIInput.Validation.Float:
                {
                    float curValue;
                    float.TryParse(value, out curValue);
                    if (!float.IsNaN(Min) && curValue < Min) value = Min.ToString(CultureInfo.InvariantCulture);
                    if (!float.IsNaN(Max) && curValue > Max) value = Max.ToString(CultureInfo.InvariantCulture);
                    break;
                }
                case UIInput.Validation.Integer:
                {
                    int curValue;
                    int.TryParse(value, out curValue);
                    if (!float.IsNaN(Min) && curValue < Min) value = Min.ToString(CultureInfo.InvariantCulture);
                    if (!float.IsNaN(Max) && curValue > Max) value = Max.ToString(CultureInfo.InvariantCulture);
                    break;
                }
            }
            return value;
        }
    }
}
