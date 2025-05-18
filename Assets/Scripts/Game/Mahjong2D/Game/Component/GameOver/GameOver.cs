using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.ImgPress;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.GameOver
{
    public class GameOver : MonoSingleton<GameOver>
    {
        /// <summary>
        /// 玩家数据
        /// </summary>
        private OverData[] playerData;
        /// <summary>
        /// 玩家数据UI
        /// </summary>
        [SerializeField]
        private GameOverItem[] _playerItems;

        [SerializeField]
        private CompressImg Img;

        [SerializeField]
        private GameObject _showParent;

        [SerializeField]
        private UILabel _roomId;

        [SerializeField]
        private UILabel _quanInfo;

        [SerializeField]
        private UILabel _roundInfo;

        [SerializeField]
        private UILabel _ruleInfo;

        [SerializeField]
        private UILabel _nowTime;

        [SerializeField]
        private UIGrid _showGrid;

        [SerializeField]
        private UILabel _roomerInfo;

        private DateTime _nowDateTime;

        /// <summary>
        /// 圈数显示
        /// </summary>
        [SerializeField]
        private string _quanShowInfo = "圈数：";

        /// <summary>
        /// 局数
        /// </summary>
        [SerializeField]
        private string _roundShowInfo = "局数：";

        [SerializeField] private string _roomerShowInfo = "房间名";
        /// <summary>
        /// 房主文本（房间名称显示）
        /// </summary>
        [SerializeField]
        private UILabel _roomerTagLabel;
        /// <summary>
        /// 房间名称
        /// </summary>
        private string _ownerName;
        private CurrentGameType GameType
        {
            get
            {
                return App.GetGameManager<Mahjong2DGameManager>().GameType;
            }
        }

        private Mahjong2DGameData Data
        {
            get
            {
                return App.GetGameData<Mahjong2DGameData>();
            }
        }

        private YieldInstruction waitForScreen;
        public override void Awake()
        {
            base.Awake();
            waitForScreen=new WaitForEndOfFrame();
            if (_showParent == null)
            {
                _showParent = transform.GetChild(0).gameObject;
            }
        }

        public void SetUserOverData(ISFSObject ob)
        {
          
            ISFSArray users;
            long time;
            int ownerId;
            GameTools.TryGetValueWitheKey(ob, out _ownerName, RequestKey.KeyOwnerName);
            GameTools.TryGetValueWitheKey(ob, out users, RequestKey.KeyUsers);
            GameTools.TryGetValueWitheKey(ob, out time, RequestCmd.ServerTime);
            GameTools.TryGetValueWitheKey(ob, out ownerId, RequestKey.KeyOwnerId);
            if (time.Equals(0))
            {
                _nowDateTime = DateTime.Now;
            }
            else
            {
                _nowDateTime = GameTools.GetSvtTime(time);
                YxDebug.LogError("服务器返回的结束时间是");
            }
            CheckResultInfo(users,ownerId);
        }

        /// <summary>
        /// 结算信息检测
        /// </summary>
        /// <param name="users"></param>
        /// <param name="ownerId"></param>
        private void CheckResultInfo(ISFSArray users, int ownerId)
        {
            var usersCount = Data.PlayerNum;
            playerData = new OverData[usersCount];
            int maxScore = 0, minScore = 0;
            var winList = new List<int>();
            var loseList = new List<int>();
            for (int i = 0; i < usersCount; i++)
            {
                ISFSObject obj = users.GetSFSObject(i);
                var item = playerData[i] = new OverData(obj, ownerId);
                var itemGold = item.gold;
                CheckMaxValue(i, itemGold, ref maxScore, winList);
                CheckMinValue(i, itemGold, ref minScore, loseList);
            }
            var winCount = winList.Count;
            for (int i = 0; i < winCount; i++)
            {
                playerData[winList[i]].isYingJia = true;
            }
            var loseCount = loseList.Count;
            for (int i = 0; i < loseCount; i++)
            {
                playerData[loseList[i]].isPaoShou = true;
            }
        }
        /// <summary>
        /// 最大数据检测
        /// </summary>
        /// <param name="checkIndex"></param>
        /// <param name="checkValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="winList"></param>
        private void CheckMaxValue(int checkIndex,int checkValue,ref int maxValue,List<int> winList)
        {
            if (checkValue > maxValue)
            {
                maxValue = checkValue;
                winList.Clear();
                winList.Add(checkIndex);
            }
            else if (checkValue == maxValue)
            {
                if (winList.Count!=0)
                {
                    winList.Add(checkIndex);
                }
            }
        }
        /// <summary>
        /// 最小数据检测
        /// </summary>
        /// <param name="checkIndex"></param>
        /// <param name="checkValue"></param>
        /// <param name="minValue"></param>
        /// <param name="loseList"></param>
        private void CheckMinValue(int checkIndex, int checkValue, ref int minValue, List<int> loseList)
        {
            if (checkValue<minValue)
            {
                minValue = checkValue;
                loseList.Clear();
                loseList.Add(checkIndex);
            }
            else if (checkValue == minValue)
            {
                if (loseList.Count != 0)
                {
                    loseList.Add(checkIndex);
                }
            }
        }

        public void OnBack2HallClick()
        {
            App.QuitGame();
        }

        public void ShowGameOverPanel()
        {
            _showParent.SetActive(true);
            if (_roomId != null)
            {
                _roomId.text = GameType.ShowRoomId.ToString();
            }
            if (_roomerInfo)
            {
                if(string.IsNullOrEmpty(_ownerName))
                {
                    _roomerInfo.text = GameType.RoomName;
                }
                else
                {
                    _roomerInfo.text = _ownerName;
                }
                
            }
            if(_roomerTagLabel)
            {
                if (!string.IsNullOrEmpty(_ownerName))
                {
                    _roomerTagLabel.text = ConstantData.RoomOwner;
                }
                else
                {
                    _roomerTagLabel.text = _roomerShowInfo;
                }
            }
            if (_roundInfo != null)
            {
                string showStr;
                if (GameType.IsQuanExist)
                {
                    if (_quanInfo != null)
                    {
                        _quanInfo.text = _quanShowInfo;
                    }
                    if (GameType.Quan >= GameType.TotalRound)
                        GameType.Quan = GameType.TotalRound - 1;
                    showStr = string.Format("{0}/{1}", GameType.Quan + 1, GameType.TotalRound);
                }
                else
                {
                    if (_quanInfo != null)
                    {
                        _quanInfo.text = _roundShowInfo;
                    }
                    if (GameType.NowRound > GameType.TotalRound)
                        GameType.NowRound = GameType.TotalRound;
                    showStr = string.Format("{0}/{1}", GameType.NowRound, GameType.TotalRound);
                }
                _roundInfo.text = showStr;
            }
            if (_ruleInfo != null)
            {
                if (!string.IsNullOrEmpty(GameType.Rules))
                {
                    _ruleInfo.text = GameType.Rules;
                }
                else
                {
                    _ruleInfo.text = Data.SaveInfo;
                }
            }
            if (_nowTime != null)
            {
                _nowTime.text = _nowDateTime.ToString("yyyy-MM-dd hh:mm:ss");
            }
            for (int i = 0; i < _playerItems.Length; i++)
            {
                if (Data.PlayerNum>i&& playerData.Length>i)
                {
                    _playerItems[i].InitInfo(playerData[i]);
                }
                else
                {
                    _playerItems[i].gameObject.SetActive(false);
                }
            }
            if (_showGrid != null)
            {
                _showGrid.repositionNow = true;
            }
        }
        /// <summary>
        /// 分享截图到朋友圈
        /// </summary>
        public void OnClickShare()
        {
            ShareImage(SharePlat.WxSenceTimeLine);
        }

        public void OnShareFriend()
        {
            
        }

        public void OnShareSession()
        {
            ShareImage(SharePlat.WxSenceSession);
        }

        private void ShareImage(SharePlat plat)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(YxTools.ShareSceenImage(plat, waitForScreen));
            }
        }
    }
}
