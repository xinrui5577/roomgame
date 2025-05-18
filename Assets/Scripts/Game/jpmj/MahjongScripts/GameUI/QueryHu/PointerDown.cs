using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.QueryHu
{
    public class PointerDown : MonoBehaviour, IPointerDownHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}