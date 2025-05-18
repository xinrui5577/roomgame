using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UiPointerDown : MonoBehaviour, IPointerDownHandler
    {
        public UnityEvent Event;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (null != Event)
            {
                Event.Invoke();
            }
        }
    }
}