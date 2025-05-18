using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerCdCtrl;
using Assets.Scripts.Game.ddz2.PokerRule;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class CmanagerListener : SelfHdCdsListener
    {

        [SerializeField]
        protected GameObject BuYaoBtn;
        [SerializeField]
        protected GameObject TiShiBtn;
        [SerializeField]
        protected GameObject ChuPaiBtn;
        [SerializeField]
        protected UIGrid BtnsGrid;

        protected bool AutoState;

        /// <summary>
        /// 复位所有选中牌按钮
        /// </summary>
        [SerializeField]
        protected UIButton RepositionAllHdCardsBtn;

        /// <summary>
        /// 处理牌判断，出牌，提示等各种处理的工具类
        /// </summary>
        private readonly CardManager _cardManager = new CardManager();

        /// <summary>
        /// 用于只能选牌的脚本
        /// </summary>
        private readonly CheckCardTool _checkCardTool = new CheckCardTool(PokerRuleUtil.GetCdsType);

        /// <summary>
        /// 最后一次出牌的各种信息
        /// </summary>
        private ISFSObject _lastOutData = new SFSObject();

        /*        /// <summary>
                /// 智能选牌信息缓存，要及时清理，用于比较上次智能选牌的结果是不是和下次一样
                /// </summary>
                private int[] _mayoutCdsTemp;*/

        /// <summary>
        /// 是否轮到我出牌
        /// </summary>
        private bool _isMyTurn;

        /// <summary>
        /// 手牌控制脚本
        /// </summary>
        [SerializeField]
        protected HdCdsCtrl HdCdctrlInstance;


        protected override void OnAwake()
        {
            //监听手牌操作事件
            Facade.EventCenter.AddEventListeners<string, HdCdCtrlEvtArgs>(GlobalConstKey.KeyHdCds, OnHdCdsCtrlEvent);

            base.OnAwake();

            InitRepositionAllHdCardsBtn();
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnAllocateCds);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypePass, OnTypePass);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOneRoundOver, OnTypeOneRoundOver);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfo);

            Facade.EventCenter.AddEventListeners<string, bool>(GlobalConstKey.KeySelfAuto, OnSelfAuto);

            Facade.EventCenter.AddEventListeners<string, bool>(GlobalConstKey.CheckLuckyCards, OnCheckLuckyCards);
        }

        private bool _autoLastOut = true;

        private void OnGetGameInfo(DdzbaseEventArgs args)
        {
            var sfsObj = args.IsfObjData;
            if (!sfsObj.ContainsKey("cargs2")) return;
            var cargsObj = sfsObj.GetSFSObject("cargs2");
            if (!cargsObj.ContainsKey("-alo")) return;
            string alo = cargsObj.GetUtfString("-alo");
            _autoLastOut = int.Parse(alo) > 0;
        }


        private void OnSelfAuto(bool state)
        {
            AutoState = state;
        }


        void InitRepositionAllHdCardsBtn()
        {
            if (RepositionAllHdCardsBtn == null) return;

            RepositionAllHdCardsBtn.onClick.Add(new EventDelegate(this, "RepositionAllHdCds"));
        }


        /// <summary>
        ///  根据手牌数据缓存刷新相关ui
        /// </summary>
        public override void RefreshUiInfo()
        {

        }


        /// <summary>
        /// 游戏发牌
        /// </summary>
        private void OnAllocateCds(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var seat = data.GetInt(GlobalConstKey.C_Sit);

            if (App.GetGameData<DdzGameData>().SelfSeat != seat || !data.ContainsKey(GlobalConstKey.C_Cards))
            {
                string error = !data.ContainsKey(GlobalConstKey.C_Cards)
                    ? "There is no cards info in type one !!"
                    : string.Format("Info seat : {0} != self seat : {1}", seat, App.GameData.SelfSeat);
                Debug.LogError(error);
                return;
            }
            var cards = data.GetIntArray(GlobalConstKey.C_Cards);
            if (cards == null || cards.Length <= 0)
            {
                Debug.LogError("Cards is null or empty !!");
                return;
            }

            InitHdCdsArray(cards);      //手牌加入手牌数组数组
            HdCdctrlInstance.ResetHandCdsWithAnim(cards);
        }

        /// <summary>
        /// 智能选牌开关
        /// </summary>
        private bool _onoffIchosecCds;

        /// <summary>
        /// 当玩家有对手牌进行点击操作时
        /// </summary>
        /// <param name="args"></param>
        private void OnHdCdsCtrlEvent(HdCdCtrlEvtArgs args)
        {
            //如果不该自己行动
            if (!_isMyTurn)
            {
                return;
            }

            //如果没有选牌
            if (args.SelectedCds.Length == 0)
            {
                if (ChuPaiBtn.activeSelf)
                {
                    SetBtnState(ChuPaiBtn, false);
                }

                _onoffIchosecCds = true;
                return;
            }

            var selectedCds = args.SelectedCds;

            var lastOutCds = _lastOutData.GetIntArray(RequestKey.KeyCards);

            //-----------------------start 智能选牌过程-------有赖子，或者开关关闭则不用智能选牌----------------------------
            bool isgetcdsWithoutCompare = false;    //标记是不是在自己出动出牌时做出的智能选牌
            int[] mayOutCds = null;
            bool selCdshasLaizi = PokerRuleUtil.CheckHaslz(selectedCds);
            if (!selCdshasLaizi && _onoffIchosecCds)
            {
                if (_lastOutData.GetInt(RequestKey.KeySeat) == App.GetGameData<DdzGameData>().SelfSeat)
                {
                    mayOutCds = _checkCardTool.GetcdsWithOutCompare(selectedCds, HdCdsListTemp.ToArray());
                    isgetcdsWithoutCompare = true;
                }
                else
                {
                    SetBtnActive(BuYaoBtn, true);
                    mayOutCds = _checkCardTool.ChkCanoutCdListWithLastCdList(selectedCds,
                                                                             _cardManager.GetTishiGroupDic, lastOutCds);
                }
            }
            //---------------------------------------------------------------------------------------end


            /*            //---start---------------使智能选牌出了相同的牌型，不重复执行-----------------------
                        var haschosemayOutCds = DDzUtil.IsTwoArrayEqual(HdCdsCtrl.SortCds(_mayoutCdsTemp), HdCdsCtrl.SortCds(mayOutCds));
                        _mayoutCdsTemp = mayOutCds;
                        //如果上次智能选牌和本次相同，则直接取消一次智能选牌
                        if (haschosemayOutCds)
                        {
                            mayOutCds = null;
                        }
                        //----------------------------------------------------------------------------------end*/



            if (mayOutCds == null || mayOutCds.Length == 0)
            {
                bool isMatch;

                //如果_lastOutData不是自己出牌
                if (_lastOutData.GetInt(RequestKey.KeySeat) != App.GetGameData<DdzGameData>().SelfSeat)
                {
                    var lastoutcds = new List<int>();

                    if (lastOutCds != null)
                    {
                        lastoutcds.AddRange(lastOutCds);
                    }

                    var cardTypeDic = _cardManager.GetAutoCardsList(selectedCds, selCdshasLaizi, lastoutcds.ToArray());
                    isMatch = cardTypeDic != null && cardTypeDic.Count > 0;
                }
                else
                {
                    var cardTypeDic = _cardManager.GetAutoCardsList(selectedCds, selCdshasLaizi, null);
                    isMatch = cardTypeDic != null && cardTypeDic.Count > 0;
                }

                if (isMatch)
                {
                    HdCdctrlInstance.UpCdList(selectedCds);
                }
                else
                {
                    if (ChuPaiBtn.activeSelf)
                    {
                        SetBtnState(ChuPaiBtn, false);
                    }
                    //DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
                    return;
                }
            }
            else
            {
                if (!ChooseMayOutCards(mayOutCds, selectedCds)) //如果选中的牌不能出
                {
                    if (ChuPaiBtn.activeSelf)
                    {
                        SetBtnState(ChuPaiBtn, false);
                    }
                    return;
                }
            }


            //经过智能检索后最后检查一遍抬出的牌是否合法----start---

            var finalType = PokerRuleUtil.GetCdsType(HdCdctrlInstance.GetUpCdList().ToArray());

            SetBtnState(ChuPaiBtn, true);
            //DDzUtil.ActiveBtn(ChuPaiBtn, DisChuPaiBtn);

            if (finalType != CardType.None && finalType != CardType.Exception)
            {
                //如果选出的牌型不是那种单牌，或者对子的小牌型，就先关闭智能选牌
                if (isgetcdsWithoutCompare && finalType != CardType.C1 && finalType != CardType.C2) _onoffIchosecCds = false;
                else if (!isgetcdsWithoutCompare) _onoffIchosecCds = false;
            }


            //------------end
        }


        protected override void OnRejoinGame(DdzbaseEventArgs args)
        {
            StopAllCoroutines();
            //与ResetHdCds，AddHdCds，RemoveHdCds相关
            base.OnRejoinGame(args);
            //显示手牌
            HdCdctrlInstance.ReSetHandCds(HdCdsListTemp.ToArray());

            var data = args.IsfObjData;
            int selfSeat = App.GetGameData<DdzGameData>().SelfSeat;
            _isMyTurn = data.ContainsKey(NewRequestKey.KeyCurrp) && data.GetInt(NewRequestKey.KeyCurrp) == selfSeat;


            //如果是选庄阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyGameStatus))
            {
                switch (data.GetInt(NewRequestKey.KeyGameStatus))
                {
                    case GlobalConstKey.StatusChoseBanker:
                    case GlobalConstKey.StatusDouble:
                        HideAllBtns();
                        return;
                }
            }

            //如果存在最后一次出牌的信息
            if (data.ContainsKey(NewRequestKey.KeyLastOut))
            {
                _lastOutData = data.GetSFSObject(NewRequestKey.KeyLastOut);
            }
            else
            {
                //是自己第一手出牌，你是地主
                _lastOutData.PutInt(RequestKey.KeySeat, selfSeat);
            }



            //没人行动，或者，不是自己行动
            if (!_isMyTurn)
            {
                HideAllBtns();
                return;
            }


            if (_lastOutData.GetInt(RequestKey.KeySeat) != selfSeat)
            {
                GetPromptCardsGroup(_lastOutData);
                if (AutoState)
                {
                    AutoFollow();
                }
                else
                {
                    OnOthersOutCds();
                }
            }
            else
            {
                HideAllBtns();
                SetBtnActive(ChuPaiBtn, true);
                SetBtnState(ChuPaiBtn, false);
                //如果自己准备出手牌
                SetAllBtnState();
                CheckCanOutOneTime();   //最后一手,如果可以全出,则全出
            }
        }


        /// <summary>
        /// 当前面有玩家出牌时,自动跟牌
        /// </summary>
        private void AutoFollow()
        {
            RepositionAllHdCds();
            if (_cardManager.GetTishiGroupList.Count > 0)
            {
                JumpUpCd(_cardManager.GetOneTishiGroup());
                OutCard();
            }
            else
            {
                Pass();
            }
        }

        /// <summary>
        /// 当前面玩家都不要时,自动出牌
        /// </summary>
        void AutoOutCard()
        {
            var cdsList = HdCdsCtrl.SortCds(HdCdsListTemp);
            SelectCards(cdsList[cdsList.Length - 1]);
            OutCard();
        }


        /// <summary>
        /// 点击背景,重置手牌的位置和出牌按钮的状态
        /// </summary>
        void RepositionAllHdCds()
        {
            HdCdctrlInstance.RepositionAllHdCds();
            SetBtnState(ChuPaiBtn, false);
        }

        /// <summary>
        /// 当确定地主后，看自己是不是地主，来判断是否显示按钮
        /// </summary>
        /// <param name="args"></param>
        protected override void OnFirstOut(DdzbaseEventArgs args)
        {
            base.OnFirstOut(args);   //底牌加入到手中

            var data = args.IsfObjData;

            var gdata = App.GetGameData<DdzGameData>();

            int speakerSeat = data.GetInt(RequestKey.KeySeat);      //地主座位号

            _isMyTurn = speakerSeat == gdata.SelfSeat;          //因为注册委托先后顺序的问题,不能用gdata.ImBanker判断

            if (!_isMyTurn)
            {
                HideAllBtns();
                return;
            }

            if (data.ContainsKey(NewRequestKey.JiaBeiSeat))
            {
                HideAllBtns();
            }
            else
            {
                SetBtnActive(ChuPaiBtn, true);
                SetBtnState(ChuPaiBtn, false);
            }

            _lastOutData.PutInt(RequestKey.KeySeat, speakerSeat);       //地主位置存入缓存
            HdCdctrlInstance.ReSetHandCds(HdCdsListTemp.ToArray());
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        private void OnDoubleOver(DdzbaseEventArgs args)
        {
            //判断是不是该自己操作出牌了
            if (_isMyTurn) StartCoroutine(ShowCtrlBtnLater(3f));
        }

        private IEnumerator ShowCtrlBtnLater(float time)
        {
            yield return new WaitForSeconds(time);

            HideAllBtns();
            if (_isMyTurn)
            {
                SetBtnState(ChuPaiBtn, false);
                SetBtnActive(ChuPaiBtn, true);
                if (AutoState)
                {
                    AutoOutCard();
                }
            }

            SetAllBtnState();
            CheckCanOutOneTime();   //最后一手,如果可以全出,则全出
        }

        /// <summary>
        /// 如果有人出牌
        /// </summary>
        /// <param name="args"></param>
        protected override void OnTypeOutCard(DdzbaseEventArgs args)
        {
            //_mayoutCdsTemp = null;
            //与ResetHdCds，AddHdCds，RemoveHdCds相关
            base.OnTypeOutCard(args);

            _lastOutData = args.IsfObjData;
            _isMyTurn = CheckMyTurn(_lastOutData);

            //自己出牌,移除手中的牌
            if (_lastOutData.ContainsKey(RequestKey.KeySeat) &&
                _lastOutData.GetInt(RequestKey.KeySeat) == App.GameData.SelfSeat)
            {
                if (_lastOutData.ContainsKey(RequestKey.KeyCards))
                {
                    var cards = _lastOutData.GetIntArray(RequestKey.KeyCards);
                    if (cards != null && cards.Length > 0)
                    {
                        HdCdctrlInstance.RemoveHdCds(cards);
                    }
                }
                RepositionAllHdCds();
            }

            if (_isMyTurn)
            {
                GetPromptCardsGroup(_lastOutData);
                if (AutoState)
                {
                    AutoFollow();
                }
                else
                {
                    OnOthersOutCds();
                }
                return;
            }

            HideAllBtns();
        }


        /// <summary>
        /// 有人pass的时候
        /// </summary>
        /// <param name="args"></param>
        void OnTypePass(DdzbaseEventArgs args)
        {
            var gdata = App.GetGameData<DdzGameData>();

            var data = args.IsfObjData;

            _isMyTurn = CheckMyTurn(data);      //查看是轮到自己说话

            //如果发送pass的玩家不是上家，则隐藏所有按钮
            if (!_isMyTurn)
            {
                HideAllBtns();
                return;
            }

            //如果最后一次出牌的玩家不是自己，则检测能不能管的上，来决定按钮状态
            if (_lastOutData.GetInt(RequestKey.KeySeat) != gdata.SelfSeat)
            {
                GetPromptCardsGroup(_lastOutData);
                if (AutoState)
                {
                    AutoFollow();
                }
                else
                {
                    OnOthersOutCds();
                }
            }
            //如果上家不出了，且最后一次出牌是自己
            else
            {
                HideAllBtns();
                SetBtnState(ChuPaiBtn, false);
                SetBtnActive(ChuPaiBtn, true);
                SetAllBtnState();
                if (!CheckCanOutOneTime() && AutoState)
                {
                    AutoOutCard();
                }
                SelectCards(HdCdctrlInstance.GetUpCdList().ToArray());
                _onoffIchosecCds = true;
            }
        }

        /// <summary>
        /// 是否是自己说话
        /// </summary>
        /// <param name="data"></param>
        bool CheckMyTurn(ISFSObject data)
        {
            //查看上次讲话的是不是自己上家
            int earlyHand = App.GetGameData<DdzGameData>().GetEarlyHand();
            return data.ContainsKey(RequestKey.KeySeat) && data.GetInt(RequestKey.KeySeat) == earlyHand;
        }


        /// <summary>
        /// 当一局游戏结算时
        /// </summary>
        /// <param name="args"></param>
        void OnTypeOneRoundOver(DdzbaseEventArgs args)
        {
            //隐藏所有出牌操作按钮
            HideAllBtns();
            //清空手牌
            HdCdsListTemp.Clear();
            _lastOutData = new SFSObject();  //重置
            if (HdCdctrlInstance != null) HdCdctrlInstance.ReSetHandCds(HdCdsListTemp.ToArray());
            RepositionAllHdCds();
        }

        /// <summary>
        /// 根据智能选牌结果，抬起手牌与之对应的牌，并过滤掉不应该抬起来的牌
        /// </summary>
        /// <param name="mayOutCds">智能选牌结果</param>
        /// <param name="selectedCds">智能检测前选择抬起的手牌</param>
        /// <returns></returns>
        private bool ChooseMayOutCards(int[] mayOutCds, int[] selectedCds)
        {
            //从选出的牌中拿出智能选牌要的牌
            var mayOutcdsListTemp = new List<int>();
            var len = mayOutCds.Length;
            for (int i = 0; i < len; i++)
            {
                mayOutcdsListTemp.Add(HdCdsCtrl.GetValue(mayOutCds[i]));
            }


            var upCdsList = new List<int>();
            len = selectedCds.Length;
            for (int i = 0; i < len; i++)
            {
                var pureValue = HdCdsCtrl.GetValue(selectedCds[i]);
                if (mayOutcdsListTemp.Contains(pureValue))
                {
                    upCdsList.Add(selectedCds[i]);
                    mayOutcdsListTemp.Remove(pureValue);
                }
            }

            //如果还有没选出的牌，则从手牌中继续选
            if (mayOutcdsListTemp.Count > 0)
            {
                len = HdCdsListTemp.Count;
                for (int i = 0; i < len; i++)
                {
                    var pureValue = HdCdsCtrl.GetValue(HdCdsListTemp[i]);
                    //如果是还没有选出的，在只能选牌牌组中的牌则添加
                    if (!upCdsList.Contains(HdCdsListTemp[i]) &&
                        mayOutcdsListTemp.Contains(pureValue))
                    {
                        upCdsList.Add(HdCdsListTemp[i]);
                        mayOutcdsListTemp.Remove(pureValue);
                    }
                }
            }

            if (upCdsList.Count != mayOutCds.Length)
            {
                return false;
            }

            HdCdctrlInstance.UpCdList(upCdsList.ToArray());

            return true;
        }




        //------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取提示手牌信息
        /// </summary>
        /// <param name="lastOutData"></param>
        private void GetPromptCardsGroup(ISFSObject lastOutData)
        {
            _cardManager.GetPromptGroup(HdCdsListTemp.ToArray(), lastOutData);
        }

        /// <summary>
        /// 当上一次玩家出牌不是自己时
        /// </summary>
        private void OnOthersOutCds()
        {
            if (_cardManager.GetTishiGroupList.Count > 0)   //当自己手牌能大过当前牌
            {
                SetBtnActive(BuYaoBtn, true);
                SetBtnActive(TiShiBtn, true);
                SetBtnActive(ChuPaiBtn, true);
                SetBtnState(ChuPaiBtn, false);
            }
            else            //当自己手牌无法大过当前牌
            {
                HideAllBtns();
                SetBtnActive(BuYaoBtn, true);
                return;
            }
            SelectCards(HdCdctrlInstance.GetUpCdList().ToArray());
            _onoffIchosecCds = true;
        }

        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="cards"></param>
        void SelectCards(int[] cards)
        {
            var args = new HdCdCtrlEvtArgs(cards);
            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyHdCds, args);
        }

        void SelectCards(int card)
        {
            int[] array = { card };
            SelectCards(array);
        }

        /// <summary>
        /// 设置所有出牌按钮的状态
        /// </summary>
        private void SetAllBtnState()
        {
            SelectCards(HdCdctrlInstance.GetUpCdList().ToArray());
            _onoffIchosecCds = true;
        }

        /// <summary>
        // 隐藏所有按钮
        /// </summary>
        private void HideAllBtns()
        {
            //终止ShowCtrlBtnLater方法，防止暂停回来异常显示出来
            StopAllCoroutines();

            SetBtnActive(BuYaoBtn, false);
            SetBtnActive(TiShiBtn, false);
            SetBtnActive(ChuPaiBtn, false);
        }

        void SetBtnActive(GameObject btn, bool active)
        {
            if (btn == null) return;

            btn.SetActive(active);

            BtnsGrid.repositionNow = true;
            BtnsGrid.Reposition();
        }


        /// <summary>
        /// 显示按钮,并设置按钮的状态
        /// </summary>
        /// <param name="btnObj"></param>
        /// <param name="couldClick"></param>
        void SetBtnState(GameObject btnObj, bool couldClick)
        {
            var btn = btnObj.GetComponent<UIButton>();
            if (btn == null) return;
            btn.GetComponent<BoxCollider>().enabled = couldClick;
            btn.state = couldClick ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
        }

        //---------以下按钮相关方法----start---------

        /// <summary>
        /// 点击不出按钮
        /// </summary>
        public void OnBuChuClick()
        {
            Pass();
        }


        /// <summary>
        /// 不要
        /// </summary>
        void Pass()
        {
            App.GetRServer<DdzGameServer>().TurnPass();
            HdCdctrlInstance.RepositionAllHdCds();
        }

        /// <summary>
        /// 点击提示按钮
        /// </summary>
        public void OnTishiClick()
        {
            HdCdctrlInstance.RepositionAllHdCds();
            var oneTishiGroup = _cardManager.GetOneTishiGroup();
            if (oneTishiGroup == null || oneTishiGroup.Length < 1) return;
            JumpUpCd(oneTishiGroup);

            //如果提示成功，把智能选牌关闭
            _onoffIchosecCds = false;

            //有提示牌组，点亮出牌按钮
            SetBtnState(ChuPaiBtn, true);
        }

        void JumpUpCd(int[] cardsVal)
        {
            foreach (var val in cardsVal)
            {
                JumpUpCd(val);
            }
        }

        void JumpUpCd(int cardVal)
        {
            HdCdctrlInstance.JustUpCd(cardVal);
        }

        /// <summary>
        /// 点击出牌按钮
        /// </summary>
        public void OnChuPaiClick()
        {
            OutCard();
        }

        void OutCard()
        {
            int[] cardArr = HdCdctrlInstance.GetUpCdList().ToArray();

            //赖子代表的牌
            var laiziRepCds = new[] { -1 };

            //类型
            int curRule = -1;
            var type = PokerRuleUtil.GetCdsType(cardArr);
            if (type != CardType.None && type != CardType.Exception) curRule = (int)type;
            App.GetRServer<DdzGameServer>().ThrowOutCard(cardArr, laiziRepCds, curRule);
        }

        //---------------end--

        /// <summary>
        /// 检测是否可以一手全出所有手牌，如果可以自动全出。
        /// </summary>
        private bool CheckCanOutOneTime()
        {
            if (!_autoLastOut) return false;

            var hdCds = HdCdsListTemp.ToArray();
            var cdsType = PokerRuleUtil.GetCdsType(hdCds);
            if (cdsType == CardType.None || cdsType == CardType.Exception || cdsType == CardType.C411) return false;
            //如果是飞机带单牌，查找是否含有炸弹，有则不自动出了   
            if (cdsType == CardType.C11122234)
            {
                var sotedCds = PokerRuleUtil.GetSortedValues(hdCds);
                var cdNum = 0;
                var curCd = -1;
                if (ExistC42(sotedCds))
                {
                    return false;
                }
                foreach (var cd in sotedCds)
                {
                    if (curCd != cd)
                    {
                        curCd = cd;
                        if (cdNum >= 4) return false;
                        cdNum = 1;
                        continue;
                    }
                    cdNum++;
                }
                if (cdNum >= 4) return false;
            }

            //赖子代表的牌
            var laiziRepCds = new[] { -1 };
            App.GetRServer<DdzGameServer>().ThrowOutCard(hdCds, laiziRepCds, (int)cdsType);
            return true;
        }
        /// <summary>
        /// 小王
        /// </summary>
        private readonly int _bigJoker = 0x51;
        /// <summary>
        /// 大王
        /// </summary>
        private readonly int _smallJoker = 0x61;
        /// <summary>
        /// 判断手牌里是否有王炸
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private bool ExistC42(int[] arr)
        {
            List<int> data = arr.ToList();
            return data.Contains(_bigJoker) && data.Contains(_smallJoker);
        }

        private void OnCheckLuckyCards(bool flag)
        {
            var result = HdCdctrlInstance.CheckLuckyCards();
            Facade.EventCenter.DispatchEvent<string, bool>(GlobalConstKey.CheckLuckyResult, result);
        }
    }
}
