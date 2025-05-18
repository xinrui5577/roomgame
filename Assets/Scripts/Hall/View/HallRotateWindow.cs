using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine; 
using com.yxixia.utile.Utiles;

namespace Assets.Scripts.Hall.View
{
    public class HallRotateWindow : YxNguiWindow
    { 
        /// <summary>
        /// 提示
        /// </summary>
        public GameObject HandTip;
        /// <summary>
        /// 最大速度
        /// </summary>
        public float MaxSpeed;
        /// <summary>
        /// 转盘或者转针
        /// </summary>
        public Transform Dial;
        /// <summary>
        /// 辅助箭头
        /// </summary>
        public Transform AssistArrow;
        /// <summary>
        /// 最大角速度(角度)
        /// </summary>
        public float MaxAngleSpeed = -240;
        /// <summary>
        /// 加速度(角度)
        /// </summary>
        public float AngleAccelerated = -30;
        /// <summary>
        /// 奖励区域
        /// </summary>
        public int[] RewardArea;

        private float _rewardIndex;
        private State _curState;
        private enum State
        {
            Normal,
            SpeedUp,
            Wait,
            SpeedDown,
            Stop
        }

        protected override void OnAwake()
        {
            InitStateTotal = 2;
//            Facade.Instance<TwManager>().SendAction("",new Dictionary<string,object>(),UpdateView);
        }

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var dict = Data as Dictionary<string, object>;
            if (dict == null) return;

            _canLaunch = true;
        }

        private bool _canLaunch = true;
        public void OnLaunch()
        {
            if (!_canLaunch) return;
            _canLaunch = false;
            if (HandTip!=null)HandTip.SetActive(false);
//            Facade.Instance<TwManager>().SendAction("",new Dictionary<string,object>(),OnGetReward);
            _curState = State.SpeedUp;
        }

        private void OnGetReward(object msg)
        {

        }

        private void FixedUpdate()
        {
            switch (_curState)
            {
                case State.SpeedUp:
                    SpeedUp();
                    break;
                case State.SpeedDown:
                    SpeedDown();
                    break;
                case State.Stop:
                    OnStop();
                    break;
                case State.Wait:
                    Waitting();
                    break;
            }
            AssistArrowRotate();
        }

        private void OnStop()
        {
//            Debug.Log(_rewardIndex);
        }

        private float _waitCount;
        private void Waitting()
        {
            if (Mathf.Abs(_waitCount) > 360)
            {
//              if()//todo 收到奖励 
                _rewardIndex = 29;
                var angle = _rewardIndex*12;//奖励角度
                var l = _lastSpeed - AngleAccelerated * 1.5f * 1.5f / 2;//总长度
                var cur = (angle - l) % 360;
                var curZ = Dial.localEulerAngles.z;
                if (Mathf.Abs(curZ - cur) < Mathf.Abs(AngleAccelerated))
                {
                    _curState = State.SpeedDown;
                    return;
                }
            }
            var z = _lastSpeed * Time.deltaTime;
            _waitCount += z;
            Dial.Rotate(0, 0, z, Space.Self);
        }

        /// <summary>
        /// 获取奖励
        /// </summary>
        /// <returns></returns>
        private void GetReward()
        {
            _curState = State.SpeedDown;
        }

        private float _curPassTime;
        private float _lastSpeed;
        /// <summary>
        /// 提速 v=rw a=v²/r     a=v²/r=（rw）²/r=rw²   a(n) = 4π²R/T²    a(n)=ω²·r
        /// </summary>
        /// <returns></returns>
        private void SpeedUp()
        {
            _lastSpeed = ChangeAngular(AngleAccelerated, _lastSpeed, Dial);
            if (Mathf.Abs(_lastSpeed) < Mathf.Abs(MaxAngleSpeed)) { return; }
            _curState = State.Wait;//SpeedDown;// 
            _waitCount = 0;
        }

        /// <summary>
        /// 减速
        /// </summary>
        /// <returns></returns>
        private void SpeedDown()
        {
            _lastSpeed = ChangeAngular(-AngleAccelerated, _lastSpeed, Dial);
            if (GlobalUtile.IsSameSign(_lastSpeed, MaxAngleSpeed)) { return; }
            _lastSpeed = 0;
            _curState = State.Stop;
        }

        /// <summary>
        /// 更改角度
        /// </summary>
        /// <param name="accelerated">加速度（角度）</param>
        /// <param name="v0">初速度（角度）</param>
        /// <param name="ts">旋转对象</param>
        /// <param name="rate"></param>
        /// <returns></returns>
        private static float ChangeAngular(float accelerated, float v0, Transform ts, float rate = 1)
        {
            var t = Time.deltaTime;
            var v1 = v0 + t * accelerated;
            var z = (v0 + v1) * t / 2;
            ts.Rotate(0, 0, z * rate, Space.Self);
            return v1;
        }

        private int _curDialTurnCount;
        private float _lastDialAngles;
        private float _curArrowSpeed;
        /// <summary>
        /// T=2π√(L/g)
        /// </summary>
        private void AssistArrowRotate()
        {
            if (AssistArrow == null) return;
            var angles = AssistArrow.localEulerAngles;// 箭头的角度
            var pz = Dial.localEulerAngles.z;//转盘的角度  
            var tAngles = pz > 180 ? (int)(pz - 360) : (int)(pz); //盘转动的个数
            var pa = tAngles % 12; //盘的角度
            if (Mathf.Abs(_lastSpeed) > 12)
            {
                if (Mathf.Abs(pa) > 6)
                {
                    angles.z = 60;
                    AssistArrow.localEulerAngles = angles;
                    return;
                }
            }
            
            var jz = angles.z; //箭头的角度
            //自由下落
            var asp = Mathf.Sin(jz * Mathf.Deg2Rad) * 10;
            _curArrowSpeed = ChangeAngular(-asp, _curArrowSpeed, AssistArrow, Mathf.Rad2Deg) * 0.98f;
            //判断是否碰撞

            var maxAngles = pa + 6;//左边最大角 
            jz = jz > 180 ? jz - 360 : jz;
            if (jz >=0)
            {
                if (maxAngles > 0)
                {
                    return;
                } 
            }
            else
            {
                if (maxAngles < 0)
                {
                    return;
                } 
            }

            var w = maxAngles*Mathf.Deg2Rad;
            var db = Mathf.Sin(w)*330;
            var lb = Mathf.Cos(w) * 330;
            var jlb = 376 - lb;
            var jw = Mathf.Atan(db/jlb);
            var tjz = jw * Mathf.Rad2Deg;
            Debug.Log(w + "  |   " + db + "  |   " + lb + "  |   " + jlb + "  |   " + jw + "   |   " + tjz);
            if (jz > -tjz)//表示没有碰撞
            {
                return;
            }
            //被挡
            _curArrowSpeed = 0;
            angles.z = -tjz;
            AssistArrow.localEulerAngles = angles; 
        }
    }
} 