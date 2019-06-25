using UnityEngine;

namespace Assets.Scripts.Hall.View
{
    public class Loading : MonoBehaviour
    {
        public UISlider Slider;
        public void OnLoadUpdate(float rate)
        {
            Slider.value = rate;
        }
    }
}
