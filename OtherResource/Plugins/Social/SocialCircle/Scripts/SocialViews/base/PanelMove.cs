using UnityEngine;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    /// <summary>
    /// panel 平移组件
    /// </summary>
    public class PanelMove : MonoBehaviour
    {
        /// <summary>
        /// 当前对象
        /// </summary>
        public static PanelMove Current;

        public delegate void OnFinished();
        /// <summary>
        /// 目标位置
        /// </summary>
        public Vector3 Target = Vector3.zero;
        /// <summary>
        /// 移动强度（可以理解为快慢）
        /// </summary>
        public float Strength = 10f;
        /// <summary>
        /// 完成回调
        /// </summary>
        public OnFinished OnFinishAction;
        UIPanel _mPanel;
        Transform _mTrans;
        UIScrollView _mDrag;
        private Vector3 _movePer;
        private float _minDistance;

        public void Ready()
        {
            enabled = true;
            _mPanel = GetComponent<UIPanel>();
            _mDrag = GetComponent<UIScrollView>();
            _mTrans = transform;
            _movePer = (Target - transform.localPosition) * Strength / 1000;
            _minDistance = (Vector3.zero - _movePer).sqrMagnitude;
        }

        protected virtual void AdvanceTowardsPosition()
        {
            bool trigger = false;
            Vector3 before = _mTrans.localPosition;
            Vector3 after = before+ _movePer;
            if ((after - Target).sqrMagnitude< _minDistance)
            {
                after = Target;
                enabled = false;
                trigger = true;
            }
            _mTrans.localPosition = after;
            Vector3 offset = after - before;
            Vector2 cr = _mPanel.clipOffset;
            cr.x -= offset.x;
            cr.y -= offset.y;
            _mPanel.clipOffset = cr;
            if (_mPanel.onClipMove!=null)
            {
                _mPanel.onClipMove(_mPanel);
            }

            if (_mDrag != null)
            {
                _mDrag.UpdateScrollbars(false);
                if (_mDrag.onMomentumMove!=null)
                {
                    _mDrag.onMomentumMove();
                }
            }
            if (trigger && OnFinishAction != null)
            {
                Current = this;
                OnFinishAction();
                Current = null;
            }
        }

        private void Update()
        {
            AdvanceTowardsPosition();
        }

        public static PanelMove Begin(GameObject go, Vector3 pos, float time, OnFinished finishCall=null)
        {
            PanelMove sp = go.GetComponent<PanelMove>();
            if (sp == null) sp = go.AddComponent<PanelMove>();
            sp.Target = pos;
            sp.Strength = time;
            sp.OnFinishAction = finishCall;
            sp.Ready();
            return sp;
        }
    }
}
