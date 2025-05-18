using System.Collections;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.SportsCarClub
{
    /// <summary>
    /// 转盘
    /// </summary>
    public class Wheel : MonoBehaviour
    {
        private static Wheel _instance;

        public static Wheel GetInstance()
        {
            return _instance ?? (_instance = new Wheel());
        }

        void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// 转盘对象
        /// </summary>
        public GameObject[] Items;
        /// <summary>
        /// 
        /// </summary>
        public BetType[] ItemsValue;
        /// <summary>
        /// 转转对象
        /// </summary>
        public GameObject Selected;
        /// <summary>
        /// 是否开始转
        /// </summary>
        private bool isTurn;
        /// <summary>
        /// 最小时间间隔
        /// </summary>
        public float MinTime;
        /// <summary>
        /// 最大时间间隔
        /// </summary>
        public float MaxTime;
        /// <summary>
        /// 当前时间间隔
        /// </summary>
        public float CurTimeSpace;

        // Use this for initialization
        void Start ()
        {
            Selected.transform.position = Items[_curItemsIndex].transform.position;
            MinStopDistance = (int)(Items.Length*0.25f);
            InitItem();
        }
	
        // Update is called once per frame
        void Update ()
        {
            DrawLottery();
        }
        /// <summary>
        /// 初始化item
        /// </summary>
        public void InitItem()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                UISprite icon = Items[i].transform.FindChild("BG").FindChild("Icon").GetComponent<UISprite>();
                icon.spriteName = "car_" + (int)ItemsValue[i % ItemsValue.Length];
                icon.MakePixelPerfect();
                icon.transform.localScale = new Vector3(0.5f,0.5f,1f);
            }
        }

        /// <summary>
        /// 当前转了几圈
        /// </summary>
        public int CurTurnCount;

        //void OnGUI()
        //{
        //    if (GUI.Button(new Rect(0,0,100,50),"Start" ))
        //    {
        //        StartTrun();
        //    }

        //    if (GUI.Button(new Rect(0, 50, 100, 50), "Stop"))
        //    {
        //        StopTrun(22);
        //    }
        //}

        /// <summary>
        /// 开始时索引
        /// </summary>
        private int startIndex;
        /// <summary>
        /// 开奖最小摇奖圈数
        /// </summary>
        private int _minTurnCount;

        /// <summary>
        /// 开始摇奖
        /// </summary>
        public void StartTrun()
        {
            isTurn = true;
            _minTurnCount = Random.Range(2, 4);
            CurTurnCount = 0;
            Selected.gameObject.SetActive(true);
            startIndex = _curItemsIndex;
            CurTimeSpace = MinTime;
            _increasingTime = 0f;
            _isReadyStop = false;
            _curStopDistance = 0;
            _increasingDis = 0;

            //操控仪表指针、方向盘变化
            CarSpeedPoint.GetInstance().CarPointStatus = CarSpeedPoint.Status.speedUp;
            Facade.Instance<MusicManager>().Play("CarEngine");
            SteeringWheel.GetInstance().DoRotate();
        }

        private int _saveTarget = -1;

        /// <summary>
        /// 停止摇奖(开奖)
        /// </summary>
        /// <param name="target">传入-1直接停止选中消失,不为-1为正常开奖</param>
        public void StopTrun(int target = -1, CschGameManager.NoParamDelegate OnFinish = null)
        {
            if (target == -1)
            {
                isTurn = false;
                Selected.gameObject.SetActive(false);
            }
            else
            {
                if (OnFinish != null)
                    DrawFinishDelegate = OnFinish;

                if (CurTurnCount < _minTurnCount)
                {
                    _saveTarget = target;
                    return;
                }

                _increasingDis = Random.Range(MinStopDistance, MinStopDistance + 5);


                _stopDistance = target <= _curItemsIndex ? target + Items.Length - _curItemsIndex : target - _curItemsIndex;
                _stopDistance += _stopDistance < _increasingDis ? Items.Length : 0;
                _increasingTime = (MaxTime - CurTimeSpace)/ _increasingDis;
                _isReadyStop = true;

                //操控仪表指针、方向盘变化
                CarSpeedPoint.GetInstance().CarPointStatus = CarSpeedPoint.Status.speedDown;
                CarSpeedPoint.GetInstance().allowShake = true;
                SteeringWheel.GetInstance().DoRotate(true);
            }
        }

        private int _increasingDis;
        //递增的time值
        private float _increasingTime;
        //计时器
        private float _timeCount = 0f;
        //当前索引
        private int _curItemsIndex = 0;
        //准备停止应该走多少格
        private int _stopDistance;
        //准备停止时实际走了多少格
        private int _curStopDistance;
        //是否准备停止
        private bool _isReadyStop;

        //最小的停止距离(索引距离)
        [HideInInspector]
        public int MinStopDistance;
        /// <summary>
        /// 摇奖结束后代理
        /// </summary>
        public CschGameManager.NoParamDelegate DrawFinishDelegate; 
        /// <summary>
        /// 摇奖
        /// </summary>
        private void DrawLottery()
        {
            if (isTurn)
            {
                _timeCount += Time.deltaTime;

                if (_timeCount >= CurTimeSpace)
                {
                    Facade.Instance<MusicManager>().Play("Move");
                    _timeCount = 0f;
                    //CurTimeSpace += _increasingTime;
                    //CurTimeSpace = CurTimeSpace > MaxTime ? MaxTime : CurTimeSpace;
                    _curItemsIndex++;
                    _curItemsIndex %= Items.Length;
                    CurTurnCount += _curItemsIndex == startIndex ? 1 : 0;
                    Selected.transform.position = Items[_curItemsIndex].transform.position;
                    if (_isReadyStop)
                    {
                        _curStopDistance ++;
                        if (_stopDistance - _curStopDistance <= _increasingDis)
                        {
                            CurTimeSpace += _increasingTime;
                            CurTimeSpace = CurTimeSpace > MaxTime ? MaxTime : CurTimeSpace;
                        }

                        if (_stopDistance == _curStopDistance)
                        {
                            isTurn = false;
                            Selected.GetComponent<TweenAlpha>().PlayForward();
                            if (DrawFinishDelegate != null)
                                DrawFinishDelegate();

                            StartCoroutine(SelectedPerticleSysCtrl());
                        }
                    }

                    if (CurTurnCount >= _minTurnCount && _saveTarget >= 0)
                    {
                        StopTrun(_saveTarget);
                        _saveTarget = -1;
                    }
                }
            }
        }

        private IEnumerator SelectedPerticleSysCtrl()
        {
            var go = transform.Find("chehang-win").gameObject;
            go.transform.position = Selected.transform.position;
            go.SetActive(true);

            yield return new WaitForSeconds(3.5f);
            go.SetActive(false);
        }
    }
}
