using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Texas
{
    public class TotalScoreView : MonoBehaviour
    {

        public UIGrid Grid;

        public TweenPosition TweenView;

        public UISprite BgSprite;

        public TotalScoreViewItem ItemPrefab;

        public int ItemHeight;

        public int BgBottomSpace;

        public GameObject ShowViewBtn;

        public GameObject CloseBtn;

        //private const string TtScoreKey = "-ttsv";

        private const string Variation = "variation";

        private readonly List<TotalScoreUserInfo> _userInfoList = new List<TotalScoreUserInfo>();

        private readonly List<TotalScoreViewItem> _itemPrefabList = new List<TotalScoreViewItem>();

        public void OnGetGameInfo(ISFSObject gameInfo)
        {
            if (!App.GetGameData<TexasGameData>().IsRoomGame)
            {
                gameObject.SetActive(false);
                return;
            }
            _userInfoList.Clear();

            if (!gameInfo.ContainsKey(RequestKey.KeyCargs2)) return;

            //var cargs2 = gameInfo.GetSFSObject(RequestKey.KeyCargs2);
            //gameObject.SetActive(cargs2.ContainsKey(TtScoreKey) && (int.Parse(cargs2.GetUtfString(TtScoreKey)) > 0));
            gameObject.SetActive(true);

            if (gameInfo.ContainsKey(RequestKey.KeyUser))
            {
                AddUserInfoToList(gameInfo.GetSFSObject(RequestKey.KeyUser));
            }
            if (gameInfo.ContainsKey(RequestKey.KeyUserList))
            {
                var userDataArray = gameInfo.GetSFSArray(RequestKey.KeyUserList);
                foreach (ISFSObject userData in userDataArray)
                {
                    AddUserInfoToList(userData);
                }
            }

            RefreshItems();
            SetBgSize();
        }

        public void AddUserInfoToList(ISFSObject userData)
        {
            var userInfo = new TotalScoreUserInfo
            {
                UserName = userData.GetUtfString(RequestKey.KeyName),
                UserId = userData.GetInt(RequestKey.KeyId),
                ServerSeat = userData.GetInt(RequestKey.KeySeat),
                TotalScore = userData.ContainsKey(Variation) ? userData.GetLong(Variation) : 0
            };

            if (GetUserInfoById(userInfo.UserId) == null)
            {
                _userInfoList.Add(userInfo);
            }
        }


        public void OnGetGameResultInfo(ISFSObject gameInfo)
        {
            if (!gameObject.activeSelf) return;
            if (!gameInfo.ContainsKey(RequestKey.KeyPlayerList)) return;
            var gdata = App.GetGameData<TexasGameData>();
            if (!gdata.IsRoomGame) return;
            _userInfoList.Clear();
            ISFSArray userDataArray = gameInfo.GetSFSArray(RequestKey.KeyPlayerList);
            foreach (ISFSObject userData in userDataArray)
            {
                if (!userData.ContainsKey("isGiveUp")) continue;
                int serverSeat = userData.GetInt(RequestKey.KeySeat);
                var userInfo = gdata.GetPlayerInfo(serverSeat, true);
                TotalScoreUserInfo ttUserInfo = new TotalScoreUserInfo
                {
                    TotalScore = userData.GetLong(Variation),
                    ServerSeat = serverSeat,
                    UserName = userInfo.NickM,
                    UserId = userInfo.Id
                };
                _userInfoList.Add(ttUserInfo);
            }

            RefreshItems();
            SetBgSize();
        }

        public void OnOtherJionin(ISFSObject userData)
        {
            AddPlayer(userData);
            SetBgSize();
            RefreshItems();
        }

        /// <summary>
        /// 刷新UI界面数据
        /// </summary>
        void RefreshItems()
        {
            int prefabCount = _itemPrefabList.Count;
            int userInfoCount = _userInfoList.Count;
            int max = Mathf.Max(prefabCount, userInfoCount);

            for (int i = 0; i < max; i++)
            {
                if (i >= prefabCount)
                {
                    CreatePrefab();
                    prefabCount++;
                }

                var prefab = _itemPrefabList[i];
                if (i >= userInfoCount)
                {
                    prefab.gameObject.SetActive(false);
                    continue;
                }
                
                var userInfo = _userInfoList[i];
                if (userInfo == null)
                {
                    prefab.gameObject.SetActive(true);
                    continue;
                }
                prefab.SetInfo(userInfo);
            }

            Grid.repositionNow = true;
            Grid.Reposition();
        }

        
        private void CreatePrefab()
        {
            var item = Instantiate(ItemPrefab);
            item.transform.parent = Grid.transform;
            item.transform.localScale = Vector3.one;
            item.gameObject.SetActive(true);
            _itemPrefabList.Add(item);
        }



        public void AddPlayer(ISFSObject userData)
        {
            if (!gameObject.activeSelf) return;

            var gdata = App.GetGameData<TexasGameData>();
            if (!gdata.IsRoomGame) return;

            if (userData.ContainsKey("user"))
            {
                userData = userData.GetSFSObject("user");
            }
            AddUserInfoToList(userData);
        }

        TotalScoreUserInfo GetUserInfoById(int id)
        {
            return _userInfoList.Find(item => item.UserId == id);
        }


        public void OnUserOut(int serverSeat)
        {
            var userInfo = _userInfoList.Find(item => item.ServerSeat == serverSeat);
            if (userInfo != null)
            {
                _userInfoList.Remove(userInfo);
            }
            SetBgSize();
        }

        /// <summary>
        /// 设置背景大小
        /// </summary>
        public void SetBgSize()
        {
            int count = _userInfoList.Count; 
            if (count == 0) return;
            int shiftY = ItemHeight*count + BgBottomSpace;
            BgSprite.height = shiftY;

            Vector3 from = TweenView.from;
            Vector3 to = new Vector3(from.x, from.y - shiftY, from.z);
            TweenView.to = to;
        }

        public void OnClickShowViewBtn()
        {
            TweenView.PlayForward();
            CloseBtn.gameObject.SetActive(true);
            ShowViewBtn.GetComponent<BoxCollider>().enabled = false;
        }

        public void OnClickCloseBtn()
        {
            TweenView.PlayReverse();
            CloseBtn.SetActive(false);
            ShowViewBtn.GetComponent<BoxCollider>().enabled = true;
        }

    }

    public class TotalScoreUserInfo
    {
        public string UserName;
        public int UserId;
        public long TotalScore;
        public int ServerSeat;
    }
}
