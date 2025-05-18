using UnityEngine.UI;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class UguiSliderAdapter : YxBaseSliderAdapter
    {
        public override YxEUIType UIType
        {
            get { return YxEUIType.Ugui; }
        }

        private Slider _slider;
        public Slider Slider
        {
            get { return _slider == null ? _slider = GetComponent<Slider>() : _slider; }
        }


        public override void SetValue(float value)
        {
            Slider.value = value;
        }

        public override float Value {
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
