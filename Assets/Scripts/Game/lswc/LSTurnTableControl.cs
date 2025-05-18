using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using UnityEngine;

namespace Assets.Scripts.Game.lswc
{
    public class LSTurnTableControl :InstanceControl
    {
        private LSRotate _rotate;

        private Animation _animation;

        private int _nowPosition = 0;
        // Use this for initialization
        void Start () {
	        FindChild();
        }

        private void FindChild()
        {
            _animation = transform.FindChild("zhizhen").GetComponent<Animation>();
            _rotate = GetComponent<LSRotate>();
            if (_rotate==null)
            {
                _rotate = gameObject.AddComponent<LSRotate>();
            }
        }

        public void SetPointPosition(int postion)
        {
            float angle = GetTotalAngles(postion);
            transform.localEulerAngles=new Vector3(0,angle,0);
        }

        public float GetTotalAngles(float angle)
        {
            //float angle=((int)360/GetAreaNumber())*postion;
            //angle += LSConstant.Num_RotateNumber*360;
            angle+= LSConstant.Num_RotateNumber*360;
            return angle;
        }

        private int GetAreaNumber()
        {
            return LSConstant.Num_TurnTable_Normal;
        }

        /// <summary>
        /// 正常播放展示动画
        /// </summary>
        public void PlayAnimation()
        {
            _animation[LSConstant.Animation_DisplayTurnTable].normalizedTime = 0;
            _animation[LSConstant.Animation_DisplayTurnTable].speed = 0.5f;
            _animation.Play(LSConstant.Animation_DisplayTurnTable);
        }

        /// <summary>
        /// 重置动画
        /// </summary>
        public void ResetAnimation()
        {
            _animation[LSConstant.Animation_DisplayTurnTable].normalizedTime = 1;
            _animation[LSConstant.Animation_DisplayTurnTable].speed = -1;
            _animation.Play(LSConstant.Animation_DisplayTurnTable);
        }

        public void Rotate(float angle,float time)
        {
            angle = GetTotalAngles(angle);
            //Debug.LogError("指针的旋转角度是" + angle);
            _rotate.StartRotate(time-10,LSConstant.RotationTime-10,angle,0);
        }

        public void QuickRoate(float angle)
        {
            transform.localEulerAngles=new Vector3(0,angle,0);
        }

        public override void OnExit()
        {
        }
    }
}
