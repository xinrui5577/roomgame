using System.Collections.Generic;
using YxFramwork.ConstDefine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelHuanAndDq), UIPanelhierarchy.Base)]
    public class PanelHuanAndDq : UIPanelBase
    {
        public TextTipItem TipText;
        public Transform ChangeTitle;
        public ChangeCardTip ChangeTip;
        public DqBtnItem[] DqBtns;
        public ObjContainer HusContainer;

        private List<EffectObject> mEffectCache = new List<EffectObject>();
        private Image[] mHuGroup;

        private void Awake()
        {
            var Event = GameCenter.EventHandle;
            Event.Subscriber((int)EventKeys.SetPlayerFlagState, SetPlayerFlagState);
            Event.Subscriber((int)EventKeys.HideChangeTitleBtn, HidenConfirmBtn);
            Event.Subscriber((int)EventKeys.TipBankerPutCard, TipBankerPutCard);
            Event.Subscriber((int)EventKeys.ShowDingqueFlag, ShowDingqueFlag);
            Event.Subscriber((int)EventKeys.SetSingleHuFlag, SetSingleHuFlag);
            Event.Subscriber((int)EventKeys.ChangeCardTip, ChangeCardTip);
            Event.Subscriber((int)EventKeys.HideHuFlag, HideHuFlag);
        }

        public override void OnGetInfoUpdate()
        {
            mHuGroup = HusContainer.GetComponent<Image>(DataCenter.MaxPlayerCount);
            for (int i = 0; i < mHuGroup.Length; i++)
            {
                mHuGroup[i].gameObject.SetActive(false);
            }
            //隐藏UI
            HideDqBtns();
            TipText.gameObject.SetActive(false);
            ChangeTip.gameObject.SetActive(false);
            ChangeTitle.gameObject.SetActive(false);
        }

        public void ChangeCardTip(EvtHandlerArgs args)
        {
            var param = args as HuanAndDqArgs;
            ChangeTip.ShowTip(param.HuanType);
        }

        public void TipBankerPutCard(EvtHandlerArgs args)
        {
            StartCoroutine(IeTipAnimation());
        }

        private IEnumerator IeTipAnimation()
        {
            TipText.ExCompShow().Content = "您是庄家 请出牌！";
            yield return new WaitForSeconds(2);
            TipText.ExCompHide();
        }

        public override void OnReconnectedUpdate()
        {
            HideHuFlag();
            HideDingqueFlag();
            MahjongUserInfo data;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                data = DataCenter.Players[i];
                if (null != data)
                {
                    int huanType = data.HuanCardType;
                    if (huanType > 0)
                    {
                        int chair = data.Chair;
                        SetPlayerDingqueFlag(chair, huanType);
                    }
                    if (data.XuezhanStatue == (int)XzmjGameStatue.hu && MahjongUtility.GameKey == GameKeys.XzmjKey)
                    {
                        mHuGroup[i].gameObject.SetActive(true);
                    }
                }
            }
        }

        public override void OnReadyUpdate()
        {
            HideHuFlag();
            HideDingqueFlag();
        }

        public override void OnEndGameUpdate()
        {
            OnReadyUpdate();
        }

        public override void OnContinueGameUpdate()
        {
            OnReadyUpdate();
        }

        public void HideHuFlag(EvtHandlerArgs args)
        {
            HideHuFlag();
        }

        public void SetSingleHuFlag(EvtHandlerArgs args)
        {
            var param = args as HuanAndDqArgs;
            if (param.Type == 2)
            {
                int chair;
                var seats = param.HuSeats;
                for (int i = 0; i < seats.Count; i++)
                {
                    chair = MahjongUtility.GetChair(seats[i]);
                    mHuGroup[chair].gameObject.SetActive(true);
                    mHuGroup[chair].GetComponent<TweenPosition>().Do((cmp) =>
                    {
                        cmp.ResetToBeginning();
                        cmp.PlayForward();
                    });
                }
            }
        }

        private void HideHuFlag()
        {
            if (mHuGroup == null) return;
            for (int i = 0; i < mHuGroup.Length; i++)
            {
                mHuGroup[i].gameObject.SetActive(false);
            }
        }

        public void ShowDingqueFlag(EvtHandlerArgs args)
        {
            var param = args as HuanAndDqArgs;
            if (param.Type == 0)
            {
                HideDingqueFlag();
                var colors = param.DingqueColors;
                MahjongUtility.PlayEnvironmentSound("feidingque");
                for (int i = 0; i < colors.Length; i++)
                {
                    int chair = MahjongUtility.GetChair(i);
                    SetPlayerDingqueFlag(chair, colors[i]);
                }
            }
        }

        private void SetPlayerDingqueFlag(int chair, int color)
        {
            PoolObjectType effectType = PoolObjectType.none;
            switch (color)
            {
                case 0x10: effectType = PoolObjectType.dqwan; break;
                case 0x20: effectType = PoolObjectType.dqtiao; break;
                case 0x30: effectType = PoolObjectType.dqtong; break;
                default: effectType = PoolObjectType.none; break;
            }
            EffectObject effect = MahjongUtility.PlayMahjongEffect(effectType);
            mEffectCache.Add(effect);

            var item = MahjongUtility.GetYxGameData().GetPlayerInfoItem<PlayerInfoItem>(chair);
            if (null != item)
            {
                var iamge = item.Owner.ExCompShow().GetComponent<Image>();
                iamge.enabled = false;
                effect.ExSetParent(iamge.transform);
            }
            effect.gameObject.SetActive(true);
            effect.Execute();
        }

        private void HideDingqueFlag()
        {
            for (int i = 0; i < mEffectCache.Count; i++)
            {
                var effect = mEffectCache[i];
                if (effect != null) DestroyImmediate(effect.gameObject);
            }
            mEffectCache.Clear();
        }

        private void ShowDqBtns(int color)
        {
            int value = -1;
            switch (color)
            {
                case 0x10: value = 0; break;
                case 0x20: value = 1; break;
                case 0x30: value = 2; break;
            }
            for (int i = 0; i < DqBtns.Length; i++)
            {
                if (i == value)
                {
                    DqBtns[i].ShowEffect();
                }
                else
                {
                    DqBtns[i].gameObject.SetActive(true);
                }
            }
        }

        private void HideDqBtns()
        {
            for (int i = 0; i < DqBtns.Length; i++)
            {
                DqBtns[i].gameObject.SetActive(false);
            }
        }

        public void SetPlayerFlagState(EvtHandlerArgs args)
        {
            var param = args as PlayerStateFlagArgs;
            if (param.CtrlState)
            {
                switch (param.StateFlagType)
                {
                    case (int)PlayerStateFlagType.Selecting:
                        TipText.ExCompShow().Content = "缺一门 才能胡！";
                        ShowDqBtns(param.SecletColor);
                        break;
                    case (int)PlayerStateFlagType.SelectCard:
                        {
                            ChangeTitle.gameObject.SetActive(true);
                            var text = ChangeTitle.transform.FindChild("title").GetComponent<Text>();
                            text.text = "选择以下3张{0}手牌！".ExFormat("<color=#FFE200FF>同花色</color>");
                        }
                        break;
                }
            }
            else
            {
                ChangeTitle.gameObject.SetActive(false);
                HideDqBtns();
                TipText.ExCompHide();
            }
        }

        /// <summary>
        /// 选牌确定
        /// </summary>
        public void OnConfirmClick()
        {
            var game = GameCenter.Scene;
            var mahHand = game.MahjongGroups.PlayerHand;
            var xzMahHand = mahHand.GetComponent<XzmjMahjongPlayerHand>();
            var selectCards = xzMahHand.SelectMahList;
            // 校验
            int iSelCount = xzMahHand.SelectMahList.Count;
            if (3 != iSelCount)
            {
                YxMessageBox.Show("请选择{0}张牌相同花色牌！".ExFormat(3));
                return;
            }
            int iColor0 = selectCards[0].Value >> 4;
            int iColor1 = selectCards[1].Value >> 4;
            int iColor2 = selectCards[2].Value >> 4;
            if (iColor0 != iColor1 || iColor1 != iColor2)
            {
                YxMessageBox.Show("请选择{0}张牌相同花色牌！".ExFormat(3));
                return;
            }
            int[] array = new int[selectCards.Count];
            for (int i = 0; i < selectCards.Count; i++)
            {
                array[i] = selectCards[i].Value;
            }
            //发送请求
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.ChangeCards);
                sfs.PutIntArray("cards", array);
                return sfs;
            });
            //移除手牌 
            for (int i = 0; i < selectCards.Count; i++)
            {
                mahHand.RemoveMahjong(selectCards[i]);
            }
            // 扣下
            game.MahjongGroups.SwitchGorup[0].AddMahToSwitch(array);
            //整理手牌
            xzMahHand.AgainSortHandCards();
            //停止定时任务
            GameCenter.Scene.TableManager.StopTimer();
            GameCenter.EventHandle.Dispatch((int)EventKeys.HideChangeTitleBtn);
        }

        public void HidenConfirmBtn(EvtHandlerArgs args)
        {
            ChangeTitle.gameObject.SetActive(false);
        }

        /// <summary>
        /// 定缺：万
        /// </summary>
        public void OnWanClick()
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.SelectColor);
                sfs.PutInt("color", 0x10);
                return sfs;
            });
            HideDqBtns();
            TipText.ExCompHide();
        }

        /// <summary>
        /// 定缺：条
        /// </summary>
        public void OnTiaoClick()
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.SelectColor);
                sfs.PutInt("color", 0x20);
                return sfs;
            });
            HideDqBtns();
            TipText.ExCompHide();
        }

        /// <summary>
        /// 定缺：筒
        /// </summary>
        public void OnTongClick()
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.SelectColor);
                sfs.PutInt("color", 0x30);
                return sfs;
            });
            HideDqBtns();
            TipText.ExCompHide();
        }
    }
}