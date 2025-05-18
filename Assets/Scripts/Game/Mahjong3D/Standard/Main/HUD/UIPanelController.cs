using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UIPanelController : MonoBehaviour
    {
        public Camera UICamera;
        public List<Transform> UIHierarchy;

        public void OnInit()
        {
            SubscriberEvent();
        }

        public void RefreshOtherPanelOnReconnected()
        {
            if (GameCenter.DataCenter.ConfigData.ScoreDouble)
            {
                GameCenter.Hud.GetPanel<PanelScoreDouble>().OnReconnectedUpdate();
            }
        }

        /// <summary>
        /// Op操作之前执行
        /// </summary>
        public void OnOperateUpdatePanel()
        {
            PanelOpreateMenu opPanel;
            if (GameCenter.Hud.TryGetPanel(out opPanel))
            {
                opPanel.HideButtons();
            }
            PanelChooseOperate choosePanel;
            if (GameCenter.Hud.TryGetPanel(out choosePanel))
            {
                choosePanel.Close();
            }
        }

        private void SubscriberEvent()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowResult, OnShowResult);
            GameCenter.EventHandle.Subscriber((int)EventKeys.QueryHuCard, OnQueryHuCard);
            GameCenter.EventHandle.Subscriber((int)EventKeys.OnEventHandUp, OnEventHandUp);
            GameCenter.EventHandle.Subscriber((int)EventKeys.OnRejoinEventHandUp, OnRejoinEventHandUp);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ScoreDoubleCtrl, OnScoreDouble);
            GameCenter.EventHandle.Subscriber((int)EventKeys.OperateMenuCtrl, OnOperateMenu);
            GameCenter.EventHandle.Subscriber((int)EventKeys.OnShowHandUp, OnShowPanelHandUp);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowGameExplain, OnShowGameExplain);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowTotalResult, OnShowTotalResult);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowTitleMessage, ShowTitleMessage);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowChooseOperate, OnShowChooseOperate);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowMahFriendsPanel, OnShowMahFriendsPanel);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowChooseXjfd, OnShowChooseXjfdPanel);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowXjfdList, OnShowXjfdListPanel);
            GameCenter.EventHandle.Subscriber((int)EventKeys.ShowPlaneShare, OnShowPlaneShare);

        }

        //显示游戏玩法
        private void OnShowGameExplain(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelGameExplain>().Open();
        }

        //查询胡牌
        private void OnQueryHuCard(EvtHandlerArgs args)
        {
            var data = args as QueryHuArgs;
            var panel = GameCenter.Hud.GetPanel<PanelQueryHuCard>();
            if (!data.PanelState)
            {
                panel.Close();
            }
            else
            {
                panel.Open(data);
            }
        }

        //提示框
        private void ShowTitleMessage(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelTitleMessage>().Open(args as ShowTitleMessageArgs);
        }

        //邀请麻友
        private void OnShowMahFriendsPanel(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelInviteFriends>().Open();
        }

        //下注（加漂）
        private void OnScoreDouble(EvtHandlerArgs args)
        {
            if (args == null)
            {
                GameCenter.Hud.GetPanel<PanelScoreDouble>().Open();
            }
            else
            {
                GameCenter.Hud.GetPanel<PanelScoreDouble>().Open(args as ScoreDoubleArgs);
            }
        }

        /// <summary>
        /// 选择菜单
        /// </summary>      
        private void OnShowChooseOperate(EvtHandlerArgs args)
        {
            var param = args as ChooseCgArgs;
            if (null == param) return;
            switch (param.Type)
            {
                case ChooseCgArgs.ChooseType.ChooseTing: GameCenter.Hud.GetPanel<PanelChooseOperate>().OnChooseTingCard(param); break;
                case ChooseCgArgs.ChooseType.ChooseCg: GameCenter.Hud.GetPanel<PanelChooseOperate>().Open(param); break;
            }
            //关闭按钮菜单界面
            GameCenter.Hud.ClosePanel<PanelOpreateMenu>();
        }

        //小结算界面
        private void OnShowResult(EvtHandlerArgs args)
        {
            if (args != null)
            {
                GameCenter.Hud.GetPanel<PanelSingleResult>().Open(args as SingleResultArgs);
            }
        }

        //大结算界面
        private void OnShowTotalResult(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelTotalResult>().Open();
        }

        //OperateMenu
        private void OnOperateMenu(EvtHandlerArgs args)
        {
            if (null == args)
            {
                GameCenter.Hud.GetPanel<PanelOpreateMenu>().Close();
            }
            else
            {
                GameCenter.Hud.GetPanel<PanelOpreateMenu>().Open(args as OpreateMenuArgs);
            }
        }

        //显示投票界面
        private void OnShowPanelHandUp(EvtHandlerArgs args)
        {
            GameCenter.Hud.OpenPanel<PanelHandup>();
        }

        //投票事件
        private void OnEventHandUp(EvtHandlerArgs args)
        {
            var eventArgs = args as HandupEventArgs;
            var panel = GameCenter.Hud.GetPanel<PanelHandup>();
            if (null != panel)
            {
                if (eventArgs.HandupType == DismissFeedBack.ApplyFor)
                {
                    panel.Open(eventArgs);
                }
                else
                {
                    panel.SetHandupState(eventArgs);
                }
            }
        }

        //重连投票事件
        private void OnRejoinEventHandUp(EvtHandlerArgs args)
        {
            var eventArgs = args as RejoinHandupEventArgs;
            var panel = GameCenter.Hud.GetPanel<PanelHandup>();
            if (null != panel)
            {
                panel.OnRejoinSetDismissInfo(eventArgs);
            }
        }

        private void OnShowChooseXjfdPanel(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelChooseXjfd>().Open();
        }

        private void OnShowXjfdListPanel(EvtHandlerArgs args)
        {
            var eventArgs = args as XjfdListArgs;
            var panel = GameCenter.Hud.GetPanel<PanelShowXjfdList>();
            if (null != panel)
            {
                panel.Open(eventArgs);
            }
        }

        private void OnShowPlaneShare(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelShare>().Open();
        }

        /// <summary>
        /// 播放特效
        /// </summary>    
        public void PlayUIEffect(PoolObjectType type)
        {
            var effectPos = UIHierarchy[(int)UIPanelhierarchy.EffectAndTip];
            PlayEffect(effectPos, type);
        }

        /// <summary>
        /// 根据玩家座位号播放特效
        /// </summary>
        /// <param name="chair">玩家游戏中座位号</param>
        /// <param name="type">特效类型 （注意：这其中会有许多不是特效的类型）</param>
        public void PlayPlayerUIEffect(int chair, PoolObjectType type)
        {
            var effectPos = GameCenter.Hud.GetPanel<PanelPlayersInfo>().PlayersOther.GetEffectPos(chair);
            PlayEffect(effectPos, type);
        }

        private void PlayEffect(Transform effectPos, PoolObjectType type)
        {
            effectPos.Do((o) =>
            {
                var obj = GameCenter.Pools.Pop<EffectObject>(type);
                if (obj != null)
                {
                    obj.ExSetParent(o);
                    obj.Execute();
                }
            });
        }

        public T SetPanel<T>(T panel, UIPanelhierarchy hierarchy) where T : UIPanelBase
        {
            if (null == panel) return null;
            var transform = UIHierarchy[(int)hierarchy] as RectTransform;
            panel.transform.ExSetParent(transform);
            panel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            panel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return panel;
        }
    }
}