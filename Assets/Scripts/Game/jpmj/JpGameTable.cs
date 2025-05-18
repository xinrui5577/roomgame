using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.jpmj.GameNotice;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameTable;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.jpmj
{
    public class JpGameTable : GameTable
    {        
        private readonly Dictionary<string, Sprite> _cdValueToName = new Dictionary<string, Sprite>(); 
        public Sprite[] CdValues;

        [SerializeField]
        public Image CdLaizi;

        [SerializeField]
        public Image CdGtou;

        [SerializeField] private TableData _tableData;
        [Tooltip("公告界面")]
        public GameNoticeWindow NoticeWindow;

        public float BuzhangEffectInterval = 1.2f;

        protected override void OnRegistEvent()
        {
            _cdValueToName.Clear();
            //初始化显示杠头和财神的图片
            foreach (var sprite in CdValues)
            {
                _cdValueToName.Add(sprite.name, sprite);
            }

            base.OnRegistEvent();

            EventDispatch.Instance.RegisteEvent((int)GameEventId.BuZhang, OnBuZhang);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.OnRefreshHandCard, OnRefreshHandCard);
        }

        private Coroutine _buZhangAniCor;
        protected void OnBuZhang(int eventid, EventData data)
        {
            var buData = (List<BuZhangData>)data.data1;
            var leaveNum = (DVoidInt)data.data2;
            if (_buZhangAniCor!=null)
            {
                StopCoroutine(_buZhangAniCor);
            }
            _buZhangAniCor=StartCoroutine(BuZhangAnimation(buData, leaveNum));
            float time = 0;
            time += buData.Count * (0.2f + GameConfig.GetInCardRoteTime + GameConfig.GetInCardWaitTime + 0.3f) + 0.1f;
            EventDispatch.Dispatch((int)NetEventId.PauseResponse, new EventData(time));
        }

        protected void OnRefreshHandCard(int eventid, EventData data)
        {           
            if (_currChair == 0)
            {
                PlayerHand.MahjongsResetPos();
            }
            else
            {
                PlayerHand.SortHandMahjong();
            }
        }

        protected IEnumerator BuZhangAnimation(List<BuZhangData> data, DVoidInt leaveNum)
        {
            var count = data.Count;
            for (int i = 0; i < count; i++)
            {
                var zhangData = data[i];
                if (zhangData!=null)
                {
                    var list = zhangData.BuZhangCards;
                    var cards = zhangData.Cards;
                    for (int j = 0; j < list.Length; j++)
                    {
                        var value = list[j];
                        var chair = zhangData.Chair;
                        HardMj[zhangData.Chair].RemoveMahjong(value);
                        Hu[zhangData.Chair].GetInMahjong(value);
                        OnRevWallMahjongFinish();
                        HardMj[zhangData.Chair].GetInMahjong(cards[j]);

                        var sound = value < 96 ? EnGameSound.gang : EnGameSound.buhua;
                        SoundManager.Instance.PlayGameEffect(chair, sound);
                        EventDispatch.Dispatch((int)UIEventId.BuzhangEffect, new EventData(chair));
                        yield return new WaitForSeconds(BuzhangEffectInterval);
                    }

                    //移除手牌中的
                    //HardMj[zhangData.Chair].RemoveMahjong(new List<int>(zhangData.BuZhangCards));
                    //添加到胡牌中
                    //Hu[zhangData.Chair].GetInMahjong(zhangData.BuZhangCards);

                    yield return new WaitForSeconds(0.2f);
                    var walPopMjlen = zhangData.BuZhangCards.Length;
//                    for (int j = 0; j < walPopMjlen; j++)
//                    {
//                        OnRevWallMahjongFinish();
//                    }
                    leaveNum(zhangData.BuZhangCards.Length);                   
                    //添加到手牌中
                    //HardMj[zhangData.Chair].GetInMahjongWithRoat(zhangData.Cards);
                    //yield return new WaitForSeconds(BuzhangEffectInterval);
                }
            }          

            foreach (var hardCards in HardMj)
            {
                if (hardCards)
                {
                    hardCards.SortHandMahjong();
                }
            }
            //补张结束 允许出牌
            App.GetRServer<NetWorkManager>().BuzhangState = false;
            //开局补张以后检索一遍自己手牌有没有漏掉的财神
            CheckCaishenBug();
        }

        private Coroutine _buZhangAni;
        protected override void OnGetInCard(EventData evn)
        {
            //有补张
            if (evn.data3 != null)
            {
                if (_buZhangAni!=null)
                {
                    StopCoroutine(_buZhangAni);
                }

                _buZhangAni=StartCoroutine(BuZhangGetInAnimation(evn));               
            }
            else
            {
                //根据是不是杠后抓的牌来决定是否倒序抓牌
                if (evn.data5 != null)
                {
                    OnRevWallMahjongFinish();
                }
                else
                {
                    WallMj[_getMahjongChair].PopMahjong();
                }


                int value = (int)evn.data1;

                //是否有赖子
                if (evn.data2 != null && CheckLaizi((int)evn.data2, value))
                    HardMj[_currChair].GetInMahjong(value, true);
                else
                    HardMj[_currChair].GetInMahjong(value);

                SoundManager.Instance.PlayCommonEffect(EnCommonSound.zhuapai);
                CheckCaishenBug();

                //第一次抓牌 隐藏塞子
                if (_isFirstGetInCard)
                {
                    _isFirstGetInCard = false;
                    Saizi.HideSaizi();
                    Timer.StartTime(GameConfig.OutCardTime);

                    //如果桌面没有打出的牌，说明是刚发牌，重连
                    bool hasthrowmj = ThrowMj.Any(throwZone => throwZone.transform.childCount > 0);
                    if (!hasthrowmj)
                    {
//                        StartCoroutine(DoRejoinGame(value, _currChair));
                    }
                }
            }
        }

        /// <summary>
        /// 做财神混乱bug的检查
        /// </summary>
        private void CheckCaishenBug()
        {
            var mjhand = HardMj[0] as JpMahjongPlayerHard;
            if (mjhand == null)
                return;
            mjhand.CheckChaiShenItem();
        }

        private void CheckCdNum()
        {
            var mjhand = HardMj[0] as JpMahjongPlayerHard;
            if (mjhand != null)
            {
                //还没给牌时手牌应该16张
                mjhand.CheckCdNum16();
            }
        }

        private IEnumerator DoRejoinGame(int lastGetinMjValue,int curChair)
        {
            var mjhand = HardMj[0] as JpMahjongPlayerHard;
            if (mjhand != null)
            {
                mjhand.SortHandMjPos();
                if (curChair == 0)
                {
                    mjhand.SetLastCardPos(lastGetinMjValue);
                }
            }
            yield return new WaitForEndOfFrame();
            App.GetRServer<NetWorkManager>().DoSendRejoinGame();
        }
    
        protected IEnumerator BuZhangGetInAnimation(EventData evn)
        {
            PlayerToken = false;
            int getMahjongChair = _getMahjongChair;
            int currChair = _currChair;
            if (currChair != 0)
            {
                base.OnGetInCard(evn);
            }

            var data = new List<int>();
            data.AddRange((List<int>)evn.data3);
            EventDispatch.Dispatch((int)NetEventId.PauseResponse, new EventData(data.Count * 1.5f));

            var leaveNum = (DVoidInt)evn.data4;
            foreach (int value in data)
            {
                if (evn.data5 != null) OnRevWallMahjongFinish();
                else
                    //正序摸牌
                    WallMj[getMahjongChair].PopMahjong();
                //添加到手牌中
                if (currChair == 0)
                    HardMj[currChair].GetInMahjong(value);
                else
                    HardMj[currChair].GetInMahjong(0);

                leaveNum(1);
                yield return new WaitForSeconds(GameConfig.GetInCardRoteTime + GameConfig.GetInCardWaitTime + 0.15f);

                //移除手牌中的
                HardMj[currChair].RemoveMahjong(value);
                //添加到胡牌中
                Hu[currChair].GetInMahjong(value);
                var sound = value < 96 ? EnGameSound.gang : EnGameSound.buhua;
                SoundManager.Instance.PlayGameEffect(currChair, sound);
                EventDispatch.Dispatch((int)UIEventId.BuzhangEffect, new EventData(currChair));
                yield return new WaitForSeconds(1f);
            }

            if (currChair == 0)
            {
                base.OnGetInCard(evn);

                if (evn.data6 != null)
                {
                    if (_tableData != null)
                    {
                        var tabledata = _tableData as JpTableData;
                        if (tabledata != null) tabledata.ClearQueryHuList();
                    }

                    EventDispatch.Dispatch((int)GameEventId.TingList, new EventData(evn.data6));
                }
            }

            //补张结束 允许出牌
            App.GetRServer<NetWorkManager>().BuzhangState = false;
            PlayerToken = true;
            CheckCaishenBug();
        }

        //protected void BuZhangGetInForJp(EventData evn)
        //{
        //    PlayerToken = false;
        //    var data = new List<int>();
        //    data.AddRange((List<int>)evn.data3);
        //    //清理补张缓存
        //    var tbData = _tableData as JpTableData;
        //    if (tbData != null) tbData.ClearBuzhangGetIn();
        //    var leaveNum = (DVoidInt)evn.data4;
        //    foreach (int value in data)
        //    {
        //        if (evn.data5 != null) OnRevWallMahjongFinish();
        //        else
        //            //正序摸牌
        //            WallMj[_getMahjongChair].PopMahjong();
        //        //添加到手牌中
        //        if (_currChair == 0)
        //            HardMj[_currChair].GetInMahjong(value);
        //        else
        //            HardMj[_currChair].GetInMahjong(0);
        //        leaveNum(1);
        //        //移除手牌中的
        //        HardMj[_currChair].RemoveMahjong(value);
        //        //添加到胡牌中
        //        Hu[_currChair].GetInMahjong(value);
        //        SoundManager.Instance.PlayGameEffect(_currChair, value < 96 ? EnGameSound.gang : EnGameSound.buhua);
        //    }
        //    base.OnGetInCard(evn);
        //    PlayerToken = true;
        //}

        /// <summary>
        /// 根据补花还是杠头，来决定是正序抽牌，还是倒序抽牌
        /// </summary>
        /// <param name="cdValue"></param>
        private void CheckMjPop(int cdValue)
        {
            //不是花，直接正序摸牌
            if (cdValue < 96)
            {
                //在墙中移除
                WallMj[_getMahjongChair].PopMahjong();
                return;
            }

            OnRevWallMahjongFinish();
        }

        protected override void OnGameEvent(GameEventId id, EventData evn)
        {
            switch (id)
            {
                case GameEventId.RevWallMahjongFinish:
                    OnRevWallMahjongFinish();
                break;
            }
        }


        private void OnRevWallMahjongFinish()
        {
            //获得最后一个牌墙的位置
            var lastChair = _getMahjongChair;
            if (lastChair != UtilFunc.GetPerChair(lastChair, WallMj.Length))
            {
                var len = WallMj.Length;
                for (int i = 0; i < len; i++)
                {
                    var curChair = UtilFunc.GetPerChair(lastChair, WallMj.Length);
                    if (curChair != _getMahjongChair && WallMj[curChair].GetMjCnt() > 0)
                    {
                        lastChair = curChair;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            WallMj[lastChair].RevPopMahjong();
        }

        protected override IEnumerator SendCardCoroutine(OnSendCardEventData cardData)
        {
            //发牌
            yield return SendCardAnimation(cardData);

            //翻牌
            if (cardData.fanpai != UtilDef.NullMj)
            {
                yield return FanPaiAnimation(cardData);
            }
            //扣下牌 排序 设置赖子 抬起
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                HardMj[i].OnSendOverSortMahjong(GameConfig.SendCardUpTime, GameConfig.SendCardUpTime + GameConfig.SendCardUpTime, cardData.laizi);
            }
        }

        protected override IEnumerator FanPaiAnimation(OnSendCardEventData data)
        {
            data.reduceMjCnt(1);
            SetFpMahjongPos(data.fanpai, data.laizi);
            yield break;
        }

        protected override void OnGameStatus(EventData evn)
        {
            base.OnGameStatus(evn);

            var status = (EnGameStatus)evn.data1;
            if (status == EnGameStatus.GameFree)
            {
                //显示牌墙
                foreach (var mahjongWall in WallMj)
                {
                    if (mahjongWall != null) mahjongWall.gameObject.SetActive(true);
                }
            }
        }

        protected override void OnGamePlay(TableData data)
        {
            if (data.IsReconect)
            {
                StopAllCoroutines();
            }

            JpMahjongPlayerHard.CaishenValue = data.Laizi;     
            App.GetRServer<NetWorkManager>().BuzhangState = false;

            base.OnGamePlay(data);
            //如果是重新链接 设置桌面信息
            if (data.IsReconect)
            {
                //显示牌墙
                foreach (var mahjongWall in WallMj)
                {
                    if (mahjongWall != null) mahjongWall.gameObject.SetActive(true);
                }

                var jjData = (JpTableData)data;
                for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
                {
                    var walPopMjlen = jjData.BuZhangList[i].Count;
                    for (int k = 0; k < walPopMjlen; k++)
                    {
                        OnRevWallMahjongFinish();
                    }


                    //设置出牌
                    int chair = data.GetChairId(i);
                    var cds = jjData.BuZhangList[i].ToArray();
                    Hu[chair].GetInMahjong(cds);

                    data.LeaveMahjongCnt -= cds.Length;
                }


                for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
                {
                    //打出牌是赖子牌，设置icon
                    foreach (var item in ThrowMj[i].GetMahjongList.FindAll(a => a.Value == data.Laizi))
                        item.IsSign = true;
                }
                //翻开一张赖子牌
                data.LeaveMahjongCnt--;
                if (data.Fanpai != -1)
                {
                    SetFpMahjongPos(data.Fanpai, data.Laizi);
                }
            }
        }

        protected override void OnGameFree(TableData data)
        {
            base.OnGameFree(data);

            if (Dnxb != null)
            {
                JpDnxbCtrl.SetDnxb(Dnxb, data.BankerSeat, data.PlayerSeat);
            }
            fanpaiMahjong = null;
            FanpaiBottom.gameObject.SetActive(false);
            if (GobLaizGtouInfo != null) GobLaizGtouInfo.SetActive(false);
        }

        protected override IEnumerator SendCardAnimation(OnSendCardEventData cardData)
        {
            int cont = GameConfig.UserHandCardCnt * UtilData.CurrGamePalyerCnt;

            if (!GameConfig.IsNeedSendCardAnimation)
            {
                //不需要发牌动画时 直接设置手牌
                WallMj[_getMahjongChair].PopMahjong(cont);
                int[] nomalCards = new int[GameConfig.UserHandCardCnt];
                int startChair = cardData.currChair;
                for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
                {
                    HardMj[startChair].GetInMahjong(startChair == 0 ? cardData.playermj : nomalCards);
                    startChair = UtilFunc.GetNextChair(startChair, UtilData.CurrGamePalyerCnt);
                }
                cardData.reduceMjCnt(cont);
                yield return 0;
            }
            else
            {
                int startChair = cardData.currChair;
                int meOffset = 0;
                while (cont > 0)
                {
                    int getInCnt = 4;
                    int[] card = new int[getInCnt];

                    WallMj[_getMahjongChair].PopMahjong(getInCnt);
                    if (startChair == 0)
                    {
                        for (int i = 0; i < card.Length; i++)
                        {
                            card[i] = cardData.playermj[meOffset + i];
                        }
                        meOffset += card.Length;
                    }
                    else
                    {
                        for (int i = 0; i < card.Length; i++)
                        {
                            card[i] = 0;
                        }
                    }

                    if (startChair == 0)
                    {
                        HardMj[startChair].OnSendMahjongForJp(card, GameConfig.SendCardUpTime, GameConfig.SendCardUpWait,
                                                              cardData.laizi);
                    }
                    else
                    {
                        HardMj[startChair].OnSendMahjong(card, GameConfig.SendCardUpTime, GameConfig.SendCardUpWait);
                    }

                    cardData.reduceMjCnt(getInCnt);

                    yield return new WaitForSeconds(GameConfig.SendCardInterval);

                    cont -= getInCnt;
                    startChair = UtilFunc.GetNextChair(startChair, UtilData.CurrGamePalyerCnt);
                }

                yield return new WaitForSeconds(GameConfig.SendCardUpTime + GameConfig.SendCardUpWait);
            }

            //发牌以后检索一遍自己手牌有没有漏掉的财神
            CheckCaishenBug();
        }

        public GameObject GobLaizGtouInfo;

    
        //设置翻牌位置
        protected override void SetFpMahjongPos(int value,int laizi=-1)
        {
            if (GobLaizGtouInfo != null) GobLaizGtouInfo.SetActive(true);

            //赖子和杠头直接显示在左上角
            string str = "tile_me_" + value;
            if (_cdValueToName.ContainsKey(str))
            {
                CdGtou.sprite = _cdValueToName[str];
            }

            str = "tile_me_" + laizi;
            if (_cdValueToName.ContainsKey(str))
            {
                CdLaizi.sprite = _cdValueToName[str];
            }
            CdGtou.SetNativeSize();
            CdLaizi.SetNativeSize();
            OnRevWallMahjongFinish();
        }

        /// <summary>
        /// 在翻牌的基础上，再翻出赖子
        /// </summary>
        /// <param name="laizi"></param>
        private void SetLiaziMahjongPos(int laizi)
        {
            Vector3 targetPos = new Vector3(
            FanpaiBottom.localPosition.x-0.15f,
            FanpaiBottom.localPosition.y + 0.1f,
            FanpaiBottom.localPosition.z
            );

            WallMj[_getMahjongChair].PopMahjong();
            fanpaiMahjong = MahjongManager.Instance.CreateCloneMajong(laizi).GetComponent<MahjongItem>();
            MahjongManager.Instance.ExchangeByValue(laizi, fanpaiMahjong);
            if (null == fanpaiMahjong) return;

            fanpaiMahjong.IsSign = true;
            fanpaiMahjong.transform.parent = transform;
            fanpaiMahjong.transform.localPosition = targetPos;
            fanpaiMahjong.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
            fanpaiMahjong.transform.localScale = Vector3.one;
            fanpaiMahjong.gameObject.SetActive(true);
        }

        protected override IEnumerator GameResult(TableData table)
        {
            if (Dnxb != null) Dnxb.Reset();

            var result = table.Result;
            int huType = table.Result.HuType;
            //为手牌赋值
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                int chair = table.GetChairId(i);
                if (chair == 0) continue;

                HardMj[chair].RemoveAllMj();
                HardMj[chair].GetInMahjong(table.UserHardCard[i].ToArray());
                HardMj[chair].SetLaizi(table.Laizi);
            }

            //如果是冲宝牌墙减1张
            if (result.ChBao)
            {
                WallMj[_getMahjongChair].PopMahjong();
            }

            //隐藏牌墙
            foreach (var mahjongWall in WallMj)
            {
                if(mahjongWall!=null)mahjongWall.gameObject.SetActive(false);
            }

            if (huType == MjRequestData.MJReqTypeLastCd)//流局
            {
                yield return new WaitForSeconds(GameConfig.GameEndLiuJuWait);

                for (int i = 0; i < HardMj.Length; i++)
                {
                    HardMj[i].GameResultRota(GameConfig.PushCardTime);
                }

                SoundManager.Instance.PlayCommonEffect(EnCommonSound.liu_ju);
            }
            else
            {

                yield return new WaitForSeconds(GameConfig.GameEndHuWait);

                if (huType == MjRequestData.MJRequestTypeHu)//别人点炮
                {
                    YxDebug.Log("一局结束 胡牌" + result.HuSeat[0] + " 点炮 " + table.OldCurrChair);

                    SoundManager.Instance.PlayGameEffect(_currChair, EnGameSound.hu);

                    int paoChair = table.OldCurrChair;

                    //一炮多想
                    if (result.HuSeat.Count > 1)
                    {
                        int index = 0;
                        foreach (int i in result.HuSeat)
                        {
                            int duoHuChair = table.GetChairId(i);
                            //判断是不是抢杠胡
                            int ctype = result.CType[i];
                            bool isQiangGangHu = ctype != 0 && (ctype & UtilDef.QiangGangHuType) != 0;

                            if (index++ == 0)
                            {
                                if (isQiangGangHu)
                                {
                                    var itemObj = MahjongManager.Instance.CreateCloneMajong(result.HuCard);
                                    var item = itemObj.GetComponent<MahjongItem>();
                                    Hu[duoHuChair].GetInMahjong(item);
                                }
                                else
                                {
                                    PaoParticle.transform.position = ThrowMj[paoChair].GetLastMjPos();
                                    Facade.Instance<MusicManager>().Play("shandian");
                                    PaoParticle.Stop();
                                    PaoParticle.Play();

                                    yield return new WaitForSeconds(1);

                                    ThrowMj[paoChair].PopMahjong();

                                    if (table.RoomInfo.IsExsitLaizi && result.HuCard == table.Laizi)
                                        Hu[duoHuChair].GetInMahjong(result.HuCard, true);
                                    else
                                        Hu[duoHuChair].GetInMahjong(result.HuCard);
                                }
                            }
                            else
                            {
                                GameObject clone = MahjongManager.Instance.CreateCloneMajong(result.HuCard);
                                var cloneMj = clone.GetComponent<MahjongItem>();
                                if (table.RoomInfo.IsExsitLaizi && result.HuCard == table.Laizi)
                                    cloneMj.IsSign = true;

                                Hu[duoHuChair].GetInMahjong(cloneMj);
                            }

                        }
                    }
                    else
                    {
                        //判断是不是抢杠胡
                        int ctype = result.CType[result.HuSeat[0]];
                        bool isQiangGangHu = ctype != 0 && (ctype & UtilDef.QiangGangHuType) != 0;

                        int huChair = table.GetChairId(result.HuSeat[0]);

                        if (isQiangGangHu)
                        {
                            var itemObj = MahjongManager.Instance.CreateCloneMajong(result.HuCard);
                            var item = itemObj.GetComponent<MahjongItem>();
                            Hu[huChair].GetInMahjong(item);
                        }
                        else
                        {
                            PaoParticle.transform.position = ThrowMj[paoChair].GetLastMjPos();
                            Facade.Instance<MusicManager>().Play("shandian");
                            PaoParticle.Stop();
                            PaoParticle.Play();

                            yield return new WaitForSeconds(1);

                            ThrowMj[paoChair].PopMahjong();

                            if (table.RoomInfo.IsExsitLaizi && result.HuCard == table.Laizi)
                                Hu[huChair].GetInMahjong(result.HuCard, true);
                            else
                                Hu[huChair].GetInMahjong(result.HuCard);
                        }

                    }
                }
                else if (huType == MjRequestData.MJReqTypeZiMo) //自摸
                {
                    YxDebug.Log("一局结束 胡牌" + result.HuSeat[0]);

                    SoundManager.Instance.PlayGameEffect(_currChair, EnGameSound.zimo);

                    int huChair = table.GetChairId(result.HuSeat[0]);
                    if (!result.ChBao && table.PlayerSeat == result.HuSeat[0])
                    {
                        HardMj[huChair].RemoveMahjong(result.HuCard);
                    }
                    if ((table.RoomInfo.IsExsitLaizi && result.HuCard == table.Laizi) || result.ChBao)
                        Hu[huChair].GetInMahjong(result.HuCard, true);
                    else
                        Hu[huChair].GetInMahjong(result.HuCard);
                }

                yield return new WaitForSeconds(GameConfig.PushCardInterval);

                foreach (int i in result.HuSeat)
                {
                    int duoHuChair = table.GetChairId(i);
                    HardMj[duoHuChair].GameResultRota(GameConfig.PushCardTime);
                }


                yield return new WaitForSeconds(GameConfig.PushCardInterval);

                for (int i = 0; i < HardMj.Length; i++)
                {
                    if (!result.HuSeat.Contains(i))
                    {

                        int chair = table.GetChairId(i);
                        HardMj[chair].GameResultRota(GameConfig.PushCardTime);
                    }
                }
            }

            if (result.ZhaNiao != null)
            {
                yield return MJMaResult(result.ZhaNiao, result.Zhongma);
            }

            float waitTime = result.ZhaNiao == null ? 1 : 4;
            yield return new WaitForSeconds(waitTime);
            yield return new WaitForSeconds(GameConfig.ShowResultWaitTime);
            //通知UI界面 出现小结算
            EventDispatch.Dispatch((int)UIEventId.Result, new EventData(table));


        }

        protected override void OnQueryHulist(int eventId, EventData evn)
        {
            QueryHulistData data = (QueryHulistData)evn.data1;
            if (null == data) return;
            if (data.Flag == 0)
                data.CardsNum = QueryResidueMahjong(data.Cards, data.Laizi);
        }

        protected override void OnOutPuCard(EventData evn)
        {
            base.OnOutPuCard(evn);

            EventDispatch.Dispatch((int)UIEventId.HideQueryHulistPnl);
        }

        protected override void OnRoomInfo(EventData evn)
        {
            base.OnRoomInfo(evn);
            var info = (RoomInfo)evn.data1;
            if (NoticeWindow)
            {
                NoticeWindow.gameObject.SetActive(true);
                NoticeWindow.enabled = true;
            }
        }
    }
}
