using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerRule;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.OutCdsPanel
{
    public class OutCdsManager : MonoBehaviour
    {

        public OutCdsListener[] Listeners;

        public ParticleSystem ChunTianPartical;


        protected void Awake()
        {
            OnAwake();
        }

        void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfo);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoinGame);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnUserReady, OnUserReady);

            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypePass, OnTypePass);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOneRoundOver, OnTypeOneRoundOver);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, OnFirstOut);

            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyShowReadyBtn, ClearAllOutCds);

        }

        private void OnGetGameInfo(DdzbaseEventArgs args)
        {
            ClearAllOutCds();

            var data = args.IsfObjData;

            //标记地主座位
            if (data.ContainsKey(NewRequestKey.KeyLandLord)) LandSeat = data.GetInt(NewRequestKey.KeyLandLord);

            if (!data.ContainsKey(NewRequestKey.KeyCurrp) || !data.ContainsKey(NewRequestKey.KeyLastOut))
                return;

            int curSeat = data.GetInt(NewRequestKey.KeyCurrp);
            var lastOutData = data.GetSFSObject(NewRequestKey.KeyLastOut);
            int lastSeat = lastOutData.GetInt(RequestKey.KeySeat);

            if (curSeat == lastSeat) return;        //如果当前出牌的玩家和最后一位出牌的玩家相同,则无需显示出过的牌

            bool isLand = lastSeat == LandSeat;
            lastSeat = ToLocalSeat(lastSeat);       //座位号转为本地位置
            //玩家出牌
            var curSpeaker = Listeners[lastSeat];
            PlayerOutCards(curSpeaker, lastOutData, isLand);
        }

        int ToLocalSeat(int serverSeat)
        {
            return App.GameData.GetLocalSeat(serverSeat);
        }

        int GetLaterHand(int seat)
        {
            return App.GetGameData<DdzGameData>().GetLaterHand(seat);
        }

        private void OnFirstOut(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat)) LandSeat = data.GetInt(RequestKey.KeySeat);
        }

        private void OnTypeOneRoundOver(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var users = data.GetSFSArray("users");
            int selfSeat = App.GameData.SelfSeat;
            int score = 0;
            int count = users.Count;
            for (int i = 0; i < count; i++)
            {
                var userData = users.GetSFSObject(i);
                int seat = ToLocalSeat(userData.GetInt(RequestKey.KeySeat));
                if (selfSeat == seat)
                {
                    score = userData.GetInt(RequestKey.KeyScore);
                }
                Listeners[seat].AllocateCds(userData.GetIntArray(RequestKey.KeyCards));
            }

            //播放春天动画
            if (data.GetInt(NewRequestKey.KeySpring) < 1) return;
            ChunTianPartical.gameObject.SetActive(true);
            string soundName = score > 0 ? "k_chuntianwin" : "k_chuntianlose";
            Facade.EventCenter.DispatchEvent(GlobalConstKey.PlaySoundAndPauseBgSound, soundName);
            ChunTianPartical.Play();
        }

        private void OnTypePass(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            int seat = data.GetInt(RequestKey.KeySeat);
            seat = ToLocalSeat(seat);       //座位号转为本地位置

            //玩家清牌
            ClearOneOutCards(Listeners[seat]);

            //下家清掉出过的牌
            ClearLaterHandOutsCards(seat);
        }


        private void OnTypeOutCard(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            int seat = data.GetInt(RequestKey.KeySeat);
            bool isLand = seat == LandSeat;     //是否是地主

            seat = ToLocalSeat(seat);           //座位号转为本地位置

            //玩家出牌
            var curSpeaker = Listeners[seat];
            PlayerOutCards(curSpeaker, data, isLand);

            //下家清掉出过的牌
            ClearLaterHandOutsCards(seat);
        }


        /// <summary>
        /// 玩家出牌
        /// </summary>
        /// <param name="listener">出牌的玩家</param>
        /// <param name="data">出牌数据</param>
        /// <param name="isLand">是否是地主</param>
        void PlayerOutCards(OutCdsListener listener, ISFSObject data, bool isLand)
        {
            var cards = data.GetIntArray(RequestKey.KeyCards);
            listener.AllocateCds(cards, isLand);
            var cardsType = GetOutCardsType(data);
            listener.CheckPartiCalPlay(cardsType);
        }

        /// <summary>
        /// 获取牌型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        CardType GetOutCardsType(ISFSObject data)
        {
            if (data.ContainsKey(RequestKey.KeyCardType))
            {
                return (CardType)data.GetInt(RequestKey.KeyCardType);
            }

            if (!data.ContainsKey(RequestKey.KeyCards)) return 0;

            var cards = data.GetIntArray(RequestKey.KeyCards);
            return PokerRuleUtil.GetCdsType(cards);
        }


        protected int LandSeat = -1;

        protected void OnRejoinGame(DdzbaseEventArgs args)
        {
            OnGetGameInfo(args);
        }

        private void OnUserReady(DdzbaseEventArgs args)
        {
            var sfs = args.IsfObjData;
            if (sfs.ContainsKey("seat"))
            {
                var seat = sfs.GetInt("seat");
                var chair = App.GetGameData<YxGameData>().GetLocalSeat(seat);

                //一局结束，当其他玩家准备了， 自己还再小结算界面时， 不清除玩家手牌
                if (chair == 0)
                {
                    //清理掉所有手牌
                    ClearAllOutCds();
                }
            }
            else
            {
                //清理掉所有手牌
                ClearAllOutCds();
            }

            //隐藏春天动画
            ChunTianPartical.Stop();
            ChunTianPartical.gameObject.SetActive(false);
        }

        private void ClearAllOutCds(DdzbaseEventArgs obj)
        {
            //清理掉所有手牌
            ClearAllOutCds();
        }

        /// <summary>
        /// 清理掉所有出过的牌
        /// </summary>
        void ClearAllOutCds()
        {
            int len = Listeners.Length;
            for (int i = 0; i < len; i++)
            {
                ClearOneOutCards(Listeners[i]);
            }
        }

        /// <summary>
        /// 清理掉某个玩家的出过的牌
        /// </summary>
        /// <param name="listener"></param>
        void ClearOneOutCards(OutCdsListener listener)
        {
            listener.ClearAllOutCds();
        }

        /// <summary>
        /// 清理掉下家的手牌
        /// </summary>
        /// <param name="seat">当前玩家座位号</param>
        void ClearLaterHandOutsCards(int seat)
        {
            int laterSeat = GetLaterHand(seat);
            ClearOneOutCards(Listeners[laterSeat]);
        }
    }
}
