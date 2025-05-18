using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.bjlb
{
    public class UserListCtrl : MonoBehaviour
    {
        public UIGrid GridBankersPrefab;
        public UIGrid GridUsersPrefab;
        public UserListModel UserItemPrefab;
        public UserListModel BankerItemPrefab;
        public UIScrollView ScrollView;

        private UIGrid _gridBankers;
        private UIGrid _gridusers;


        protected YxBaseGameUserInfo[] BankersInfo;

        protected void Awake()
        {
            if (_gridBankers != null)
            {
                _gridBankers.onCustomSort = (ts1, ts2) =>
                {
                    if (ts1.name == "banker")
                    {
                        return 1;
                    }
                    if (ts2.name == "banker")
                    {
                        return -1;
                    }
                    return 0;
                };
            }
        }

      

        /// <summary>
        /// 刷新banker数据
        /// </summary>
        /// <param name="responseData"></param>
        public virtual void RefreshBanker(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("bankers"))
                return;

            var banbers = responseData.GetSFSArray("bankers");
            var b = responseData.GetInt("banker");
            var bankTotal = responseData.ContainsKey("bankTotal") ? responseData.GetLong("bankTotal") : 0;
            var bankerLens = banbers.Count;
            BankersInfo = new YxBaseGameUserInfo[bankerLens];
            var gdata = App.GetGameData<BjlGameData>();
            gdata.BankSeat = b;
            var curBanker = gdata.CurrentBanker;
            var isHaveMe = false;
            var selfInfo = gdata.GetPlayerInfo();
            if (bankerLens < 1)//系统庄
            {
                SetBankerType();
                var sysBanker = new BjlUserInfo
                {
                    NickM = "系统庄",
                    Seat = -1,
                    CoinA = long.MaxValue,
                    WinTotalCoin = bankTotal
                };
                SetBankerType();
                if(curBanker != null)
                    curBanker.UpdateView(sysBanker);
                if (_gridBankers != null) _gridBankers.gameObject.SetActive(false);
                RefreahAll(UserListType.Banker);
                return;
            }

            var index = 0;

            if (curBanker == null)
                return;
            var bankerInfo = curBanker.GetInfo<BjlUserInfo>();
            //遍历庄家
            foreach (ISFSObject banber in banbers)
            {
                var bseat = banber.GetInt("seat");
                var ttGold = banber.GetLong("ttgold");

                //同步本地数据
                YxBaseGameUserInfo user = gdata.GetPlayerInfo(bseat, true);
                if (user == null) continue;

                user.CoinA = ttGold;

                BankersInfo[index++] = user;

                if (user.Seat == b)
                {
                    if (bankerInfo == null || bankerInfo.Seat != b || bankerInfo.NickM != user.NickM) //换庄家以后局数重新开始计数
                    {
                        user.TotalCount = 0;
                    }
                    else
                    {
                        user.TotalCount = bankerInfo.TotalCount;
                    }
                    user.WinTotalCoin = bankTotal;
                    curBanker.UpdateView(user);
                }

                if (selfInfo.Seat == user.Seat)
                {
                    isHaveMe = true;
                }
            }
            SetBankerType(isHaveMe, b);

            RefreahAll(UserListType.Banker);
        }

        protected void SetBankerType(bool haveSelf = false, int bankerSeat = -1)
        {
            var self = App.GameData.GetPlayerInfo();
            var gMgr = App.GetGameManager<BjlGameManager>();
            var bankerCtr = gMgr.BankerCtrl;
            if (haveSelf)//有自己
            {
                //限制自己为庄家时下注
                if (bankerSeat == self.Seat)
                {
                    gMgr.BetCtrl.HideChip();
                }
                bankerCtr.SetBankerType(BankerType.StayedBanker);
            }
            else
            {
                gMgr.BetCtrl.ShowChip();
                bankerCtr.SetBankerType(self.CoinA >= bankerCtr.MinApplyBanker
                    ? BankerType.CanBecomeBanker
                    : BankerType.CanNotBeBanker);
            }
        }

        private UserListType _curUserlistType;
        /// <summary>
        /// 玩家列表事件
        /// </summary>
        public virtual void OnUserButtonClick(bool focus)
        {
            if (!focus) return;
            ChangeUserList(UserListType.User);
            ScrollView.ResetPosition();
            if(_gridusers != null)
                _gridusers.Reposition();
        }

        /// <summary>
        /// 庄家列表事件,Toggle
        /// </summary>
        public virtual void OnBankerButtonClick(bool focus)
        {
            if (!focus) return;
            ChangeUserList(UserListType.Banker);
            ScrollView.ResetPosition();
        }

        /// <summary>
        /// 庄家列表事,件Button
        /// </summary>
        public virtual void OnBankerBtnClick()
        {
            ChangeUserList(UserListType.Banker);
            RefreahAll(UserListType.Banker);
            ScrollView.ResetPosition();
        }

        protected string[] Players = new string[0];
        public void RefreshPlayer(string[] players)
        {
            Players = players;
            RefreahAll(UserListType.User);
        }

        protected virtual void RefreahAll(UserListType ult, int front = 0)
        {
            switch (ult)
            {
                case UserListType.User:
                    {
                        if (Players == null || Players.Length <= 0)
                        {
                            if (_gridusers != null)
                                Destroy(_gridusers.gameObject);
                            return;
                        }
                        YxWindowUtils.CreateMonoParent(GridUsersPrefab, ref _gridusers);
                        var len = front > 0 && front < Players.Length ? front : Players.Length;
                        for (var i = 0; i < len; i++)
                        {
                            var userItem = YxWindowUtils.CreateItem(UserItemPrefab, _gridusers.transform);
                            userItem.SetInfo(Players[i]);
                        }
                        _gridusers.gameObject.SetActive(_curUserlistType == ult);
                        _gridusers.Reposition();
                    }
                    break;

                case UserListType.Banker:
                    {
                        if (BankersInfo == null || BankersInfo.Length <= 0)
                        {
                            if (_gridBankers != null)
                                Destroy(_gridBankers.gameObject);
                            return;
                        }
                        YxWindowUtils.CreateMonoParent(GridBankersPrefab, ref _gridBankers);
                        var bseat = App.GetGameData<BjlGameData>().BankSeat;
                        var len = front > 0 && front < BankersInfo.Length ? front : BankersInfo.Length;
                        for (var i = 0; i < len; i++)
                        {
                            var banker = BankersInfo[i];
                            var userItem = YxWindowUtils.CreateItem(BankerItemPrefab, _gridBankers.transform);
                            userItem.SetInfo(banker, banker.Seat == bseat);
                        }
                        _gridBankers.gameObject.SetActive(_curUserlistType == ult);
                        _gridBankers.Reposition();
                    }
                    break;
            }
        }

        public void ChangeUserList(UserListType type)
        {
            _curUserlistType = type;
            if (_gridBankers != null) _gridBankers.gameObject.SetActive(type == UserListType.Banker);
            if (_gridusers != null) _gridusers.gameObject.SetActive(type == UserListType.User);
        }
    }

    /// <summary>
    /// 玩家列表状态
    /// </summary>
    public enum UserListType
    {
        User, //显示玩家
        Banker//显示庄家
    }
}