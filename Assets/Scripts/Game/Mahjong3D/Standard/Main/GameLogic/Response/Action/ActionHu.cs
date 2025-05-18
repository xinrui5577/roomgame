using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionHu : AbsCommandAction
    {
        protected SingleResultArgs.HuResultType mResultType = SingleResultArgs.HuResultType.HuEndu;
        protected SingleResultArgs mArgs;
        protected ContinueTaskContainer mDianpaoTask;
        protected ContinueTaskContainer mLastCdTask;
        protected ContinueTaskContainer mZimoTask;
        protected ContinueTaskContainer mHuTask;

        protected Func<int, string> HuMusicFunc;

        public void OnHu(ISFSObject data)
        {
            //重置玩家准备状态
            DataCenter.Players.ResetReadyState();
            GameCenter.Controller.SingleHuState = true;
            GameCenter.Instance.SetIgonreReconnectState(true);
            if (HuMusicFunc == null)
            {
                HuMusicFunc = (ctype) =>
                {
                    var target = "";
                    var config = DataCenter.Config;
                    if (config.SpecialHuTypes.Length <= 0) return target;
                    for (int i = 0; i < config.SpecialHuTypes.Length; i++)
                    {
                        var temp = config.SpecialHuTypes[i];
                        if (GameUtils.BinaryCheck(temp.HuTypeValue, ctype))
                        {
                            if (config.IsPlayHunHeHuSound && !temp.IsOnly)
                            {
                                target += temp.HuTypeName;
                            }
                            else
                            {
                                return temp.HuTypeName;
                            }
                        }
                    }
                    return "";
                };
            }
            ParseData(data);
            SetPlayersHandCards();
            Game.TableManager.StartTimer(0);
            if (GameCenter.Shortcuts.CheckState(GameSwitchType.AiAgency))
            {
                //关闭托管
                GameCenter.EventHandle.Dispatch((int)EventKeys.AiAgency, new AiAgencyArgs() { State = false });
            }
            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            //隐藏提示标记
            Game.TableManager.GetParts<MahjongOutCardFlag>(TablePartsType.OutCardFlag).Hide();
            //关闭查听
            GameCenter.EventHandle.Dispatch((int)EventKeys.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }

        public virtual void GameResultAction(ISFSObject data) { }

        /// <summary>
        /// 流局
        /// </summary>     
        public virtual void LastCdAction(ISFSObject data)
        {
            OnHu(data);
            if (null == mLastCdTask)
            {
                mLastCdTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => LiujuTask())
                .AppendFuncTask(() => ShowBaoTipTask())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mLastCdTask.Start();
        }

        public virtual void ZimoAction(ISFSObject data)
        {
            OnHu(data);
            if (null == mZimoTask)
            {
                mZimoTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => ZimoTask())
                .AppendFuncTask(() => HandcardCtrlTask())
                .AppendFuncTask(() => ShowBaoTipTask())
                .AppendFuncTask(() => ZhaNiaoAnimation())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mZimoTask.Start();
        }

        public virtual void DianpaoAction(ISFSObject data)
        {
            OnHu(data);
            if (null == mDianpaoTask)
            {
                mDianpaoTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => YipaoDuoxiangTask())
                .AppendFuncTask(() => DianpaoTask())
                .AppendFuncTask(() => HandcardCtrlTask())
                .AppendFuncTask(() => ShowBaoTipTask())
                .AppendFuncTask(() => ZhaNiaoAnimation())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mDianpaoTask.Start();
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        protected virtual void ParseData(ISFSObject data)
        {
            MahjongResult result;
            var huSeats = new List<int>();
            var mahjongResults = new List<MahjongResult>();
            var playersData = data.GetSFSArray(RequestKey.KeyPlayerList);

            var players = DataCenter.Players;
            for (int i = 0; i < playersData.Count; i++)
            {
                result = new MahjongResult(i);
                var player = playersData.GetSFSObject(i);
                if (player.ContainsKey(RequestKey.KeyCardType))
                {
                    int type = player.GetInt(RequestKey.KeyType);
                    if (type >= 1)
                    {
                        result.HuFlag = true;
                        result.HuSeat = i;
                        huSeats.Add(i);
                    }
                    result.UserHuType = type;
                }
                result.PuGlod = player.GetInt("pu");
                result.CType = player.TryGetInt("ctype");
                result.HuCard = player.TryGetInt("hucard");
                result.TotalGold = player.GetLong("ttgold");
                result.HuInfo = player.TryGetString("hname");
                result.Gold = player.TryGetInt(RequestKey.KeyGold);
                result.NiaoGold = player.GetInt(AnalysisKeys.KeyNiao);
                result.PiaoGlod = player.GetInt(AnalysisKeys.KeyPiao);
                result.GangGlod = player.GetInt(AnalysisKeys.KeyGGang);
                result.HuGold = player.GetInt(AnalysisKeys.KeyGHu) + player.TryGetInt(AnalysisKeys.KeyGHua);
                mahjongResults.Add(result);
                //添加cpg数据
                ISFSArray Groups = player.TryGetSFSArray(AnalysisKeys.KeyMjGroup);
                if (Groups != null)
                {
                    var playerData = players[result.Chair];
                    playerData.ClearCpgData();
                    for (int j = 0; j < Groups.Count; j++)
                    {
                        var cpg = MahjongUtility.CreateCpg(Groups.GetSFSObject(j));
                        playerData.AddCpgData(cpg);
                    }
                }
                //获取胡牌明细信息
                result.SetDeatil(player);
            }
            //更新手牌
            var cards = data.GetSFSArray(RequestKey.KeyCardsArr);
            for (int i = 0; i < cards.Count; i++)
            {
                var chair = MahjongUtility.GetChair(i);
                players[chair].HardCards = new List<int>(cards.GetIntArray(i));
                GameUtils.SortMahjong(players[chair].HardCards);
            }
            mArgs = new SingleResultArgs()
            {
                HuSeats = huSeats,
                Result = mahjongResults,
                Bao = data.TryGetInt("bao"),
                HuCard = data.TryGetInt("huCard"),
                HuType = data.TryGetInt(RequestKey.KeyType),
                PiaoHu = data.ContainsKey(AnalysisKeys.KeyPiaoHu),
                ZhongMa = data.TryGetIntArray(AnalysisKeys.KeyZhongma),
                ZhaMa = data.TryGetIntArray(AnalysisKeys.KeyZhaNiao),
                ChBao = data.TryGetInt(AnalysisKeys.KeyChBao) == 1,
                MoBao = data.TryGetInt(AnalysisKeys.KeyMoBao) == 1,
            };
            //解析胡牌顺序
            if (data.ContainsKey("hushunxu"))
            {
                ISFSArray array = data.GetSFSArray("hushunxu");
                if (array != null)
                {
                    var huSort = new Dictionary<int, int>();
                    for (int i = 0; i < array.Count; i++)
                    {
                        var sfs = array.GetIntArray(i);
                        for (int j = 0; j < sfs.Length; j++)
                        {
                            var chair = MahjongUtility.GetChair(sfs[j]);
                            if (!huSort.ContainsKey(chair))
                            {
                                huSort.Add(chair, i);
                            }
                        }
                    }
                    mArgs.HuSort = huSort;
                }
            }
            mArgs.ResultType = mResultType;
            GameCenter.DataCenter.Game.BaoCard = mArgs.Bao;
            DataCenter.Room.NextBaner = data.TryGetInt(AnalysisKeys.KeyNextBanker);
            GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(null);
            DataCenter.Game.Laozhuang = data.TryGetInt("lzcnt");
        }

        /// <summary>
        /// 为所有玩家设置手牌
        /// </summary>
        protected void SetPlayersHandCards()
        {
            List<int> array;
            MahjongHand handCards;
            var huChair = -1;
            if (mArgs.HuSeats.Count > 0)
            {
                huChair = MahjongUtility.GetChair(mArgs.HuSeats[0]);
            }
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                handCards = Game.MahjongGroups.MahjongHandWall[i];
                handCards.RemoveAllMj();
                if (DataCenter.Game.FenzhangFlag)
                {
                    if (mArgs.HuType == NetworkProls.ZiMo && huChair == i)
                    {
                        //移除自摸牌
                        DataCenter.Players[i].HardCards.Remove(mArgs.HuCard);
                    }
                    array = new List<int>(DataCenter.Players[i].HardCards);
                    //移除分张牌               
                    array.Remove(DataCenter.Players[i].FenzhangCard);
                }
                else
                {
                    if (mArgs.HuType == NetworkProls.ZiMo && huChair == i)
                    {
                        //移除自摸牌
                        DataCenter.Players[i].HardCards.Remove(mArgs.HuCard);
                        array = new List<int>(DataCenter.Players[i].HardCards);
                    }
                    else
                    {
                        array = DataCenter.Players[i].HardCards;
                    }
                }
                handCards.GetInMahjong(array);
                handCards.SetLaizi(DataCenter.Game.LaiziCard);
            }
        }

        protected virtual IEnumerator<float> LiujuTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            var group = Game.MahjongGroups;
            for (int i = 0; i < group.MahjongHandWall.Count; i++)
            {
                group.MahjongHandWall[i].GameResultRota(Config.TimeHuAniInterval);
            }
            yield return Config.TimeHuAniInterval;
        }

        protected virtual IEnumerator<float> ZimoTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            yield return Config.TimeHuAniInterval;
            var huCard = mArgs.HuCard;
            var huChair = MahjongUtility.GetChair(mArgs.HuSeats[0]);
            string huType = "";
            if (DataCenter.Config.PlaySpecialHuSound)
            {
                huType = IsSpecialHu(mArgs.Result[mArgs.HuSeats[0]].CType);
            }
            if (!string.IsNullOrEmpty(huType))
            {
                MahjongUtility.PlayOperateSound(huChair, huType);
                GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(huChair, PoolObjectType.zimo);
            }
            else if (mArgs.PiaoHu)
            {
                MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.piaohu);
            }
            else if (mArgs.MoBao)
            {
                //播放摸宝特效
                MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.mobao);
            }
            else if (mArgs.ChBao)
            {
                //播放冲宝特效
                MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.chongbao);
            }
            else
            {
                MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.zimo);
            }
            //设置胡牌，分张除外
            if (!DataCenter.Game.FenzhangFlag)
            {
                SetHuCard(huChair, huCard);
            }
        }

        /// <summary>
        /// 一炮多响
        /// </summary> 
        protected virtual IEnumerator<float> YipaoDuoxiangTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            if (mArgs.HuType == NetworkProls.Hu && mArgs.HuSeats.Count > 1)
            {
                yield return Config.TimeHuAniInterval;
                var index = 0;
                var seats = mArgs.HuSeats;
                var paoChair = GameCenter.DataCenter.CurrOpChair;
                for (int i = 0; i < seats.Count; i++)
                {
                    var duoHuChair = MahjongUtility.GetChair(seats[i]);
                    //判断是不是抢杠胡
                    var ctype = mArgs.Result[seats[0]].CType;
                    bool isQiangGangHu = ctype != 0 && (ctype & NetworkProls.QiangGangHuType) != 0;
                    if (index++ == 0)
                    {
                        if (isQiangGangHu)
                        {
                            var item = Game.MahjongCtrl.PopMahjong(mArgs.HuCard);
                            Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(item);
                        }
                        else
                        {
                            var effect = MahjongUtility.PlayMahjongEffectAndAudio(PoolObjectType.shandian);
                            effect.transform.position = Game.MahjongGroups.MahjongThrow[paoChair].GetLastMjPos();
                            effect.Execute();
                            yield return 0.5f;
                            Game.MahjongGroups.MahjongThrow[paoChair].PopMahjong();
                            Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(mArgs.HuCard);
                        }
                    }
                    else
                    {
                        var clone = Game.MahjongCtrl.PopMahjong(mArgs.HuCard);
                        var cloneMj = clone.GetComponent<MahjongContainer>();
                        Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(mArgs.HuCard);
                        Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(cloneMj);
                    }
                    MahjongUtility.PlayOperateEffect(duoHuChair, PoolObjectType.hu);
                }
            }
        }

        protected virtual IEnumerator<float> DianpaoTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            if (mArgs.HuType == NetworkProls.Hu && mArgs.HuSeats.Count == 1)
            {
                yield return Config.TimeHuAniInterval;
                //判断是不是抢杠胡
                var seat = mArgs.HuSeats[0];
                var ctype = mArgs.Result[seat].CType;
                var huChair = MahjongUtility.GetChair(seat);
                var duoHuChair = MahjongUtility.GetChair(seat);
                var isQiangGangHu = ctype != 0 && (ctype & NetworkProls.QiangGangHuType) != 0;
                if (isQiangGangHu)
                {
                    SetHuCard(huChair, mArgs.HuCard);
                }
                else
                {
                    var paoChair = GameCenter.DataCenter.CurrOpChair;
                    var effect = MahjongUtility.PlayMahjongEffect(PoolObjectType.shandian);
                    effect.transform.position = Game.MahjongGroups.MahjongThrow[paoChair].GetLastMjPos();
                    effect.Execute();
                    MahjongUtility.PlayEnvironmentSound("shandian");
                    yield return 0.5f;
                    Game.MahjongGroups.MahjongThrow[paoChair].PopMahjong();
                    SetHuCard(duoHuChair, mArgs.HuCard).Laizi = GameCenter.DataCenter.IsLaizi(mArgs.HuCard);
                }
                string huType = "";
                if (DataCenter.Config.PlaySpecialHuSound)
                {
                    huType = IsSpecialHu(mArgs.Result[mArgs.HuSeats[0]].CType);
                }
                if (!string.IsNullOrEmpty(huType))
                {
                    MahjongUtility.PlayOperateSound(huChair, huType);
                    GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(duoHuChair, PoolObjectType.hu);
                }
                else if (mArgs.PiaoHu)
                {
                    MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.piaohu);
                }
                else
                {
                    MahjongUtility.PlayOperateEffect(duoHuChair, PoolObjectType.hu);
                }
            }
        }

        protected virtual IEnumerator<float> ZhaNiaoAnimation()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            if (mArgs.ZhaMa != null)
            {
                yield return Config.TimeZhaniaoAni * 2;
                var zhaArr = mArgs.ZhaMa;
                var zhongArr = mArgs.ZhongMa;
                if (zhaArr != null && zhaArr.Length != 0)
                {
                    //有中码
                    var flag = null != zhongArr && zhongArr.Length > 0;

                    ZhaniaoArgs args = new ZhaniaoArgs();
                    args.ZhaMaList.AddRange(zhaArr);
                    if (flag)
                    {
                        args.ZhongMaAllList.AddRange(zhongArr);
                    }
                    GameCenter.Hud.GetPanel<PanelZhaniao>().Open(args);

                    float time = (zhaArr.Length) * 0.7f + 0.5f;
                    yield return time;
                }
            }
        }

        protected virtual IEnumerator<float> HandcardCtrlTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            if (mArgs.HuType != NetworkProls.LastCd)
            {
                yield return Config.TimePushCardInterval;
                var huChair = new List<int>();
                var group = Game.MahjongGroups;
                for (int i = 0; i < mArgs.HuSeats.Count; i++)
                {
                    int chair = MahjongUtility.GetChair(mArgs.HuSeats[i]);
                    group.MahjongHandWall[chair].GameResultRota(Config.TimeHuAniInterval);
                    huChair.Add(chair);
                }
                yield return Config.TimePushCardInterval;
                for (int i = 0; i < group.MahjongHandWall.Count; i++)
                {
                    if (!huChair.Contains(i))
                    {
                        group.MahjongHandWall[i].GameResultRota(Config.TimeHuAniInterval);
                    }
                }
            }
        }

        /// <summary>
        /// 宝牌提示
        /// </summary>    
        protected virtual IEnumerator<float> ShowBaoTipTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            if (mArgs.Bao > 0)
            {
                var list = new List<int>() { mArgs.Bao };
                GameCenter.Hud.GetPanel<PanelExhibition>().Open(list);
                yield return Config.TimeBaoTip;
            }
        }

        protected MahjongContainer SetHuCard(int chair, int card)
        {
            var item = Game.MahjongCtrl.PopMahjong(card);
            Game.MahjongGroups.MahjongOther[chair].GetInMahjong(item);
            Game.TableManager.ShowOutcardFlag(item);
            item.gameObject.SetActive(true);
            return item;
        }

        protected void ActionCallback()
        {
            if (GameUtils.CheckStopTask()) return;

            SetGameData();
            GameCenter.EventHandle.Dispatch((int)EventKeys.ShowResult, mArgs);
            var fsm = GameCenter.GameProcess;
            if (!fsm.IsCurrState<StateGameContinue>())
            {
                fsm.ChangeState<StateGameEnd>();
            }
        }

        protected void SetGameData()
        {
            MahjongResult info;
            var scoreList = new Dictionary<int, long>();
            for (int i = 0; i < mArgs.Result.Count; i++)
            {
                info = mArgs.Result[i];
                scoreList.Add(info.Chair, info.TotalGold);
            }
            GameCenter.EventHandle.Dispatch((int)EventKeys.PlayAddScore, new SetScoreArgs()
            {
                ScoreDic = scoreList,
                Type = (int)SetScoreType.EndScore,
            });
        }

        /// <summary>
        /// 判断是否为特殊胡法
        /// </summary>
        /// <param name="ctype">胡牌时服务器发来的ctype</param>
        /// <returns></returns>
        protected string IsSpecialHu(int ctype)
        {
            return HuMusicFunc(ctype);
        }
    }
}
