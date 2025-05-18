using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    /// <summary>
    /// 本脚本目前只为了适应iPhone x的刘海屏
    /// </summary>
    public class CanvasFit : MonoBehaviour
    {
        public Canvas Canvas;
        [Tooltip("左偏移")]
        public float LeftOffset = 44;
        [Tooltip("右偏移")]
        public float RightOffset = 0;

        private void Awake()
        {
            FitCanvas();
        }

        private void FitCanvas()
        {
#if UNITY_IPHONE||UNITY_EDITOR
            if (Screen.width == 2436 && Screen.height == 1125)
            {
                transform.GetComponent<RectTransform>().offsetMin = new Vector2(LeftOffset, 0f);
                transform.GetComponent<RectTransform>().offsetMax = new Vector2(RightOffset, 0f);
            }
#endif
        }
    }
}
