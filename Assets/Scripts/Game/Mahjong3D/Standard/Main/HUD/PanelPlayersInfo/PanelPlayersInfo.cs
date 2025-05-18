using System.Collections.Generic;
using YxFramwork.View;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum SetScoreType
    {
        EndScore,
        AddScoreAndEffect,
    }

    [UIPanelData(typeof(PanelPlayersInfo), UIPanelhierarchy.Base)]
    public class PanelPlayersInfo : UIPanelBase, IReplayCycle
    {
        public PlayersOther PlayersOther;
        private ContinueTaskContainer mScoreEffectTask;
        private SetScoreArgs mScoreArgs;

        public PlayerInfoItem this[int index]
        {
            get
            {
                var item = MahjongUtility.GetYxGameData().PlayerList[index];
                if (null != item) { return item as PlayerInfoItem; }
                return null;
            }
        }

        private void Awake()
        {
            var Event = GameCenter.EventHandle;
            Event.Subscriber((int)EventKeys.UpdateCurrOpPlayer, OnUpdateCurrOpPlayer);
            Event.Subscriber((int)EventKeys.PlayerReady, OnPlayerReady);
            Event.Subscriber((int)EventKeys.PlayAddScore, PlaySetScore);
            Event.Subscriber((int)EventKeys.PlayerJoin, OnPlayerJoin);
            Event.Subscriber((int)EventKeys.PlayerOut, OnPlayerOut);
            Event.Subscriber((int)EventKeys.SetBanker, OnSetBanker);
            Event.Subscriber((int)EventKeys.OnTing, OnTing);
            //扩展
            Event.Subscriber((int)EventKeys.SetPlayerFlagState, OnSetPlayerFlagState);
            Event.Subscriber((int)EventKeys.HideTitleAni, OnHideTitleAni);
        }

        public override void OnInit()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnReplayCycle()
        {
            OnGetInfoUpdate();
        }

        public override void OnGetInfoUpdate()
        {
            PlayersOther.GetContainer(DataCenter.MaxPlayerCount);
            //设置Seat_Type位置
            var seatItem = FindObjectOfType<YxSeatFormation>();
            if (null != seatItem)
            {
                var rect = seatItem.GetComponent<RectTransform>();
                seatItem.transform.ExSetParent(transform);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }
            PlayersOther.HideReadyAni();
            if (DataCenter.Config.ShowOwnerSign)
            {
                int ownerChair = DataCenter.Players.OwnerSeat.ExSeatS2C();
                SetOwner(ownerChair);
            }
        }

        public override void OnReadyUpdate()
        {
            SetPlayersInfo();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                this[i].Reset();
            }
        }

        public override void OnReconnectedUpdate()
        {
            PlayersOther.HideReadyAni();
            PlayersOther.HideTitleAni();
            SetPlayersInfo();
        }

        public override void OnStartGameUpdate()
        {
            PlayersOther.HideReadyAni();
            SetBanker(DataCenter.BankerChair);
        }

        public override void OnContinueGameUpdate()
        {
            PlayersOther.HideReadyAni();
            PlayersOther.HideTitleAni();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                this[i].Reset();
            }
        }

        /// <summary>
        /// 设置当前玩家信息
        /// </summary>    
        private void SetPlayersInfo()
        {
            //设置玩家信息       
            MahjongUserInfo data;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                data = DataCenter.Players[i];
                if (null != data)
                {
                    SetPlayerInfo(i, data);
                }
            }
        }

        /// <summary>
        /// 新玩家进入
        /// </summary>     
        private void OnPlayerJoin(EvtHandlerArgs args)
        {
            var data = args as PlayerInfoArgs;
            if (null == data) return;
            SetPlayerInfo(data.Chair, DataCenter.Players[data.Chair]);
        }

        /// <summary>
        /// 玩家退出
        /// </summary>  
        private void OnPlayerOut(EvtHandlerArgs args)
        {
            var data = args as PlayerInfoArgs;
            if (null == data) return;
            int chair = data.Chair;
            this[chair].Reset();
            PlayersOther.PlayerOut(chair);
        }

        /// <summary>
        /// 设置玩家Item信息
        /// </summary>     
        private void SetPlayerInfo(int chair, MahjongUserInfo data)
        {
            if (!GameCenter.GameProcess.IsCurrState<StateGamePlaying>())
            {
                PlayersOther.ReadyState(chair, data.IsReady);
            }
            PlayerInfoItem item = this[chair];
            item.IsTing = data.IsAuto;
        }

        /// <summary>
        /// 更新当前操作玩家
        /// </summary>      
        private void OnUpdateCurrOpPlayer(EvtHandlerArgs args)
        {
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                PlayerInfoItem item = this[i];
                if (null != item)
                {
                    item.IsCurrOp = DataCenter.CurrOpChair == i;
                }
            }
        }

        /// <summary>
        /// 设置分数
        /// </summary>       
        private void PlaySetScore(EvtHandlerArgs args)
        {
            mScoreArgs = args as SetScoreArgs;
            var type = (SetScoreType)mScoreArgs.Type;
            switch (type)
            {
                case SetScoreType.EndScore:
                    mScoreArgs.ScoreDic.ExIterationAction((dic) =>
                    {
                        this[dic.Key].SetGlod(dic.Value);
                    });
                    break;
                case SetScoreType.AddScoreAndEffect:
                    if (null == mScoreEffectTask)
                    {
                        mScoreEffectTask = ContinueTaskManager.NewTask().AppendFuncTask(() => DelaytimePlayEffect());
                    }
                    mScoreEffectTask.Start();
                    break;
            }
        }

        /// <summary>
        /// 延迟播放加分特效
        /// </summary>       
        private IEnumerator<float> DelaytimePlayEffect()
        {
            yield return mScoreArgs.DelayTime;
            mScoreArgs.ScoreDic.ExIterationAction((dic) =>
            {
                PlayersOther.ScoreGroup[dic.Key].Show(dic.Value);
                this[dic.Key].AddGlod(dic.Value);
            });
            PlayersOther.ScoreGroup.Play();
        }

        /// <summary>
        /// 显示听牌
        /// </summary>       
        private void OnTing(EvtHandlerArgs args)
        {
            var param = args as OnTingArgs;
            this[param.TingChair].IsTing = true;
        }

        /// <summary>
        /// 玩家准备
        /// </summary>     
        private void OnPlayerReady(EvtHandlerArgs args)
        {
            var data = args as PlayerInfoArgs;
            if (null == data) return;
            PlayersOther.ReadyState(data.Chair, true);
        }

        private void OnSetBanker(EvtHandlerArgs args)
        {
            SetBanker(DataCenter.BankerChair);
        }

        /// <summary>
        /// 设置庄家
        /// </summary>   
        private void SetBanker(int chair)
        {
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                this[i].IsBanker = i == chair;
            }
        }

        /// <summary>
        /// 设置房主
        /// </summary>     
        private void SetOwner(int chair)
        {
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                if (i == chair)
                {
                    this[i].IsOwner = true;
                }
                else
                {
                    this[i].IsOwner = false;
                }
            }
        }

        private void OnHideTitleAni(EvtHandlerArgs args)
        {
            PlayersOther.HideTitleAni();
        }

        private void OnSetPlayerFlagState(EvtHandlerArgs args)
        {          
            var param = args as PlayerStateFlagArgs;           
            if (param.CtrlState)
            {
                PlayerStateFlagType flag = (PlayerStateFlagType)param.StateFlagType;
                for (int i = 1; i < DataCenter.MaxPlayerCount; i++)
                {
                    switch (flag)
                    {
                        case PlayerStateFlagType.Selecting:
                        case PlayerStateFlagType.SelectCard:
                            PlayersOther.SetTitleFlag(i, (int)PlayerStateFlagType.Selecting);
                            break;
                        case PlayerStateFlagType.Other:
                            PlayersOther.SetTitleFlag(i, param.DiySprite);
                            break;
                    }
                }
            }
            else
            {
                PlayersOther.HideTitleAni();
            }
        }
    }
}
