using System.Collections;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.SportsCarClub
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
                var oldBnaker = Banker.Info;
                if (oldBnaker == null || oldBnaker.NickM != newBanker.NickM)
                {
                    YxDebug.Log("重置庄家信息!!");
                    newBanker.TotalCount = 0;
                    newBanker.WinTotalCoin = 0;
                    Banker.Info = newBanker;
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
        public GameObject ListItem;
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

        public void RefreshPlayerList(string[] sArray)
        {
            ISFSArray pingData = new SFSArray();
            for (int i = 0; i < sArray.Length; i++)
            {
                string[] data = sArray[i].Split(',');
                ISFSObject obj = new SFSObject();
                obj.PutUtfString("name", data[0]);
                obj.PutLong("ttgold", long.Parse(data[1]));
                pingData.AddSFSObject(obj);
            }

            RefreshPlayerList(pingData);
        }

        /// <summary>
        /// 刷新玩家列表
        /// </summary>
        /// <param name="data"></param>
        public void RefreshPlayerList(ISFSArray data)
        {
            int len = data.Count >= PlayerListMinCount ? data.Count : PlayerListMinCount;
            //删除多余的Item
            for (int i = PlayerListItems.Count - 1; i >= len; i--)
            {
                Destroy(PlayerListItems[i] as GameObject);
                PlayerListItems.RemoveAt(i);
            }
            for (int i = 0; i < len; i++)
            {
                GameObject gob;
                if (PlayerListItems.Count <= i)
                {
                    gob = Instantiate(ListItem) as GameObject;
                    PlayerListItems.Add(gob);
                    gob.transform.parent = PlayerGrid.transform;
                    gob.transform.localScale = Vector3.one;
                }

                gob = PlayerListItems[i] as GameObject;
                if (gob != null)
                {
                    var namelabel = gob.transform.FindChild("Name").GetComponent<UILabel>();
                    var goldlabel = gob.transform.FindChild("Gold").GetComponent<UILabel>();
                    //数据赋值
                    if (i < data.Count)
                    {
                        namelabel.text = data.GetSFSObject(i).GetUtfString("name");
                        var gold = data.GetSFSObject(i).GetLong("ttgold");
                        goldlabel.text = "￥" + YxUtiles.GetShowNumberForm(gold);
                    }
                    else
                    {
                        namelabel.text = "";
                        goldlabel.text = "";
                    }
                }
            }

            CtrlCoinSpriteVis(PlayerListItems);

            PlayerGrid.Reposition();
        }
        /// <summary>
        /// 庄家列表最小数量
        /// </summary>
        public int BankerListMinCount;
        /// <summary>
        /// 下次做庄的玩家信息
        /// </summary>
        public UILabel NextBankerNameLabel;
        public UILabel NextBankerGoldLabel;
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

            NextBankerNameLabel.text = "等待玩家开店...";
            NextBankerGoldLabel.text = "￥0";

            int index = 0;
            bool isTop = true;

            for (int i = 0; i < len; i++)
            {
                GameObject gob;
                if (BankerListItems.Count <= i)
                {
                    gob = Instantiate(ListItem);
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
                else if (i < data.Count && isTop)
                {
                    isTop = false;
                    NextBankerNameLabel.text = data.GetSFSObject(i).GetUtfString("username");
                    NextBankerGoldLabel.text = "￥" + YxUtiles.GetShowNumberForm(data.GetSFSObject(i).GetLong("ttgold"));
                }
                else if (i < data.Count)
                {
                    index++;
                    gob.transform.FindChild("Name").GetComponent<UILabel>().text = data.GetSFSObject(i).GetUtfString("username");
                    var gold = data.GetSFSObject(i).GetLong("ttgold");
                    gob.transform.FindChild("Gold").GetComponent<UILabel>().text = "￥" + YxUtiles.GetShowNumberForm(gold);
                }
                else
                {
                    gob.transform.FindChild("Name").GetComponent<UILabel>().text = "";
                    gob.transform.FindChild("Gold").GetComponent<UILabel>().text = "";
                }
            }

            CtrlCoinSpriteVis(BankerListItems);

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
                DownBankerBtn.GetComponent<UIButton>().isEnabled = true;
                DownBankerBtn.SetActive(false);
                UpBankerBtn.SetActive(true);
            }
        }

        /// <summary>
        /// 打开列表
        /// </summary>
        /// <param name="lt"></param>
        public void OpenList(ListType lt)
        {
            if (CurListType == lt)
            {
                lt = ListType.None;
            }

            switch (lt)
            {
                case ListType.None:
                    CurListType = lt;
                    BankerList.SetActive(false);
                    PlayerList.SetActive(false);
                    break;
                case ListType.Banker:
                    CurListType = ListType.Banker;
                    BankerList.SetActive(true);
                    PlayerList.SetActive(false);
                    break;
                case ListType.Player:
                    CurListType = ListType.Player;
                    BankerList.SetActive(false);
                    PlayerList.SetActive(true);
                    break;
            }
        }

        public void OpenPlayerList()
        {
            OpenList(ListType.Player);
        }

        public void OpenBankerList()
        {
            OpenList(ListType.Banker);
        }

        public void CloseList()
        {
            OpenList(ListType.None);
        }

        #endregion

        //控制金币图标显隐
        private void CtrlCoinSpriteVis(ArrayList arrayList)
        {
            foreach (var item in arrayList)
            {
                if (((GameObject)item).transform.FindChild("Name").GetComponent<UILabel>().text != ""
                    && ((GameObject)item).transform.FindChild("Gold").GetComponent<UILabel>().text != "")
                {
                    ((GameObject)item).transform.FindChild("CoinSprite").gameObject.SetActive(true);
                }
                else
                {
                    ((GameObject)item).transform.FindChild("CoinSprite").gameObject.SetActive(false);
                }
            }
        }
    }
}
