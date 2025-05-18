using System;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerCdCtrl;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using Assets.Scripts.Game.ddz2.PokerRule;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 处理游戏局数，低分，倍数等公共游戏信息
    /// </summary>
    public class GameInfoListener : ServEvtListener
    {

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGameInfo);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoinGame);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnUserReady, OnUserReady);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, TypeFirstOut);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeGrab, OnTypeGrab);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFlow, OnTypeFlow);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnBeginNewGame, OnBeginNewGame);

            TrunBackAllDipai();
        }

        private void OnTypeFlow(DdzbaseEventArgs obj)
        {
            int curRound = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound);
            curRound--;
            _gameInfoTemp.PutInt(NewRequestKey.KeyCurRound, curRound);
        }

        /// <summary>
        /// 游戏信息缓存
        /// </summary>
        private ISFSObject _gameInfoTemp;
        private int[] _dPaicdsTemp;


        [SerializeField]
        protected UILabel AnteLabel;
        /// <summary>
        /// 临时存储倍数
        /// </summary>
        private int _beishu;
        [SerializeField]
        protected UILabel BeiShuLabel;
        [SerializeField]
        protected UILabel RoundLabel;
        [SerializeField]
        protected UILabel RoomIdLabel;

        /// <summary>
        /// 地主获得的底牌
        /// </summary>
        [SerializeField]
        protected DipaicdItem[] DpaiCds;

        /// <summary>
        /// 底牌的动画
        /// </summary>
        //[SerializeField] protected GameObject[] DipaiCdsAnims;

        /// <summary>
        /// 底牌的grid
        /// </summary>
        [SerializeField]
        protected UIGrid DpGrid;

        /// <summary>
        /// 局数的Obj，娱乐模式中不显示局数
        /// </summary>
        [SerializeField]
        protected GameObject RoundObj;
        /// <summary>
        /// 房间号Obj,娱乐模式中不显示房间号
        /// </summary>
        [SerializeField]
        protected GameObject RoomIdObj;

        /// <summary>
        /// 房间信息
        /// </summary>
        [SerializeField]
        protected GameObject RoomInfoView;
        /// <summary>
        /// gameinfo
        /// </summary>
        /// <param name="args"></param>
        protected void OnGameInfo(DdzbaseEventArgs args)
        {
            SetGameInfo(args);

            if (RoomInfoView == null) return;
            bool isRoomType = App.GetGameData<DdzGameData>().IsRoomGame;
            RoomInfoView.SetActive(isRoomType);
        }

        protected void OnRejoinGame(DdzbaseEventArgs args)
        {
            OnGameInfo(args);
        }


        private void SetGameInfo(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data == null) throw new Exception("得到了空的服务器信息");
            _gameInfoTemp = data;
            RefreshUiInfo();
        }

        public override void RefreshUiInfo()
        {
            if (_gameInfoTemp == null) return;

            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyAnte))
            {
                var ante = _gameInfoTemp.GetInt(NewRequestKey.KeyAnte);
                AnteLabel.text = YxUtiles.ReduceNumber(ante);
            }

            if (_gameInfoTemp.ContainsKey(RequestKey.KeyScore))
            {

                var score = (_gameInfoTemp.GetInt(RequestKey.KeyScore));
                var user = _gameInfoTemp.GetSFSObject(RequestKey.KeyUser);
                var ttscore = score;
                if (user.ContainsKey(NewRequestKey.KeyRate))
                {
                    ttscore = score * user.GetInt(NewRequestKey.KeyRate);
                }
                _beishu = ttscore;
                BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
            }

            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyMaxRound) &&
                _gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound) && RoundLabel != null)
            {
                RoundLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) + "/" +
                           _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);
                RoundObj.SetActive(true);
            }

            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyRoomId) && RoomIdLabel != null)
            {
                RoomIdLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyRoomId).ToString(CultureInfo.InvariantCulture);
                RoomIdObj.SetActive(true);
            }

            if (_gameInfoTemp.ContainsKey(RequestKey.KeyState))
            {
                int state = _gameInfoTemp.GetInt(RequestKey.KeyState);
                DpGrid.gameObject.SetActive(state > GlobalConstKey.StatusIdle);
            }
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyLandCds))
            {
                _dPaicdsTemp = _gameInfoTemp.GetIntArray("landCards");
            }
            SetDpaiCds();    
        }

        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        private void TypeFirstOut(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            _dPaicdsTemp = data.GetIntArray(RequestKey.KeyCards);

            PlayDipaiTurnAnim();

            //如果没有叫倍数，默认倍数为1
            BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);

            /*            try
                        {
                            var curbeishu = int.Parse(BeiShuLabel.text);
                            if (curbeishu < 1) BeiShuLabel.text = "1";
                        }
                        catch (Exception e)
                        {
                            YxDebug.LogError("BeiShuLabel有问题："+ e.Message);
                            BeiShuLabel.text = "1";
                        }*/
        }

        /// <summary>
        /// 播放翻牌动画
        /// </summary>
        protected void PlayDipaiTurnAnim()
        {
            DpGrid.gameObject.SetActive(true);
            int dipaiCount = DpaiCds.Length;
            int dipaiValCount = _dPaicdsTemp.Length;

            for (int i = 0; i < dipaiCount; i++)
            {
                var card = DpaiCds[i];
                if (i >= dipaiValCount)
                {
                    card.gameObject.SetActive(false);
                    continue;
                }
                card.gameObject.SetActive(true);
                card.TurnCard(_dPaicdsTemp[i], true);
            }
        }


        /// <summary>
        /// 当玩家准备时
        /// </summary>
        /// <param name="args"></param>
        protected void OnUserReady(DdzbaseEventArgs args)
        {
            TrunBackAllDipai();
            _beishu = 0;
            BeiShuLabel.text = "0";
        }

        /// <summary>
        /// 当底注改变时候
        /// </summary>
        /// <param name="args"></param>
        protected void OnChangDizhu(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey("ante"))
            {
                int ante = data.GetInt("ante");
                AnteLabel.text = YxUtiles.ReduceNumber(ante);
            }
            if (data.ContainsKey("rate"))
            {
                int rate = data.GetInt("rate");
                if (rate > 1)
                {
                    StartCoroutine(ChangeTextSize());
                }
            }
        }

        /// <summary>
        /// 抢地主时
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeGrab(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var score = data.GetInt(RequestKey.KeyScore);


            //如果有人叫分小于等于之前叫分，忽略之
            if (_beishu > score) return;

            _beishu = score;
            _gameInfoTemp.PutInt(RequestKey.KeyScore, score);
            BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 有人出牌时,检查是否有炸弹火箭等，从而显示加倍
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeOutCard(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var cards = data.GetIntArray(RequestKey.KeyCards);
            cards = HdCdsCtrl.SortCds(cards);
            var cdsType = PokerRuleUtil.GetCdsType(cards);
            if (cdsType == CardType.C4 || cdsType == CardType.C42 || cdsType == CardType.C5)
            {
                try
                {
                    int mul = 2;
                    //var curbeishu = int.Parse(BeiShuLabel.text);
                    _beishu *= mul;
                    BeiShuAnimAtion(mul);
                    BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    YxDebug.LogError("BeiShuLabel有问题：" + e.Message);
                }
            }
        }

        public MultipleAnim MulAnim;

        void BeiShuAnimAtion(int mul)
        {
            if (MulAnim == null)
                return;
            MulAnim.PlayMultipleAnim(mul);
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        private void OnDoubleOver(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var rates = data.GetIntArray("jiabei");
            var len = rates.Length;
            var selfSeat = App.GetGameData<DdzGameData>().SelfSeat;
            for (int i = 0; i < len; i++)
            {
                if (i != selfSeat) continue;
                int rate = rates[i];
                rate = rate > 0 ? rate : 1;
                _beishu = _gameInfoTemp.GetInt(RequestKey.KeyScore) * rate;
                MulAnim.PlayMultipleAnim(rate);
                BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 设置底牌
        /// </summary>
        void SetDpaiCds()
        {
            //如果游戏没有开始,无需显示底牌
            if (!App.GameData.IsGameStart)
            {
                DpGrid.gameObject.SetActive(false);
                return;
            }
            if (_dPaicdsTemp == null || _dPaicdsTemp.Length < 1 || _dPaicdsTemp.Length > DpaiCds.Length)
            {
                return;
            }

            DpGrid.gameObject.SetActive(true);
            var dpvalueLen = _dPaicdsTemp.Length;

            var dpGobLen = DpaiCds.Length;
            for (int i = 0; i < dpGobLen; i++)
            {
                var card = DpaiCds[i];
                if (i >= dpvalueLen)
                {
                    card.gameObject.SetActive(false);
                    continue;
                }

                card.SetLayer(card.GetComponent<UISprite>().depth);
                card.SetCdValue(_dPaicdsTemp[i]);
            }
            DpGrid.repositionNow = true;
            DpGrid.Reposition();
        }

        /// <summary>
        /// 发牌时，计算局数
        /// </summary>
        /// <param name="args"></param>
        void OnBeginNewGame(DdzbaseEventArgs args)
        {
            if (_gameInfoTemp == null) return;
            DpGrid.gameObject.SetActive(true);
            //更新局数label
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyMaxRound) &&
                _gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound))
            {
                int curRound = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound);
                curRound++;
                RoundLabel.text = curRound + "/" +
                                  _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);
                _gameInfoTemp.PutInt(NewRequestKey.KeyCurRound, curRound);
            }
        }

        /// <summary>
        /// 把所有底牌背过去
        /// </summary>
        private void TrunBackAllDipai()
        {
            if (DpaiCds == null) return;
            foreach (var dpItem in DpaiCds)
            {
                dpItem.TrunBackCd();
            }
        }


        private IEnumerator ChangeTextSize()
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
        }
    }
}
