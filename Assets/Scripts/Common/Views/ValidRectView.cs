using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views
{
    public class ValidRectView : YxView {

        /// <summary>
        ///
        /// </summary>
        public Vector2 Size = new Vector2();

        private Transform _transform;

        public Transform SelfTs {
            get
            {
                return _transform == null ? _transform = transform : _transform;
            }
        }

        /// <summary>
        /// Ë¢ÐÂÎ»ÖÃ
        /// </summary>
        public void OnFreshPostition(Vector3 pos)
        {
            var ts = SelfTs;
            if (ts == null) { return; }
            var mainCamera = App.UI.GetWindowCamera();
            if (mainCamera == null) { return; }
            var vpos = mainCamera.WorldToViewportPoint(pos);
            var vx = vpos.x;
            var vy = vpos.y;
            var minX = Size.x;
            var maxX = 1 + minX;
            var minY = Size.y;
            var maxY = 1 + minY;
            if (vx < minX)
            {
                vpos.x += Size.x;
            }
            else if (vx > maxX)
            {
                vpos.x -= Size.x;
            }
            if (vy < minY)
            {
                vpos.y += Size.y;
            }
            else if (vy > maxY)
            {
                vpos.y -= Size.y;
            }
            ts.position = mainCamera.ViewportToWorldPoint(vpos);
            
            //            var pos = ts.position;
            //            var curY = pos.y;
            //            if (0.5 - Mathf.Abs(curY) < height)
            //            {
            //                pos.y = curY < 0 ? curY - height : curY + height;
            //            }
            //            transform.position = pos;
        }
    }
}
