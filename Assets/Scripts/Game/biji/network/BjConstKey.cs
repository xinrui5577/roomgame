using System.Collections.Generic;
using System.Linq;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.biji.network
{
    public class BjTypeKey
    {
        /*开始*/
        public const int TypeToStart = 1;
        /*发牌*/
        public const int TypeSendCard = 2;
        /*比牌*/
        public const int TypeCompare = 3;
        /*结算*/
        public const int TypeResult = 4;
        /*确认牌的头中尾*/
        public const int TypePutCard = 5;
        /*放弃*/
        public const int TypeGiveUp = 7;
        /*错误*/
        public const int TypeError = 9999;

    }

    public class BjRejoinTypeKey
    {
        /*放牌*/
        public const int TypePutCard = 0;
        /*比牌*/
        public const int TypeCompareCard = 1;
        /*等待开始*/
        public const int TypeWaitForStart = 2;
    }

    public class BjRequestConstKey
    {

    }

    public class PutCardsData
    {
        public int Seat;
        public List<int[]> PutCards = new List<int[]>();
        public List<int[]> RealCards=new List<int[]>();

        public PutCardsData(ISFSObject data)
        {
            Seat = data.ContainsKey("seat") ? data.GetInt("seat") : -1;
            var daoList = data.ContainsKey("daolist") ? data.GetSFSArray("daolist") : null;
            if (daoList != null)
            {
                for (int i = 0; i < daoList.Count; i++)
                {
                    var obj = daoList.GetSFSObject(i);
                    var cards = obj.ContainsKey("cards") ? obj.GetIntArray("cards") : null;
                    PutCards.Add(cards);
                    var realCards = obj.ContainsKey("realcards") ? obj.GetIntArray("realcards") : null;
                    RealCards.Add(realCards);
                }
            }
        }
    }

    public class CompareData
    {
        public int Seat;
        public int XiPaiScore;
        public List<int> DaosScore = new List<int>();
        public List<int[]> DaosCards = new List<int[]>();
        public List<int[]> DaoRealCards=new List<int[]>();
        public List<int> DaosTypes = new List<int>();
        public int[] XiPaiInfos;

        public CompareData(ISFSObject data)
        {
            Seat = data.ContainsKey("seat") ? data.GetInt("seat") : -1;
            var scoreData = data.ContainsKey("score") ? data.GetSFSObject("score") : null;
            if (scoreData != null)
            {
                XiPaiScore = scoreData.ContainsKey("xipaiscore") ? scoreData.GetInt("xipaiscore") : -1;
                var daoScore = scoreData.ContainsKey("daoscore") ? scoreData.GetSFSArray("daoscore") : null;
                if (daoScore != null)
                {
                    for (int i = 0; i < daoScore.Count; i++)
                    {
                        var obj = daoScore.GetSFSObject(i);
                        var score = obj.ContainsKey("score") ? obj.GetInt("score") : -1;
                        DaosScore.Add(score);
                    }
                }
            }

            var daosData = data.ContainsKey("daos") ? data.GetSFSObject("daos") : null;
            if (daosData != null)
            {
                var daoInfo = daosData.ContainsKey("daoinfo") ? daosData.GetSFSArray("daoinfo") : null;
                if (daoInfo != null)
                {
                    for (int i = 0; i < daoInfo.Count; i++)
                    {
                        var obj = daoInfo.GetSFSObject(i);
                        var cards = obj.ContainsKey("cards") ? obj.GetIntArray("cards") : null;
                        DaosCards.Add(cards);
//                        Debug.LogError("此道的牌值");
//                        YxDebug.LogArray(cards);
                        var realCards = obj.ContainsKey("realcards") ? obj.GetIntArray("realcards") : null;
//                        Debug.LogError("此道的牌真值");
//                        YxDebug.LogArray(realCards);
                        DaoRealCards.Add(realCards);
                                            
                        var type = obj.ContainsKey("type") ? obj.GetInt("type") : -1;
                        DaosTypes.Add(type);
                    }
                }

                XiPaiInfos = daosData.ContainsKey("xipaiinfo") ? daosData.GetIntArray("xipaiinfo") : null;
            }
        }
    }

    public class ResultData
    {
        public List<int> WinSeats = new List<int>();
        public List<int> LoseSeats = new List<int>();
        public Dictionary<int, int> TotalResultDataList = new Dictionary<int, int>();

        public ResultData(ISFSObject data)
        {
            var score = data.ContainsKey("score") ? data.GetSFSArray("score") : null;
            if (score != null)
            {
                for (int i = 0; i < score.Count; i++)
                {
                    var obj = score.GetSFSObject(i);
                    var scoreData = obj.ContainsKey("score") ? obj.GetInt("score") : -1;
                    var seat = obj.ContainsKey("seat") ? obj.GetInt("seat") : -1;
                    TotalResultDataList[seat] = scoreData;
                    if (scoreData > 0)
                    {
                        WinSeats.Add(seat);
                    }
                    else
                    {
                        LoseSeats.Add(seat);
                    }
                }
            }
        }
    }

    public class HupData
    {
        public int UserId;
        public string UserName;
        public int Type;
        public bool IsSelf;
        public bool IsRoomOwner;
        public int CdTime;
        public HupData(ISFSObject data)
        {
            UserId = data.GetInt("userId");
            UserName = data.GetUtfString("username");
            Type = data.GetInt("type");
            CdTime = data.ContainsKey("cdTime") ? data.GetInt("cdTime") : 300;
        }
    }

    public class TtResultUserData
    {
        public int UserId;
        public string UserName;
        public int UserGold;
        public string UserHead;
        public short Sex;
        public int XiPaiCnt;
        public int TouXiangCnt;
        public bool IsWinner;
        public bool IsRoomOwner;

        public TtResultUserData(ISFSObject data)
        {
            UserId = data.GetInt("id");
            UserGold = data.GetInt("gold");
            XiPaiCnt = data.GetInt("xipaicnt");
            TouXiangCnt = data.GetInt("touxiangcnt");

            UserName = data.ContainsKey("nick") ? data.GetUtfString("nick") : "";
            UserHead = data.ContainsKey("avatar") ? data.GetUtfString("avatar") : "";
            Sex = data.ContainsKey("sex") ? data.GetShort("sex") : (short)-1;

        }
    }
}