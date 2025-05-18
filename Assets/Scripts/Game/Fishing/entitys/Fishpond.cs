using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 鱼塘
    /// </summary>
    public class Fishpond : MonoBehaviour
    {
        /// <summary>
        /// 大小
        /// </summary>
        public Vector2 Size = new Vector2(1136,640);
        public Camera GameCamera;
        public Rect BoundRect { get; protected set; }

        void Start()
        {
            ResetBound();
        }

        /// <summary>
        /// 刷新边界
        /// </summary>
        public void ResetBound()
        {
            var theCamera = App.GameManager.GameUiCamera;
            var csize = theCamera.orthographicSize;
            var h = csize * 2 * 100;
            var sw = theCamera.pixelWidth;
            var sh = theCamera.pixelHeight;
            var mw = Size.x;
            var mh = Size.y;
            var sRate = (float)sw / sh;
            var w = sRate * h;
            var mapRate = mw / mh;
            var rate = w / h;
            float scale;
            if (mapRate < rate)
            {
                scale = w / mw;
            }
            else
            {
                scale = h / mh;
            }
            scale /= 100;
            var localScale = transform.localScale;
            localScale.x = scale;
            localScale.y = scale;
            transform.localScale = localScale;
            var rect = BoundRect;
            rect.xMin = -mw / 2;
            rect.xMax = mw / 2;
            rect.yMin = -mh / 2;
            rect.yMax = mh / 2;
            rect.center = Vector2.zero;
            BoundRect = rect;
            Facade.EventCenter.DispatchEvent(EFishingEventType.ResizeFishponBound, rect);
        }

        private void OnDrawGizmos()
        {
        }
    }
}
