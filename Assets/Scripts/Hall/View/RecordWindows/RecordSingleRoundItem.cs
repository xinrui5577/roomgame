using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Hall_01.Item;
using fastJSON;
using UnityEngine;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    /// <summary>
    /// 一局游戏的数据
    /// </summary>
    public class RecordSingleRoundItem : MonoBehaviour
    {
        /// <summary>
        /// 显示Id对象
        /// </summary>
        public UILabel ShowIdUiLabel;
        /// <summary>
        /// 显示ID
        /// </summary>
        public int ShowId;
        /// <summary>
        /// 回放ID
        /// </summary>
        public string ReplayId;
        /// <summary>
        /// 自己的当前局得分
        /// </summary>
        public string SelfRecord;
        /// <summary>
        /// 时间 时分秒
        /// </summary>
        public string Time;
        /// <summary>
        /// 网页回放地址
        /// </summary>
        public string Url;
        /// <summary>
        /// 是否本地回放
        /// </summary>
        public bool IsLocalReplay;
        /// <summary>
        /// 是否允许进入回放
        /// </summary>
        public bool IsAllowReplay;
        /// <summary>
        /// 玩家数据
        /// </summary>
        public PlayerRecordData[] PlayersData;
        /// <summary>
        /// 回放url
        /// </summary>
        private string _keyUrl = "url";
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(int id,IDictionary data,string host)
        {
            gameObject.SetActive(true);
            ShowId = id;
            FormatUrl(data,host);
            gameObject.GetComponent<UISprite>().depth = id;
            ShowIdUiLabel.depth = id + 1;
            ShowIdUiLabel.text = id.ToString();
        }

        public void FormatUrl(IDictionary data,string host)
        {
            Url = string.Format("{0}{1}", host,data[_keyUrl]);
        }
        /// <summary>
        /// 解析玩家信息
        /// </summary>
        /// <param name="playerData"></param>
        private void ParsePlayer(string playerData)
        {
            IDictionary players = (Dictionary<string, object>)JSON.Instance.Parse(playerData);
            PlayersData = new PlayerRecordData[players.Count];
            int i = 0;
            foreach (var key in players.Keys)
            {
                PlayersData[i] = new PlayerRecordData
                {
                    PlayerName = key.ToString(),
                    ScoreNum = int.Parse(players[(string)key].ToString())
                   
                };
                i++;
            }
        }
    }
}
