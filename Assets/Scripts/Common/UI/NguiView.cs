using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.UI
{
    public class NguiView : YxView { 
        /// <summary>
        /// 间距,x:左 y:上 z:右 w:下
        /// </summary>
        [Tooltip("间距,x:左 y:上 z:右 w:下")]
        public Quaternion Padding;

        protected UIWidget _uiWidget;

        protected override void OnStart()
        {
            base.OnStart();
            _uiWidget = GetComponent<UIWidget>();
        }
        /// <summary>
        /// 区域
        /// </summary>
        public Bounds Bounds
        {
            get
            {
                return NGUIMath.CalculateRelativeWidgetBounds(transform, transform);
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            UpdateWidget();
        }

        private Vector2 _boundSize;
        public Vector2 BoundSize
        {
            get { return _boundSize; }
        }

        [ContextMenu("Execute")]
        public virtual Vector2 UpdateWidget(int width=0,int height=0)
        {
            if (_uiWidget == null) return Vector2.zero;
            var size = Bounds.size;
            if (width > 0) size.x = width;
            if (height > 0) size.y = height;
            _uiWidget.width = (int)size.x;
            _uiWidget.height = (int)size.y;
            _boundSize = size;
            return size;
        }
         
        private void OnDrawGizmos()
        {
            var b = Bounds;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1f, 1f, 1f);
            Gizmos.DrawWireCube(new Vector3(b.center.x, b.center.y, b.min.z), new Vector3(b.size.x, b.size.y, 0f));
        }
    }
}
