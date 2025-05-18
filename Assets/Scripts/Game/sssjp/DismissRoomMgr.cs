using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.View;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.sssjp
{
    public class DismissRoomMgr : MonoBehaviour
    {

        // Use this for initialization
        protected void Start()
        {
            _agreeBtn.onClick.Add(new EventDelegate(() => { DismissRoom(3); SetMannerBtn(false); }));

            _disagreeBtn.onClick.Add(new EventDelegate(() => { DismissRoom(-1); SetMannerBtn(false); }));
        }

        /// <summary>
        /// 隐藏解散房间窗口(隐藏子层级)
        /// </summary>
        public void HideRoomDismiss()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                _itemList[i].ResetPlayerType();
            }
        }

        /// <summary>
        /// 预制体
        /// </summary>
        [SerializeField]
        private DismissItem _itemPrefab = null;


        /// <summary>
        /// 玩家投票信息元素的Grid
        /// </summary>
        [SerializeField]
        private UIGrid _dismissGrid = null;

        /// <summary>
        /// 同意解散房间按钮
        /// </summary>
        [SerializeField]
        private UIButton _agreeBtn = null;

        /// <summary>
        /// 不同意解散房间按钮
        /// </summary>
        [SerializeField]
        private UIButton _disagreeBtn = null;


        /// <summary>
        /// 解散房间倒计时Label
        /// </summary>
        [SerializeField]
        private UILabel _countDownValue = null;



        [SerializeField]
        private string _timeFormat = "{0:D2}";


        private readonly List<DismissItem> _itemList = new List<DismissItem>();

        /// <summary>
        /// 解散房间的倒计时
        /// </summary>
        private int _dismissTime;

        public bool PlayersDismissState()
        {
            if (_itemList.Count == 0)
            {
                return false;
            }
            return true;
//            var amount = 0;
//            for (int i = 0; i < _itemList.Count; i++)
//            {
//                if (_itemList[i].PlayerType == 3)
//                {
//                    amount++;
//                }
//            }
//            return amount == _itemList.Count;
        }

        /// <summary>
        /// 显示解散房间的窗口
        /// </summary>
        /// <param name="passTime">已经经过的时间</param>
        public void ShowRoomDismiss(int passTime = 0)
        {
            if (gameObject.activeSelf)
            {
                return;
            }

            _dismissGrid.transform.DestroyChildren();
            _itemList.Clear();

            var gdata = App.GetGameData<SssGameData>();
            var parent = _dismissGrid.transform;

            foreach (var user in gdata.PlayerList)
            {
                var userInfo = user.Info;
                if (userInfo == null || int.Parse(userInfo.UserId) <= 0)
                {
                    continue;
                }

                DismissItem item = Instantiate(_itemPrefab);
                item.transform.parent = parent;
                item.transform.localScale = Vector3.one;
                item.PlayerName = userInfo.NickM;
                item.PlayerId = int.Parse(userInfo.UserId);
                item.PlayerIdLabel = userInfo.UserId;
                PortraitDb.SetPortrait(userInfo.AvatarX, item.HeadImage, userInfo.SexI);
                item.gameObject.SetActive(true);
                item.PlayerType = 2;
                _itemList.Add(item);
            }

            SetMannerBtn(true);
            _dismissGrid.hideInactive = true;
            _dismissGrid.repositionNow = true;
            _dismissGrid.Reposition();

            gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(true);

            _dismissTime = 300 - passTime;
            StartCoroutine(CountDownDismiss());
        }


        /// <summary>
        /// 更新玩家投票结果
        /// </summary>
        /// <param name="id">玩家id</param>
        /// <param name="playerType">玩家投票态度,3为同意,-1为拒绝</param>
        public void UpdateDismissInfo(int id, int playerType)
        {
            if (playerType == 2)
            {
                ShowRoomDismiss();
            }

            SetType(id, playerType == 2 ? 3 : playerType);

            if (id == App.GameData.GetPlayerInfo().Id)
            {
                SetMannerBtn(false);
            }

            if (playerType < 0)
            {
                _dismissTime = 2;
            }
        }

        void SetType(int id, int playType)
        {
            var item = _itemList.Find(ditem => ditem.PlayerId == id);
            if (item == null)
            {
                YxDebug.LogError("未能找到玩家");
                return;
            }
            item.PlayerType = playType;
        }


        /// <summary>
        /// 设置投票按钮是否隐藏
        /// </summary>
        /// <param name="showBtn"></param>
        public void SetMannerBtn(bool showBtn)
        {
            _agreeBtn.gameObject.SetActive(showBtn);
            _disagreeBtn.gameObject.SetActive(showBtn);
        }

        /// <summary>
        /// 发起和决定解散房间
        /// 2发起解散，3同意，-1拒绝
        /// </summary>
        public void DismissRoom(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", int.Parse(App.GetGameData<SssGameData>().GetPlayerInfo().UserId));
            iobj.PutInt(YxFramwork.ConstDefine.RequestKey.KeyType, yon);

            IRequest request = new ExtensionRequest("hup", iobj);
            App.GetRServer<SssjpGameServer>().SendRequest(request);
        }


        /// <summary>
        /// 投票解散房间的倒计时
        /// </summary>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        IEnumerator CountDownDismiss()
        {
            while (_dismissTime > 0)
            {
                _countDownValue.text = string.Format(_timeFormat, --_dismissTime);
                yield return new WaitForSeconds(1.0f);
            }
            transform.GetChild(0).gameObject.SetActive(false);
            gameObject.SetActive(false);
            _dismissGrid.transform.DestroyChildren();
        }

        /// <summary>
        /// 设置解散房间按钮的事件,将此方法引入到解散房间按钮的OnClick事件中即可
        /// </summary>
        public void SetDismissRoomBtn()
        {
            var gdata = App.GetGameData<SssGameData>();
            //房间模式下
            if (gdata.IsRoomGame)
            {
                //如果正在游戏,则需要发起投票,否则玩家自主决定
                if (gdata.IsPlayed)
                {
                    if (gameObject.activeSelf)
                    {
                        transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else
                    {
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要发起投票,解散房间么?",
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnLeft)
                                {
                                    //如果没有显示,则发送消息,否则不发送
                                    if (!gameObject.activeSelf)
                                    {
                                        DismissRoom(2);
                                    }
                                    else
                                    {
                                        //为不重复发送解散房间信息,在其下设置子层级用于屏蔽
                                        YxMessageBox.Show(new YxMessageBoxData()
                                        {
                                            Msg = "请不要频繁发出解散请求",
                                        });

                                        transform.GetChild(0).gameObject.SetActive(true);
                                    }
                                }

                            },
                            IsTopShow = true,
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle

                        });
                    }
                }
                //非游戏状态下
                else
                {
                    //房主可以直接解散房间
                    if (!gdata.DaiKai && App.GetGameManager<SssjpGameManager>().IsRoomOwner)
                    {
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要解散房间吗?",
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnLeft)
                                {
                                    //非茶馆开房
                                    var teaId = App.GetGameData<SssGameData>().TeaID;
                                    Regex reg = new Regex("^[0-9]+$");
                                    Match ma = reg.Match(teaId);
                                    if (!ma.Success)
                                    {
                                        IRequest req = new ExtensionRequest("dissolve", new SFSObject());
                                        App.GetRServer<SssjpGameServer>().SendRequest(req);
                                    }
                                    else
                                    {
                                        App.GetGameManager<SssjpGameManager>().SendLeaveState(0);
                                        App.QuitGame();
                                    }
                                }
                            },
                            IsTopShow = true,
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                        });
                    }
                    //普通玩家则退出房间
                    else
                    {
                        //为防服务器bug,本地临时修改
                        //YxMessageBox.Show("您不是房主,游戏开始前无法解散房间");
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要退出房间么?",
                            Listener = (box, btnName) =>
                            {
                                if (btnName == YxMessageBox.BtnLeft)
                                {
                                    App.GetGameManager<SssjpGameManager>().SendLeaveState(0);
                                    App.QuitGame();
                                }
                            },
                            IsTopShow = true,
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                        });
                    }
                }
            }

            //非开放模式下
            else
            {

                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "确定要退出房间么?",
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            App.GetGameManager<SssjpGameManager>().SendLeaveState(0);
                            App.QuitGame();
                        }
                    },
                    IsTopShow = true,
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                });
            }
        }

        /// <summary>
        /// 重连显示解散房间投票
        /// </summary>
        /// <param name="gameInfo">重连信息</param>
        public void ShowDismissViewOnJion(ISFSObject gameInfo)
        {
            //int passTime = (int)(gameInfo.GetLong("hupnow") - gameInfo.GetLong("hupstart"));
            int passTime = (int)(gameInfo.GetLong("ct") - gameInfo.GetLong("hupstart"));
            ShowRoomDismiss(passTime);

            if (gameInfo.ContainsKey("hup"))
            {
                string[] hupPlayers = gameInfo.GetUtfString("hup").Split(',');
                for (int i = 0; i < hupPlayers.Length; i++)
                {
                    int id = int.Parse(hupPlayers[i]);
                    UpdateDismissInfo(id, 3);
                }
            }
        }
    }
}