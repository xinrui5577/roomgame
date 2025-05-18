using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener
{
    public class SelfHdCdsListener : ServEvtListener
    {


        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, OnFirstOut);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoinGame);
        }

        public override void RefreshUiInfo()
        {
            
        }

        /// <summary>
        /// 手牌信息缓存
        /// </summary>
        protected readonly List<int> HdCdsListTemp = new List<int>();


        /// <summary>
        /// 重置手牌
        /// </summary>
        /// <param name="cards"></param>
        protected virtual void InitHdCdsArray(int[] cards)
        {
            if (cards == null || cards.Length < 1) return;
            HdCdsListTemp.Clear();
            HdCdsListTemp.AddRange(cards);
        }

        /// <summary>
        /// 添加手牌,过滤掉可能重复发的牌
        /// </summary>
        /// <param name="extrcds"></param>
        protected virtual void AddHdCds(int[] extrcds)
        {
            foreach (var extrcd in extrcds)
            {
                if (HdCdsListTemp.Contains(extrcd))
                    continue;
                HdCdsListTemp.Add(extrcd);
            }
        }

        /// <summary>
        /// 移除手牌
        /// </summary>
        /// <param name="rmoveCds">要移除的手牌</param>
        protected virtual void RemoveHdCds(int[] rmoveCds)
        {
            var len = rmoveCds.Length;
            for (int i = 0; i < len; i++)
            {
                if(HdCdsListTemp.Contains(rmoveCds[i]))
                    HdCdsListTemp.Remove(rmoveCds[i]);
            }
        }


        /// <summary>
        /// 重连游戏
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnRejoinGame(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (!data.ContainsKey(RequestKey.KeyUser))
            {
                YxDebug.LogEvent("服务器没有发送本人信息");
                return;
            }

            var userSelf = data.GetSFSObject(RequestKey.KeyUser);
            if (!userSelf.ContainsKey(GlobalConstKey.C_Cards))
            {
                if (App.GameData.IsGameStart)
                {
                    YxDebug.LogEvent("服务器没有发送手牌信息");
                }
                return;
            }

            var cards = userSelf.GetIntArray(GlobalConstKey.C_Cards);
            if (cards == null || cards.Length < 1)
            {
                YxDebug.LogEvent("服务器发送手牌信息错误");
                return;
            }

            InitHdCdsArray(cards);
            RefreshUiInfo();
        }

       

        /// <summary>
        /// 如果是地主，则获得底牌
        /// </summary>
        protected virtual void OnFirstOut(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //地主座位
            if (data.GetInt(RequestKey.KeySeat) != App.GetGameData<DdzGameData>().SelfSeat)
            {
                return;
            }
            var extrcds  = data.GetIntArray(RequestKey.KeyCards);
            AddHdCds(extrcds);
            RefreshUiInfo();   
        }


        /// <summary>
        /// 出牌阶段
        /// </summary>
        protected virtual void OnTypeOutCard(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            
            var seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<DdzGameData>().SelfSeat)
            {
                RemoveHdCds(data.GetIntArray(RequestKey.KeyCards));
                RefreshUiInfo();
            }
        }
    }
}
