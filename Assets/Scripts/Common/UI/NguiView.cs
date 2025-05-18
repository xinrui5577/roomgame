using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.UI
{
    public class NguiView : YxView
    { 
        protected override void OnStart()
        {
            var widgetAdapter = GetWidgetAdapter();
            if (widgetAdapter == null)
            {
                gameObject.AddComponent<NguiWidgetAdapter>();
            }
            base.OnStart();
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            UpdateWidget();
        }

        private void OnDrawGizmos()
        {
            var b = Bounds;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0,0,205);
            Gizmos.DrawWireCube(new Vector3(b.center.x, b.center.y, b.min.z), new Vector3(b.size.x, b.size.y, 0f));
        }
    }
}
