using System;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.PokerCdCtrl
{
    public class LinkMove : MonoBehaviour
    {
        public float MaxDistance;

        private Transform _lastNode;

        private bool _nodeMove;

        private int _width;

        public Vector3 _target;

        private Action MoveFuction;

        // Update is called once per frame
        protected void Update ()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                testMove = true;
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                transform.localPosition = new Vector3(0, -180, 0);
                testMove = false;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                var tp = GetComponent<TweenPosition>();
                if (tp != null)
                {
                    tp.ResetToBeginning();
                    tp.PlayForward();
                }
            }

       
            MoveFuction();
        
            TestMove();
        }

        private bool testMove;
        void TestMove()
        {
            if (!testMove) return;
            var v3 = Vector3.MoveTowards(transform.localPosition, _target, MaxDistance);
            transform.localPosition = v3;
        }


        public void Move()
        {
            if (!_nodeMove) return;

            var baseV3 = _lastNode.localPosition;
            var v3 = Vector3.MoveTowards(transform.localPosition, _target, MaxDistance);

            float dic = Vector3.Distance(baseV3, v3);
            Debug.LogError(dic);
            if (dic > _width)
            {
                v3 = new Vector3(baseV3.x - _width, v3.y, v3.z);
            }
            transform.localPosition = v3;
            if (v3.x <= _target.x)
            {
                transform.localPosition = _target;
                StopMove();
            }
        }

        public void Set(int width, Vector3 target, Transform lastNode = null)
        {
            _lastNode = lastNode;
            _width = width;
            _target = target;
        }

        public void SetNode(int width,Vector3 target, Transform lastNode = null)
        {
            if (lastNode == null) return;
            _lastNode = lastNode;
            _width = width;
            _target = target;
        }


        public void BeginMove(Vector3 target,float duration ,EventDelegate onfinish = null)
        {
            var tp = GetComponent<TweenPosition>();
            tp.from = transform.localPosition;
            tp.to = target;
            tp.onFinished = new System.Collections.Generic.List<EventDelegate> {onfinish};

            tp.ResetToBeginning();
            tp.PlayForward();
        }

        public void Shift()
        {
            if (_lastNode == null) return;
            var lastV = _lastNode.localPosition;
            var tempTarget = new Vector3(lastV.x - _width, lastV.y, lastV.z);
        }

        public void ShiftFirst()
        {
            var v3 = Vector3.MoveTowards(transform.localPosition, _target, MaxDistance);
            transform.localPosition = v3;
        }

        public void BeginMove()
        {
            if (_lastNode == null)
            {
                YxDebug.LogError("No last node !!");
                MoveFuction = ShiftFirst;
            }
            else
            {
                MoveFuction = Shift;
            }
            
            enabled = _nodeMove = true;
        }

        public void StopMove()
        {
            _nodeMove = enabled = false;
        }
    }
}
