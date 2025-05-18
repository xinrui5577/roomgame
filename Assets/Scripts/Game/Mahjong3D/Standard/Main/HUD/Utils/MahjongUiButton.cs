using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongUiButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsOn = true;
        public bool HasAudio = true;
        public float Highbright = 1.3f;
        public float Nomalbright = 1f;
        public UnityEvent Event;
        private Image mImg;

        private void Start()
        {
            mImg = GetComponent<Image>();
            var mate = GameCenter.Assets.MaterialAssets.GetAsset<Material>("ButtonHighLight");
            if (mate != null)
            {
                mImg.material = Instantiate(mate);
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (null != mImg && IsOn)
            {
                mImg.material.SetFloat("_Bright", Highbright);
            }
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (null != mImg && IsOn)
            {
                Event.Invoke();
                mImg.material.SetFloat("_Bright", Nomalbright);
                if (HasAudio)
                {
                    MahjongUtility.PlayEnvironmentSound("uiclick");
                }
            }
        }
    }
}