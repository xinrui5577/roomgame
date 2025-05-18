using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionCpg : AbsCommandAction
    {
        protected EnGroupType mCpgType;
        protected CpgData mCpgData;
        protected int mCurrOpChair;
        protected int mOldOpChair;
        protected bool mGangBao;

        protected CpgModel CpgModel;

        protected void SetData(ISFSObject data)
        {
            DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
            mGangBao = data.ContainsKey("bao");
            mCurrOpChair = DataCenter.CurrOpChair;
            mOldOpChair = DataCenter.OldOpChair;
            mCpgData = MahjongUtility.CreateCpg(data);
            mCpgData.Laizi = DataCenter.Game.LaiziCard; //cpg中有赖子牌，标记Icon
            mCpgType = mCpgData.Type;
            DataCenter.Players[DataCenter.CurrOpChair].IsTuiDan = data.ContainsKey("tuidan");
            if (mCpgType != EnGroupType.ZhuaGang && IsNotXjfdType(mCpgType))
            {
                //将cpg信息添加到玩家数据中
                GameCenter.DataCenter.Players[mCurrOpChair].CpgDatas.Add(mCpgData);
            }
            CpgModel = new CpgModel(data);
        }

        protected bool IsNotXjfdType(EnGroupType type)
        {
            switch (type)
            {
                case EnGroupType.XiaoJi:
                case EnGroupType.YaoDan:
                case EnGroupType.JiuDan:
                case EnGroupType.ZFBDan:
                case EnGroupType.XFDan:
                    return false;
            }
            return true;
        }

        public bool IsGangBao
        {
            get { return mGangBao; }
        }

        public virtual void ResponseCpgAction(ISFSObject data)
        {
            CpgLogic(data);
            PlayEffect(data);
        }

        public virtual void ResponseXFGAction(ISFSObject data)
        {
            ResponseCpgAction(data);
        }

        public virtual void ResponseJueGangAction(ISFSObject data)
        {
            ResponseCpgAction(data);
        }

        public virtual void ResponseSelfGangAction(ISFSObject data)
        {
            ResponseCpgAction(data);
        }

        protected void CpgLogic(ISFSObject data)
        {
            SetData(data);
            var cpgData = mCpgData;
            //如果是抓杠 改变类型
            if (cpgData.Type == EnGroupType.ZhuaGang)
            {
                var cpg = (CpgZhuaGang)cpgData;
                //抓杠时 会有两条数据回来 当消息为true 杠成功了
                if (cpg.Ok == false) return;
                var cpgList = DataCenter.Players[mCurrOpChair].CpgDatas;
                for (int i = 0; i < cpgList.Count; i++)
                {
                    var cpgItem = cpgList[i];
                    if (cpgItem.Type == EnGroupType.Peng && cpgItem.Card == cpgData.Card)
                    {
                        cpgList[i] = cpgData;
                        break;
                    }
                }
                if (0 == mCurrOpChair && !mGangBao)
                {
                    //删除手牌数据
                    DataCenter.Players[mCurrOpChair].HardCards.Remove(cpgData.Card);
                }
            }
            else if (cpgData.Type == EnGroupType.AnGang && mGangBao)
            {
                if (0 == mCurrOpChair)
                {
                    var temp = 0;
                    var tempList = cpgData.GetHardCards();
                    for (int i = 0; i < tempList.Count - 1; i++)
                    {
                        DataCenter.OneselfData.HardCards.Remove(tempList[i]);
                        temp++;
                    }
                }
            }
            else
            {
                //删除手牌数据
                if (0 == mCurrOpChair)
                {
                    var list = cpgData.GetHardCards();
                    for (int i = 0; i < list.Count; i++)
                    {
                        DataCenter.Players[mCurrOpChair].HardCards.Remove(list[i]);
                    }
                }
            }
            MahjongGroupsManager group = Game.MahjongGroups;
            //抓杠是特殊的 如果放回的消息ok 为false 证明正在确认是否有抢杠胡 为ture时表示 杠成功了
            if (cpgData.Type == EnGroupType.ZhuaGang)
            {
                var zhuanggang = (CpgZhuaGang)cpgData;
                if (!zhuanggang.Ok)
                {
                    group.PlayerToken = false;
                }
                else
                {
                    //如果是抓杠 移除手牌中抓的
                    group.MahjongHandWall[mCurrOpChair].RemoveMahjong(cpgData.Card);
                    //设置吃碰杠
                    group.MahjongCpgs[mCurrOpChair].SetCpg(cpgData);
                }
            }
            else
            {
                group.MahjongHandWall[mCurrOpChair].RemoveMahjong(cpgData.GetHardCards());
            }
            //如果是别人打出的牌
            if (cpgData.GetOutPutCard() != DefaultUtils.DefValue)
            {
                group.MahjongThrow[mOldOpChair].PopMahjong(cpgData.Card);
                //隐藏箭头
                Game.TableManager.GetParts<MahjongOutCardFlag>(TablePartsType.OutCardFlag).Hide();
            }
            group.MahjongCpgs[mCurrOpChair].SetCpg(cpgData);
            //如果吃碰杠之后 cpg 加 手牌数量 大于 手牌数量 需要打牌设置最后一张
            if (group.MahjongCpgs[mCurrOpChair].GetHardMjCnt() + group.MahjongHandWall[mCurrOpChair].MahjongList.Count > DataCenter.Config.HandCardCount)
            {
                group.MahjongHandWall[mCurrOpChair].SetLastCardPos(DefaultUtils.DefValue);
            }
            //麻将记录
            RecordMahjong(cpgData);
        }

        protected void PlayScoreEffect(ISFSObject data)
        {
            if (!data.ContainsKey(AnalysisKeys.GangGold)) return;
            var scoresSfsObj = data.GetSFSObject(AnalysisKeys.GangGold);
            var scoreList = new Dictionary<int, long>();
            var datas = scoresSfsObj.GetKeys();
            for (int i = 0; i < datas.Length; i++)
            {
                var chair = MahjongUtility.GetChair(int.Parse(datas[i]));
                scoreList[chair] = scoresSfsObj.GetInt(datas[i]);
            }
            GameCenter.EventHandle.Dispatch((int)EventKeys.PlayAddScore, new SetScoreArgs()
            {
                DelayTime = 1.5f,
                ScoreDic = scoreList,
                Type = (int)SetScoreType.AddScoreAndEffect,
            });
        }

        protected void RecordMahjong(CpgData cpgData)
        {
            if (cpgData.Type == EnGroupType.AnGang && (cpgData.Seat != DataCenter.OneselfData.Seat))
            {
                //其他玩家暗杠不进行统计
                if (!DataCenter.Config.ShowAnGang) return;
            }
            if (DataCenter.CurrOpChair != 0 && null != cpgData)
            {
                GameCenter.Shortcuts.MahjongQuery.AddRecordMahjongs(cpgData.GetCardDatas);
            }
        }

        protected void PlayEffect(ISFSObject data)
        {
            if (mCpgData.Type == EnGroupType.ZhuaGang)
            {
                // 抢杠胡不播放特效
                var cpg = (CpgZhuaGang)mCpgData;
                if (cpg.Ok == false) return;
            }

            PoolObjectType effect;
            switch (mCpgType)
            {
                case EnGroupType.Chi:
                    effect = PoolObjectType.chi;
                    break;
                case EnGroupType.Peng:
                    effect = PoolObjectType.peng;
                    break;
                default:
                    //播放特效 
                    if (DataCenter.Config.IsPlaySpecialEffects)
                        MahjongUtility.PlayEnvironmentEffect(mCurrOpChair, PoolObjectType.longjuanfeng);
                    effect = PoolObjectType.gang;
                    PlayScoreEffect(data);
                    break;
            }
            MahjongUtility.PlayOperateEffect(DataCenter.CurrOpChair, effect);
        }

        /// <summary>
        /// 潜江麻将，赖子杠 
        /// </summary>     
        public virtual void ResponseLaiZiGangAction(ISFSObject data)
        {
            DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
            var chair = DataCenter.CurrOpChair;
            var card = data.TryGetInt(RequestKey.KeyCard);
            var item = Game.MahjongGroups.MahjongOther[chair].GetInMahjong(card);
            item.gameObject.SetActive(true);
            item.Laizi = DataCenter.IsLaizi(card);
            if (chair != 0)
            {
                GameCenter.Shortcuts.MahjongQuery.AddRecordMahjong(card);
            }
            else
            {
                DataCenter.Players[0].HardCards.Remove(card);
            }
        }

        public virtual void OnResponseCpgXjfd(ISFSObject data) { }
    }
}
