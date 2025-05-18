using System.Collections;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Tool;
using YxFramwork.View;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.toubao
{
    public class BankerManager : MonoBehaviour
    {
        #region 庄家相关

        public YxBaseGamePlayer Banker;

        /***********庄家显示*************/
        public UILabel BankerNameLabel;
        public UILabel BankerGlodLabel;
        public UILabel BankerScoresLabel;
        public UILabel BankerNumLabel;

        /// <summary>
        /// 设置庄家信息
        /// </summary>
        public void SetBankerInfo(ISFSObject data)
        {
            YxBaseGameUserInfo bankerInfo = null;
            if (data == null)
            {
                bankerInfo = new YxBaseGameUserInfo
                {
                    NickM = "系统",
                    CoinA = 1147483647,
                    Seat = -1,
                    TotalCount = 0,
                    WinTotalCoin = 0
                };
                Banker.SetMaxCoin(1147483646);
                Banker.Info = bankerInfo;
            }
            else
            {
                var newBanker = new YxBaseGameUserInfo();
                newBanker.Parse(data);
                newBanker.NickM = data.GetUtfString("username");
                var oldBnaker = Banker.Info;
                if (oldBnaker == null || oldBnaker.NickM != newBanker.NickM)
                {
                    YxDebug.Log("重置庄家信息!!");
                    newBanker.NickM = data.GetUtfString("username");
                    newBanker.CoinA = data.GetLong("ttgold");
                    newBanker.TotalCount = 0;
                    newBanker.WinTotalCoin = 0;
                    bankerInfo = newBanker;
                    Banker.Info = bankerInfo;
                }
            }
        }

        public bool IsBanker(int seat)
        {
            var info = Banker.Info;
            return info != null && info.Seat == seat;
        }

        #endregion

        #region 预备庄家列表


        #endregion

        #region 玩家列表
        /// <summary>
        /// 列表Item
        /// </summary>
        public GameObject BankerListItem;
        public GameObject BankerItem;
        /*********列表grid*********/
        public UIGrid BankerGrid;
        public UIGrid PlayerGrid;
        /*********列表父节点*********/
        public GameObject BankerList;
        public GameObject PlayerList;
        /********Items对象们*********/
        public ArrayList PlayerListItems = new ArrayList();
        public ArrayList BankerListItems = new ArrayList();
        /// <summary>
        /// 当前列表类型
        /// </summary>
        public ListType CurListType = ListType.None;
        /// <summary>
        /// 玩家列表最小数量
        /// </summary>
        public int PlayerListMinCount;

        /// <summary>
        /// 刷新玩家列表
        /// </summary>
        /// <param name="data"></param>
        public void RefreshPlayerList(ISFSArray data, int banker)
        {
            int len = data.Count >= PlayerListMinCount ? data.Count : PlayerListMinCount;

            for (int i = PlayerListItems.Count - 1; i >= len; i--)
            {
                Destroy(PlayerListItems[i] as GameObject);
                PlayerListItems.RemoveAt(i);
            }

            //UpOrDown(data);

            int index = 0;
            bool isTop = true;

            for (int i = 0; i < len; i++)
            {
                GameObject gob;
                if (PlayerListItems.Count <= i)
                {
                    gob = Instantiate(BankerItem);
                    PlayerListItems.Add(gob);
                    gob.transform.parent = PlayerGrid.transform;
                    gob.transform.localScale = Vector3.one;
                }

                gob = PlayerListItems[index] as GameObject;
                if (gob == null) { continue; }
                //数据赋值
                if (i < data.Count && data.GetSFSObject(i).GetInt("seat") == banker) { continue; }
                else if (i < data.Count)
                {
                    index++;
                    gob.transform.FindChild("Name").GetComponent<UILabel>().text = data.GetSFSObject(i).GetUtfString("username");
                    var gold = data.GetSFSObject(i).GetLong("ttgold");
                    UILabel goldLabel = gob.transform.FindChild("Gold").GetComponent<UILabel>();
                    GameObject go = goldLabel.transform.FindChild("Sprite").gameObject;
                    if (go != null) go.SetActive(true);
                    goldLabel.text = YxUtiles.GetShowNumberForm(gold);
                }
                else
                {
                    gob.transform.FindChild("Name").GetComponent<UILabel>().text = "";
                    UILabel goldLabel = gob.transform.FindChild("Gold").GetComponent<UILabel>();
                    GameObject go = goldLabel.transform.FindChild("Sprite").gameObject;
                    if (go != null) go.SetActive(false);
                    goldLabel.text = "";
                }
            }
            PlayerGrid.Reposition();
        }
        /// <summary>
        /// 庄家列表最小数量
        /// </summary>
        public int BankerListMinCount;
        /// <summary>
        /// 上下庄按键
        /// </summary>
        public GameObject UpBankerBtn;
        public GameObject DownBankerBtn;

        /// <summary>
        /// 刷新庄家列表
        /// </summary>
        /// <param name="data"></param>
        /// <param name="banker"></param>
        public void RefreshBankerList(ISFSArray data, int banker)
        {
            int len = data.Count >= BankerListMinCount ? data.Count : BankerListMinCount;
            //删除多余的Item
            for (int i = BankerListItems.Count - 1; i >= len; i--)
            {
                Destroy(BankerListItems[i] as GameObject);
                BankerListItems.RemoveAt(i);
            }

            UpOrDown(data);

            int index = 0;
            bool isTop = true;

            for (int i = 0; i < len; i++)
            {
                GameObject gob;
                if (BankerListItems.Count <= i)
                {
                    gob = Instantiate(BankerListItem);
                    BankerListItems.Add(gob);
                    gob.transform.parent = BankerGrid.transform;
                    gob.transform.localScale = Vector3.one;
                }

                gob = BankerListItems[index] as GameObject;
                if (gob == null) { continue; }
                //数据赋值
                if (i < data.Count && data.GetSFSObject(i).GetInt("seat") == banker)
                {
                    SetBankerInfo(data.GetSFSObject(i));
                }
                else if (i < data.Count)
                {
                    index++;
                    gob.transform.FindChild("Name").GetComponent<UILabel>().text = data.GetSFSObject(i).GetUtfString("username");
                    var gold = data.GetSFSObject(i).GetLong("ttgold");
                    gob.transform.FindChild("Gold").GetComponent<UILabel>().text = YxUtiles.GetShowNumberForm(gold);
                }
                else
                {
                    gob.transform.FindChild("Name").GetComponent<UILabel>().text = "";
                    gob.transform.FindChild("Gold").GetComponent<UILabel>().text = "";
                }
            }
            BankerGrid.Reposition();
        }

        public void UpOrDown(ISFSArray data)
        {
            bool haveSelf = false;
            for (int i = 0; i < data.Count; i++)
            {
                if (data.GetSFSObject(i).GetInt("seat") == App.GameData.SelfSeat)
                {
                    haveSelf = true;
                }
            }
            if (haveSelf)
            {
                DownBankerBtn.SetActive(true);
                UpBankerBtn.SetActive(false);
            }
            else
            {
                //DownBankerBtn.GetComponent<UIButton>().isEnabled = true;
                DownBankerBtn.SetActive(false);
                UpBankerBtn.SetActive(true);
                App.GetGameData<GlobalData>().BankerApplying = false;
                ChangeTheBt();
            }
        }


        #endregion

        public int BankerLimit = 0;
        public UISprite Sure;
        public UISprite Cancle;

        public void ClickBankerApply()
        {
            if (App.GameData.GetPlayerInfo().CoinA < BankerCtrl.GetInstance().MinApplyBanker)
            {
                var limit = YxUtiles.GetShowNumberToString(BankerCtrl.GetInstance().MinApplyBanker);
                YxMessageBox.Show("开店资金需要" + limit + "金币.");
                Debug.Log("开店资金需要" + limit + "金币.");
                return;
            }
            App.GetRServer<TouBaoGameServer>().ApplyBaker(!App.GetGameData<GlobalData>().BankerApplying);
        }

        public void ChangeTheBt()
        {
            if (App.GetGameData<GlobalData>().BankerApplying)
            {
                Cancle.gameObject.SetActive(true);
                Sure.gameObject.SetActive(false);
            }
            else
            {
                Cancle.gameObject.SetActive(false);
                Sure.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 申请下庄
        /// </summary>
        public void ApplyQuit()
        {
            App.GetRServer<TouBaoGameServer>().ApplyQuit();
        }
    }
}
