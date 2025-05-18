using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.pdk.DDzGameListener
{
    public class SelfHdCdsListener : ServEvtListener
    {


        protected override void OnAwake()
        {
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAllocateCds);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, OnFirstOut);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);

        }

        public override void RefreshUiInfo()
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// 手牌信息缓存
        /// </summary>
        protected readonly List<int> HdCdsListTemp = new List<int>();


        /// <summary>
        /// 重置手牌
        /// </summary>
        /// <param name="cards"></param>
        protected virtual void ResetHdCds(int[] cards)
        {
            HdCdsListTemp.Clear();
            if(cards==null) return;
            HdCdsListTemp.AddRange(cards);
        }

        /// <summary>
        /// 添加手牌
        /// </summary>
        /// <param name="extrcds"></param>
        protected virtual void AddHdCds(int[] extrcds)
        {
            HdCdsListTemp.AddRange(extrcds);
        }

        /// <summary>
        /// 移除手牌
        /// </summary>
        /// <param name="rmoveCds">要移除的手牌</param>
        protected virtual void RemoveHdCds(int[] rmoveCds)
        {
            var len = rmoveCds.Length;
            for (int i = 0; i < len; i++)
                HdCdsListTemp.Remove(rmoveCds[i]);
        }


        /// <summary>
        /// 重连游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnRejoinGame(object sender,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (!data.ContainsKey(RequestKey.KeyUser)) return ;

            var userSelf = data.GetSFSObject(RequestKey.KeyUser);
            if (!userSelf.ContainsKey(GlobalConstKey.C_Cards)) return;

            var cards = userSelf.GetIntArray(GlobalConstKey.C_Cards);
            if (cards == null || cards.Length<0) return;

            ResetHdCds(cards);
            RefreshUiInfo();
        }

        /// <summary>
        /// 游戏开局，服务器给玩家自己发牌时
        /// </summary>
        protected virtual void OnAllocateCds(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var seat = data.GetInt(GlobalConstKey.C_Sit);
            if (App.GetGameData<GlobalData>().GetSelfSeat != seat || !data.ContainsKey(GlobalConstKey.C_Cards)) return;
            var cards = data.GetIntArray(GlobalConstKey.C_Cards);

            ResetHdCds(cards);
            RefreshUiInfo();        
        }

        /// <summary>
        /// 如果是地主，则获得底牌
        /// </summary>
        protected virtual void OnFirstOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //地主座位
            if(data.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetSelfSeat) return;
            var extrcds  = data.GetIntArray(RequestKey.KeyCards);
            AddHdCds(extrcds);
            RefreshUiInfo();   
        }



        /// <summary>
        /// 如果是自己出牌则出牌
        /// </summary>
        protected virtual void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                RemoveHdCds(data.GetIntArray(RequestKey.KeyCards));
                RefreshUiInfo();
            }

        }
    }
}
