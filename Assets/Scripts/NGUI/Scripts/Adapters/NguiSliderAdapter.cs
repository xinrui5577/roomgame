using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class NguiSliderAdapter : YxBaseSliderAdapter {

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        private UISlider _slider;
        public UISlider Slider
        {
            get { return _slider == null ? _slider = GetComponent<UISlider>() : _slider; }
        }

        public override void SetValue(float value)
        {
            Slider.Set(value,false);
        }

        public override float Value
        {
            get
            {
                var slider = Slider;
                return slider == null ? 0 : slider.value;
            }
            set
            {
                if (Slider != null)
                {
                    Slider.value = value;
                }
            }
        }
    }
}
