using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.jpmj
{
    public class JpQueryHuList : QueryHuList
    {
        private readonly Dictionary<int,ISFSObject> _huqueryCacheList= new Dictionary<int, ISFSObject>();
        public override void Query(ISFSObject obj, DVoidSfsObject sendCall, EventData evn)
        {
            currMahjong = (MahjongIcon)evn.data1;
            if (currMahjong == null) return;

               //向服务器发出请求
            if (MahjongIcon.Flag.Ting == currMahjong.CurrFlag)
            {
                if (!_huqueryCacheList.ContainsKey(currMahjong.MahjongItem.Value))
                {
                    //听的牌，通过服务器查询
                    obj.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeGetHuCards);
                    obj.PutInt(_keyCard, currMahjong.MahjongItem.Value);
                    sendCall(obj);
                }
                else
                {
                    SetMahHuListInfo(currMahjong.MahjongItem.Value);
                }
            }


/*            currMahjong = (MahjongIcon)evn.data1;
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
/*                //不向服务器发出请求
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
                }#1#

                //if()

            }*/
        }

        public override void OnQueryHulist(ISFSObject servdata)
        {
            var card = servdata.GetInt(_keyCard);
            if (_huqueryCacheList.ContainsKey(card))
            {
                _huqueryCacheList[card] = servdata;
            }
            else
            {
                _huqueryCacheList.Add(card, servdata);
            }
            SortServDataInfo(card);

            SetMahHuListInfo(card);

        }


        /// <summary>
        /// 拆分听牌数据，让每行的牌数合理
        /// </summary>
        /// <param name="cardkey"></param>
        private void SortServDataInfo(int cardkey)
        {
            if (_huqueryCacheList.ContainsKey(cardkey))
            {
                var hulistCache = _huqueryCacheList[cardkey].GetSFSArray(_keyHulist);
                ISFSArray sfsArray = new SFSArray();

                foreach (ISFSObject obj in hulistCache)
                {
                    var sfsobj = new SFSObject();

                    var cardsOrg = obj.GetIntArray(_keyCards);
                    var legalCadList = new List<int>();
                    foreach (var cd in cardsOrg)
                    {
                        if (D2MahjongMng.Instance.IsContainMahjongMeKey(cd))
                        {
                            legalCadList.Add(cd);
                        }
                    }

                    var tai = obj.GetInt(_keyTai);
                    var hua = obj.GetInt(_keyHua);
                    var cnt = obj.GetInt(_keyCnt);
                    sfsobj.PutIntArray(_keyCards, legalCadList.ToArray());
                    sfsobj.PutInt(_keyTai, tai);
                    sfsobj.PutInt(_keyHua, hua);
                    sfsobj.PutInt(_keyCnt, cnt);
                    sfsArray.AddSFSObject(sfsobj);
                }

                _huqueryCacheList[cardkey].PutSFSArray(_keyHulist, sfsArray);
            }
        }

/*        private bool IsHasCache(ISFSObject obj)
        {
            if (_huqueryListCache == null)
            {
                _huqueryListCache = obj;
                return false;
            }


            return true;
        }*/

        public override void ClearCache()
        {
            _huqueryCacheList.Clear();
            JpQueryHuPnl.Instance.ClearMahJongGroup();

            base.ClearCache();

        }


        [SerializeField]
        private string _keyCards = "cards";
        [SerializeField]
        private string _keyTai = "tai";
        [SerializeField]
        private string _keyHua = "hua";
        [SerializeField]
        private string _keyCnt = "cnt";

        [SerializeField]
        private string _keyHulist = "hulist";
        [SerializeField]
        private string _keyCard = "card";

        private void SetMahHuListInfo(int card)
        {
            if (_huqueryCacheList.ContainsKey(card) && _huqueryCacheList[card].ContainsKey(_keyHulist))
            {
                JpQueryHuPnl.Instance.SetMahjongHulistInfo(_huqueryCacheList[card].GetSFSArray(_keyHulist), card);
            }
        }
    }
}
