using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.car
{
    public class CarIconView : MonoBehaviour
    {
        public EventObject EventObj;

        public GameObject Car;
        public GameObject Select;
        public List<GameObject> AllCarIcons;
        public List<GameObject> MoveCircles;
        public List<GameObject> FollowCircles;
        public List<GameObject> WinAears;

        /// <summary>
        /// 当前转了几圈
        /// </summary>
        public int CurTurnCount;
        /// <summary>
        /// 开始时索引
        /// </summary>
        private int _startIndex;
        /// <summary>
        /// 当前索引
        /// </summary>
        private int _curItemsIndex;
        /// <summary>
        /// 计时器
        /// </summary>
        private float _timeCount;
        /// <summary>
        /// 开奖最小摇奖圈数
        /// </summary>
        private int _minTurnCount;
        /// <summary>
        /// 摇奖
        /// </summary>
        private Vector3 _nextPos = Vector3.zero;
        private float _cardSpeed = 0f;
        private float _plushCarSpeed = 900f;
        private float _plushTurnSpeed = 20f;
        private int _plushSpeedCnt = 5;
        private float _tempCarSpeed;
        private float _tempTurnSpeed;
        private float _plushSpeedTime;
        private List<Transform> _carPosList;
        private int _saveTarget = -1;
        private int _curCar;
        private bool _stop;
        private CarStatus _carStatus;

        private enum CarStatus
        {
            Stop,
            SpeedUp,
            MaxSpeed,
            ReadyStop,
            SlowDown
        }

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "MoveCircle":
                    Clear();
                    StartMove();
                    break;
                case "StopCircle":
                    StopMove();
                    break;
                case "Start":
                    StartRoll();
                    break;
                case "Result":
                    StopRoll((RollResult)data.Data);
                    break;
            }
        }
        protected void Start()
        {
            _carPosList = new List<Transform>();
            foreach (GameObject o in AllCarIcons)
            {
                var to = o.transform.GetChild(0);
                _carPosList.Add(to);
                to.parent = Car.transform.parent;
            }
        }
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartRoll();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                StartMove();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                StopMove();
            }
            DrawLottery();
        }

        private void StartMove()
        {
            _stop = false;
            foreach (var circle in MoveCircles)
            {
                circle.SetActive(true);
            }

            IEnumerator ien = MoveLightCircles();
            StartCoroutine(ien);
        }

        IEnumerator MoveLightCircles()
        {
            while (!_stop)
            {
                yield return new WaitForSeconds(1f);
                foreach (var t in MoveCircles)
                {
                    var index = int.Parse(t.name);
                    index++;
                    index = index % 32;
                    t.transform.position = AllCarIcons[index].transform.position;
                    t.name = index.ToString();
                }
            }
        }

        private void StopMove()
        {
            foreach (var circle in MoveCircles)
            {
                circle.SetActive(false);
            }
            _stop = true;
        }


        protected Vector3 GetFrameDis(float dis, Vector3 nextPos, Vector3 curPos)
        {
            Vector3 xl = (nextPos - curPos).normalized;
            int tt = (int)(Math.Abs(xl.x) + Math.Abs(xl.y));
            float x = dis / tt * xl.x;
            float y = dis / tt * xl.y;
            return new Vector3(x, y);
        }

        protected bool IsNext(Vector3 speed, Vector3 nextPos, Vector3 curPos)
        {
            if (Math.Abs(speed.x) > Math.Abs(speed.y))
            {
                if (speed.x > 0)
                {
                    //向右
                    return curPos.x + speed.x >= nextPos.x;
                }
                //向左
                return curPos.x + speed.x <= nextPos.x;
            }
            if (speed.y > 0)
            {
                //向上
                return curPos.y + speed.y >= nextPos.y;
            }
            //向下
            return curPos.y + speed.y <= nextPos.y;
        }

        protected int GetNextPosIndex(int index)
        {
            index++;
            index %= AllCarIcons.Count;
            CurTurnCount += index == _startIndex ? 1 : 0;
            return index;
        }

        protected int GetPerPosIndex(int index)
        {
            return (index - 1 + AllCarIcons.Count) % AllCarIcons.Count;
        }

        protected float GetDisPosToPos()
        {
            int curr = _curItemsIndex;
            int cnt = 0;
            float dis = 0;
            while (cnt < _plushSpeedCnt)
            {
                int next = GetNextPosIndex(curr);
                Vector3 begin = _carPosList[curr].localPosition;
                Vector3 end = _carPosList[next].localPosition;
                dis += Math.Abs(Vector3.Distance(begin, end));
                curr = next;
                cnt++;
            }

            dis += Math.Abs(Vector3.Distance(Car.transform.localPosition, _carPosList[_curItemsIndex].localPosition));
            return dis;
        }

        protected float GetPlushSpeedTime()
        {
            float averageSpeed = _plushCarSpeed / 2 + _cardSpeed;
            float dis = GetDisPosToPos();
            return dis / averageSpeed;
        }

        protected void DrawLottery()
        {

            if (_carStatus != CarStatus.Stop)
            {
                //速度变化
                if (_carStatus == CarStatus.SpeedUp)
                {
                    //加速
                    if ((_timeCount += Time.deltaTime) >= _plushSpeedTime)
                    {
                        _timeCount = _plushSpeedTime;
                        _carStatus = CarStatus.MaxSpeed;
                    }
                    _tempCarSpeed = _cardSpeed + _timeCount / _plushSpeedTime * _plushCarSpeed;
                    _tempTurnSpeed = _tempTurnSpeed + _timeCount / _plushSpeedTime * _plushTurnSpeed;
                }
                else if (_carStatus == CarStatus.SlowDown)
                {
                    if ((_timeCount -= Time.deltaTime) <= 0)
                    {
                        _timeCount = 0;
                    }
                    _tempCarSpeed = _cardSpeed + _timeCount / _plushSpeedTime * _plushCarSpeed;
                    _tempTurnSpeed = _tempTurnSpeed + _timeCount / _plushSpeedTime * _plushTurnSpeed;
                }

                if (_nextPos == Vector3.zero)
                {
                    _curItemsIndex = GetNextPosIndex(_curItemsIndex);
                }

                Vector3 curPos = Car.transform.localPosition;
                _nextPos = _carPosList[_curItemsIndex].localPosition;
                var speed = GetFrameDis(_tempCarSpeed * Time.deltaTime, _nextPos, curPos);
                //根据位子 来计算是否是切换下一个点
                if (IsNext(speed, _nextPos, curPos))
                {
                    if (_carStatus == CarStatus.MaxSpeed && CurTurnCount >= _minTurnCount)
                    {
                        _carStatus = CarStatus.ReadyStop;
                    }
                    //开始减速
                    int jianIndex = (_saveTarget - _plushSpeedCnt + _carPosList.Count) % _carPosList.Count;
                    if (_carStatus == CarStatus.ReadyStop && _curItemsIndex == jianIndex)
                    {
                        _plushSpeedTime = GetPlushSpeedTime();
                        _timeCount = _plushSpeedTime;
                        _carStatus = CarStatus.SlowDown;

                    }
                    //停止
                    if (_carStatus == CarStatus.SlowDown && _curItemsIndex == _saveTarget)
                    {
                        float dic = Vector3.Distance(Car.transform.localPosition,
                        _carPosList[_curItemsIndex].localPosition);
                        speed = GetFrameDis(dic, _nextPos, curPos);
                        _nextPos = Vector3.zero;
                        _carStatus = CarStatus.Stop;

                        MoveFollowCircle();

                        WinAears[_curCar].SetActive(true);
                        Select.transform.position = AllCarIcons[_saveTarget].transform.position;
                        Select.SetActive(true);
                    }
                    else
                    {
                        _curItemsIndex = GetNextPosIndex(_curItemsIndex);
                        MoveFollowCircle();
                    }
                }

                Car.transform.localPosition = Car.transform.localPosition + speed;

                Vector3 curEuler = Car.transform.eulerAngles;
                Vector3 nextEuler = _carPosList[_curItemsIndex].eulerAngles;
                if (curEuler != nextEuler)
                {
                    float tspeed = _tempTurnSpeed * -1 * Time.deltaTime;
                    float nextE = curEuler.z + tspeed;

                    if (nextE <= nextEuler.z)
                    {
                        nextE = nextEuler.z;
                    }

                    if (nextE <= 0)
                    {
                        nextE = 360 + nextE;
                    }

                    Car.transform.eulerAngles = new Vector3(curEuler.x, curEuler.y, nextE);
                }

                //Debug.Log("car status " + _carStatus + " curindex " + _curItemsIndex + " stop index " + _saveTarget);   
            }
        }

        /// <summary>
        /// 开始摇奖
        /// </summary>
        private void StartRoll()
        {
            _minTurnCount = 1;
            CurTurnCount = 0;
            _startIndex = _curItemsIndex;
            _carStatus = CarStatus.SpeedUp;
            _plushSpeedTime = GetPlushSpeedTime();
            _timeCount = 0;
            _plushSpeedCnt = Random.Range(2, 8);
        }
        /// <summary>
        /// 停止摇奖(开奖)
        /// </summary>
        private void StopRoll(RollResult rollData)
        {
            int target = rollData.CarIdx;
            _saveTarget = target;
            _curCar = rollData.Car;

            for (int i = 0; i < FollowCircles.Count; i++)
            {
                FollowCircles[i].SetActive(false);
            }

            if (CurTurnCount < _minTurnCount)
            {
                return;
            }
            _carStatus = CarStatus.ReadyStop;

        }


        private void MoveFollowCircle()
        {
            for (int i = 0; i < FollowCircles.Count; i++)
            {
                FollowCircles[i].SetActive(true);
                var index = int.Parse(FollowCircles[i].name);
                index = index + 1;
                index = index % 32;
                FollowCircles[i].transform.position = AllCarIcons[index].transform.position;
                FollowCircles[i].name = index.ToString();
            }
        }

        private void Clear()
        {
            WinAears[_curCar].SetActive(false);
            Select.SetActive(false);
            foreach (var t in FollowCircles)
            {
                t.SetActive(false);
            }
        }
    }

}
