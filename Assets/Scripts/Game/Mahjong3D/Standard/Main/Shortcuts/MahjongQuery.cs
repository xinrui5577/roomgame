using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongQuery : ShortcutsPart, IGameEndCycle
    {
        //麻将使用记录
        private Dictionary<int, int> mRecordMahjong = new Dictionary<int, int>();

        public void Print()
        {
            UnityEngine.Debug.LogError(" --------------------------------- ");
            var temp = from objDic in mRecordMahjong
                       orderby objDic.Key descending
                       select objDic;

            foreach (var item in temp)
            {
                UnityEngine.Debug.LogError(item.Key + " " + item.Value);
            }
        }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnGameEndCycle()
        {
            Clear();
        }

        public void Clear()
        {
            mRecordMahjong.Clear();
        }

        public int Query(int cardValue)
        {
            if (mRecordMahjong.ContainsKey(cardValue))
            {
                int num = 4 - mRecordMahjong[cardValue];
                return num >= 0 ? num : 0;
            }
            return 4;
        }

        public void AddRecordMahjongs(IList<int> cards)
        {
            if (cards == null) return;
            for (int i = 0; i < cards.Count; i++)
            {
                AddRecordMahjong(cards[i]);
            }
        }

        public void AddRecordMahjong(int cardValue)
        {
            if (cardValue == 0) return;
            if (mRecordMahjong.ContainsKey(cardValue))
            {
                mRecordMahjong[cardValue] += 1;
            }
            else
            {
                mRecordMahjong[cardValue] = 1;
            }
        }

        public void RemoveRecordMahjong(int cardValue)
        {
            if (mRecordMahjong.ContainsKey(cardValue))
            {
                mRecordMahjong[cardValue] -= 1;
            }
        }

        public void OnReconnectRecordMahjong()
        {
            mRecordMahjong.Clear();
            var db = GameCenter.DataCenter;
            for (int i = 0; i < db.MaxPlayerCount; i++)
            {
                //出牌
                AddRecordMahjongs(db.Players[i].OutCards);
                //手牌 
                AddRecordMahjongs(db.Players[i].HardCards);
                //吃碰杠牌
                List<CpgData> cpglist = db.Players[i].CpgDatas;
                if (cpglist.Count > 0)
                {
                    for (int j = 0; j < cpglist.Count; j++)
                    {
                        AddRecordMahjongs(cpglist[j].GetAllCardDatas);
                    }
                }
            }
        }

        /// <summary>
        /// 手牌显示查询标记
        /// </summary>
        public void ShowQueryTip(IList<int> cards)
        {
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(cards);
        }

        public void ShowQueryTipOnOperate(IList<int> cards)
        {
            ShowQueryTip(null);
            var db = GameCenter.DataCenter;
            if (!db.OneselfData.IsAuto && null != cards && cards.Count > 0)
            {
                GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(cards);
                GameCenter.EventHandle.Dispatch((int)EventKeys.QueryBtnCtrl, new PanelTriggerArgs() { QueryBtnState = true });
            }
            else if (db.OneselfData.IsAuto)
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.QueryBtnCtrl, new PanelTriggerArgs() { QueryBtnState = true });
            }
            else
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.QueryBtnCtrl, new PanelTriggerArgs() { QueryBtnState = false });
            }
        }
    }
}