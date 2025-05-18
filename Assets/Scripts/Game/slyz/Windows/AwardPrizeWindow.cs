using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.slyz.GameScene;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.slyz.Windows
{
    /// <summary>
    /// 中大奖界面
    /// </summary>
    public class AwardPrizeWindow : YxNguiWindow
    {
        public UILabel NameLabel;
        public UILabel NoticeTipLabel;
        public UILabel AppearTimesLabel;
        public UISprite TitleSprite;
        public UISprite PrizeStyleSprite;
        public UISprite[] CardsList;
        public string[] ImageTypeSpriteNames = new string[10];
        public string[] TitleTypeSpriteNames = new string[10];

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<ESlyzEventType, object>(ESlyzEventType.FreshPrizeList, UpdateView);
        }

        protected override void OnFreshView()
        {
            if (!IsShow()) return;
            var gdata = App.GetGameData<SlyzGameData>();
            int maxPrizeIndex = -1, cardType = -1;
            var len = gdata.StartData.CardTeamList.Count;
            for (short i = 0; i < len; i++)
            {
                var team = gdata.StartData.CardTeamList[i];
                if (CardTeam.TYPE_HL > team.type || cardType >= team.type) { continue;}
                cardType = team.type;
                maxPrizeIndex = i;
            }

            if (maxPrizeIndex < 0 || cardType < 0)
            {
                YxDebug.LogError("Judge prize satudation was wrong in LeftPanel.cs! ");
                return;
            }
            var prizeTeam = gdata.StartData.CardTeamList[maxPrizeIndex];
            // TITLE显示 判断是否要显示皇家同花顺TITLE
            UpdateTitle(prizeTeam.type);
            
            // 根据获奖类型换提示图片
            UpdatePrizeStyle(prizeTeam.type);
             
            // 玩家名字
            if (NameLabel != null)
            {
                var player = gdata.GetPlayer(gdata.SelfLocalSeat);
                var userName = player.Nick;
                NameLabel.text = userName;
            }
            // 刷新牌型
            UpdateCards(prizeTeam.cards);

            // 中奖提示
            if (NoticeTipLabel != null)
            {
                NoticeTipLabel.text = prizeTeam.type == CardTeam.TYPE_HL ? string.Format("以【{0}】获得{1}银两", prizeTeam.typeName, YxUtiles.GetShowNumber(prizeTeam.gold)) : string.Format("以【{0}】获得{1}银两奖池奖金", prizeTeam.typeName, YxUtiles.GetShowNumber(prizeTeam.caijin));
            }

            // 出现次数
           
            UpdateAppearTimes(gdata.StartData.CardStatistics, cardType);
        }

        private void UpdatePrizeStyle(int type)
        {
            if (PrizeStyleSprite == null) { return;}
            var count = ImageTypeSpriteNames.Length;
            if (count < 1) return;
            if (type >= count) return;
            PrizeStyleSprite.spriteName = ImageTypeSpriteNames[type];
            PrizeStyleSprite.MakePixelPerfect();
        }

        private void UpdateTitle(int type)
        {
            if (TitleSprite == null){ return; }
            var count = TitleTypeSpriteNames.Length;
            if (count < 1) return;
            if (type >= count) return;
            var sprite = TitleTypeSpriteNames[type];
            TitleSprite.spriteName = sprite;
            TitleSprite.MakePixelPerfect();
        }

        /// <summary>
        /// 出现次数
        /// </summary>
        private void UpdateAppearTimes(IList<CardStatistics> list, int cardType)
        {
            if (AppearTimesLabel == null){return;}
            var len = list.Count;
            for (var i = 0; i < len; i++)
            {
                var cardStatistic = list[i];
                if (cardStatistic.Type != cardType) { continue;}
                AppearTimesLabel.text = cardStatistic.TypeCount.ToString();
                Facade.EventCenter.DispatchEvent<ESlyzEventType,object>(ESlyzEventType.FreshAccountList);
                return;
            }
        }

        private void UpdateCards(IList<int> cards)
        {
            var count = CardsList.Length;
            // 显示牌
            for (var i = 0; i < count; i++)
            {
                var cardValue = cards[i];
                var cardValue16 = "0x" + cardValue.ToString("X2");
                var cardSprite = CardsList[i];//transform.FindChild("Poker_" + i).GetComponent<UISprite>();
                var cardAni = cardSprite.gameObject.AddComponent<CardAni>();
                cardAni.CardName = cardValue16;
                cardAni.Speed = 5;
                cardAni.Play();
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
            var gdata = App.GetGameData<SlyzGameData>();
            if (gdata == null) return;
            // 不满足用户设置的暂停自动开始条件
            var musicMgr = Facade.Instance<MusicManager>();
            var clip = musicMgr.Play("WinBig");
            var time = clip != null ? clip.length : 5;
            if (!gdata.IsStopAutoStart)
            {
                Invoke("Hide", time);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            var gdata = App.GetGameData<SlyzGameData>();
            if (gdata == null) return;
            gdata.IsShowPrizeMaskComplete = true;
        }
    }
}
