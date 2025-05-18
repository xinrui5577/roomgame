using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Hall.View.NnRecordWindow
{
    public class NnRecordDetialWindow : YxTabPageWindow
    {
        public UILabel UserName;
        public UIGrid UsersNameGrid;
        public UILabel UserTotleGold;
        public UIGrid UserTotleGoldGrid;
        public NnRecordDetialItemView NnRecordDetialItemView;
        public UITable Table;

        private bool _isOk;
        private Dictionary<string, int> _eachTotleGold=new Dictionary<string, int>(); 
        private readonly List<UserData> _userDatas=new List<UserData>();
        private readonly List<UserData> _singleUserData=new List<UserData>();

        protected override void OnFreshView()
        {
            if(Data==null)return;
            if (!(Data is Dictionary<string, object>)) return;
            var dataInfo = Data as Dictionary<string, object>;
            var detail = dataInfo.ContainsKey("detail") ? dataInfo["detail"] : null;
            if(detail==null)return;
            if (!(detail is List<object>)) return;
            var detailInfos = detail as List<object>;
            var count = detailInfos.Count;
            var index = 0;
            foreach (var detailDatas in detailInfos)
            {
                if (!(detailDatas is Dictionary<string, object>)) continue;
                var detailInfo = detailDatas as Dictionary<string, object>;
                var replayId = detailInfo.ContainsKey("replay_id") ? detailInfo["replay_id"].ToString() : "";
                List<int> _gold=new List<int>();
                var infoH = detailInfo.ContainsKey("info_h") ? detailInfo["info_h"] : null;
                if (infoH != null)
                {
                    if (infoH is List<object>)
                    {
                        var infos = infoH as List<object>;
                        foreach (var info in infos)
                        {
                            if (!(info is Dictionary<string, object>)) continue;
                            var userInfo = info as Dictionary<string, object>;
                            var userData = new UserData
                                {
                                    Id = userInfo.ContainsKey("id")
                                             ? userInfo["id"].ToString()
                                             : "",
                                    Name = userInfo.ContainsKey("name")
                                               ? userInfo["name"].ToString()
                                               : "",
                                    Gold = userInfo.ContainsKey("gold")
                                               ? int.Parse(userInfo["gold"].ToString())
                                               : 0
                                };
                            if(!_isOk)_singleUserData.Add(userData);
                            _userDatas.Add(userData);
                            _gold.Add(userData.Gold);
                        }
                        _isOk = true;
                    }
                }
                var item=YxWindowUtils.CreateItem(NnRecordDetialItemView, Table.transform);

                item.Round.text = string.Format("第{0}局",count);
                item.OnFreshGold(_gold);
                index++;
                if (count-- % 2 == 0)
                {
                    item.Bg.spriteName = "";
                }

                var heads = detailInfo.ContainsKey("head_s") ? detailInfo["head_s"] : null;
                if (heads == null) continue;
                if (!(heads is Dictionary<string, object>)) continue;
                var headInfo = heads as Dictionary<string, object>;
                foreach (var eachData in _singleUserData)
                {
                    if(headInfo[eachData.Id] ==null)return;
                    if (!(headInfo[eachData.Id] is Dictionary<string, object>)) continue;
                    var headDetail = headInfo[eachData.Id] as Dictionary<string, object>;
                    var gold = headDetail.ContainsKey("gold") ? int.Parse(headDetail["gold"].ToString()) : 0;
                    if (_eachTotleGold.ContainsKey(eachData.Id) == false)
                    {
                        _eachTotleGold.Add(eachData.Id, gold);
                    }
                    else
                    {
                        _eachTotleGold[eachData.Id] += gold;
                    }
                    var headDatas=headDetail.ContainsKey("data")?headDetail["data"]:null;
                    if (!(headDatas is Dictionary<string, object>)) continue;
                    var headData = headDatas as Dictionary<string, object>;
                    var rob = headData.ContainsKey("rob")
                                  ? int.Parse(headData["rob"].ToString())
                                  : -1;
                    bool isHasRob = rob != -1;
                    bool norob = headData.ContainsKey("notRob");
                    var niuType = headData.ContainsKey("niuType")
                                      ? int.Parse(headData["niuType"].ToString())
                                      : 0;
                    var ante = headData.ContainsKey("ante")
                                   ? int.Parse(headData["ante"].ToString())
                                   : 0;
                    var cards = headData.ContainsKey("cards") ? headData["cards"] : null;
                    if (cards == null) continue;
                    var card=cards as List<object>;
                    var banker = headData.ContainsKey("banker");
                    item.OnFreshCards(isHasRob, norob,rob, niuType, ante, card, banker);
                }
            }
            Table.repositionNow = true;
           
            foreach (var t in _singleUserData)
            {
                var nameItem = YxWindowUtils.CreateItem(UserName, UsersNameGrid.transform);
                nameItem.text = t.Name;
                var goldItem = YxWindowUtils.CreateItem(UserTotleGold, UserTotleGoldGrid.transform);
                goldItem.text = _eachTotleGold[t.Id].ToString(CultureInfo.InvariantCulture);
            }
        }

       
    }
    public class UserData
    {
        public string Id;
        public string Name;
        public int Gold;
    }
}
