using Assets.Scripts.Game.lswc.Data;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.lswc
{
    public class LSBigFourType : MonoBehaviour
    {
        private Animation _curAnimation;

        private  Vector3 _from=new Vector3(0,100,0);

        private Vector3 _to=new Vector3(0,20,0);

        private float _aniSpeed = 0.5f;

        private float _moveTime = 1;

        public void Init()
        {
            Find();
            HideBigFour();
        }

        private void Find()
        {
            _curAnimation = GetComponent<Animation>();
        }

        public void PlayAnimation()
        {
            Sequence mySequence = DOTween.Sequence();
            gameObject.SetActive(true);
            Tweener t=transform.DOLocalMove(_to,_moveTime);
            mySequence.Append(t);
            _curAnimation[LSConstant.Animation_BigFourTypeRotate].speed = _aniSpeed;
            //LSTurnTableControl.Instance.PlayAnimation();
        }

        public void HideBigFour()
        {
            gameObject.SetActive(false);
            transform.localPosition = _from;
        }
    }
}
