using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class EfFishLocker : MonoBehaviour { 
        public tk2dSprite SprCard;
        private Transform _tsCard;
        private Vector3 _localPos;
        private float _rotateAngle;

        public float RotateOffset = 4.42762752F;
        public float RotateSpd = 640F;

        protected void Start () 
        {
            _tsCard = transform;
            _localPos = transform.localPosition;
        }

        public void Show(string lockName)
        {
            if (string.IsNullOrEmpty(lockName)) return;
            var flag = SprCard.SetSprite(lockName);
            gameObject.SetActive(flag);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        protected void Update()
        {
            _tsCard.localPosition = _localPos + new Vector3(RotateOffset * Mathf.Sin(_rotateAngle * Mathf.Deg2Rad), RotateOffset * Mathf.Cos(_rotateAngle * Mathf.Deg2Rad), 0F);
            _rotateAngle += Time.deltaTime * RotateSpd;
        }

    }
}
