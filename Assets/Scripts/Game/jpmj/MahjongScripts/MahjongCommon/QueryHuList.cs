using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using UnityEngine;
using Sfs2X.Entities.Data;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using YxFramwork.ConstDefine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public class QueryHuList : MonoBehaviour
    {
        protected Dictionary<int, List<int>> mahjongQueryCache = new Dictionary<int, List<int>>();
        protected int leaveMahjongCnt { get { return data.LeaveMahjongCnt; } }
        protected int laizi { get { return data.Laizi; } }
        protected TableData data;

        //当前查询的麻将
        protected MahjongIcon currMahjong;

        void Awake()
        {
            data = GetComponent<TableData>();
        }

        public virtual void ClearCache()
        {
            mahjongQueryCache.Clear();
        }

        public virtual void Query(ISFSObject obj, DVoidSfsObject sendCall, EventData evn)
        {
            currMahjong = (MahjongIcon)evn.data1;
            if (currMahjong == null) return;

            //向服务器发出请求
            if (MahjongIcon.Flag.Ting == currMahjong.CurrFlag && !mahjongQueryCache.ContainsKey(currMahjong.MahjongItem.Value))
            {
                //听的牌，通过服务器查询
                obj.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeGetHuCards);
                obj.PutInt("card", currMahjong.MahjongItem.Value);
                sendCall(obj);
            }
            else
            {
                //不向服务器发出请求
                QueryHulistData hulistData = new QueryHulistData((int)currMahjong.CurrFlag, currMahjong.MahjongItem.Value, currMahjong.MahjongItem.MahjongIndex, laizi);

                switch (currMahjong.CurrFlag)
                {
                    case MahjongIcon.Flag.Ting:
                        hulistData.Cards = mahjongQueryCache[currMahjong.MahjongItem.Value];
                        //任意牌
                        if (hulistData.Cards.Exists((a) => { return a == 0; })) 
                            hulistData.LeaveMahjongCnt = leaveMahjongCnt;
                        EventDispatch.Dispatch((int)GameEventId.QueryHulist, new EventData(hulistData));
                        break;
                    case MahjongIcon.Flag.Youjin:
                        hulistData.LeaveMahjongCnt = leaveMahjongCnt;
                        EventDispatch.Dispatch((int)GameEventId.QueryHulist, new EventData(hulistData));
                        break;
                }

            }
        }

        public virtual void OnQueryHulist(ISFSObject data)
        {
            //int[] arr = data.GetIntArray("hulist");
            
            var hulist = data.GetSFSArray("hulist");
            var intList = new List<int>();
            foreach (ISFSObject sfsobj in hulist)
            {
                var cds = sfsobj.GetIntArray("cards");
                var len = cds.Length;
                for (int i = 0; i < len; i++)
                {
                    if (D2MahjongMng.Instance.IsContainMahjongMeKey(cds[i]))
                    {
                        intList.Add(cds[i]);
                    }
                }
            }
           
            int[] arr = intList.ToArray();

            int cardValue = (int)data.GetInt("card");

            if (currMahjong == null || currMahjong.MahjongItem.Value != cardValue || mahjongQueryCache.ContainsKey(cardValue))
                return;

            mahjongQueryCache.Add(cardValue, FilterCards(arr));

            QueryHulistData hulistData = new QueryHulistData((int)MahjongIcon.Flag.Ting, currMahjong.MahjongItem.Value, currMahjong.MahjongItem.MahjongIndex, laizi);
            hulistData.Cards = mahjongQueryCache[currMahjong.MahjongItem.Value];

            if (null != arr || arr.Length > 0)
            {
                //任意牌
                if (Array.Exists(arr, (a) => { return a == 0; }))               
                    hulistData.LeaveMahjongCnt = leaveMahjongCnt;             
                    
                EventDispatch.Dispatch((int)GameEventId.QueryHulist, new EventData(hulistData));               
            }
        }

        private List<int> FilterCards(int[] arr)
        {
            if (arr == null || arr.Length == 0) return null;

            List<int> list = new List<int>();

            //过滤东西南北中发白 多余的牌 72 73 75 76...
            for (int i = 0; i < arr.Length; i++)
            {
                int value = arr[i];
                if (value <= 65 || value == 68 || value == 71 || value == 74 || value == 81 || value == 84 || value == 87)
                {
                    list.Add(arr[i]);
                }
            }

            //hulist中有赖子牌 并且还是花牌 
            if (Array.Exists(arr, (a) => { return (a == laizi) && (laizi >= 96); }))
            {
                if (laizi >= 96 && laizi < 100)
                {
                    for (int i = 96; i < 100; i++)
                        if (i != laizi)
                            list.Add(i);
                }
                else
                {
                    for (int i = 100; i < 104; i++)
                        if (i != laizi)
                            list.Add(i);
                }
                list.Remove(laizi);
            }

            list.Sort((a, b) =>
            {
                if (a > b) return 1;
                if ((a < b)) return -1;
                return 0;
            });

            return list;
        }
    }
}