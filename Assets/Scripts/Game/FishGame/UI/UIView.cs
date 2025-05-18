using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.UI
{
    public class UIView : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Rect _bound;
        public delegate void UpdateBounddelegate(Rect bound);

        public UpdateBounddelegate UpdateBoundEvent;
        public virtual Rect Bound
        {
            get
            {
                return gameObject.activeSelf ? _bound : new Rect(_bound.x, _bound.y, 0, 0);
            }
        }

        public void Display(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public virtual void SetBound(float x, float y, float w, float h)
        { 
            _bound.Set(x,y,w,h);
            if (UpdateBoundEvent != null) UpdateBoundEvent(_bound);
        }

        public virtual void SetBound(Rect rect)
        {
            SetBound(rect.x,rect.y,rect.width,rect.height);
        }

        public bool IsActivite()
        {
            return gameObject.activeSelf;
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var bound = Bound;
            var v2P = bound.x + bound.width;
            var v3P = bound.y + bound.height;
            var v1 = transform.TransformPoint(new Vector3(bound.x, bound.y, 0));
            var v2 = transform.TransformPoint(new Vector3(v2P, bound.y, 0));
            var v3 = transform.TransformPoint(new Vector3(v2P, v3P, 0));
            var v4 = transform.TransformPoint(new Vector3(bound.x, v3P, 0)); 
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v4);
            Gizmos.DrawLine(v4, v1); 
        }
    }
}
