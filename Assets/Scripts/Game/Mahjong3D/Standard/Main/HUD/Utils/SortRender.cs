using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class SortRender : MonoBehaviour
    {
        public int Order;
        public bool IsUI;
        private void Start()
        {
            if (IsUI)
            {
                Canvas canvas = GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = gameObject.AddComponent<Canvas>();
                }
                canvas.overrideSorting = true;
                canvas.sortingOrder = Order;
            }
            else
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renders.Length; i++)
                {
                    renders[i].sortingOrder = Order;
                }
            }
        }
    }
}