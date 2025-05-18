using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.jpmj.MahjongScripts.Helper;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class GameTable : MonoBehaviour
    {
        public Saizi Saizi;
        public DnxbCtl Dnxb;
        public GameTimer Timer;
        public OutCardIcon OutCardJianTou;
        public TextMesh TableInfo;

        public MahjongWall[] WallMj = new MahjongWall[UtilDef.GamePlayerCnt];
        public MahjongHard[] HardMj = new MahjongHard[UtilDef.GamePlayerCnt];
        public MahjongThrow[] ThrowMj = new MahjongThrow[UtilDef.GamePlayerCnt];
        public MahjongCpgGroup[] Cpgs = new MahjongCpgGroup[UtilDef.GamePlayerCnt];

        public Transform FanpaiBottom;//宝牌那个坑

        public MahjongGroup[] Hu = new MahjongGroup[UtilDef.GamePlayerCnt];

        public ParticleSystem PaoParticle;

        public NewTable NewTable;

        protected int _getMahjongChair;

        protected int _currChair;             //当前操作用户
        protected int _oldChair;              //上一次操作用户

        protected bool _isFirstGetInCard = true;     //如果第一次抓牌

        protected int _sendCardBeginChair;
        protected Coroutine _currCoroutine;

        public ParticleSystem LongJuanFeng;

        protected virtual bool PlayerToken
        {
            set { PlayerHand.HasToken = value; }
            get { return PlayerHand.HasToken; }
        }

        protected virtual MahjongPlayerHard PlayerHand
        {
            get { return (MahjongPlayerHard)HardMj[0]; }
        }

        void Awake()
        {
            OnRegistEvent();
            Init();
        }

        protected virtual void OnRegistEvent()
        {
            EventDispatch.Instance.RegisteEvent((int)GameEventId.GameStatus, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.SendCard, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.GetInCard, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.OutPuCard, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.Cpg, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.Hu, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.Ting, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.Banker, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.SaiziPoint, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.ChangeCurrUser, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.FlagMahjong, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.CleareFlagMahjong, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.WallMahjongFinish, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.Result, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.FanPai, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.RoomInfo, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.ChooseTing, OnChoosTing);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.ChooseTingCancel, OnChoosTingCancel);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.QueryHulist, OnQueryHulist);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.GetBao, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.RemoveOneWallCard, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.OnTrusteeship, OnTrusteeship);
        }

        void Start()
        {
            Saizi.ShowSaizi();
        }

        //事件 回调
        //--------------------------------------------------------
        protected virtual void OnRecvEvent(int eventId, EventData evn)
        {
            switch ((GameEventId)eventId)
            {
                case GameEventId.GameStatus:
                    OnGameStatus(evn);
                    break;
                case GameEventId.GetInCard:
                    OnGetInCard(evn);
                    break;
                case GameEventId.OutPuCard:
                    OnOutPuCard(evn);
                    break;
                case GameEventId.Cpg:
                    OnCpg(evn);
                    break;
                case GameEventId.Hu:
                    OnHu(evn);
                    break;
                case GameEventId.Ting:
                    OnTing(evn);
                    break;
                case GameEventId.Banker:
                    SetBanker(evn);
                    break;
                case GameEventId.SaiziPoint:
                    SetSaiziPoint(evn);
                    break;
                case GameEventId.ChangeCurrUser:
                    ChangeCurrUser(evn);
                    break;
                case GameEventId.FlagMahjong:
                    OnFlagMahjong(evn);
                    break;
                case GameEventId.CleareFlagMahjong:
                    OnCleareFlagMahjong(evn);
                    break;
                case GameEventId.WallMahjongFinish:
                    OnWallMahjongFinish(evn);
                    break;
                case GameEventId.SendCard:
                    AutoDeskAni(evn);
                    //OnSendCard(evn);
                    break;
                case GameEventId.Result:
                    OnGameResult(evn);
                    break;
                case GameEventId.FanPai:
                    OnFanPai(evn);
                    break;
                case GameEventId.RoomInfo:
                    OnRoomInfo(evn);
                    break;
                case GameEventId.GetBao:
                    GetBao(evn);
                    break;
                case GameEventId.RemoveOneWallCard:
                    RemoveOneWallCard(evn);
                    break;
                default:
                    OnGameEvent((GameEventId)eventId, evn);
                    break;
            }
        }

        protected void RemoveOneWallCard(EventData evn)
        {
            WallMj[_getMahjongChair].PopMahjong();
            IsRomove = (bool)evn.data1;
        }

        protected virtual void OnGameEvent(GameEventId id, EventData evn)
        {
            YxDebug.Log("game table 没有处理的事件 " + id);
        }

        protected virtual void OnGameStatus(EventData evn)
        {
            EnGameStatus status = (EnGameStatus)evn.data1;
            TableData data = (TableData)evn.data2;
            //根据游戏不同的状态来更新UI
            YxDebug.Log("游戏状态 " + status);
            switch (status)
            {
                case EnGameStatus.GameFree:
                    OnGameFree(data);
                    break;
                case EnGameStatus.GameReady:
                    break;
                case EnGameStatus.GameSendCard:
                    HideMahjongWall(data);
                    break;
                case EnGameStatus.GamePlay:
                    OnGamePlay(data);
                    break;
                case EnGameStatus.GameEnd:
                    break;
            }

            if (NewTable != null)
            {
                switch (status)
                {
                    case EnGameStatus.GameFree:
                        NewTable.ChangeWallState(false);
                        break;
                    case EnGameStatus.GameReady:
                        NewTable.ChangeWallState(false);
                        break;
                    case EnGameStatus.GameSendCard:
                        NewTable.ChangeWallState(false);
                        break;
                    case EnGameStatus.GamePlay:
                        NewTable.ChangeWallState(true);
                        break;
                    case EnGameStatus.GameEnd:
                        NewTable.ChangeWallState(false);
                        break;
                }
            }
        }

        //判断赖子是特殊牌 春夏秋冬 梅兰竹菊
        protected virtual bool CheckLaizi(int laizi, int card)
        {
            bool result = false;

            if (laizi == card) return result = true;

            //判断赖子是春夏秋冬
            if (laizi >= 96)
            {
                if ((card >= 96) && (card < 100))
                    result = true;

                if (card >= 100 && card < 104)
                    result = true;
            }

            return result;
        }

        protected virtual void OnGetInCard(EventData evn)
        {
            //第一次抓牌 隐藏塞子
            if (_isFirstGetInCard)
            {
                _isFirstGetInCard = false;
                Saizi.HideSaizi();
                Timer.StartTime(GameConfig.OutCardTime);
            }

            WallMj[_getMahjongChair].PopMahjong();
            int value = (int)evn.data1;

            //是否有赖子
            if (evn.data2 != null && CheckLaizi((int)evn.data2, value))
                HardMj[_currChair].GetInMahjong(value, true);
            else
                HardMj[_currChair].GetInMahjong(value);

            SoundManager.Instance.PlayCommonEffect(EnCommonSound.zhuapai);
        }

        protected virtual void OnOutPuCard(EventData evn)
        {
            int value = (int)evn.data1;
            int laizi = (int)evn.data2;
            var outItem=HardMj[_currChair].ThrowOut(value);
            if (outItem==null)
            {
                App.GetRServer<NetWorkManager>().DoSendRejoinGame();
                return;
            }
            ThrowMj[_currChair].GetInMahjong(value);
            MahjongItem item = ThrowMj[_currChair].GetLastMahjong();

            //如果打出的牌是赖子，显示赖子标记
            if (CheckLaizi(laizi, value))
            {
                item.IsSign = true;
            }

            OutCardJianTou.Show(item);

            SoundManager.Instance.PlayMj(_currChair, value);
            //本家 还处于 选着听的时候取消听牌点显示
            if (_currChair == 0 && PlayerHand.IsChooseTing)
            {
                PlayerHand.OnCancelTing();
            }
        }

        private bool IsRomove = true; //如果杠宝就不用再牌堆添牌了，此为标记
        protected virtual void HuanBaoCard(int value)
        {
            if (!IsRomove)
            {
                IsRomove = true;
                return;
            }
            WallMj[_getMahjongChair].PopMahjong();
            ThrowMj[_currChair].GetInMahjong(value);
            MahjongItem item = ThrowMj[_currChair].GetLastMahjong();
            item.IsSign = true;
        }

        protected virtual void OnCpg(EventData evn)
        {
            CpgData value = (CpgData)evn.data1;

            //抓杠是特殊的 如果放回的消息ok 为false 证明正在确认是否有抢杠胡 为ture时表示 杠成功了
            if (value.Type == EnGroupType.ZhuaGang)
            {
                CpgZhuaGang data = (CpgZhuaGang)value;
                if (!data.Ok)
                    PlayerToken = false;
                else
                {
                    //如果抓杠的是宝，则回收宝，不移除手牌
                    if (fanpaiMahjong != null && fanpaiMahjong.Value == value.Card)
                    {
                        MahjongManager.Instance.Recycle(fanpaiMahjong);
                        fanpaiMahjong = null;
                    }
                    else
                    {
                        //如果是抓杠 移除手牌中抓的
                        HardMj[_currChair].RemoveMahjong(value.Card);
                    }


                    //设置吃碰杠
                    Cpgs[_currChair].SetCpg(value);


                }
                PlayLongJuanFeng(_currChair);
                SoundManager.Instance.PlayGameEffect(_currChair, EnGameSound.gang);
                return;
            }

            //如果是别人打出的牌
            if (value.GetOutPutCard() != UtilDef.NullMj)
            {
                ThrowMj[_oldChair].PopMahjong();
                OutCardJianTou.Hide();
            }

            if (evn.data2 != null && (bool)evn.data2)
            {
                //暗杠宝专用，其他不走这，走else
                if (fanpaiMahjong != null)
                {
                    MahjongManager.Instance.Recycle(fanpaiMahjong);
                    fanpaiMahjong = null;
                }
                WallMj[_getMahjongChair].PopMahjong();
                IsRomove = false;
                List<int> AnGangBaoList = new List<int>();
                List<int> orgList = value.GetHardCards();
                for (int i = 0; i < value.GetHardCards().Count - 1; i++)
                {
                    AnGangBaoList.Add(orgList[i]);
                }
                HardMj[_currChair].RemoveMahjong(AnGangBaoList);
            }
            else
            {
                HardMj[_currChair].RemoveMahjong(value.GetHardCards());

                if (evn.data3 != null)
                {
                    //排序麻将
                    int playerSeat = (int)evn.data3;
                    if (_currChair == playerSeat)
                    {
                        PlayerHand.SortHandMahjong();
                    }
                }
            }
            Cpgs[_currChair].SetCpg(value);
            //如果吃碰杠之后 cpg 加 手牌数量 大于 手牌数量 需要打牌设置最后一张
            if (Cpgs[_currChair].GetHardMjCnt() + HardMj[_currChair].GetMjCnt() > GameConfig.UserHandCardCnt)
            {
                HardMj[_currChair].SetLastCardPos(UtilDef.NullMj);
            }

            //播放声音
            switch (value.Type)
            {
                case EnGroupType.Chi:
                    SoundManager.Instance.PlayGameEffect(_currChair, EnGameSound.chi);
                    break;
                case EnGroupType.Peng:
                    SoundManager.Instance.PlayGameEffect(_currChair, EnGameSound.peng);
                    break;
                default:
                    SoundManager.Instance.PlayGameEffect(_currChair, EnGameSound.gang);
                    PlayLongJuanFeng(_currChair);
                    break;
            }
        }


        Vector3[] LjfPostion = { new Vector3(0, 0, 2.5f), new Vector3(-2.5f, 0, 0), new Vector3(0, 0, -2.5f), new Vector3(2.5f, 0, 0) };
        protected virtual void PlayLongJuanFeng(int chair)
        {
            if (LongJuanFeng == null)
            {
                return;
            }
            LongJuanFeng.transform.position = LjfPostion[chair];
            LongJuanFeng.Stop();
            LongJuanFeng.Play();
            Facade.Instance<MusicManager>().Play("windy_rainy");
        }

        protected virtual void OnHu(EventData evn)
        {
        }

        protected virtual void OnChoosTing(int eventid, EventData data)
        {
            var tings = (int[])data.data1;
            PlayerHand.SetChooseTingPai(tings);
        }

        protected virtual void OnChoosTingCancel(int eventid, EventData data)
        {
            PlayerHand.OnCancelTing();
        }

        protected virtual void OnTing(EventData evn)
        {
            HardMj[_currChair].SetTingPai();
            if (bao != 0 && fanpaiMahjong == null)
            {
                bool anbao = false;
                if (evn.data1 != null)
                {
                    anbao = (bool)evn.data1;
                }
                StartCoroutine(FanPaiAnimationForBao(bao, !anbao));
            }
        }

        protected virtual void SetBanker(EventData evn)
        {
            //庄家的位子是 发牌对应的位子
            int banker = (int)evn.data1;
            _getMahjongChair = (banker + 2) % 4;
            _sendCardBeginChair = _getMahjongChair;
            Dnxb.SetSaiziDir(banker);
        }

        protected virtual void SetSaiziPoint(EventData evn)
        {
            int[] diceArr = (int[])evn.data1;
            Saizi.ShowSaizi();
            Saizi.Play((byte)diceArr[0], (byte)diceArr[1], GameConfig.SiziTime);

            int startIndex = GetWallStratIndex(diceArr);
            //设置麻将开始的位子
            WallMj[_getMahjongChair].StartIndex = startIndex;

            SoundManager.Instance.PlayCommonEffect(EnCommonSound.saizi);
        }

        protected virtual void ChangeCurrUser(EventData evn)
        {
            _currChair = (int)evn.data1;
            _oldChair = (int)evn.data2;

            PlayerToken = _currChair == 0;
            Dnxb.SetCurr(_currChair);
            Timer.StartTime(GameConfig.OutCardTime);
        }

        protected virtual void OnFlagMahjong(EventData evn)
        {
            int value = (int)evn.data1;
            foreach (MahjongThrow mahjongThrow in ThrowMj)
            {
                mahjongThrow.SignItemByValueGreen(value);
            }

            SoundManager.Instance.PlayCommonEffect(EnCommonSound.get);
        }

        protected virtual void OnCleareFlagMahjong(EventData evn)
        {
            foreach (MahjongThrow mahjongThrow in ThrowMj)
            {
                mahjongThrow.ReplyItem();
            }
        }

        protected virtual void OnWallMahjongFinish(EventData evn)
        {
            //当是空的时候 是正好完事开始在下一长城拿麻将
            _getMahjongChair = UtilFunc.GetPerChair(_getMahjongChair, WallMj.Length);
            //当有剩余时 在下一长城中继续出牌
            if (evn.data1 != null)
            {
                int leaveCnt = (int)evn.data1;
                YxDebug.Log("位子 " + _getMahjongChair + "继承移除麻将 " + leaveCnt);
                WallMj[_getMahjongChair].PopMahjong(leaveCnt);
            }
        }

        protected virtual void AutoDeskAni(EventData evn)
        {
            if (NewTable != null)
            {
                float waitTime = GameConfig.NewTableAniWaitTime;
                EventDispatch.Dispatch((int)NetEventId.PauseResponse, new EventData(waitTime));
                NewTable.AniStart(OnSendCard, evn);
            }
            else
            {
                OnSendCard(evn);
            }

        }

        protected virtual void OnSendCard(EventData evn)
        {
            var cardData = (OnSendCardEventData)evn.data1;
            if (GameConfig.UserHandCardCnt != cardData.playermj.Length)
            {
                YxDebug.LogError("服务器 发送的 手牌个数与 game config 不同");
                return;
            }
            if (_currCoroutine!=null)
            {
                StopCoroutine(_currCoroutine);
            }
            _currCoroutine = StartCoroutine(SendCardCoroutine(cardData));

            float time = 0;
            if (GameConfig.IsNeedSendCardAnimation)
            {
                //发牌时间
                time += (GameConfig.SendCardInterval + 0.1f) * 4 * UtilData.CurrGamePalyerCnt;
                time += GameConfig.SendCardUpTime + GameConfig.SendCardUpWait;
            }
            //翻牌
            if (cardData.fanpai != UtilDef.NullMj)
            {
                time += GameConfig.CameraMoveTime * 2 + 1;
            }
            //扣牌时间
            time += GameConfig.SendCardUpTime * 3;
            time += 0.2f;
            EventDispatch.Dispatch((int)NetEventId.PauseResponse, new EventData(time));
        }

        protected virtual IEnumerator SendCardCoroutine(OnSendCardEventData cardData)
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

            if (TrusteeshipHelper.Instance != null)
            {
                TrusteeshipHelper.Instance.IsAllowTrusteeship = true;
            }
        }

        protected virtual IEnumerator SendCardAnimation(OnSendCardEventData cardData)
        {
            YxDebug.Log("sendCardTime " + Time.time);
            int cont = GameConfig.UserHandCardCnt * UtilData.CurrGamePalyerCnt;
            Debug.LogError("GameConfig.UserHandCardCnt:" + GameConfig.UserHandCardCnt);
            Debug.LogError("UtilData.CurrGamePalyerCnt:" + UtilData.CurrGamePalyerCnt);
            Debug.LogError("cont:" + cont);
            int startChair = cardData.currChair;

            if (!GameConfig.IsNeedSendCardAnimation)
            {
                int startIndex = GetWallStratIndex(cardData.SaiziPoint);
                WallMj[_getMahjongChair].StartIndex = startIndex;

                //不需要发牌动画时 直接设置手牌
                WallMj[_getMahjongChair].PopMahjong(cont);

                int[] nomalCards = new int[GameConfig.UserHandCardCnt];

                for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
                {
                    if (startChair == 0)
                    {
                        HardMj[startChair].GetInMahjong(cardData.playermj);
                    }
                    else
                        HardMj[startChair].GetInMahjong(nomalCards);

                    startChair = UtilFunc.GetNextChair(startChair, UtilData.CurrGamePalyerCnt);
                }
                cardData.reduceMjCnt(cont);
                yield return 0;
            }
            else
            {

                int meOffset = 0;
                int jj = 1;
                while (cont > 0)
                {
                    Debug.LogError("cont while :" + cont);
                    int getInCnt = 4;
                    if (cont <= 4)
                        getInCnt = 1;

                    int[] card = new int[getInCnt];

                    WallMj[_getMahjongChair].PopMahjong(getInCnt);
                    if (startChair == 0)
                    {
                        Facade.Instance<MusicManager>().Play("zhuapai");
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
                    HardMj[startChair].OnSendMahjong(card, GameConfig.SendCardUpTime, GameConfig.SendCardUpWait);

                    cardData.reduceMjCnt(getInCnt);

                    YxDebug.Log("send card time " + jj++);
                    Debug.LogError("send card time " + jj++);
                    yield return new WaitForSeconds(GameConfig.SendCardInterval);

                    cont -= getInCnt;
                    startChair = UtilFunc.GetNextChair(startChair, UtilData.CurrGamePalyerCnt);
                }

                yield return new WaitForSeconds(GameConfig.SendCardUpTime + GameConfig.SendCardUpWait);
            }
            Debug.LogError("循環結束");
            YxDebug.Log("sendCardTime " + Time.time);
        }

        protected virtual IEnumerator FanPaiAnimation(OnSendCardEventData data)
        {
            //获得牌
            MahjongItem fanPaiItem = WallMj[_getMahjongChair].GetNowFristMj();
            //转动摄像机
            CameraAni.Self.PlayFar2Near(fanPaiItem.transform, GameConfig.CameraMoveTime);
            yield return new WaitForSeconds(GameConfig.CameraMoveTime);
            //为牌赋值
            MahjongManager.Instance.ExchangeByValue(data.fanpai, fanPaiItem);
            //翻牌
            fanPaiItem.RotaTo(new Vector3(-180, 0, 0), 0.2f);
            yield return new WaitForSeconds(1);
            //播放特效

            //转回摄像机
            CameraAni.Self.PlayN2F(GameConfig.CameraMoveTime);
            yield return new WaitForSeconds(GameConfig.CameraMoveTime);
            //打牌
            WallMj[_getMahjongChair].PopMahjong();
            //减少牌个数显示
            data.reduceMjCnt(1);
            //通知UI层 显示翻牌动画
            EventDispatch.Dispatch((int)UIEventId.FanPai, new EventData(data.fanpai));
        }

        /// <summary>
        /// 一局结算回放
        /// </summary>
        private Coroutine _oneResultCoroutine;
        protected virtual void OnGameResult(EventData evn)
        {
            EventDispatch.Dispatch((int)UIEventId.HideQueryHulistPnl);
            EventDispatch.Dispatch((int)UIEventId.DisableTrusteeship);

            if (TrusteeshipHelper.Instance != null)
            {
                TrusteeshipHelper.Instance.IsAllowTrusteeship = false;
            }

            TableData table = (TableData)evn.data1;
            if (_oneResultCoroutine!=null)
            {
                StopCoroutine(_oneResultCoroutine);
            }
            _oneResultCoroutine = StartCoroutine(GameResult(table));
        }

        protected virtual IEnumerator GameResult(TableData table)
        {
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
                    if (!result.ChBao)
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
            gameObject.SetActive(false);
        }

        //扎鸟结果动画
        protected virtual IEnumerator MJMaResult(int[] MJArr, int[] zhongma)
        {
            int index = MJArr.Length;
            MahjongItem item = null;
            List<MahjongItem> list = new List<MahjongItem>();

            GameObject temp;
            Transform startPos;

            MahjongItem lastItem = WallMj[_sendCardBeginChair].GetLastMj();
            for (int i = 1; i < UtilDef.GamePlayerCnt; i++)
            {
                if (lastItem == null)
                {
                    lastItem = WallMj[(_sendCardBeginChair + i) % 4].GetLastMj();
                }
                else
                {
                    break;
                }
                if (i == UtilDef.GamePlayerCnt - 1 && lastItem == null)
                {
                    yield break;
                }
            }
            startPos = lastItem.transform;

            for (int i = 0; i < index; i++)
            {
                temp = MahjongManager.Instance.CreateCloneMajong(MJArr[i]);
                if (temp == null) continue;

                item = temp.GetComponent<MahjongItem>();
                temp.transform.position = startPos != null ? startPos.position : Vector3.zero;

                iTween.RotateTo(item.gameObject, new Vector3(75.6f, 180, 0), 1.5f);
                if (index == 1)
                {
                    iTween.MoveTo(item.gameObject, new Vector3(0, 4, 2.7f + (i % 2) * 0.6f), 1.5f);
                }
                else
                {
                    iTween.MoveTo(item.gameObject,
                                  new Vector3(-Mathf.Floor(i / 2) * 0.5f + (index / 4) * 0.5f - 0.25f, 4, 2.7f + (i % 2) * 0.6f),
                                  1.5f);
                }
                yield return new WaitForSecondsRealtime(0.15f);
                list.Add(item);
            }

            yield return new WaitForSecondsRealtime(0.5f);

            for (int i = 0; i < list.Count; i++)
            {
                if (null != zhongma && zhongma.Length > 0)
                {
                    if (!Array.Exists(zhongma, (a) => { return a == list[i].Value; }))
                    {
                        list[i].Mesh.GetComponent<MeshRenderer>().material = MahjongManager.Instance.MahjongMaterialGay;
                    }
                }
                else
                    list[i].Mesh.GetComponent<MeshRenderer>().material = MahjongManager.Instance.MahjongMaterialGay;
            }

        }

        #region
        protected void OnFanPai(EventData evn)
        {
            int fanpai = (int)evn.data1;
            int laizi = (int)evn.data2;
            int[] playerHardCard = (int[])evn.data3;

            //翻牌动画之前的是 发牌 如果翻牌没有翻完 停止翻牌动画
            //为手牌赋值 
            if (_currCoroutine != null)
            {
                StopCoroutine(_currCoroutine);

                MahjongManager.Instance.Reset();

                foreach (MahjongHard hard in HardMj)
                {
                    hard.Reset();
                }

                foreach (MahjongWall wall in WallMj)
                {
                    wall.Reset();
                    wall.GetInMahjong(0);
                }
                _getMahjongChair = _sendCardBeginChair;
                int cont = GameConfig.UserHandCardCnt * UtilDef.GamePlayerCnt;
                WallMj[_getMahjongChair].PopMahjong(cont);
                //重新设置手牌
                int[] nomalHardCard = new int[GameConfig.UserHandCardCnt];
                for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
                {
                    if (i == 0)
                    {
                        HardMj[i].GetInMahjong(playerHardCard);
                        continue;
                    }
                    HardMj[i].GetInMahjong(nomalHardCard);
                }
            }

        }
        #endregion//占时无用的函数

        protected virtual void OnRoomInfo(EventData evn)
        {
            var info = (RoomInfo)evn.data1;
            //设置桌面信息
            string tableInfo = "房间ID:" + info.RoomID;
            tableInfo += "\r\n底分:" + info.Ante;
            tableInfo += "\r\n" + info.GetRoomRuleString();
            TableInfo.text = tableInfo;
            //设置牌墙中牌的个数
            int mjCnt = info.SysMahjongCnt / WallMj.Length;

            int mjTpye;//麻将摆的类型 0 每个墙是一样的 1 上下两家多出一摞
            if (mjCnt % 2 == 0)
                mjTpye = 0;
            else
            {
                mjTpye = 1;
                mjCnt -= 1;
            }

            for (int i = 0; i < WallMj.Length; i++)
            {
                if (mjTpye == 0)
                    WallMj[i].SetRowCnt(mjCnt / 2);
                else if (mjTpye == 1)
                    WallMj[i].SetRowCnt(i % 2 == 0 ? mjCnt / 2 + 1 : mjCnt / 2);
            }
        }

        //--------------------------------------------------------
        protected virtual void OnGameFree(TableData data)
        {
            gameObject.SetActive(true);
            MahjongManager.Instance.Reset();

            foreach (MahjongWall mahjongWall in WallMj)
            {
                mahjongWall.Reset();
                mahjongWall.GetInMahjong(0);
                mahjongWall.StartIndex = 0;
            }

            foreach (MahjongHard mahjongHard in HardMj)
            {
                mahjongHard.Reset();
            }

            foreach (MahjongThrow mahjongThrow in ThrowMj)
            {
                mahjongThrow.Reset();
            }

            foreach (MahjongCpgGroup mahjongCpg in Cpgs)
            {
                mahjongCpg.Reset();
            }

            foreach (MahjongGroup mahjongHu in Hu)
            {
                mahjongHu.Reset();
            }

            if (PlayerPrefs.HasKey("MjColor"))
            {
                int index = PlayerPrefs.GetInt("MjColor");
                MahjongManager.Instance.ChangeMjClothes(index);
            }

            Saizi.ShowSaizi();

            Dnxb.SetPlayerDnxb((EnDnxbDir)data.PlayerSeat);

            Timer.Reset();

            OutCardJianTou.Hide();

            Dnxb.Reset();

            _isFirstGetInCard = true;
            if (FanpaiBottom != null)
            {
                FanpaiBottom.gameObject.SetActive(false);
                Transform AnBaoEffect = FanpaiBottom.transform.FindChild("changchun-anbaowenzi");
                if (AnBaoEffect != null)
                {
                    AnBaoEffect.gameObject.SetActive(false);
                }
            }
            bao = 0;
            fanpaiMahjong = null;
        }

        protected virtual void OnGamePlay(TableData data)
        {
            //如果是重新链接 设置桌面信息
            if (data.IsReconect)
            {
                OnGameFree(data);
                HideMahjongWall(data);
                _currChair = data.CurrChair;

                _getMahjongChair = (data.GetChairId(data.BankerSeat) + 2) % 4;
                _sendCardBeginChair = _getMahjongChair;

                int wallOutCnt = data.RoomInfo.SysMahjongCnt - data.LeaveMahjongCnt;

                Saizi.HideSaizi();

                //设置麻将开始的位子
                int startIndex = GetWallStratIndex(data.SaiziPoint);
                WallMj[_getMahjongChair].StartIndex = startIndex;

                WallMj[_getMahjongChair].PopMahjong(wallOutCnt);

                for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
                {
                    //设置出牌
                    int Chair = data.GetChairId(i);
                    ThrowMj[Chair].GetInMahjong(data.UserOutCard[i].ToArray());
                    //设置手牌
                    if (data.UserHardCard[i].Count == 0)
                    {
                        int[] valuse = new int[data.UserHardCardNum[i]];
                        for (int j = 0; j < valuse.Length; j++)
                        {
                            valuse[j] = 0;
                        }

                        HardMj[Chair].GetInMahjong(valuse);
                    }
                    else
                    {
                        HardMj[Chair].GetInMahjong(data.UserHardCard[i].ToArray());
                    }
                    //设置吃碰杠牌
                    Cpgs[Chair].SetCpgArray(data.UserCpg[i].ToArray());
                    //听牌
                    if (data.IsTing[i])
                    {
                        HardMj[Chair].SetTingPai();
                    }

                }
                //设置手牌中的赖子
                if (data.Laizi != UtilDef.NullMj)
                {
                    //不排序
                    PlayerHand.SetLaizi(data.Laizi);
                }

                //设置骰子
                Dnxb.SetCurr(_currChair);
                //设置倒计时
                Timer.StartTime(data.ReconnectTime);
                //根据状态 来显示最后的牌
                if (data.ReconectStatus == 0)//需要打牌
                {
                    if (data.CurrChair == 0)
                    {
                        PlayerToken = true;
                    }
                    //设置最后麻将的位子
                    HardMj[_currChair].SetLastCardPos(data.GetInMahjong);
                    HardMj[_currChair].SetTingPaiNeedOutCard();
                }
                else if (data.ReconectStatus == 1)//牌已出 等待其他玩家响应
                {
                    PlayerToken = false;
                }


                //设置上次出牌用户的箭头标记
                if (data.OldCurrSeat != UtilDef.NullSeat)
                {
                    MahjongItem OutPutMj = ThrowMj[data.OldCurrChair].GetLastMahjong();
                    if (OutPutMj != null)
                        OutCardJianTou.Show(OutPutMj);
                }

                if (TrusteeshipHelper.Instance != null)
                {
                    TrusteeshipHelper.Instance.OnDisableTrusteeshipClick();
                }
            }
            else
            {
                Saizi.HideSaizi();
            }

            if (data.CurrSeat == data.PlayerSeat)
            {
                if (null != data.Tinglist && data.UsingQueryHu)
                    EventDispatch.Dispatch((int)GameEventId.TingList, new EventData(data.Tinglist));
            }
            if (data.bao != UtilDef.NullMj)
            {
                EventDispatch.Dispatch((int)NetEventId.ReJionShowBao);
            }
        }

        protected virtual int GetWallStratIndex(int[] saiziPoint)
        {
            int point = saiziPoint.Sum() * 2;
            while (point >= WallMj[_getMahjongChair].GetMahjongList.Count)
            {
                point -= WallMj[_getMahjongChair].GetMahjongList.Count;
                _getMahjongChair = (_getMahjongChair + 1) % UtilDef.GamePlayerCnt;
            }
            return point;
        }

        protected int bao;
        protected virtual void GetBao(EventData evn)
        {
            bool isTing = (bool)evn.data3;
            bao = (int)evn.data1;
            bool anbao = false;
            if (evn.data4 != null)
            {
                anbao = (bool)evn.data4;
            }
            if (isTing)
            {
                if (fanpaiMahjong == null)
                {
                    if (bao != 0)
                    {
                        StartCoroutine(FanPaiAnimationForBao(bao, !anbao));
                    }
                }
                else
                {
                    MahjongManager.Instance.ExchangeByValue(bao, fanpaiMahjong);
                }
            }
            if (evn.data2 != null)
            {
                int lastLaizi = (int)evn.data2;
                if (lastLaizi == 0)
                {
                    return;
                }
                if (lastLaizi != 0)
                {
                    EventDispatch.Dispatch((int)UIEventId.ShowHuanBaoEffect);
                    HuanBaoCard(lastLaizi);
                }
            }


        }
        protected MahjongItem fanpaiMahjong;
        private bool _wait;
        protected virtual IEnumerator FanPaiAnimationForBao(int fanpai, bool liangBao = true)
        {
            if (_wait)
            {
                yield return new WaitForSeconds(1);
                _wait = false;
            }
            SetBaoMahjongPos(fanpai, liangBao);

            //转动摄像机
            CameraAni.Self.PlayFar2Near(fanpaiMahjong.transform, GameConfig.CameraMoveTime);
            yield return new WaitForSeconds(GameConfig.CameraMoveTime);
            yield return new WaitForSeconds(1);
            //播放特效

            //转回摄像机
            CameraAni.Self.PlayN2F(GameConfig.CameraMoveTime);
            yield return new WaitForSeconds(GameConfig.CameraMoveTime);
        }

        protected virtual void SetBaoMahjongPos(int value, bool LiangBao)
        {
            FanpaiBottom.gameObject.SetActive(true);
            FanpaiBottom.GetComponent<ParticleSystem>().Play();
            WallMj[_getMahjongChair].PopMahjong();
            Vector3 targetPos = new Vector3(
                    FanpaiBottom.localPosition.x,
                    FanpaiBottom.localPosition.y + 0.1f,
                    FanpaiBottom.localPosition.z
                    );


            fanpaiMahjong = MahjongManager.Instance.GetMahjong(value);
            if (value != 0)
            {
                MahjongManager.Instance.ExchangeByValue(value, fanpaiMahjong);
            }

            if (null == fanpaiMahjong) return;

            fanpaiMahjong.IsSign = true;
            fanpaiMahjong.transform.parent = transform;
            fanpaiMahjong.transform.localPosition = targetPos;
            float x = 90;
            if (!LiangBao)
            {
                x = -90;
                Transform AnBaoEffect = FanpaiBottom.transform.FindChild("changchun-anbaowenzi");
                if (AnBaoEffect)
                {
                    AnBaoEffect.gameObject.SetActive(true);
                }
            }
            fanpaiMahjong.transform.localRotation = Quaternion.Euler(new Vector3(x, 180, 0));
            fanpaiMahjong.transform.localScale = Vector3.one;
            fanpaiMahjong.gameObject.SetActive(true);
        }



        /// <summary>
        /// 隐藏牌墙
        /// </summary>
        /// <param name="data"></param>
        protected virtual void HideMahjongWall(TableData data)
        {

        }

        protected virtual void OnQueryHulist(int eventId, EventData evn)
        {
            QueryHulistData data = (QueryHulistData)evn.data1;
            if (null == data) return;

            //自己手牌
            List<MahjongItem> handlist = HardMj[0].GetMahjongList;
            MahjongItem mah = handlist.Find((a) => { return a.Value == data.CardValue && a.MahjongIndex == data.CardIndex; });

            if (data.Flag == 0)
                data.CardsNum = QueryResidueMahjong(data.Cards, data.Laizi);

            if (null == mah) return;
            EventDispatch.Dispatch((int)UIEventId.ShowQueryHulistPnl, new EventData(data, mah.transform));
        }

        protected virtual List<int> QueryResidueMahjong(List<int> arr, int laizi)
        {
            List<int> list = new List<int>();
            int num = 0;

            //查询手牌
            for (int i = 0; i < arr.Count; i++)
            {
                //查询手牌
                var list1 = HardMj[0].GetMahjongList.FindAll((a) => { return a.Value == arr[i]; });
                if (null != list1)
                    num += list1.Count;

                //查询出牌
                for (int j = 0; j < ThrowMj.Length; j++)
                {
                    var temp = ThrowMj[j].GetMahjongList.FindAll((a) => { return a.Value == arr[i]; });
                    if (null != temp)
                        num += temp.Count;
                }

                //查询cpg 
                for (int j = 0; j < Cpgs.Length; j++)
                {
                    for (int z = 0; z < Cpgs[j].CpgList.Count; z++)
                    {
                        var temp = Cpgs[j].CpgList[z].MahjongList.FindAll((a) => { return a.Value == arr[i]; });
                        if (null != temp)
                            num += temp.Count;
                    }
                }

                int value = 0;

                //赖子牌是花牌
                if (arr[i] < 96)
                    value = laizi.Equals(arr[i]) ? 3 - num : 4 - num;
                else
                    value = 1 - num;

                list.Add(value >= 0 ? value : 0);
                num = 0;
            }
            return list;
        }

        //设置翻牌位置
        protected virtual void SetFpMahjongPos(int fanpai, int laizi = -1)
        {
            if (fanpai == 0) return;
            FanpaiBottom.gameObject.SetActive(true);
            FanpaiBottom.GetComponent<ParticleSystem>().Play();

            Vector3 targetPos = new Vector3(
                    FanpaiBottom.localPosition.x,
                    FanpaiBottom.localPosition.y + 0.1f,
                    FanpaiBottom.localPosition.z
                    );

            WallMj[_getMahjongChair].PopMahjong();
            fanpaiMahjong = MahjongManager.Instance.GetMahjong(fanpai);
            MahjongManager.Instance.ExchangeByValue(fanpai, fanpaiMahjong);
            if (null == fanpaiMahjong) return;

            if (fanpai == laizi)
                fanpaiMahjong.IsSign = true;

            fanpaiMahjong.transform.parent = transform;
            fanpaiMahjong.transform.localPosition = targetPos;
            fanpaiMahjong.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
            fanpaiMahjong.transform.localScale = Vector3.one;
            fanpaiMahjong.gameObject.SetActive(true);
        }

        public virtual void OnTrusteeship(int eventid, EventData data)
        {
            MahjongPlayerHard MjPlayer = (MahjongPlayerHard)HardMj[0];
            MjPlayer.Trusteeship(TrusteeshipHelper.Instance.IsTrusteeship);

            if (TrusteeshipHelper.Instance != null)
            {
                if (TrusteeshipHelper.Instance.IsHaveOperator()) return;

                if (TrusteeshipHelper.Instance.IsTrusteeship && MjPlayer.GetMahjongList.Count != 0)
                {
                    EventDispatch.Dispatch((int)TableDataEventId.OnTrusteeshipClick, new EventData(MjPlayer.GetMahjongList[MjPlayer.GetMahjongList.Count - 1].Value, (bool)data.data1));
                }
            }
        }

        protected virtual void Init() { }

    }
}
