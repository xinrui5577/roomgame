using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Item;
using com.yxixia.utile.YxDebug;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Game.lswc
{
    public class LSCameraManager : InstanceControl
    {
        Vector3 _outRotation = new Vector3(60, -90, 0);

        Vector3 _outPosition = new Vector3(70, 100, 5);

        Vector3 _moveDownPosition = new Vector3(40, 30, 0);

        Vector3 _moveDownRotation = new Vector3(15, -90, 0);

        private Vector3 _startPosition;

        private Vector3 _startRotation;

        private Vector3 _startParentPosition;

        private Vector3 _startParentRotation;

        private float _animalMoveDely = 0.1f;

        private void Awake()
        {
            _startPosition = transform.localPosition;
            _startRotation = transform.localEulerAngles;
            _startParentPosition = transform.parent.transform.localPosition;
            _startParentRotation = transform.parent.transform.localEulerAngles;
        }

        public void Reset()
        {
            transform.localPosition = _startPosition;
            transform.localEulerAngles = _startRotation;
            transform.parent.transform.localPosition = _startParentPosition;
            transform.parent.transform.localEulerAngles = _startParentRotation;
        }

        public void ZoomOut(float time)
        {
            var tPos = transform.DOLocalMove(_outPosition, time);
            var tRot = transform.DOLocalRotate(_outRotation, time);
            var sequence = DOTween.Sequence();
            sequence.Append(tPos);
            sequence.Join(tRot);
        }

        public bool IsMoving = false;

        public void RotateToPosition(float angle, float time)
        {
            IsMoving = true;
            var tweener = transform.parent.DOLocalRotate(new Vector3(0, angle, 0), time).SetEase(Ease.OutQuart).SetDelay(_animalMoveDely);
            tweener.OnComplete(delegate()
            {
                IsMoving = false;
            });
        }

        public float GetAngle(int pos)
        {
            YxDebug.LogError("相机需要移动的位置是：" + pos);
            float result;
            if (pos == 0)
            {
                result = 0;
            }
            else if (pos >= 18)
            {
                result = pos * -15;
            }
            else if (pos > 10)
            {
                result = (24 - pos) * 15;
            }
            else
            {
                result = pos * 15;
            }
            return result;
        }

        private float _lookAnimalTime = 0.5f;

        public void MoveDown(float time, float delyTime, LSAnimalItem item)
        {
            IsMoving = true;
            var mySquence = DOTween.Sequence();
            mySquence.AppendInterval(delyTime);
            if (item != null)
            {
                var tLookTween = transform.DOLookAt(item.transform.position, 0.5f);
                mySquence.Append(tLookTween);
            }
            var tPos = transform.DOLocalMove(_moveDownPosition, time);
            mySquence.Append(tPos);
            var tEuler = transform.DOLocalRotate(_moveDownRotation, time);
            mySquence.Join(tEuler);
            mySquence.OnComplete(delegate()
            {
                IsMoving = false;
            });
        }

        //public float GetRotateAngle(int pos)
        //{
        //    int oppos = (pos + 36)%24;
        //    float result;
        //    if(oppos==0)
        //    {
        //        result=0;
        //    }
        //    else if(oppos>=18)
        //    {
        //        result=oppos*-15;
        //    }
        //    else if (oppos > 10)
        //    {
        //        result=(24 - oppos)*15;
        //    }
        //    else
        //    {
        //        result=oppos*15;
        //    }
        //    Debug.LogError("目前的指针位置是 "+pos+"对面的位置是："+oppos+" 应该旋转的角度是："+result);
        //    return result;            
        //}
        public override void OnExit()
        { 
        }
    }
}
