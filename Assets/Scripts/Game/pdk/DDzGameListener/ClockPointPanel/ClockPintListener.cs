using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDzGameListener.ClockPointPanel
{
    public class ClockPintListener : ServEvtListener
    {
        
        [SerializeField] 
        protected UISprite PointSelf;
        [SerializeField]
        protected UISprite PointRight;
        [SerializeField]
        protected UISprite PointLeft;

        [SerializeField]
        protected UILabel CuntDownLabel;

        [SerializeField] protected int CuntTime=20;

        protected override void OnAwake()
        {
            
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
            //Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, OnTypeFirstOut);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, AllocateCds);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
            gameObject.SetActive(false);
        }

        public override void RefreshUiInfo()
        {
            //throw new System.NotImplementedException();
        }


        private void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //没人行动，隐藏所有
            if (!data.ContainsKey(NewRequestKey.KeyCurrp))
            {
                gameObject.SetActive(false);
                return;
            }
     
            ShowPointAndCuntDown(data.GetInt(NewRequestKey.KeyCurrp));

        }

        /// <summary>
        /// 发牌时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AllocateCds(object sender, DdzbaseEventArgs args)
        {
            //判断发牌后是否是自己先行动出牌
            var data = args.IsfObjData;
            if (data.ContainsKey(GlobalConstKey.C_Bkp))
            {
                ShowPointAndCuntDown(data.GetInt(GlobalConstKey.C_Bkp));
            }
        }

/*
        /// <summary>
        /// 当确定地主后，看自己是不是地主，来判断是否显示按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            ShowPointAndCuntDown(args.IsfObjData.GetInt(RequestKey.KeySeat));
        }*/

        /// <summary>
        /// 当有人出牌时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            AfterSomeBodyAction(args.IsfObjData.GetInt(RequestKey.KeySeat));
        }


        /// <summary>
        /// 有人pass的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            AfterSomeBodyAction(args.IsfObjData.GetInt(RequestKey.KeySeat));
        }


        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 当某人行动时要进行的动作
        /// </summary>
        private void AfterSomeBodyAction(int actionPlayerSeat)
        {
            var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
            var leftSeat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            var rightSeat = App.GetGameData<GlobalData>().GetRightPlayerSeat;

            //3人玩的时候
            if (App.GetGameData<GlobalData>().PlayerMaxNum == 3)
            {
                if (actionPlayerSeat == selfSeat) ShowPointAndCuntDown(rightSeat);
                else if (actionPlayerSeat == rightSeat) ShowPointAndCuntDown(leftSeat);
                else if (actionPlayerSeat == leftSeat) ShowPointAndCuntDown(selfSeat);
                return;
            }

            //2人玩的时候
            if (actionPlayerSeat == selfSeat) ShowPointAndCuntDown(rightSeat);
            else if (actionPlayerSeat == rightSeat) ShowPointAndCuntDown(selfSeat);

        }

        /// <summary>
        /// 箭头指向方位和倒计时开始
        /// </summary>
        /// <param name="playerSeat"></param>
        private void ShowPointAndCuntDown(int playerSeat)
        {
            HideAllPoints();
            if (playerSeat == App.GetGameData<GlobalData>().GetSelfSeat) PointSelf.gameObject.SetActive(true);
            else if (playerSeat == App.GetGameData<GlobalData>().GetRightPlayerSeat) PointRight.gameObject.SetActive(true);
            else if (playerSeat == App.GetGameData<GlobalData>().GetLeftPlayerSeat) PointLeft.gameObject.SetActive(true);

            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(ReClock(CuntTime, playerSeat == App.GetGameData<GlobalData>().GetSelfSeat));
        }

        private IEnumerator ReClock(int cuntTime, bool isSelfTurn = false)
        {
            CuntDownLabel.gameObject.SetActive(true);
            while (cuntTime > 0)
            {
                CuntDownLabel.text = cuntTime.ToString(CultureInfo.InvariantCulture);
                yield return new WaitForSeconds(1);
                cuntTime--;
                if (isSelfTurn && (cuntTime == 5 || cuntTime <= 0))
                {
                    Facade.Instance<MusicManager>().Play("BirdCalls");
                }
            }
            CuntDownLabel.text = "0";
        }


        /// <summary>
        /// 隐藏所有
        /// </summary>
        private void HideAllPoints()
        {
            PointSelf.gameObject.SetActive(false);
            PointRight.gameObject.SetActive(false);
            PointLeft.gameObject.SetActive(false);
        }
    }
}
