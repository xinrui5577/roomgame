using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.UI
{
    public class SizeFollowWidget : MonoBehaviour
    {
        /// <summary>
        /// 跟随对象
        /// </summary>
        public UIWidget FollowTarget;
        /// <summary>
        /// 偏移量
        /// </summary>
        public Vector2 Offset;
        /// <summary>
        /// 变化对象
        /// </summary>
        private UIPanel panel;
        /// <summary>
        /// 是否跟随
        /// </summary>
        public bool IsFollow;

        // Use this for initialization
        void Start () {
            if (FollowTarget == null)
            {
                YxDebug.Log("跟随对象为空,请设置跟随对象");
                IsFollow = false;
                return;
            }

            panel = transform.GetComponent<UIPanel>();

            if (panel == null)
            {
                YxDebug.Log("此脚本只能应用在UIPanel上!");
                IsFollow = false;
                return;
            }

            Vector4 region = new Vector4(panel.baseClipRegion.x, panel.baseClipRegion.y,
                FollowTarget.localSize.x + Offset.x >= 1f ? FollowTarget.localSize.x + Offset.x:1f,
                FollowTarget.localSize.y + Offset.y >= 1f ? FollowTarget.localSize.y + Offset.y:1f);
            panel.baseClipRegion = region;

        }
	
        // Update is called once per frame
        void Update () {

            if (IsFollow)
            {
                if (!Mathf.Approximately(panel.baseClipRegion.z,FollowTarget.localSize.x) || !Mathf.Approximately(panel.baseClipRegion.w, FollowTarget.localSize.y))
                {
                    Vector4 region = new Vector4(panel.baseClipRegion.x, panel.baseClipRegion.y,
                        FollowTarget.localSize.x + Offset.x >= 1f ? FollowTarget.localSize.x + Offset.x : 1f,
                        FollowTarget.localSize.y + Offset.y >= 1f ? FollowTarget.localSize.y + Offset.y : 1f);
                    panel.baseClipRegion = region;
                }
            }

        }
    }
}
