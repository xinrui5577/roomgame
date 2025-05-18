using System.Globalization;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.brnn
{
    public class UserListCtrl : MonoBehaviour
    {

        public UIGrid GridBankers;
        public UIGrid GridUsers;
        public GameObject ModelList;
        public GameObject ModelListBanker;
        public UIScrollBar UsBar;
        public UIScrollView ScrollView;

        public void RefreshBanker(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BrnnGameData>();
            foreach (GameObject i in CommonObject.CurrentBankerList)
            {
                if (i == null) continue;
                i.SetActive(false);
                Destroy(i);
            }
            CommonObject.CurrentBankerList.Clear();
            ISFSArray banbers = responseData.GetSFSArray("bankers");
            int b = responseData.GetInt("banker");
            
            bool isHaveMe = false;
            if (banbers == null || banbers.Size() == 0)
            {
                ApplyCtrl.Instance.SetStutus(gdata.GetPlayerInfo().CoinA >= gdata.MiniApplyBanker ? 1 : 2);
                gdata.CurrentBanker.Info = new YxBaseUserInfo
                {
                    NickM = "系统庄",
                    CoinA = long.MaxValue
                };
                return;
            }

            for (int i = 0; i < banbers.Size(); i++)
            {
                var banber = banbers.GetSFSObject(i);
                var brnnUser = new YxBaseUserInfo
                    {
                        Seat = banber.GetInt("seat"),
                        CoinA = banber.GetLong("ttgold"),
                        NickM = banber.GetUtfString("name")
                    };

                //同步本地玩家数据
                var player = gdata.GetPlayerInfo(brnnUser.Seat, true);
                if(player != null)
                    player.CoinA = brnnUser.CoinA;    

                //刷新庄家数据
                if (brnnUser.Seat == b)
                {
                    var bankerInfo = gdata.GetPlayerInfo(b,true);

                    if (bankerInfo != null)
                    {
                        bankerInfo.CoinA = brnnUser.CoinA;
                        bankerInfo.NickM = brnnUser.NickM;
                        bankerInfo.Seat = brnnUser.Seat;
                        
                        gdata.CurrentBanker.Info = bankerInfo;
                    }

                    GameObject temp = CloneUser(ModelList, GridBankers.gameObject);
                    temp.name = "0";
                    temp.GetComponent<UserListModel>().SetInfo(brnnUser, true, i + 1);
                    CommonObject.CurrentBankerList.Add(temp);
                }
                else
                {
                    GameObject temp = CloneUser(ModelList, GridBankers.gameObject);
                    temp.name = (9999999999 - brnnUser.CoinA).ToString(CultureInfo.InvariantCulture);
                    temp.GetComponent<UserListModel>().SetInfo(brnnUser, false, i + 1);
                    CommonObject.CurrentBankerList.Add(temp);
                }
                if (gdata.GetPlayerInfo().Seat == brnnUser.Seat)
                {
                    isHaveMe = true;
                }
            }

            
            if (isHaveMe)
            {
                ApplyCtrl.Instance.SetStutus(0);
            }
            else
            {
                ApplyCtrl.Instance.SetStutus(App.GameData.GetPlayerInfo().CoinA >=
                                             App.GetGameData<BrnnGameData>().MiniApplyBanker
                                                 ? 1
                                                 : 2);
            }
            GridBankers.repositionNow = true;
            GridBankers.Reposition();
            ScrollView.UpdatePosition();
        }
        private GameObject CloneUser(GameObject cloned, GameObject p)
        {
            var temp = Instantiate(cloned);
            temp.transform.parent = p.transform;
            temp.transform.localScale = Vector3.one;
            temp.SetActive(true);
            return temp;
        }

        protected void Start()
        {
            Invoke("OnBankerButtonClick", 1f);
        }

        public void OnUserButtonClick()
        {
            GridBankers.gameObject.SetActive(false);
            GridUsers.gameObject.SetActive(true);
            GridUsers.repositionNow = true;
            GridUsers.Reposition();
            ScrollView.ResetPosition();
        }
        public void OnBankerButtonClick()
        {
            GridBankers.gameObject.SetActive(true);
            GridUsers.gameObject.SetActive(false);
            GridBankers.repositionNow = true;
            GridBankers.Reposition();
            ScrollView.ResetPosition();
        }
        public void RefreshPlayer(string[] players)
        {
            foreach (GameObject i in CommonObject.CurrentPlayerList)
            {
                if (i == null) continue;
                i.SetActive(false);
                Destroy(i);
            }
            CommonObject.CurrentPlayerList.Clear();
            for (int i = 0; i < players.Length; i++)
            {
                GameObject temp = CloneUser(ModelList, GridUsers.gameObject);
                CommonObject.CurrentPlayerList.Add(temp);
                temp.GetComponent<UserListModel>().SetInfo(players[i], i + 1);
            }
         
            GridUsers.Reposition();
            ScrollView.UpdatePosition();
        }
    }
}
