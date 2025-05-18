using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class GameMoveManager : MonoBehaviour
    {
        [HideInInspector]
        public int _speedTimes = 20;
        [HideInInspector]
        public bool IsStartRollJetton = false;

        /// <summary>
        /// 每列父物体的名字前缀
        /// </summary>
        public string LineName = "Line_0";
        /// <summary>
        /// 共有几列图片
        /// </summary>
        public int LineNum = 3;
        /// <summary>
        /// 每列中图片名字的前缀
        /// </summary>
        public string PosName = "pos_0";
        /// <summary>
        /// 每列中图片的数量
        /// </summary>
        public int PosNum = 4;
        /// <summary>
        /// 图集中筹码图片的名字前缀
        /// </summary>
        public string SpriteName = "main_";
        /// <summary>
        /// 初始速度
        /// </summary>
        public float StartSpeed = 30;
        /// <summary>
        /// 最大速度
        /// </summary>
        public float ConstantSpeed = 50;
        /// <summary>
        /// 最后滚动次数
        /// </summary>
        public int EndTime = 6;
        protected List<GameObject>[] AllChild;
        protected List<Vector3>[] AllChildPos;
        

        protected float dis = 0;//图片的Y到什么位置时为一个轮回
        //需要重置的属性
        protected float _speed;//三列初始滚动速度
        protected float[] Speeds;//各列最大匀速时的速度  初步定为50
        protected int[] EndRollNums;//各列速度减到一定程度时每列会在滚动6次
        protected float _totalTime;//计时
        protected bool _isUp = true;//是否为加速阶段

        protected float[] HoldTimes;//各列匀速的时间初始0.5s每有一列增加1s

        protected virtual void Start()
        {
            _speed = StartSpeed;
            InitvalHoldTime();
            InitRollData();
        }

        #region 移动图片并自动停止
        protected virtual void FixedUpdate()
        {
            if (IsStartRollJetton)
            {
                if (_speed < Speeds[0] && _isUp)
                {
                    _speed += 0.2f;
                    MoveIcon();
                }
                else
                {
                    _isUp = false;
                    _totalTime += Time.deltaTime;
                    for (int i = 0; i < HoldTimes.Length; i++)
                    {
                        if (_totalTime >= HoldTimes[i])
                        {
                            if (Speeds[i] > 30)
                            {
                                Speeds[i] -= 0.2f;
                                MoveIcon(i, Speeds[i]);
                            }
                            else
                            {
                                MoveLineIconInEnd(i, i);
                            }
                        }
                        else
                        {
                            MoveIcon(i, i);
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 各列图片共同运动
        /// </summary>
        protected virtual void MoveIcon()
        {
            for (int i = 0; i < AllChild.Length; i++)
            {
                for (int j = 0; j < AllChild[i].Count; j++)
                {
                    if (AllChild[i][j].GetComponent<MoveJettonIcon>().Move(_speed * Time.deltaTime * _speedTimes))
                    {
                        ResetJettonPos(i);
                        ChangeJettonOrder(AllChild[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 从给的参数开始,剩下的列共同运动
        /// </summary>
        protected virtual void MoveIcon(int num, int line)
        {
            if (num >= AllChild.Length)
                return;
            for (int i = num; i < AllChild.Length; i++)
            {
                for (int j = 0; j < AllChild[i].Count; j++)
                {
                    if (AllChild[i][j].GetComponent<MoveJettonIcon>().Move(Speeds[line] * Time.deltaTime * _speedTimes))
                    {
                        ResetJettonPos(i);
                        ChangeJettonOrder(AllChild[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 根据给个参数,特定的列以特定的速度运动
        /// </summary>
        protected virtual void MoveIcon(int num, float speed)
        {
            if (num >= AllChild.Length)
                return;
            for (int j = 0; j < AllChild[num].Count; j++)
            {
                if (AllChild[num][j].GetComponent<MoveJettonIcon>().Move(speed * Time.deltaTime * _speedTimes))
                {
                    ResetJettonPos(num);
                    ChangeJettonOrder(AllChild[num]);
                }
            }
        }
        /// <summary>
        /// 每列最后阶段倒数几轮的滚动
        /// </summary>
        protected virtual void MoveLineIconInEnd(int num, int line)
        {
            if (EndRollNums[num] > 4)
            {
                for (int j = 0; j < AllChild[num].Count; j++)
                {
                    if (AllChild[num][j].GetComponent<MoveJettonIcon>().Move(Speeds[line] * Time.deltaTime * _speedTimes))
                    {
                        EndRollNums[num]--;
                        ResetJettonPos(num);
                        ChangeJettonOrder(AllChild[num]);
                    }
                }
            }
            else if (EndRollNums[num] >= 1 && EndRollNums[num] <= 4)
            {
                for (int j = 0; j < AllChild[num].Count; j++)
                {
                    if (AllChild[num][j].GetComponent<MoveJettonIcon>().Move(Speeds[line] * Time.deltaTime * _speedTimes))
                    {
                        EndRollNums[num]--;
                        ResetJettonPos(num);
                        if (EndRollNums[num] != 0)
                        {
                            var _response = App.GetGameData<OverallData>().Response;
                            AllChild[num][0].GetComponent<UISprite>().spriteName = _response.GetJettonName(SpriteName);
                        }
                        else
                        {
                            ChangeJettonOrder(AllChild[num]);
                            if (num == HoldTimes.Length - 1)
                            {
                                InitData();
                                EventDispatch.Dispatch((int)EventID.GameEventId.WhenIconStop);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 把滚出边界的图片移动到列表的队首,并初始他们的位置
        /// </summary>
        protected virtual void ResetJettonPos(int i)
        {
            GameObject go = AllChild[i][AllChild[i].Count - 1];
            AllChild[i].Remove(go);
            AllChild[i].Insert(0, go);
            for (int j = 0; j < AllChild[i].Count; j++)
            {
                AllChild[i][j].transform.localPosition = AllChildPos[i][j];
            }
        }

        /// <summary>
        /// 随机改变图片
        /// </summary>
        protected virtual void ChangeJettonOrder(List<GameObject> go)
        {
            string newName = RandonSpriteName();
            go[0].GetComponent<UISprite>().spriteName = newName;
        }

        /// <summary>
        /// 确定最后显示的图片
        /// </summary>
        protected virtual void ShowStopSprite(List<GameObject> go)
        {
            var _response = App.GetGameData<OverallData>().Response;
            go[0].GetComponent<UISprite>().spriteName = _response.GetJettonName(SpriteName);//这个图片的顺序是从下到上从左到右一列一列显示的
        }

        /// <summary>
        /// 随机图片名
        /// </summary>
        /// <returns></returns>
        protected virtual string RandonSpriteName()
        {
            return null;
        }
        #endregion

        #region 点击停止按钮时调用
        public virtual void StopRoll()
        {
            //停止播放滚动声音
            InitData();
            StopRollJettonNow();
            EventDispatch.Dispatch((int)EventID.GameEventId.WhenIconStop);
        }
        protected virtual void StopRollJettonNow()
        {
            for (int i = 0; i < AllChild.Length; i++)
            {
                for (int j = 0; j < AllChild[i].Count; j++)
                {
                    AllChild[i][j].transform.localPosition = AllChildPos[i][j];
                }
            }
            List<int> _jettons = App.GetGameData<OverallData>().Response.JettonList;
            int num = 0;
            //获得要显示的图片名
            for (int j = 0; j < AllChild.Length; j++)
            {
                for (int i = 0; i < AllChild[j].Count; i++)
                {
                    if (i == 0)
                    {
                        string newName = RandonSpriteName();
                        AllChild[j][i].GetComponent<UISprite>().spriteName = newName;
                    }
                    else
                    {
                        AllChild[j][i].GetComponent<UISprite>().spriteName = SpriteName + _jettons[num++];
                    }
                }
            }
        }
        #endregion

        public virtual void GetJettonOrder()
        {
            App.GetGameData<OverallData>().Response.RegroupIconList();
        }

        protected virtual void InitData()
        {
            IsStartRollJetton = false;
            _speed = StartSpeed;
            _totalTime = 0;
            _isUp = true;
            InitRollData();
        }
        protected virtual void InitvalHoldTime()
        {
            float t = 0.5f;
            HoldTimes = new float[LineNum];
            for (int i = 0; i < LineNum; i++)
            {
                HoldTimes[i] = t;
                t += 1;
            }
        }
        protected virtual void InitRollData()
        {
            if (Speeds == null || EndRollNums == null)
            {
                Speeds = new float[LineNum];
                EndRollNums = new int[LineNum];
            }
            for (int i = 0; i < LineNum; i++)
            {
                Speeds[i] = ConstantSpeed;
                EndRollNums[i] = EndTime;
            }
        }
    }
}