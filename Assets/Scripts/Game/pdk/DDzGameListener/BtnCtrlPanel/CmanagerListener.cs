using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Assets.Scripts.Game.pdk.PokerCdCtrl;
using Assets.Scripts.Game.pdk.PokerRule;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDzGameListener.BtnCtrlPanel
{
    public class CmanagerListener : SelfHdCdsListener
    {
        [SerializeField]
        protected GameObject TiShiBtn;
        [SerializeField]
        protected GameObject ChuPaiBtn;

        [SerializeField]
        protected GameObject DisTiShiBtn;
        [SerializeField]
        protected GameObject DisChuPaiBtn;

        /// <summary>
        /// 点击出牌时不能出时显示的信息
        /// </summary>
        [SerializeField]
        protected GameObject FloatInfoWhenCantOutCds;

        /// <summary>
        /// 处理牌判断，出牌，提示等各种处理的工具类
        /// </summary>
        private CardManager _cardManager;

        /// <summary>
        /// 用于只能选牌的脚本
        /// </summary>
        private readonly CheckCardTool _checkCardTool = new CheckCardTool(PokerRuleUtil.GetCdsType);

        /// <summary>
        /// 最后一次出牌的各种信息
        /// </summary>
        private ISFSObject _lastOutData = new SFSObject();

        /// <summary>
        /// 标记地主座位
        /// </summary>
        private int _landSeat;

        /// <summary>
        /// 标记是否下家只剩一张牌
        /// </summary>
        private bool _isRpLeftOne = false;
        private bool _isRpLeftOneFirst = false;//标记是否下家第一次剩一个牌
        /// <summary>
        /// 手牌控制脚本
        /// </summary>
        [SerializeField]
        protected HdCdsCtrl HdCdctrlInstance;

        protected override void OnAwake()
        {   //监听手牌操作事件
            HdCdsCtrl.AddHdSelCdsEvt(OnHdCdsCtrlEvent);
            //获得cardmanager
            _cardManager = new CardManager();
            base.OnAwake();
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);

            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);

            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);

        }

        void Start()
        {
            App.GetGameData<GlobalData>().OnhandCdsNumChanged = OnSomeBodyHandCdsChanged;
        }

        /// <summary>
        /// 当有玩家卡牌数量减少时
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="leftHandCdNum"></param>
        private void OnSomeBodyHandCdsChanged(int userSeat, int leftHandCdNum)
        {
            if (userSeat == App.GetGameData<GlobalData>().GetRightPlayerSeat && leftHandCdNum < 2) _isRpLeftOne = true;

            if (_isRpLeftOne&& !_isRpLeftOneFirst )
            {
                Facade.Instance<MusicManager>().Play("Special_alert");
                _isRpLeftOneFirst = true;
            }
        }

        protected override void OnAllocateCds(object sender, DdzbaseEventArgs args)
        {
            //发牌后重置开关
            _isRpLeftOne = false;
            _isRpLeftOneFirst = false;

            //base.OnAllocateCds(sender,args);----------
            var data = args.IsfObjData;
            var seat = data.GetInt(GlobalConstKey.C_Sit);
            if (App.GetGameData<GlobalData>().GetSelfSeat != seat || !data.ContainsKey(GlobalConstKey.C_Cards)) return;
            var cards = data.GetIntArray(GlobalConstKey.C_Cards);

            ResetHdCds(cards);

            //------------------------end------------------------------------------------------------------------


            //判断发牌后是否是自己先行动出牌
            if (data.ContainsKey(GlobalConstKey.C_Bkp) &&
                data.GetInt(GlobalConstKey.C_Bkp) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                DisableAllBtns();

                if (_lastOutData == null) _lastOutData = new SFSObject();
                _lastOutData.PutInt(RequestKey.KeySeat, App.GetGameData<GlobalData>().GetSelfSeat);
                _lastOutData.PutIntArray(RequestKey.KeyCards, new int[] { });

                if (HdCdctrlInstance != null) HdCdctrlInstance.ReSetHandCds(HdCdsListTemp.ToArray(), true);
            }
            else
            {
                if (HdCdctrlInstance != null) HdCdctrlInstance.ReSetHandCds(HdCdsListTemp.ToArray(), true, true);
            }

        }

        /// <summary>
        ///  根据手牌数据缓存刷新相关ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            if (HdCdctrlInstance != null) HdCdctrlInstance.ReSetHandCds(HdCdsListTemp.ToArray());
        }

        /// <summary>
        /// 只能选牌开关
        /// </summary>
        private bool _onoffIchosecCds = false;

        /// <summary>
        /// 当玩家有对手牌进行点击操作时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnHdCdsCtrlEvent(object sender, HdCdCtrlEvtArgs args)
        {
            //如果不该自己行动
            if (!ChuPaiBtn.activeSelf && !DisChuPaiBtn.activeSelf) return;

            //如果没有选牌
            if (args.SelectedCds.Length == 0)
            {
                DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
                _onoffIchosecCds = true;
                return;
            }

            var selectedCds = args.SelectedCds;
            var lastOutCds = _lastOutData.GetIntArray(RequestKey.KeyCards);

            //如果直接全选了所有手牌，且不是关别家的牌是自己主动出的情况下，检查能不能一次全出
            if (selectedCds.Length == HdCdsListTemp.Count && _lastOutData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                var canOutType = CheckCanOutCdsOneTime(new CdSplitStruct(HdCdsListTemp.ToArray()));
                if (canOutType != CardType.None && canOutType != CardType.Exception)
                {
                    HdCdctrlInstance.UpAllHandCds();
                    DDzUtil.ActiveBtn(ChuPaiBtn, DisChuPaiBtn);
                    return;
                }
            }



            //-----------------------start 智能选牌过程-------有赖子，或者开关关闭则不用智能选牌----------------------------
            bool isgetcdsWithoutCompare = false;//标记是不是在自己出动出牌时做出的智能选牌
            int[] mayOutCds = null;
            bool selCdshasLaizi = PokerRuleUtil.CheckHaslz(selectedCds);
            if (!selCdshasLaizi && _onoffIchosecCds)
            {
                if (_lastOutData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
                {
                    mayOutCds = _checkCardTool.GetcdsWithOutCompare(selectedCds, HdCdsListTemp.ToArray());
                    isgetcdsWithoutCompare = true;
                }
                else
                {
                    //DDzUtil.ActiveBtn(BuYaoBtn, DisBuYaoBtn);
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
                if (_lastOutData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetSelfSeat)
                {
                    var lastoutcds = new List<int>();
                    lastoutcds.AddRange(lastOutCds);
                    var cardTypeDic = _cardManager.CheckCanGuanCds(selectedCds, selCdshasLaizi, lastoutcds.ToArray());
                    isMatch = cardTypeDic != null && cardTypeDic.Count > 0;
                }
                else
                {
                    var cardTypeDic = _cardManager.CheckCanGuanCds(selectedCds, selCdshasLaizi, null);
                    isMatch = cardTypeDic != null && cardTypeDic.Count > 0;
                }

                //Debug.LogError("isMatch: " + isMatch);
                if (isMatch)
                {
                    HdCdctrlInstance.UpCdList(selectedCds);
                }
                else
                {
                    DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
                    return;
                }
            }
            else
            {
                if (!ChooseMayOutCards(mayOutCds, selectedCds))
                {
                    DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
                    return;
                }
            }

            FinalOutCdsCheck(isgetcdsWithoutCompare);
        }

        /// <summary>
        /// 最终检测手牌
        /// </summary>
        /// <param name="isgetcdsWithoutCompare">是不是在没有其他人出牌情况下</param>
        private void FinalOutCdsCheck(bool isgetcdsWithoutCompare)
        {

            //经过智能检索后最后检查一遍抬出的牌是否合法----start---

            var finalType = PokerRuleUtil.GetCdsType(HdCdctrlInstance.GetUpCdList().ToArray());
            if (finalType != CardType.None && finalType != CardType.Exception)
            {
                //如果选出的牌型不是那种单牌，或者对子的小牌型，就先关闭智能选牌
                if (isgetcdsWithoutCompare && finalType != CardType.C1 && finalType != CardType.C2) _onoffIchosecCds = false;
                else if (!isgetcdsWithoutCompare) _onoffIchosecCds = false;

                DDzUtil.ActiveBtn(ChuPaiBtn, DisChuPaiBtn);
            }
            else
            {
                DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
            }

            //------------end
        }

        protected override void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            //与ResetHdCds，AddHdCds，RemoveHdCds相关
            base.OnRejoinGame(sender, args);

            var data = args.IsfObjData;

            //如果是选庄阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyState))
            {
                switch (data.GetInt(NewRequestKey.KeyState))
                {
                    case GlobalConstKey.StatusChoseBanker:
                        {
                            HideAllBtns();
                            return;
                        }
                    case GlobalConstKey.StatusDouble:
                        {
                            HideAllBtns();
                            return;
                        }
                    case GlobalConstKey.StatusIdle:
                        {
                            HideAllBtns();
                            return;
                        }
                    case GlobalConstKey.GameNotStart:
                        {
                            HideAllBtns();
                            return;
                        }
                }
            }


            //如果存在最后一次出牌的信息
            if (data.ContainsKey(NewRequestKey.KeyLastOut) && data.ContainsKey(NewRequestKey.KeyLastoutP))
            {
                _lastOutData.PutInt(RequestKey.KeySeat, data.GetInt(NewRequestKey.KeyLastoutP));
                _lastOutData.PutIntArray(RequestKey.KeyCards, data.GetIntArray(NewRequestKey.KeyLastOut));
            }

            //没人行动，或者，不是自己行动
            if (!data.ContainsKey(NewRequestKey.KeyCurrp) ||
                data.GetInt(NewRequestKey.KeyCurrp) != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                HideAllBtns();
                return;
            }

            if (_lastOutData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                OnNotSelfOutCds(_lastOutData, true);
            }
            else
            {
                //如果自己准备出手牌
                DisableAllBtns();
            }

        }

        /// <summary>
        /// 当确定地主后，看自己是不是地主，来判断是否显示按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnFirstOut(object sender, DdzbaseEventArgs args)
        {
            base.OnFirstOut(sender, args);

            var data = args.IsfObjData;

            _landSeat = data.GetInt(RequestKey.KeySeat);
            if (_landSeat != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                HideAllBtns();
                return;
            }

            _lastOutData.PutInt(RequestKey.KeySeat, _landSeat);

            //如果没有加倍设置
            if (!data.GetBool(NewRequestKey.KeyJiaBei))
            {
                DisableAllBtns();
            }
            else
            {
                HideAllBtns();
            }


        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        private void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            //判断是不是该自己操作出牌了
            if (_landSeat == App.GetGameData<GlobalData>().GetSelfSeat) DisableAllBtns();
        }


        /// <summary>
        /// 如果有人出牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            //_mayoutCdsTemp = null;
            //与ResetHdCds，AddHdCds，RemoveHdCds相关

            if (HdCdctrlInstance != null) HdCdctrlInstance.SetActiveHdcdsBack(false);

            base.OnTypeOutCard(sender, args);

            _lastOutData = args.IsfObjData;

            if (App.GetGameData<GlobalData>().PlayerMaxNum == 3)
            {
                if (_lastOutData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
                {
                    OnNotSelfOutCds(_lastOutData);
                    return;
                }
            }
            else
            {
                if (_lastOutData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetRightPlayerSeat)
                {
                    OnNotSelfOutCds(_lastOutData);
                    return;
                }
            }

            HideAllBtns();
        }


        /// <summary>
        /// 有人pass的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            //3人时
            if (App.GetGameData<GlobalData>().PlayerMaxNum == 3)
            {
                //如果发送pass的玩家不是上家，则隐藏所有按钮
                if (args.IsfObjData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetLeftPlayerSeat)
                {
                    HideAllBtns();
                    return;
                }
            }
            //2人时
            else
            {
                //如果发送pass的玩家不是上家，则隐藏所有按钮
                if (args.IsfObjData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetRightPlayerSeat)
                {
                    HideAllBtns();
                    return;
                }
            }



            //如果最后一次出牌的玩家不是自己，则检测能不能管的上，来决定按钮状态
            if (_lastOutData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                OnNotSelfOutCds(_lastOutData);
            }
            //如果其他人都不出了（上家不出，说明是其他人都不出了），且最后一次出牌是自己
            else
            {
                DisableAllBtns();
            }
        }


        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            //隐藏所有出牌操作按钮
            HideAllBtns();

            StartCoroutine(OnGameOverieumer());
        }

        private IEnumerator OnGameOverieumer()
        {
            yield return new WaitForSeconds(2f);

            //清空手牌
            ResetHdCds(new int[] { });
            RefreshUiInfo();
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
        /// 当上一次玩家出牌不是自己时
        /// </summary>
        /// <param name="lastOutData">最后一次出牌的信息</param>
        /// <param name="isRejoin">标记是否是重连进入的</param>
        private void OnNotSelfOutCds(ISFSObject lastOutData, bool isRejoin = false)
        {
            //DDzUtil.ActiveBtn(BuYaoBtn, DisBuYaoBtn);
            DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
            //玩家剩余最后4张牌，牌型三带一，最后一次出牌信息三带二，特殊改为玩家三带二计算（特殊情况）
            var lastcdType = CardType.None;
                lastcdType = PokerRuleUtil.GetCdsType(lastOutData.GetIntArray(RequestKey.KeyCards));
            if (HdCdsListTemp.ToArray().Length == 4 && lastcdType == CardType.C32)
            {
                var sortedLastOutCd = PokerRuleUtil.GetSortedValues(HdCdsListTemp.ToArray());
                var dictoNum = new Dictionary<int, int>();
                foreach (var cd in sortedLastOutCd)
                {
                    if (!dictoNum.ContainsKey(cd))
                    {
                        dictoNum[cd] = 1;
                        continue;
                    }
                    dictoNum[cd]++;
                }
                var dictoNumTwo = dictoNum;
                foreach (var cd in dictoNum.Keys.Where(cd => dictoNum[cd] == 1))
                {
                    dictoNumTwo[cd]++;
                    break;
                }
                List<int> TempHdCdsListTemp = new List<int>();
                foreach (var item in dictoNumTwo)
                {
                    for (int i = 0; i < item.Value; i++)
                    {
                        TempHdCdsListTemp.Add(item.Key);
                    }
                }
                endchupaiV(isRejoin, TempHdCdsListTemp, lastOutData);
            }
            else
            {
                endchupaiV(isRejoin, HdCdsListTemp, lastOutData);
            }

            var args = new HdCdCtrlEvtArgs(HdCdctrlInstance.GetUpCdList().ToArray());

            OnHdCdsCtrlEvent(null, args);
            _onoffIchosecCds = true;
        }

        //检测最后一手牌全出
        private void endchupaiV(bool isRejoin, List<int> Tcards, ISFSObject lastOutData)
        {
            //Debug.Log("<color=#0021FFFF>" + "检测最后一手牌全出" + "</color>");
            _cardManager.FindCds(Tcards.ToArray(), lastOutData);
            var tishiGpCunt = _cardManager.GetTishiGroupList.Count;
            if (tishiGpCunt > 0)
            {
                DDzUtil.ActiveBtn(TiShiBtn, DisTiShiBtn);
                //如果不是重连则自动提示，重连现在自动提示牌不提起来，有些问题，需要查一下，先保证不出错，重连时就不自动预提示了，需要手动点提示
                if (tishiGpCunt == 1)
                {
                    //如果直接一手牌全出，则自动出牌
                    if (_cardManager.GetTishiGroupList[0].Length == Tcards.Count)
                    {
                        foreach (var purecdvalue in Tcards.ToArray())
                        {
                            HdCdctrlInstance.JustUpCd(purecdvalue);
                        }
                        OnChuPaiClick();
                    }
                }
                if (!isRejoin) OnTishiClick();
            }
            else
            {
                //DDzUtil.DisableBtn(TiShiBtn, DisTiShiBtn);
                HdCdctrlInstance.RepositionAllHdCds();
                HideAllBtns();
            }
        }

        /// <summary>
        /// 显示按钮，并disable所有按钮
        /// </summary>
        private void DisableAllBtns()
        {
            //DDzUtil.DisableBtn(BuYaoBtn, DisBuYaoBtn);
            DDzUtil.DisableBtn(TiShiBtn, DisTiShiBtn);
            DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);

            var hdcdSplitGp = new CdSplitStruct(HdCdsListTemp.ToArray());
            //如果含有炸弹则跳过
            if (hdcdSplitGp.FourCds.Count < 1)
            {
                var canOutType = CheckCanOutCdsOneTime(hdcdSplitGp);
                //防止发送消息不成功，直接点亮出牌按钮，让玩家有再次发送的机会
                if (canOutType != CardType.None && canOutType != CardType.Exception)
                {
                    HdCdctrlInstance.UpAllHandCds();
                    DDzUtil.ActiveBtn(ChuPaiBtn, DisChuPaiBtn);
                    return;
                }
            }

            var args = new HdCdCtrlEvtArgs(HdCdctrlInstance.GetUpCdList().ToArray());
            OnHdCdsCtrlEvent(null, args);
            _onoffIchosecCds = true;

        }

        /// <summary>
        // 隐藏所有按钮
        /// </summary>
        private void HideAllBtns()
        {
            //BuYaoBtn.SetActive(false);
            TiShiBtn.SetActive(false);
            ChuPaiBtn.SetActive(false);

            //DisBuYaoBtn.SetActive(false);
            DisTiShiBtn.SetActive(false);
            DisChuPaiBtn.SetActive(false);
        }


        /// <summary>
        /// 检测是否可以一次把牌全出去的牌型
        /// </summary>
        private CardType CheckCanOutCdsOneTime(CdSplitStruct hdcdSplitGp)
        {
            var cdType = PokerRuleUtil.GetCdsType(HdCdsListTemp.ToArray());

            //如果可以一次出，则直接全出
            if (cdType != CardType.None && cdType != CardType.Exception)
            {
                GlobalData.ServInstance.ThrowOutCard(HdCdsListTemp.ToArray(), new int[] { -1 }, (int)cdType);
                return cdType;
            }

            //如果是3张或者3带1的情况直接发送出牌信息
            if (CheckOutCdsC3OrC31())
            {
                GlobalData.ServInstance.ThrowOutCard(HdCdsListTemp.ToArray(), new int[] { -1 }, (int)CardType.C32);
                return CardType.C32;
            }

            if (CheckOutCdsC1112223434(hdcdSplitGp))
            {
                GlobalData.ServInstance.ThrowOutCard(HdCdsListTemp.ToArray(), new int[] { -1 }, (int)CardType.C1112223434);
                return CardType.C1112223434;
            }

            return CardType.None;
        }

        /// <summary>
        /// 检查是不是最后一手牌的，3张，或者 3带1
        /// </summary>
        private bool CheckOutCdsC3OrC31()
        {
            var len = HdCdsListTemp.Count;
            switch (len)
            {
                //3张情况
                case 3:
                    return PokerRuleUtil.GetValue(HdCdsListTemp[0]) == PokerRuleUtil.GetValue(HdCdsListTemp[1])
                           && PokerRuleUtil.GetValue(HdCdsListTemp[1]) == PokerRuleUtil.GetValue(HdCdsListTemp[2]);
                //3带1情况
                case 4:

                    var cdsValues = PokerRuleUtil.GetSortedValues(HdCdsListTemp.ToArray());
                    return cdsValues[0] == cdsValues[2] || cdsValues[1] == cdsValues[3];
            }

            return false;
        }

        private bool CheckOutCdsC1112223434(CdSplitStruct hdcdSplitGp)
        {
            var len = HdCdsListTemp.Count;
            if (len >= 6)
            {
                var sanLianList = new List<int>();
                sanLianList.AddRange(hdcdSplitGp.ThreeCds);
                sanLianList.AddRange(hdcdSplitGp.FourCds);

                sanLianList.Sort();
                var posbLian = PokerRuleUtil.GetAllPossibleShun(sanLianList.ToArray(), 2);
                var posbLianLen = posbLian.Count;
                if (posbLian.Count < 1) return false;
                //找到最长的三连
                posbLian.Sort(DDzUtil.SortCdsintesByLen);

                //最终得到最大三连牌长度
                var maxThreeLianCdsLen = posbLian[posbLianLen - 1].Length;

                if ((len - maxThreeLianCdsLen * 3) <= (maxThreeLianCdsLen * 2)) return true;
            }

            return false;
        }

        //---------以下按钮相关方法----start---------

        /// <summary>
        /// 点击不出按钮
        /// </summary>
        public void OnBuChuClick()
        {
            GlobalData.ServInstance.TurnPass();
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
            foreach (var purecdvalue in oneTishiGroup)
            {
                HdCdctrlInstance.JustUpCd(purecdvalue);
            }

            //如果提示成功，把智能选牌关闭
            _onoffIchosecCds = false;

            //_mayoutCdsTemp = (int[]) oneTishiGroup.Clone();
            //有提示牌组，点亮出牌按钮
            DDzUtil.ActiveBtn(ChuPaiBtn, DisChuPaiBtn);
        }

        /// <summary>
        /// 点击出牌按钮
        /// </summary>
        public void OnChuPaiClick()
        {
            //Debug.Log("<color=#0021FFFF>" + "点击出牌按钮点击出牌按钮点击出牌按钮点击出牌按钮点击出牌按钮点击出牌按钮" + "</color>");
            int[] cardArr = HdCdctrlInstance.GetUpCdList().ToArray();

            //判断是否需要检测黑三先出,如果手牌有黑三，出的牌没黑三不能出牌
            if (App.GetGameData<GlobalData>().IsHeiSanFirst)
            {
                bool hdhasHeisan = HdCdsListTemp.Any(cd => PokerRuleUtil.GetValue(cd) == 3 && PokerRuleUtil.GetColor(cd) == 4);

                if (hdhasHeisan)
                {
                    bool otcdHasHeisan = cardArr.Any(cd => PokerRuleUtil.GetValue(cd) == 3 && PokerRuleUtil.GetColor(cd) == 4);
                    if (!otcdHasHeisan)
                    {
                        ShowWarnInfo("有黑3必须先出黑3！");
                        return;
                    }
                }
            }

            if (_isRpLeftOne && cardArr.Length == 1 && HdCdsListTemp.Count > 1)
            {
                var cdsValue = PokerRuleUtil.GetSortedValues(HdCdsListTemp.ToArray());
                if (PokerRuleUtil.GetValue(cardArr[0]) != PokerRuleUtil.GetValue(cdsValue[cdsValue.Length - 1]))
                {
                    ShowWarnInfo("下家报单必须出最大！");
                    return;
                }
            }


            SendOutCdsRequest(cardArr);
        }

        /// <summary>
        /// 发送出牌消息
        /// </summary>
        /// <param name="handCardArr"></param>
        private void SendOutCdsRequest(int[] handCardArr)
        {
            //Debug.Log("<color=#0021FFFF>" + "发送出牌消息发送出牌消息发送出牌消息发送出牌消息发送出牌消息发送出牌消息" + "</color>");

            //赖子代表的牌
            var laiziRepCds = new int[] { -1 };

            //类型
            int curRule = -1;
            var type = PokerRuleUtil.GetCdsType(handCardArr);
            if (type != CardType.None && type != CardType.Exception)
            {
                curRule = (int)type;
            }
            else
            {
                curRule = (int)CheckCanOutCdsOneTime(new CdSplitStruct(handCardArr));
            }


            GlobalData.ServInstance.ThrowOutCard(handCardArr, laiziRepCds, curRule);
        }

        /// <summary>
        /// 不能出牌时显示的信息
        /// </summary>
        /// <param name="infotxt"></param>
        void ShowWarnInfo(string infotxt)
        {

            var gobClone = NGUITools.AddChild(FloatInfoWhenCantOutCds.transform.parent.gameObject,
                                              FloatInfoWhenCantOutCds);
            gobClone.GetComponentInChildren<UILabel>().text = infotxt;
            gobClone.transform.localScale = new Vector3(1f, 1f, 1f);
            var tweenColor = gobClone.GetComponent<TweenColor>();
            EventDelegate.Add(tweenColor.onFinished, () => Destroy(gobClone), true);
            gobClone.SetActive(true);
        }

        //---------------end--
    }
}
