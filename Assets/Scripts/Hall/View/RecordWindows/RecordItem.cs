using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Hall_01.Item;
using fastJSON;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    /// <summary>
    /// 回放对象item
    /// </summary>
    public class RecordItem : MonoBehaviour
    { 
        private RecordData _data;

        [SerializeField]
        private PlayerRecordInfo[] _players;
        [SerializeField]
        private UILabel _date;
        [SerializeField]
        private UIGrid _playersGrid;
        [SerializeField]
        private GameObject[] _lines;
        [SerializeField]
        private UIGrid _linesGrid;
        /// <summary>
        /// 房间类型
        /// </summary>
        [SerializeField]
        private UILabel _roomType;
        /// <summary>
        /// 记录Id
        /// </summary>
        [SerializeField]
        private UILabel _recordId;
        /// <summary>
        /// 回放按键item
        /// </summary>
        [SerializeField]
        private GameObject _replayBtnItem;
        /// <summary>
        /// 回放布局
        /// </summary>
        [SerializeField]
        private UIGrid _replayGrid;

        /// <summary>
        /// 回放按键点击事件
        /// </summary>
        public void OnClickReplay(GameObject gob)
        {
            if (gob == null || gob.GetComponent<RecordSingleRoundItem>() == null)
            {
                return;
            }

            RecordSingleRoundItem single = gob.GetComponent<RecordSingleRoundItem>();

            if (single.IsAllowReplay)
            {
                Application.OpenURL(single.Url);
                //if (single.IsLocalReplay)
                //{
                //    //todo 进入回放
                //}
                //else
                //{
                //    Application.OpenURL(single.Url);
                //}
            }
            else
            {
                YxMessageBox.Show("回放功能暂未开放,请耐心等待!");
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        public void Init(RecordData data)
        {
            _data = data;
            for (int i = 0; i < 4; i++)
            {
                if (_data.PlayersData.Length > i && _data.PlayersData[i]!=null)
                {
                    _players[i].gameObject.SetActive(true);
                    _players[i].SetData(_data.PlayersData[i],false);
                }
                else
                {
                    _players[i].gameObject.SetActive(false);
                }          
            }

            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].SetActive(i + 1 < _data.PlayersData.Length);
            }

            List<object> replays = _data.ReplayDatas;
            for (int i = 0; i < replays.Count; i++)
            {
                RecordSingleRoundItem gob = NGUITools.AddChild(_replayGrid.gameObject, _replayBtnItem).GetComponent<RecordSingleRoundItem>();
                gob.Init(i,(Dictionary<string,object>)replays[i],_data.WebHost);
                UIEventListener.Get(gob.gameObject).onClick = OnClickReplay;
            }

            _roomType.text ="[f27c7c]开房模式[-]";
            _replayGrid.repositionNow = true;
            _linesGrid.repositionNow = true;
            _playersGrid.repositionNow = true;
            _date.text = _data.Time;
            _recordId.text = (_data.Index + 1).ToString();
        }

    }
    /// <summary>
    /// 游戏记录数据
    /// </summary>
    public class RecordData
    {
        public PlayerRecordData[] PlayersData;
        /// <summary>
        /// 单局数据
        /// </summary>
        public List<object> ReplayDatas;
        /// <summary>
        /// 时间
        /// </summary>
        public string Time;
        /// <summary>
        /// 索引？
        /// </summary>
        public int Index;
        /// <summary>
        /// 当前玩家的host
        /// </summary>
        public string WebHost;
        public void Parse(int id, IDictionary data)
        {
            //todo 数据转换
            Index = id;
            Time = data["create_dt"].ToString();
            WebHost = data["web_host"].ToString();
            ParsePlayer((List<object>)data["info_h"]);
            ReplayDatas = (List<object>)data["detail"];
        }
        /// <summary>
        /// 解析玩家信息
        /// </summary>
        /// <param name="playerData"></param>
        private void ParsePlayer(List<object> playerData)
        {
            var players = playerData;
            PlayersData = new PlayerRecordData[players.Count];
            int i = 0;
            foreach (var player in players)
            {
                var playerdata = (Dictionary<string, object>)player;
                long id = (long)(playerdata["id"]);
                string name = (string)playerdata["name"];
                long gold = (long)playerdata["gold"];
                PlayersData[i] = new PlayerRecordData
                {
                    PlayerName = name,
                    ScoreNum = gold,
                    ID = id
                };
                i++;
            }
        }
    }

}
