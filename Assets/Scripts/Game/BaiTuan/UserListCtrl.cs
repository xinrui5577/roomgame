using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.BaiTuan
{
    public class UserListCtrl : MonoBehaviour
    {
        public UIGrid GridBankersPrefab;
        public UITable GridUsersPrefab;
        public GameObject BankerItemPrefab;
        public GameObject UserItemPrefab;

        public bool IsHasMe;
        public UIScrollView ScrollView;

        protected GameObject CloneUser(GameObject cloned, GameObject p)
        {
            var temp = Instantiate(cloned);
            temp.transform.parent = p.transform;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localScale = Vector3.one;
            temp.SetActive(true);
            return temp;
        }

        public virtual void RefreshBanker(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BtwGameData>();
            while (gdata.CurrentBankerList.Count != 0)
            {
                Destroy(gdata.CurrentBankerList[0]);
                gdata.CurrentBankerList.RemoveAt(0);
            }
            gdata.CurrentBankerList.Clear();
            if (!responseData.ContainsKey("bankers")) return;
            var bankers = responseData.GetSFSArray("bankers");
            var b = responseData.GetInt("banker");
            gdata.BankSeat = b;
            bool isHasMe = false;
            YxBaseGameUserInfo info = null;

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
            GridBankersPrefab.Reposition();
        }
        public void OnBankersClick(bool focus)
        {
            if (!focus) return;
            ChangeUserList(UserListType.Banker);
            ScrollView.ResetPosition();
        }
        private GameObject CloneUser(GameObject cloned)
        {
            GameObject temp = Instantiate(cloned);
            temp.transform.parent = cloned.transform.parent;
            temp.transform.localScale = Vector3.one;
            temp.SetActive(true);
            return temp;
        }

        public void OnPlayersClick(bool focus)
        {
            if (!focus) return;
            ChangeUserList(UserListType.User);
            ScrollView.ResetPosition();
        }

        public void RefreshPlayer(string[] players)
        {
            var gdata = App.GetGameData<BtwGameData>();
            while (gdata.CurrentPlayerList.Count != 0)
            {
                Destroy(gdata.CurrentPlayerList[0]);
                gdata.CurrentPlayerList.RemoveAt(0);
            }
            gdata.CurrentPlayerList.Clear();
            foreach (var player in players)
            {
                var userItem = CloneUser(UserItemPrefab, GridUsersPrefab.gameObject);
                gdata.CurrentPlayerList.Add(userItem);
                userItem.GetComponent<UserListModel>().SetInfo(player);
            }
            GridUsersPrefab.gameObject.SetActive(_curUserlistType == UserListType.User);
            GridUsersPrefab.Reposition();
        }

        private UserListType _curUserlistType;
        public void ChangeUserList(UserListType type)
        {
            _curUserlistType = type;
            if (GridBankersPrefab != null)
            {
                GridBankersPrefab.gameObject.SetActive(type == UserListType.Banker);
                if (type == UserListType.Banker) GridBankersPrefab.Reposition();
            }
            if (GridUsersPrefab != null)
            {
                GridUsersPrefab.gameObject.SetActive(type == UserListType.User);
                if (type == UserListType.User) GridUsersPrefab.Reposition();
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
}