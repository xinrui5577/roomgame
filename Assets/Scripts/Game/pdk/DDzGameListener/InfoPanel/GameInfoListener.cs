using System;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.pdk.PokerCdCtrl;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using Assets.Scripts.Game.pdk.PokerRule;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 处理游戏局数，低分，倍数等公共游戏信息
    /// </summary>
    public class GameInfoListener : ServEvtListener
    {
        protected override void OnAwake()
        {
            PdkGameManager.AddOnGameInfoEvt(OnGameInfo);
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoinGame);
          //  Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, TypeFirstOut);
           // Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
          //  Ddz2RemoteServer.AddOnchangeDizhu(OnChangDizhu);
           // Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
           // Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);

            //TrunBackAllDipai();
        }

        /// <summary>
        /// 游戏信息缓存
        /// </summary>
        private ISFSObject _gameInfoTemp;

        //[SerializeField] protected UILabel AnteLabel;
        [SerializeField] protected UILabel BeiShuLabel;
        [SerializeField] protected UILabel RoundLabel;
        [SerializeField] protected UILabel RoomIdLabel;

        /// <summary>
        /// gameinfo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            SetGameInfo(sender, args);
        }

        protected void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            SetGameInfo(sender, args);
        }

       
        private void SetGameInfo(object sender,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data == null) throw new Exception("得到了空的服务器信息");
            _gameInfoTemp = data;
            RefreshUiInfo();
        }

        public override void RefreshUiInfo()
        {
/*            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyAnte)) 
                AnteLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyAnte).ToString(CultureInfo.InvariantCulture);*/

            if (_gameInfoTemp.ContainsKey(RequestKey.KeyScore))
            {

                var score = (_gameInfoTemp.GetInt(RequestKey.KeyScore));
                var user = _gameInfoTemp.GetSFSObject(RequestKey.KeyUser);
                var ttscore = score;
                if (user.ContainsKey(NewRequestKey.KeyRate))
                {
                    ttscore = score*user.GetInt(NewRequestKey.KeyRate);
                }
                BeiShuLabel.text = ttscore.ToString(CultureInfo.InvariantCulture);
            }

            //游戏人没齐的时候，局数是0 所以要变成1局
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound) && _gameInfoTemp.ContainsKey(NewRequestKey.KeyMaxRound))
            {
                var curRound = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound);
                if (curRound < 1)
                    curRound = 1;
                RoundLabel.text = curRound + "/" +
                _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);
            }

            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyRoomId)) 
                RoomIdLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyRoomId).ToString(CultureInfo.InvariantCulture);

/*            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyLandCds))
                _dPaicdsTemp = _gameInfoTemp.GetIntArray("landCards");*/
              
            //SetDpaiCds();
        }


/*        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        private void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            _dPaicdsTemp = data.GetIntArray(RequestKey.KeyCards);
            SetDpaiCds();

            //更新局数信息缓存
            _gameInfoTemp.PutInt(NewRequestKey.KeyCurRound, _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) + 1);
        }*/


/*        /// <summary>
        /// 当玩家准备时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            TrunBackAllDipai();
        }*/

/*        /// <summary>
        /// 当底注改变时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnChangDizhu(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey("ante"))
            {
                int ante = data.GetInt("ante");
                AnteLabel.text = ante.ToString(CultureInfo.InvariantCulture);
            }
            if (data.ContainsKey("rate"))
            {
                int rate = data.GetInt("rate");
                if (rate > 1)
                {
                    StartCoroutine(ChangeTextSize());
                }
            }
        }*/

/*        /// <summary>
        /// 抢地主时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeGrab(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var score = data.GetInt(RequestKey.KeyScore);
            _gameInfoTemp.PutInt(RequestKey.KeyScore,score);
            BeiShuLabel.text = score.ToString(CultureInfo.InvariantCulture);
        }*/

        /// <summary>
        /// 有人出牌时,检查是否有炸弹火箭等，从而显示加倍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
/*            var cards = data.GetIntArray(RequestKey.KeyCards);
            cards = HdCdsCtrl.SortCds(cards);
            var cdsType = PokerRuleUtil.GetCdsType(cards);
            if (cdsType == CardType.C4 || cdsType == CardType.C42 || cdsType == CardType.C5)
            {
                var curbeishu = int.Parse(BeiShuLabel.text);
                try
                {
                    BeiShuLabel.text = (curbeishu*2).ToString(CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }*/
        }


/*        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        private void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var rates = data.GetIntArray("jiabei");
            var len = rates.Length;
            var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
            for (int i = 0; i < len; i++)
            {
                if (i != selfSeat) continue;

                BeiShuLabel.text = (_gameInfoTemp.GetInt(RequestKey.KeyScore) * rates[i]).ToString(CultureInfo.InvariantCulture);
            }
        }*/

/*        /// <summary>
        /// 设置底牌
        /// </summary>
        void SetDpaiCds()
        {
            if (_dPaicdsTemp == null || _dPaicdsTemp.Length <1 || _dPaicdsTemp.Length>DpaiCds.Length)
            {
                return;
            }
            var dpvalueLen = _dPaicdsTemp.Length;

            var dpGobLen = DpaiCds.Length;
            for (int i = 0; i < dpGobLen; i++)
            {
                if (i >= dpvalueLen)
                {
                    DpaiCds[i].gameObject.SetActive(false);
                    continue;
                }

                DpaiCds[i].SetLayer(DpaiCds[i].GetComponent<UISprite>().depth);
                DpaiCds[i].SetDipaiValue(_dPaicdsTemp[i]);
            }
            DpGrid.repositionNow = true;
        }*/

        /// <summary>
        /// 发牌时，计算局数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound))
                //更新局数信息缓存
                _gameInfoTemp.PutInt(NewRequestKey.KeyCurRound, _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) + 1);

            //更新局数label
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyMaxRound) &&
                _gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound))
            {
                RoundLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) + "/" +
                  _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);
                //播放第一局的敲锣声音
                if (_gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) == 1)
                {
                    Facade.Instance<MusicManager>().Play("gamestart");
                }
            }
        }

/*        /// <summary>
        /// 把所有底牌背过去
        /// </summary>
        private void TrunBackAllDipai()
        {
            if (DpaiCds == null) return;
            foreach (var dpItem in DpaiCds)
            {
                dpItem.TrunBackCd();
            }
        }*/


/*        private IEnumerator ChangeTextSize()
        {
            yield return new WaitForSeconds(3);
            int times = 0;
            while (times < 3)
            {
                times++;
                int size = 40;
                AnteLabel.fontSize = size;
                yield return 1;
                while (size > 25)
                {
                    size--;
                    AnteLabel.fontSize = size;
                    yield return 1;
                }
                yield return new WaitForSeconds(1);
            }
        }*/
    }
}
