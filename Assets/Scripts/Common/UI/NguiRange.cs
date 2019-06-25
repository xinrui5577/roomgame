
namespace Assets.Scripts.Common.UI
{
    public class NguiRange : NguiCRComponent
    {
        public UIInput InputLabel;
        public int Max;
        public int Min;

        protected override void OnFreshCRCView(ItemData itemData)
        {
            if (itemData == null) return;
            var range = itemData.Name.Split(',');
            if (range.Length > 1)
            {
                int.TryParse(range[0], out Min);
                int.TryParse(range[1], out Max);
            }
            InputLabel.value = itemData.Value;
        }

        public void OnLeft()
        {
            var idata = GetData<ItemData>();
            if (idata == null) return;
            var ipts = idata.Value;
            int input;
            int.TryParse(ipts,out input);
            if (input <= Min) return;
            input--;
            ipts = input.ToString();
            idata.Value = ipts;
            InputLabel.value = ipts;
        }

        public void OnRight()
        {
            var idata = GetData<ItemData>();
            if (idata == null) return;
            var ipts = idata.Value;
            int input;
            int.TryParse(ipts,out input);
            if (input >= Max) return;
            input++;
            ipts = input.ToString();
            idata.Value = ipts;
            InputLabel.value = ipts;
        }
    }
}
