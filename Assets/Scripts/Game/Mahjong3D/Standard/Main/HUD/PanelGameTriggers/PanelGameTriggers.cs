using YxFramwork.ConstDefine;
using YxFramwork.View;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum PulldowmBtnType
    {
        Wechat,
        Ready,
        GameExplan,
        Setting,
        DismissRoom,
        Return,
        ExchangeRoom,
        MahjongFriends,
        QueryBtn,
        GameRule,
        AiAgency,
        AiAgencyCancel,
        KinFriends,
    }

    [RequireComponent(typeof(GameObjectCollections))]
    [UIPanelData(typeof(PanelGameTriggers), UIPanelhierarchy.Base)]
    public class PanelGameTriggers : UIPanelBase
    {
        public GameObject Group;
        public TriggerBtnItem[] BtnsMangaer;

        private GameObjectCollections mCollections;

        private void Awake()
        {
            Group.SetActive(false);
            mCollections = GetComponent<GameObjectCollections>();
        }

        public override void OnInit()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.ReadyBtnCtrl, ReadyBtnCtrl);
            GameCenter.EventHandle.Subscriber((int)EventKeys.QueryBtnCtrl, QueryBtnCtrl);
            GameCenter.EventHandle.Subscriber((int)EventKeys.QueryBtnCtrl, QueryBtnCtrl);
            GameCenter.EventHandle.Subscriber((int)EventKeys.AiAgency, AgencyAi);
            SetLanguage();
        }

        /// <summary>
        /// 设置言语
        /// </summary>
        private void SetLanguage()
        {
            if (GameCenter.DataCenter.Config.Localism)
            {
                MahjongUtility.LanguageVoice = (int)CtrlSwitchType.Close;
            }
            else
            {
                MahjongUtility.LanguageVoice = (int)CtrlSwitchType.Open;
            }
        }

        private TriggerBtnItem BtnSetActive(PulldowmBtnType type, bool isOn)
        {
            TriggerBtnItem item;
            for (int i = 0; i < BtnsMangaer.Length; i++)
            {
                item = BtnsMangaer[i];
                if (item.Type == type)
                {
                    item.gameObject.SetActive(isOn);
                    return item;
                }
            }
            return null;
        }

        public override void OnGetInfoUpdate()
        {
            var db = GameCenter.DataCenter;
            //玩法说明
            BtnSetActive(PulldowmBtnType.GameExplan, db.Room.RoomType == MahRoomType.YuLe);
            if (db.Room.RoomType == MahRoomType.FanKa)
            {
                //微信邀请按钮状态
                if (db.MaxPlayerCount != db.Players.CurrPlayerCount)
                {
                    BtnSetActive(PulldowmBtnType.Wechat, !db.IsGamePlaying);
                }
                //邀请好友按钮控制
                if (db.ConfigData.InviteMahFriends && db.MaxPlayerCount != db.Players.CurrPlayerCount)
                {
                    BtnSetActive(PulldowmBtnType.MahjongFriends, !db.IsGamePlaying);
                }
                //邀请亲友按钮控制
                if (db.ConfigData.InviteKinFriends && db.MaxPlayerCount != db.Players.CurrPlayerCount)
                {
                    BtnSetActive(PulldowmBtnType.KinFriends, !db.IsGamePlaying);
                }
            }
            //规则按钮控制
            BtnSetActive(PulldowmBtnType.GameRule, DataCenter.Room.RoomType == MahRoomType.FanKa);
            BtnSetActive(PulldowmBtnType.AiAgency, DataCenter.Config.AiAgency);
        }

        public override void OnStartGameUpdate()
        {
            BtnSetActive(PulldowmBtnType.Wechat, false);           
            BtnSetActive(PulldowmBtnType.KinFriends, false);
            BtnSetActive(PulldowmBtnType.MahjongFriends, false);
        }

        public override void OnReconnectedUpdate()
        {
            BtnSetActive(PulldowmBtnType.Wechat, false);            
            BtnSetActive(PulldowmBtnType.KinFriends, false);
            BtnSetActive(PulldowmBtnType.MahjongFriends, false);
            //显示查听按钮
            var list = DataCenter.OneselfData.TingList;
            if (null != list && list.Count > 0 || DataCenter.OneselfData.IsAuto)
            {
                SetQueryBtnState(true);
            }
        }

        public override void OnEndGameUpdate()
        {
            SetQueryBtnState(false);
        }

        /// <summary>
        /// 准备按钮控制
        /// </summary>      
        public void ReadyBtnCtrl(EvtHandlerArgs args)
        {
            PanelTriggerArgs data = args as PanelTriggerArgs;
            BtnSetActive(PulldowmBtnType.Ready, data.ReadyState);
        }

        /// <summary>
        /// 打开下拉菜单
        /// </summary>
        public void OnOpenXiaLaClick()
        {            
            if (DataCenter == null || GameCenter.GameProcess.IsCurrState<StateGameSceneInit>())
            {
                return;
            }

            Group.SetActive(true);
            BtnSetActive(PulldowmBtnType.ExchangeRoom, DataCenter.Room.RoomType == MahRoomType.YuLe);
            if (DataCenter.Room.RoomType == MahRoomType.FanKa)
            {
                BtnSetActive(PulldowmBtnType.DismissRoom, DataCenter.OneselfData.IsOwner);
                if (DataCenter.IsGamePlaying)
                {
                    BtnSetActive(PulldowmBtnType.DismissRoom, true);
                }
            }
            else
            {
                BtnSetActive(PulldowmBtnType.DismissRoom, false);
            }
        }

        /// <summary>
        /// 关闭下拉菜单
        /// </summary>
        public void OnCloseXiaLaClick()
        {
            Group.SetActive(false);
        }

        /// <summary>
        /// 准备
        /// </summary>
        public void OnReadyClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SPlayerReady);
            BtnSetActive(PulldowmBtnType.Ready, false);
            Group.SetActive(false);
        }

        /// <summary>
        /// 邀请微信好友
        /// </summary>
        public void OnInvitationClick()
        {
            if (GameCenter.DataCenter.Config.ShareFormat)
            {
                GameUtils.WechatShareTempate();
            }
            else
            {
                GameUtils.WechatShare();
            }
            Group.SetActive(false);
        }

        /// <summary>
        /// 解散房间
        /// </summary>
        public void OnDismissRoom()
        {
            YxMessageBox.Show("是否要解散房间？", null, (box, btnName) =>
            {
                if (btnName.Equals(YxMessageBox.BtnLeft))
                {
                    GameCenter.EventHandle.Dispatch<C2SDismissRoomArgs>((int)EventKeys.C2SDismissRoom, (param) =>
                    {
                        param.DismissType = (int)DismissFeedBack.ApplyFor;
                    });
                }
            }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            Group.SetActive(false);
        }

        /// <summary>
        /// 更换房间
        /// </summary>
        public void OnChangeRoomClick()
        {
            if (GameCenter.GameProcess.IsCurrState<StateGamePlaying>())
            {
                YxMessageBox.Show("游戏中不能换房间！");
            }
            else
            {
                GameCenter.Network.ChangeRoomEvent();
            }
            Group.SetActive(false);
        }

        /// <summary>
        /// 返回大厅
        /// </summary>
        public void OnReturnRoomClick()
        {
            var db = GameCenter.DataCenter;
            if (db.Room.RoomType == MahRoomType.YuLe)
            {
                //娱乐房，一局结束后允许返回大厅
                if (GameCenter.GameProcess.IsCurrState<StateGamePlaying>())
                {
                    YxMessageBox.Show("游戏中不能返回大厅！");
                    return;
                }
            }
            else
            {
                if (db.IsGamePlaying)
                {
                    YxMessageBox.Show("游戏中不能返回大厅！");
                    return;
                }
            }
            YxMessageBox.Show("是否要返回大厅？", null, (box, btnName) =>
            {
                if (btnName.Equals(YxMessageBox.BtnLeft))
                {
                    MahjongUtility.QuitGame();
                }
            }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }

        /// <summary>
        /// 邀请麻友
        /// </summary>
        public void OnInvitMahFriendsClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.ShowMahFriendsPanel);
        }


        /// <summary>
        /// 显示麻将规则界面
        /// </summary>
        public void OnGameRuleClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.ShowGameRule);
        }

        /// <summary>
        /// 查询胡牌按钮状态
        /// </summary>      
        public void QueryBtnCtrl(EvtHandlerArgs args)
        {
            PanelTriggerArgs data = args as PanelTriggerArgs;
            SetQueryBtnState(data.QueryBtnState);
        }

        private void SetQueryBtnState(bool state)
        {
            BtnSetActive(PulldowmBtnType.QueryBtn, state/* && GameCenter.DataCenter.ConfigData.MahjongQuery*/);
        }

        /// <summary>
        /// 查询可以胡的牌
        /// </summary>
        public void OnQueryHuCard()
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.GetHuCards);
                if (GameCenter.DataCenter.Config.QueryTingInRate)
                {
                    sfs.PutBool("rate", true);
                }
                return sfs;
            });
        }

        /// <summary>
        /// 游戏玩法说明
        /// </summary>
        public void OnGameExplainClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.ShowGameExplain);
        }

        /// <summary>
        /// 设置
        /// </summary>
        public void OnSettingClick()
        {
            GameCenter.Hud.OpenPanel<PanelSetting>();
            Group.SetActive(false);
        }

        /// <summary>
        /// 点击托管
        /// </summary>
        public void OnAiAgencyClick()
        {
            bool flag = GameCenter.Shortcuts.SwitchCombination.IsOpen((int)GameSwitchType.PowerAiAgency);
            if (flag && !DataCenter.OneselfData.IsAuto && GameCenter.GameProcess.IsCurrState<StateGamePlaying>())
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.AiAgency, new AiAgencyArgs() { State = true });
                Group.SetActive(false);
            }
        }

        public void OnCancelAIAgencyClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.AiAgency, new AiAgencyArgs() { State = false });
        }

        public void AgencyAi(EvtHandlerArgs args)
        {
            BtnSetActive(PulldowmBtnType.AiAgencyCancel, (args as AiAgencyArgs).State);
        }

        /// <summary>
        /// 邀请亲友
        /// </summary>
        public void OnInvitationKinFriendsClick()
        {
            var dic = new Dictionary<string, object>()
            {
                {"rndId", GameCenter.DataCenter.Room.RoomID}
            };
            var window = YxWindowManager.OpenWindow("SocialFriendListWindow");
            if (window)
            {
                window.UpdateView(dic);
            }
        }
    }
}