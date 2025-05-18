using UnityEngine;
using System.Collections;
using System.Linq;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using YxFramwork.View;
using YxFramwork.Common;


namespace Assets.Scripts.Game.paijiu.Mgr
{
    public class DismissRoomMgr : MonoBehaviour
    {

        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _agreeBtn.onClick.Add(new EventDelegate(() => { App.GetRServer<PaiJiuGameServer>().DismissRoom(3); }));

            _disagreeBtn.onClick.Add(new EventDelegate(() => { App.GetRServer<PaiJiuGameServer>().DismissRoom(-1); }));

            _closeBtn.onClick.Add(new EventDelegate(() => { transform.GetChild(0).gameObject.SetActive(false); }));

            gameObject.SetActive(false);
        }


        /// <summary>
        /// 隐藏解散房间窗口(隐藏子层级)
        /// </summary>
        public void HideRoomDismiss()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }


        public UILabel TitelLabel;

        /// <summary>
        /// 玩家投票信息元素数组
        /// </summary>
        public DismissMsgItem[] DismissItems = null;

        /// <summary>
        /// 玩家投票信息元素的Grid
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UIGrid _dismissGrid = null;


        /// <summary>
        /// 解散房间的倒计时
        /// </summary>
        private int _dismissTime;

        /// <summary>
        /// 同意解散房间按钮
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UIButton _agreeBtn = null;

        /// <summary>
        /// 不同意解散房间按钮
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UIButton _disagreeBtn = null;

        /// <summary>
        /// 关闭解散房间窗口
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UIButton _closeBtn = null;


        /// <summary>
        /// 解散房间倒计时Label
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UILabel _countDownValue = null;

        public GameObject Container;

        /// <summary>
        /// 显示解散房间的窗口
        /// </summary>
        /// <param name="passTime">已经经过的时间</param>
        public void ShowDismissRoom(int passTime = 0)
        {
            SetMannerBtn(true);
            _dismissGrid.hideInactive = true;
            _dismissGrid.Reposition();

            gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(true);

            _dismissTime = 300 - passTime;
            if (!_isCd)
            {
                _isCd = true;
                StartCoroutine(CountDownDismiss());
            }
        }


        bool _isCd;
        void ResetDismissRoom()
        {
            int index = 0;

            //            PaiJiuPlayer[] users =App.GetGameManager<PaiJiuGameManager>().SeatSort;

            var userList = App.GetGameData<PaiJiuGameData>().PlayerList;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < userList.Length; i++)
            {
                PaiJiuPlayer user = (PaiJiuPlayer)userList[i];
                if (user == null || user.Info == null || user.Info.Id <= 0)
                {
                    continue;
                }

                DismissMsgItem item = DismissItems[index++];
                item.PlayerName = user.Info.NickM;
                item.PlayerId = user.Info.Id;
                item.PlayerType = 2;
            }

            for (int i = 0; i < DismissItems.Length; i++)
            {
                DismissItems[i].gameObject.SetActive(i < index);
            }

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
                ResetDismissRoom();
                ShowDismissRoom();
            }

            var item = GetItem(id);


            if (playerType == 2)
            {
                item.PlayerType = 3;
                TitelLabel.text = string.Format("[432C08]玩家 [F53939]{0} [432C08]申请解散房间,请等待其他玩家选择..", item.PlayerName);
            }
            else
            {
                item.PlayerType = playerType;
                if (playerType == -1)
                {
                    _dismissTime = 3;
                }
            }

            var userId = App.GetGameData<PaiJiuGameData>().GetPlayerInfo().UserId;

            if (id == int.Parse(userId))
            {
                SetMannerBtn(false);
            }
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


        DismissMsgItem GetItem(int id)
        {
            return DismissItems.FirstOrDefault(t => t.PlayerId == id);
        }


        /// <summary>
        /// 投票解散房间的倒计时
        /// </summary>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator CountDownDismiss()
        {
            while (_dismissTime > 0)
            {
                _countDownValue.text = string.Format("{0:D2}秒", --_dismissTime);
                yield return new WaitForSeconds(1.0f);
            }
            transform.GetChild(0).gameObject.SetActive(false);
            gameObject.SetActive(false);
            _isCd = false;
            StopCoroutine(CountDownDismiss());
        }

        /// <summary>
        /// 设置解散房间按钮的事件,将此方法引入到解散房间按钮的OnClick事件中即可
        /// </summary>
        public void SetDismissRoomBtn()
        {

            //房间模式下
            if (App.GetGameData<PaiJiuGameData>().IsRoomGame)
            {
                //如果正在游戏,则需要发起投票,否则玩家自主决定
                if (App.GetGameData<PaiJiuGameData>().IsPlayed)
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
                                        App.GetRServer<PaiJiuGameServer>().DismissRoom(2);
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
                    if (App.GameData.SelfSeat == 0)
                    {
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要解散房间吗?",
                            Listener = (box, btnName) =>
                             {
                                 if (btnName == YxMessageBox.BtnLeft)
                                 {
                                     IRequest req = new ExtensionRequest("dissolve", new SFSObject());
                                     App.GetRServer<PaiJiuGameServer>().SendRequest(req);
                                 }
                             },
                            IsTopShow = true,
                            BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                        });

                    }
                    //普通玩家则退出房间
                    else
                    {
                        YxMessageBox.Show(new YxMessageBoxData()
                        {
                            Msg = "确定要退出房间么?",
                            Listener = (box, btnName) =>
                                {
                                    if (btnName == YxMessageBox.BtnLeft)
                                    {
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
        public void ShowDismissOnRejion(ISFSObject gameInfo)
        {
            int passTime = (int)(gameInfo.GetLong("svt") - gameInfo.GetLong("hupstart"));
            ResetDismissRoom();
            ShowDismissRoom(passTime);

            if (gameInfo.ContainsKey("hup"))
            {
                string[] hupPlayers = gameInfo.GetUtfString("hup").Split(',');
                for (int i = 0; i < hupPlayers.Length; i++)
                {
                    int id = int.Parse(hupPlayers[i]);
                    UpdateDismissInfo(id, 3);
                }
                TitelLabel.text = string.Format("[432C08]玩家 [F53939]{0} [432C08]申请解散房间,请等待其他玩家选择..", GetItem(int.Parse(hupPlayers[0])).PlayerName);
            }

        }
    }

}