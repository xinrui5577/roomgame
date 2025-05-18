using System.Globalization;
using System.Linq;
using Assets.Scripts.Common.Adapters;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ttz
{
    public class BankerListCtrl : MonoBehaviour
    {
        public UIGrid GridBankers;
        public GameObject BankerItem;
        public UIScrollView ScrollView;
        public GameObject Bg;
        public NguiLabelAdapter BankLimitLabel;


        private bool _isShow = false;
        public Vector3 TargetVector3 = new Vector3(390, 210, 0);

        //先保留以后可能用
        //private UIGrid _gridBankers;
        //public void RefreshBanker(ISFSObject responseData)
        //{
        //    var gdata = App.GetGameData<BrttzGameData>();
        //    if (gdata.CurrentBankerList.Count != 0)
        //    {
        //        Destroy(gdata.CurrentBankerList[0]);
        //        gdata.CurrentBankerList.RemoveAt(0);
        //    }
        //    gdata.CurrentBankerList.Clear();
        //    if (!responseData.ContainsKey("bankers")) return;
        //    var bankers = responseData.GetSFSArray("bankers");
        //    var b = responseData.GetInt("banker");
        //    gdata.BankSeat = b;
        //    IsHasMe = false;
        //    YxBaseGameUserInfo info = null;
        //    if (b == -1)
        //    {
        //        info = new YxBaseGameUserInfo
        //        {
        //            NickM = "系统",
        //            CoinA = 1147483647,
        //            Seat = -1,
        //            TotalCount = 0,
        //            WinTotalCoin = 0
        //        };
        //        gdata.BankerPlayer.SetMaxCoin(1147483646);
        //        gdata.BankerPlayer.Info = info;
        //        gdata.BankerPlayer.HideBankerTime();
        //        return;
        //    }

        //    if (bankers.Size() == 0)
        //    {
        //        gdata.CurrentBanker.Seat = b;
        //    }

        //    YxWindowUtils.CreateMonoParent(GridBankers, ref _gridBankers);
        //    _gridBankers.transform.localPosition = GridBankers.transform.localPosition;
        //    for (int i = 0; i < bankers.Size(); i++)
        //    {
        //        var banker = bankers.GetSFSObject(i);
        //        var user = new YxBaseUserInfo
        //        {
        //            Seat = banker.GetInt("seat"),
        //            CoinA = banker.GetLong("ttgold"),
        //            NickM = banker.GetUtfString("username")
        //        };

        //        if (user.Seat == b)
        //        {
        //            var bankerInfo = gdata.GetPlayerInfo(b, true);
        //            if (bankerInfo != null)
        //            {
        //                bankerInfo.CoinA = user.CoinA;
        //                bankerInfo.NickM = user.NickM;
        //                bankerInfo.Seat = user.Seat;
        //                bankerInfo.WinTotalCoin = 0;
        //                gdata.BankerPlayer.Info = bankerInfo;
        //            }
        //            GameObject userItem = CloneUser(BankerItem, _gridBankers.gameObject);
        //            userItem.name = "0";
        //            userItem.GetComponent<UserListModel>().SetInfo(user, true, i + 1);
        //            gdata.CurrentBankerList.Add(userItem);
        //        }
        //        else
        //        {
        //            GameObject userItem = CloneUser(BankerItem, _gridBankers.gameObject);
        //            userItem.name = (9999999999 - user.CoinA) + "";
        //            userItem.GetComponent<UserListModel>().SetInfo(user, false, i + 1);
        //            gdata.CurrentBankerList.Add(userItem);
        //        }

        //        if (gdata.SelfSeat == user.Seat)
        //        {
        //            IsHasMe = true;
        //        }
        //    }
        //    _gridBankers.gameObject.SetActive(true);
        //    _gridBankers.Reposition();
        //    var applyCtrl = App.GetGameManager<BrttzGameManager>().ApplyCtrl;
        //    if (IsHasMe)
        //    {
        //        applyCtrl.SetStutus(0);
        //    }
        //    else
        //    {
        //        applyCtrl.SetStutus(gdata.GetPlayerInfo().CoinA >= gdata.MiniApplyBanker ? 1 : 2);
        //    }
        //}


        private GameObject CloneUser(GameObject cloned, GameObject p)
        {
            var temp = Instantiate(cloned);
            temp.transform.parent = p.transform;
            temp.transform.localScale = Vector3.one;
            temp.SetActive(true);
            return temp;
        }


        public void RefreshBankerList(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            while (gdata.CurrentBankerList.Count != 0)
            {
                Destroy(gdata.CurrentBankerList[0]);
                gdata.CurrentBankerList.RemoveAt(0);
            }
            gdata.CurrentBankerList.Clear();
            if (!responseData.ContainsKey(Parameter.Bankers) || !responseData.ContainsKey(Parameter.Banker)) return;
            var bankers = responseData.GetSFSArray(Parameter.Bankers);
            var b = responseData.GetInt(Parameter.Banker);
            bool isHasMe = false;
            if (bankers == null || bankers.Size() == 0)
            {
                if (b == -1)
                {
                    var info = new YxBaseGameUserInfo
                    {
                        NickM = "系统",
                        CoinA = 1147483647,
                        Seat = -1,
                        TotalCount = 0,
                        WinTotalCoin = 0
                    };
                    gdata.BankerPlayer.SetMaxCoin(1147483646);
                    gdata.BankerPlayer.Info = info;
                    gdata.BankerPlayer.HideBankerTime();
                }
                ApplyCtrl.Instance.SetStutus(gdata.GetPlayerInfo().CoinA >= gdata.MiniApplyBanker ? 1 : 2);
                return;
            }
            var len = bankers.Size();
            for (int i = 0; i < len; i++)
            {
                var banker = bankers.GetSFSObject(i);
                var user = new YxBaseGameUserInfo
                {
                    Seat = banker.GetInt(Parameter.Seat),
                    CoinA = banker.GetLong("ttgold"),
                    NickM = banker.GetUtfString(Parameter.UserName)
                };

                if (user.Seat == b)
                {
                    var oldBanker = gdata.BankerPlayer.Info;
                    var bankerInfo = gdata.GetPlayerInfo(b, true);
                    if (oldBanker == null || oldBanker.NickM != user.NickM)
                    {
                        bankerInfo.CoinA = user.CoinA;
                        bankerInfo.NickM = user.NickM;
                        bankerInfo.Seat = user.Seat;
                        bankerInfo.WinTotalCoin = 0;
                        gdata.BankerPlayer.Info = bankerInfo;
                        gdata.BankerPlayer.SetBankerTime(0);
                    }
                    else gdata.BankerPlayer.Coin = user.CoinA;
                    var temp = CloneUser(BankerItem, GridBankers.gameObject);
                    temp.name = "0";
                    temp.GetComponent<UserListModel>().SetInfo(user, true, i + 1);
                    gdata.CurrentBankerList.Add(temp);
                }
                else
                {
                    var bankerItem = CloneUser(BankerItem, GridBankers.gameObject);
                    bankerItem.name = i + "";
                    bankerItem.GetComponent<UserListModel>().SetInfo(user, false, i + 1);
                    gdata.CurrentBankerList.Add(bankerItem);
                }
                if (gdata.SelfSeat == user.Seat)
                {
                    isHasMe = true;
                }
            }
            var applyCtrl = App.GetGameManager<BrttzGameManager>().ApplyCtrl;
            if (isHasMe)
            {
                applyCtrl.SetStutus(0);
            }
            else
            {
                applyCtrl.SetStutus(gdata.GetPlayerInfo().CoinA >= gdata.MiniApplyBanker ? 1 : 2);
            }
            GridBankers.enabled = true;
            GridBankers.Reposition();
            if (BankLimitLabel != null)
            {
                BankLimitLabel.Text(YxUtiles.ReduceNumber(gdata.MiniApplyBanker));
            }
        }

        public void MoveBgOnClick()
        {
            var sp = Bg.GetComponent<SpringPosition>();
            if (_isShow)
            {
                sp.target = TargetVector3;
                TargetVector3 = Bg.transform.localPosition;
                sp.enabled = true;
            }
            else
            {
                sp.target = TargetVector3;
                TargetVector3 = Bg.transform.localPosition;
                sp.enabled = true;
            }
            _isShow = !_isShow;
        }

    }
}