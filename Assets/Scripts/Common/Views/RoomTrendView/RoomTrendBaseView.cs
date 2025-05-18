using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views.RoomTrendView
{
    public class RoomTrendBaseView : YxView
    {
        [Tooltip("游戏状态的描述")]
        public List<string> GameStateDes;
        [Tooltip("游戏状态的描述的Label")]
        public UILabel GameStateDesLabel;
        [Tooltip("游戏下注进行的过程")]
        public UITexture ProgressBar;
        [Tooltip("总的局数")]
        public UILabel TotalRound;
        [Tooltip("游戏数据的统计")]
        public List<UILabel> ShowTotals;
        [Tooltip("正常显示的走势图")]
        public YxView DishTrendView;

        private TrendData _trendData;
        private bool _fresh;
        private int _roomId=-1;
        private ITrendCfg _trendCfg;

        public override void Init(object initData)
        {
            base.Init(initData);
            _roomId = (int)initData;
            _trendCfg = GetComponent<ITrendCfg>();

        }

        protected override void OnFreshView()
        {
            if (Data == null)
            {
                return;
            }
            GameRecordCallBack(Data);
        }

        private void GameRecordCallBack(object data)
        {
            var trendData = ParseTrendData(data);
            if (DishTrendView)
            {
                DishTrendView.UpdateView(trendData);
            }
            SendSecondAction();
        }
        /// <summary>
        /// 处理服务器发过来的数据
        /// </summary>
        /// <param name="data"></param>
        protected List<ITrendReciveData> ParseTrendData(object data)
        {
            var gameData = data as Dictionary<string, object>;
            _trendData = new TrendData(gameData);
            var datas = new List<ITrendReciveData>();
            if (gameData != null)
            {
                var records = gameData.ContainsKey("records") ? gameData["records"] : null;
                var recordDatas = records as List<object>;
                if (recordDatas != null)
                {
                    foreach (var record in recordDatas)
                    {
                        var recordData = _trendCfg.CreatTrendReciveData(record);
                        datas.Add(recordData);
                    }
                }
            }

            if (TotalRound)
            {
                TotalRound.text = datas.Count.ToString();
            }
            return datas;
        }
        /// <summary>
        /// 根据时间发送下次请求
        /// </summary>
        public virtual void SendSecondAction()
        { 
            StartCoroutine("SetGameShow");
            StartCoroutine("SetGameState");
            StartCoroutine("GetLeftXz");
        }

        private IEnumerator SetGameShow()
        {
            if (ShowTotals.Count != 0)
            {
                for (int i = 0; i < ShowTotals.Count; i++)
                {
                    var labelName = ShowTotals[i].name;
                    if (_trendData.TotalDic.ContainsKey(labelName))
                    {
                        ShowTotals[i].text = TotalRound ? _trendData.TotalDic[labelName].ToString() : string.Format("{0}/20", _trendData.TotalDic[labelName]);
                    }
                }
            }

            if (_trendData.GameType == 0)
            {
                _fresh = false;
                GameStateDesLabel.TrySetComponentValue(GameStateDes[0]);
            }
            else
            {
                if (_trendData.CurLeftTime != _trendData.CdXiaZhu)
                {
                    ProgressBar.fillAmount = (1.0f / _trendData.CdXiaZhu) * _trendData.CurLeftTime;

                    _fresh = true;
//                    Debug.LogError("ProgressBar.fillAmount" + ProgressBar.fillAmount);
                    var curLeftTime = _trendData.CurLeftTime;
                    while (curLeftTime > 0)
                    {
                        curLeftTime--;
                        var str = string.Format("{0} {1}", GameStateDes[_trendData.GameType], curLeftTime);
                        GameStateDesLabel.TrySetComponentValue(str);
                        if (curLeftTime == 0)
                        {
                            GameStateDesLabel.TrySetComponentValue(GameStateDes[0]);
                        }

                        yield return new WaitForSeconds(1);
                    }
                }
            }
        }

        private IEnumerator GetLeftXz()
        {
            var leftXz = _trendData.LeftXz;
            var time = _trendData.CdXiaZhu;
            while (leftXz > 0)
            {
                leftXz--;
                if (leftXz == 0)
                {
                    ProgressBar.fillAmount = 1;
                    _fresh = true;
                    while (time > 0)
                    {
                        time--;
                        if (time == 0)
                        {
                            GameStateDesLabel.TrySetComponentValue(GameStateDes[0]);
                            yield break;
                        }
                        var str = string.Format("{0} {1}", GameStateDes[1], time);
                        GameStateDesLabel.TrySetComponentValue(str);

                        yield return new WaitForSeconds(1);
                    }
                }
                yield return new WaitForSeconds(1);
            }
        }

        private IEnumerator SetGameState()
        {
            var leftRequestTime = _trendData.AllLeftTime;

            while (leftRequestTime > 0)
            {
                leftRequestTime--;
//                Debug.LogError("leftRequestTime" + leftRequestTime);
                if (leftRequestTime == 0)
                {
                    var dic = new Dictionary<string, object>();
                    dic["roomId"] = _roomId;
//                    Facade.Instance<TwManager>().SendAction("room.getGameRecord", dic, GameRecordCallBack, false, null, false);

                }

                yield return new WaitForSeconds(1);
            }
        }

        protected void Update()
        {
            if (_fresh)
            {
                ProgressBar.fillAmount -= (1.0f / _trendData.CdXiaZhu) * Time.deltaTime;

                if (ProgressBar.fillAmount <= 0)
                {
                    _fresh = false;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_roomId==-1)
            {
                return;
            }
//            HallController.Instance.SendGetGameRecordByRoomId(_roomId,UpdateView);
        }

        protected override void OnDisable()
        {
            base.OnDisable(); 
            StopCoroutine("SetGameShow");
            StopCoroutine("SetGameState");
            StopCoroutine("GetLeftXz");
        }
    }

    public class TrendData
    {
        /// <summary>
        /// 下注的总时间
        /// </summary>
        public int CdXiaZhu;

        /// <summary>
        /// 当前剩余的时间
        /// </summary>
        public int CurLeftTime;

        /// <summary>
        /// 剩余下注时间
        /// </summary>
        public int LeftXz;

        /// <summary>
        /// 总计剩余的时间
        /// </summary>
        public int AllLeftTime;

        /// <summary>
        /// 当前游戏的状态
        /// </summary>
        public int GameType;

        /// <summary>
        /// 数据出现次数的汇总
        /// </summary>
        public Dictionary<string,int> TotalDic=new Dictionary<string,int>();


        public TrendData(Dictionary<string, object> data)
        {
            CdXiaZhu = data.ContainsKey("cdXiaZhu") ? int.Parse(data["cdXiaZhu"].ToString()) : -1;
            CurLeftTime = data.ContainsKey("curLeftTime") ? int.Parse(data["curLeftTime"].ToString()) : -1;
            LeftXz = data.ContainsKey("leftXZ") ? int.Parse(data["leftXZ"].ToString()) : -1;
            AllLeftTime = data.ContainsKey("allLeftTime") ? int.Parse(data["allLeftTime"].ToString()) : -1;
            GameType = data.ContainsKey("type") ? int.Parse(data["type"].ToString()) : -1;
            var recordsNum = data["recordsNum"];
            var recordTotal = recordsNum as Dictionary<string, object>;
            if (recordTotal != null)
            {
                var z = recordTotal.ContainsKey("z") ? int.Parse(recordTotal["z"].ToString()) : 0;
                TotalDic["z"] = z;
                var h = recordTotal.ContainsKey("h") ? int.Parse(recordTotal["h"].ToString()) : 0;
                TotalDic["h"] = h;
                var x = recordTotal.ContainsKey("x") ? int.Parse(recordTotal["x"].ToString()) : 0;
                TotalDic["x"] = x;
                var t = recordTotal.ContainsKey("t") ? int.Parse(recordTotal["t"].ToString()) : 0;
                TotalDic["t"] = t;
                var d = recordTotal.ContainsKey("d") ? int.Parse(recordTotal["d"].ToString()) : 0;
                TotalDic["d"] = d;
            }
        }
    }
}
