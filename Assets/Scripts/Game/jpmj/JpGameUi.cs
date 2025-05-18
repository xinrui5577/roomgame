using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using UnityEngine;
using YxFramwork.Common;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;

namespace Assets.Scripts.Game.jpmj
{
    /// <summary>
    /// 精品麻将的gameui
    /// </summary>
    public class JpGameUi : GameUI
    {

        [SerializeField]
        protected GameObject GameTable;

        [SerializeField] protected BigCdStyleTypePlay BigCdSyTypePlay;

        [SerializeField] protected JpMahjongPlayerHard JpMjplayerHdcd;
        private Coroutine _setShowInfoCoroutine;

        protected override void RegistEvent()
        {
            base.RegistEvent();
            EventDispatch.Instance.RegisteEvent((int)UIEventId.BuzhangEffect, BuzhangEffect);
        }

        protected void BuzhangEffect(int eventId, EventData evn)
        {
            var chair = (int)evn.data1;
            PlayersPnl.PlayEffect(chair, EnCpgEffect.gang);
        }

        protected override void OnResult(EventData evn)
        {
            var table = (TableData)evn.data1;
            if (_setShowInfoCoroutine != null)
            {
                StopCoroutine(_setShowInfoCoroutine);
            }
            _setShowInfoCoroutine=StartCoroutine("OnSetshowinfoLater", table);
        }
        
        private IEnumerator OnSetshowinfoLater(TableData table)
        {
            BigCdSyTypePlay.Reset();
            var seconds = BigCdSyTypePlay.OnHuResult(table);
            yield return new WaitForSeconds(seconds);
            GetOneRoundResult().SetShowInfo(table, new[] { DefHeadWoman, DefHeadMan });
        }

        protected override void OnGamePlay(TableData data)
        {
            JpMjplayerHdcd.HasToken = data.ReconectStatus == 0;
            base.OnGamePlay(data);
        }

        public override void OnHideHulist(int evtId, EventData evn)
        {
            base.OnHideHulist(evtId, evn);
            if (JpQueryHuPnl.Instance != null) JpQueryHuPnl.Instance.HideJpqueryHuPnl();
        }

        protected override void OnUserReady(EventData evn)
        {
            base.OnUserReady(evn);

            //chair为0
            if ((int)evn.data1 == 0)
            {
                GameBtnPnl.GameReadyBtn.SetActive(false);
            }

            App.GetRServer<NetWorkManager>().NeedAutoRejoin = true;
        }

        protected override void OnHu(EventData evn)
        {
            App.GetRServer<NetWorkManager>().NeedAutoRejoin = false;
            base.OnHu(evn);
        }
    }
}
