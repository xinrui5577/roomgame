using Assets.Scripts.Hall.View.RecordWindows;
using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using UnityEngine;
using LitJson;

namespace Assets.Skins.SkinResource.skin_0019.Scripts.Tea.Page.TeaTotalRecordContainer
{
    public class JpmjRecordDetailWindow : RecordDetailWindow
    {
        [Tooltip("Sss战绩详情界面")]
        public JpmjSssRecordDetailWindow SssRecordDetailWindow;
        [Tooltip("Sss战绩请求协议")]
        public string SssActionName;

        private Dictionary<int, SssReplayFrameData> _SssReplayDic = new Dictionary<int, SssReplayFrameData>();
        private List<HeadData> _HeadDatas = new List<HeadData>();

        public void ShowSssRecordDetailWindow(JpmjRecordDetailItem item)
        {
            var dic = new Dictionary<string, object>();
            dic["replay_id"] = item.CurData.ReplayId;
            Facade.Instance<TwManager>().SendAction(SssActionName, dic, OnRequestSssRecordInfo);
        }

        private void OnRequestSssRecordInfo(object data)
        {
            if (data == null) return;

            _HeadDatas.Clear();
            _SssReplayDic.Clear();
            var dataDic = (Dictionary<string, object>)data;

            // 获取回放数据
            var bodysData = dataDic["body_s"].ToString();
            if (string.IsNullOrEmpty(bodysData)) return;

            var opGroup = bodysData.Split(';');
            for (int i = 0; i < opGroup.Length; i++)
            {
                var message = opGroup[i];
                if (!string.IsNullOrEmpty(message))
                {
                    var seat = int.Parse(message[0].ToString());
                    if (!_SssReplayDic.ContainsKey(seat))
                    {
                        _SssReplayDic[seat] = new SssReplayFrameData();
                    }
                    var frameData = _SssReplayDic[seat];
                    frameData.SetSssReplayFrameData(message);
                }
            }

            //解析头像数据
            var headsData = (Dictionary<string, object>)dataDic["head_s"];
            foreach (var item in headsData)
            {
                var head = (Dictionary<string, object>)item.Value;
                _HeadDatas.Add(new HeadData(head));
            }

            //设置头像信息
            for (int i = 0; i < _HeadDatas.Count; i++)
            {
                var seat = _HeadDatas[i].Seat;
                if (_SssReplayDic.ContainsKey(seat))
                {
                    _SssReplayDic[seat].SetHeadInfo(_HeadDatas[i]);
                }
            }

            SssRecordDetailWindow.ShowWindow(_SssReplayDic);
        }
    }

    public class SssReplayFrameData
    {
        public int Special { get; private set; }

        public HeadData HeadData { get; private set; }

        public List<SssReplayCardsData> CardsData { get; private set; }

        public List<SssReplayLineScoreData> LineScoreData { get; private set; }

        /// <summary>
        /// 总分
        /// </summary>
        public int AountScore;
        /// <summary>
        /// 打人次数
        /// </summary>
        public int ShootCount;
        /// <summary>
        /// 被打次数
        /// </summary>
        public int BeShootCount;
        /// <summary>
        /// 全垒打
        /// </summary>
        public bool IsQuanleida;

        public void SetSssReplayFrameData(string message)
        {
            //解析协议号
            var porocol = int.Parse(message[2].ToString());
            if (porocol == 0) return;

            var str = message.Replace(@"\", "");
            var s = message.IndexOf("{");
            var e = message.LastIndexOf("}");
            str = message.Substring(s, e - s + 1);
            //解析数据为json对象
            var obj = JsonMapper.ToObject(str);

            switch (porocol)
            {
                case 1: AnalysisCard(obj); break;
                case 2: AnalysisScore(obj); break;
            }
        }

        public void SetHeadInfo(HeadData headData)
        {
            HeadData = headData;
        }

        public void AnalysisCard(JsonData data)
        {
            /*0,1:{
               "special": 51,
               "duninfo":[
               {"cards": [75, 59, 27, 54, 22],"type": 6},
               {"cards": [29, 24, 23, 21, 18],"type": 5},
               {"cards": [77, 60, 40],"type": 0}
               ]}:0*/

            Special = (int)data["special"];
            var duninfo = data["duninfo"];

            CardsData = new List<SssReplayCardsData>();
            for (int i = 0; i < duninfo.Count; i++)
            {
                var tempType = (int)duninfo[i]["type"];
                var tempCards = duninfo[i]["cards"];
                var cards = new List<int>();
                for (int j = 0; j < tempCards.Count; j++)
                {
                    cards.Add((int)tempCards[j]);
                }
                var cardsData = new SssReplayCardsData(cards, tempType);
                CardsData.Add(cardsData);
            }
            CardsData.Reverse();
        }

        public void AnalysisScore(JsonData data)
        {
            /*{"score": 32,
              	"dunscore": [
                {"add": 12,"normal": 9},
                {"add": 0,"normal": -2},
                {"add": 0,"normal": 3}
                ],
              	"basesocre": [10, 6, 6, 0],
              	"bedaqiang": [],
              	"daqiang": [0],
              	"quanleida": false
              }*/

            AountScore = (int)data["score"];//总分
            IsQuanleida = (bool)data["quanleida"];//全垒打
            ShootCount = data["daqiang"].Count;//打枪数
            BeShootCount = data["bedaqiang"].Count;//被打枪数

            //每到分数
            var dunscore = data["dunscore"];
            LineScoreData = new List<SssReplayLineScoreData>();
            for (int i = 0; i < dunscore.Count; i++)
            {
                var add = (int)dunscore[i]["add"];
                var normal = (int)dunscore[i]["normal"];
                var scoreData = new SssReplayLineScoreData(add, normal);
                LineScoreData.Add(scoreData);
            }
            LineScoreData.Reverse();
        }
    }

    public struct SssReplayCardsData
    {
        public List<int> Cards;
        public int Type;

        public SssReplayCardsData(List<int> cards, int type)
        {
            Cards = cards;
            Type = type;
        }
    }

    public struct SssReplayLineScoreData
    {
        public int Add;
        public int Normal;

        public SssReplayLineScoreData(int add, int normal)
        {
            Add = add;
            Normal = normal;
        }
    }
}