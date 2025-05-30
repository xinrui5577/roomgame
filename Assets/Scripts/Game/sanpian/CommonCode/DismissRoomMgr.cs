﻿using System.Collections;
using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.server;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.sanpian.CommonCode
{
    public class DismissRoomMgr:MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            _agreeBtn.onClick.Add(new EventDelegate(() => { DismissRoom(3); SetMannerBtn(false); }));

            _disagreeBtn.onClick.Add(new EventDelegate(() => { DismissRoom(-1); SetMannerBtn(false); }));

            _closeBtn.onClick.Add(new EventDelegate(() => { transform.GetChild(0).gameObject.SetActive(false); }));

            Container = transform.GetChild(0).gameObject;

            Container.SetActive(false);
        }

        /// <summary>
        /// 隐藏解散房间窗口(隐藏子层级)
        /// </summary>
        public void HideRoomDismiss()
        {
            Container.SetActive(false);
        }

        /// <summary>
        /// 内容
        /// </summary>
        public GameObject Container;

        /// <summary>
        /// 玩家投票信息元素数组
        /// </summary>
        public DismissMsgItem[] DismissItems = null;

        /// <summary>
        /// 玩家投票信息元素的Grid
        /// </summary>
        [SerializeField]
        private UIGrid _dismissGrid = null;


        /// <summary>
        /// 解散房间的倒计时
        /// </summary>
        private int _dismissTime = 0;

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
        /// 关闭解散房间窗口
        /// </summary>
        [SerializeField]
        private UIButton _closeBtn = null;


        /// <summary>
        /// 解散房间倒计时Label
        /// </summary>
        [SerializeField]
        private UILabel _countDownValue = null;

        /// <summary>
        /// 显示解散房间的窗口
        /// </summary>
        /// <param name="passTime">已经经过的时间</param>
        public void ShowRoomDismiss(int passTime = 0)
        {
            if (Container.activeSelf)
            {
                return;
            }

            int index = 0;

            int selfId = App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Id;


            PlayerCtrl[] users = App.GetGameManager<SanPianGameManager>().PlayerArr;
            foreach (PlayerCtrl user in users)
            {
                if (user == null || user.userInfo == null || user.userInfo.Id <= 0)
                {
                    continue;
                }

                DismissMsgItem item = DismissItems[index++];
                item.PlayerName = user.userInfo.Name;
                item.PlayerID = user.userInfo.Id;
                item.PlayerType = 2;
            }

            for (int i = 0; i < DismissItems.Length; i++)
            {
                DismissItems[i].gameObject.SetActive(i < index);
            }
            SetMannerBtn(true);
            _dismissGrid.hideInactive = true;
            _dismissGrid.Reposition();

            Container.SetActive(true);

            //_dismissRoom.transform.GetChild(0).gameObject.SetActive(true);

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
                ShowRoomDismiss();

            for (int i = 0; i < DismissItems.Length; i++)
            {
                if (DismissItems[i].PlayerID == id)
                {

                    if (playerType == 2)
                    {
                        ShowRoomDismiss();
                        DismissItems[i].PlayerType = 3;

                    }
                    else
                    {
                        DismissItems[i].PlayerType = playerType;
                        if (playerType == -1)
                        {
                            _dismissTime = 3;
                        }
                    }

                }
            }

            if (id == App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Id)
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




        /// <summary>
        /// 发起和决定解散房间
        /// 2发起解散，3同意，-1拒绝
        /// </summary>
        public void DismissRoom(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Id);
            iobj.PutInt(YxFramwork.ConstDefine.RequestKey.KeyType, yon);

            IRequest request = new ExtensionRequest("hup", iobj);
            App.GetRServer<SanPianGameServer>().SendRequest(request);
        }


        /// <summary>
        /// 投票解散房间的倒计时
        /// </summary>
        IEnumerator CountDownDismiss()
        {
            while (_dismissTime > 0)
            {
                _countDownValue.text = string.Format("剩余时间 : {0:D2}秒", --_dismissTime);
                yield return new WaitForSeconds(1.0f);
            }
            Container.SetActive(false);

            StopCoroutine(this.CountDownDismiss());
        }

        /// <summary>
        /// 设置解散房间按钮的事件,将此方法引入到解散房间按钮的OnClick事件中即可
        /// </summary>
        public void SetDismissRoomBtn()
        {

            //房间模式下
            if (App.GetGameData<SanPianGameData>().IsRoomGame)
            {
                //如果正在游戏,则需要发起投票,否则玩家自主决定
                if (App.GetGameData<SanPianGameData>().IsPlayed)
                {
                    if (!Container.activeSelf)
                    {
                    //    this.transform.GetChild(0).gameObject.SetActive(true);
                    //}
                    //else
                    //{
                        YxMessageBox.Show("确定要发起投票,解散房间么?", "", (box, name) =>
                        {
                            if (name == YxMessageBox.BtnLeft)
                            {
                                //如果没有显示,则发送消息,否则不发送
                                if (!Container.activeSelf)
                                {
                                    DismissRoom(2);
                                }
                                else
                                {
                                    //为不重复发送解散房间信息,在其下设置子层级用于屏蔽
                                    YxMessageBox.Show("请不要频繁发出解散请求!!", 5);
                                    Container.SetActive(true);
                                }
                            }

                        }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                    }
                }
                //非游戏状态下
                else
                {

                    //房主可以直接解散房间
                    if (App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Id == App.GetGameData<SanPianGameData>().OwnerId)
                    {

                        YxMessageBox.Show("确定要解散房间吗?", "", (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                IRequest req = new ExtensionRequest("dissolve", new SFSObject());
                                App.GetRServer<SanPianGameServer>().SendRequest(req);
                            }
                        }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                    }
                    //普通玩家则退出房间
                    else
                    {

                        YxMessageBox.Show("确定要退出房间么!!!?", "", (box, btnName) =>
                        {

                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                App.QuitGame();
                            }
                        }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                    }
                }
            }

            //非开放模式下
            else
            {
                YxMessageBox.Show("确定要退出房间么?", "", (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        App.QuitGame();
                    }
                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            }


        }

        /// <summary>
        /// 重连显示解散房间投票
        /// </summary>
        /// <param name="gameInfo">重连信息</param>
        public void ShowDismissOnRejion(ISFSObject gameInfo)
        {
            int passTime = (int)(gameInfo.GetLong("hupnow") - gameInfo.GetLong("hupstart"));
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