using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mdx
{
    public class PlayerBetListCtrl : MonoBehaviour
    {

        public GameObject View;

        public GameObject ListItem;

        public UIGrid Grid;

        public int MaxCount;

        public string BetFormat = "[{0}] 下注 {1}金币 {2}";

        public string WinFormat = "[{0}] 赢取 {1}金币";

        public string LostFormat = "[{0}] 失去 {1}金币";

        private readonly List<GameObject> _objectsList = new List<GameObject>();
        private readonly List<BetListData> _dataList = new List<BetListData>();


        public void GetResultPlayerInfo(ISFSObject data)
        {
            if (!data.ContainsKey("playerlist")) return;
            var array = data.GetSFSArray("playerlist");
            if (array == null || array.Count == 0) return;
            foreach (ISFSObject item in array)
            {
                int gold = item.GetInt("win");
                if (gold > 0)
                {
                    AddListInfo(item.GetUtfString("username"), gold, -1);
                }
            }
        }


        public void AddListInfo(string playerName, int gold,int p)
        {
            BetListData data = new BetListData
            {
                PlayerName = playerName,
                Gold = YxUtiles.ReduceNumber(gold),
                Side = p,
            };
            _dataList.Insert(0, data);
            if (_dataList.Count > MaxCount)
            {
                var betData = _dataList[MaxCount];
                _dataList.Remove(betData);
            }
            if (Grid.gameObject.activeInHierarchy)
            {
                RefreshView();
            }
        }                 



        public void RefreshView()
        {
            int count = _dataList.Count;
           
            for (int i = 0; i < count; i++)
            {
                var data = _dataList[i];
                var temp = GetItem(i);
                var label = temp.GetComponent<UILabel>();
                int p = data.Side;
                if (p < 0)
                {
                    label.text = string.Format(WinFormat, data.PlayerName, data.Gold);
                }
                else
                {
                    label.text = string.Format(BetFormat, data.PlayerName, data.Gold, data.Side == 0 ? "押大" : "押小");
                }
                //string.Format(data.Side >= 0 ? BetFormat : data.Gold > 0 ? WinFormat : LostFormat, data.PlayerName,
                    //    data.Gold,data.Side >);
                temp.SetActive(true);
            }

            Grid.repositionNow = true;
            Grid.Reposition();
        }  

        GameObject GetItem(int index)
        {
            if (_objectsList.Count <= index)
            {
                var temp = Instantiate(ListItem);
                temp.transform.parent = ListItem.transform.parent;
                temp.transform.localScale = ListItem.transform.localScale;
                _objectsList.Add(temp);
                return temp;
            }
            return _objectsList[index];
        }

        public void OnClickShowBetListViewBtn()
        {
            View.SetActive(true);
            RefreshView();
        }

        public void OnClickCloseBtn()
        {
            View.SetActive(false);
        }

        internal void AddBetInfo(ISFSObject response)
        {
            int seat = response.GetInt("seat");
            int gold = response.GetInt("gold");
            int p = MdxTools.GetP(response.GetUtfString("p")) ;
            var userInfo = App.GameData.GetPlayerInfo(seat, true);
            if (userInfo == null) return;
            AddListInfo(userInfo.NickM, gold, p);
        }

        public void AddGroupBetInfo(ISFSObject response)
        {
            if (!response.ContainsKey("coin"))
                return;
            var sfsArray = response.GetSFSArray("coin");

            int count = sfsArray.Count;
            for (int i = 0; i < count; i++)
            {
                ISFSObject item = sfsArray.GetSFSObject(i);
                AddBetInfo(item);
            }
        }

        public void CleanView()
        {
            _dataList.Clear();
            foreach (var item in _objectsList)
            {
                item.SetActive(false);
            }
        }
    }


    public class BetListData
    {
        public string PlayerName;
        public string Gold;
        public int Side;
    }

}
