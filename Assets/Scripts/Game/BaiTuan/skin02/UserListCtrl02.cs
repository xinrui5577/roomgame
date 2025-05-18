using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class UserListCtrl02 : UserListCtrl
    {
        public override void RefreshBanker(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BtwGameData>();
            while (gdata.CurrentBankerList.Count != 0)
            {
                Destroy(gdata.CurrentBankerList[0]);
                gdata.CurrentBankerList.RemoveAt(0);
            }
            gdata.CurrentBankerList.Clear();
            if (GridBankersPrefab.GetChildList().Count != 0)
            {
                foreach (var child in GridBankersPrefab.GetChildList())
                {
                    child.transform.parent = transform;
                    Destroy(child.gameObject);
                }
            }
            if (!responseData.ContainsKey("bankers")) return;
            var bankers = responseData.GetSFSArray("bankers");
            var b = responseData.GetInt("banker");
            gdata.BankSeat = b;
            bool isHasMe = false;
            YxBaseGameUserInfo info = null;
            if (bankers == null || bankers.Size() == 0)
            {
                if (b == -1)
                {
                    info = new YxBaseGameUserInfo
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

            for (int i = 0; i < bankers.Size(); i++)
            {
                var banker = bankers.GetSFSObject(i);
                var user = new YxBaseUserInfo
                {
                    Seat = banker.GetInt("seat"),
                    CoinA = banker.GetLong("ttgold"),
                    NickM = banker.GetUtfString("username")
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

                        gdata.BankerPlayer.Info = bankerInfo;
                        gdata.BankerPlayer.SetBankerTime(0);
                    }
                    var userItem = CloneUser(BankerItemPrefab, GridBankersPrefab.gameObject);
                    userItem.name = "0";
                    userItem.GetComponent<UserListModel>().SetInfo(user, true, i + 1);
                    gdata.CurrentBankerList.Add(userItem);
                }
                else
                {
                    var userItem = CloneUser(BankerItemPrefab, GridBankersPrefab.gameObject);
                    userItem.name = i + "";
                    userItem.GetComponent<UserListModel>().SetInfo(user, false, i + 1);
                    gdata.CurrentBankerList.Add(userItem);
                }

                if (gdata.SelfSeat == user.Seat)
                {
                    isHasMe = true;
                }
            }

            var applyCtrl = App.GetGameManager<BtwGameManager>().ApplyCtrl;
            if (isHasMe)
            {
                applyCtrl.SetStutus(0);
            }
            else
            {
                applyCtrl.SetStutus(gdata.GetPlayerInfo().CoinA >= gdata.MiniApplyBanker ? 1 : 2);
            }
            GridBankersPrefab.gameObject.SetActive(true);
            GridBankersPrefab.repositionNow = true;
            GridBankersPrefab.Reposition();
        }


        public void OnBankersClick()
        {
            GridBankersPrefab.gameObject.SetActive(true);
            GridUsersPrefab.gameObject.SetActive(false);
            GridBankersPrefab.Reposition();
            ScrollView.ResetPosition();
        }
    }
}