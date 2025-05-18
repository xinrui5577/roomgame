using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XzmjActionHu : ActionHu
    {
        protected bool mFlag;
        protected int mHuType;
        protected int mHuCard;
        protected int[] mHuGolds;
        protected List<int> mHuSeats;
        protected List<int> mHuCardList;//胡牌座位

        public void SetData(ISFSObject data)
        {
            mFlag = false;
            mHuSeats = new List<int>();
            if (data.ContainsKey("huseat"))
            {
                mHuSeats.Add(data.GetInt("huseat"));
            }
            else if (data.ContainsKey("huseatlist"))
            {
                var iSeatList = data.GetIntArray("huseatlist");
                for (int i = 0; i < iSeatList.Length; i++)
                {
                    mHuSeats.Add(iSeatList[i]);
                }
            }
            mHuType = data.GetInt(RequestKey.KeyType);
            mHuGolds = data.GetIntArray("score");
            mHuCard = data.GetInt("hucard");
            //抢杠胡
            if (data.ContainsKey("ctype"))
            {
                var ctype = data.GetInt("ctype");
                var db = GameCenter.DataCenter;
                //抢杠胡
                if ((ctype != 0) && ((ctype & NetworkProls.QiangGangHuType) != 0) && (db.CurrOpSeat == db.OneselfData.Seat))
                {
                    mFlag = true;
                }
            }
        }

        public override void GameResultAction(ISFSObject data)
        {
            OnHuGameResult(data);
        }

        public override void LastCdAction(ISFSObject data)
        {
            OnHuGameResult(data);
        }

        public override void ZimoAction(ISFSObject data)
        {
            OnHuSingle(data);
        }

        public override void DianpaoAction(ISFSObject data)
        {
            OnHuSingle(data);
        }

        protected void OnHuGameResult(ISFSObject data)
        {
            GameCenter.Instance.SetIgonreReconnectState(true);

            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            mResultType = SingleResultArgs.HuResultType.HuSingle;
            base.LastCdAction(data);
        }

        /// <summary>
        /// 一玩家胡之后接着玩
        /// </summary>  
        protected virtual void OnHuSingle(ISFSObject data)
        {
            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            GameCenter.GameLogic.SetDelayTime(1.5f);
            SetData(data);
            if (null == mHuTask)
            {
                mHuTask = ContinueTaskManager.NewTask().AppendFuncTask(() => SingleHuTask());
            }
            mHuTask.Start();
        }

        private IEnumerator<float> SingleHuTask()
        {
            //移除牌，播放特效
            if (mHuType != NetworkProls.ZiMo)
            {
                var currChair = DataCenter.CurrOpChair;
                var effect = MahjongUtility.PlayMahjongEffectAndAudio(PoolObjectType.shandian);
                effect.transform.position = Game.MahjongGroups.MahjongThrow[currChair].GetLastMjPos();
                effect.Execute();
                yield return 0.8f;
                Game.MahjongGroups.MahjongThrow[currChair].PopMahjong();
            }
            for (int i = 0; i < mHuSeats.Count; i++)
            {
                var chair = MahjongUtility.GetChair(mHuSeats[i]);
                if (mHuType == NetworkProls.ZiMo)
                {
                    Game.MahjongGroups.MahjongHandWall[chair].PopMahjong();
                    MahjongUtility.PlayOperateEffect(chair, PoolObjectType.zimo);
                }
                else
                {
                    MahjongUtility.PlayOperateEffect(chair, PoolObjectType.hu);
                }
                SetHuCard(chair, mHuCard);
                Game.MahjongGroups.MahjongHandWall[chair].SetHandCardState(HandcardStateTyps.SingleHu);
            }
            //抢杠胡
            if (mFlag)
            {
                DataCenter.OneselfData.HardCards.Remove(mHuCard);
                Game.MahjongGroups.PlayerHand.OnQiangganghu(mHuCard);
            }
            //加分特效
            Dictionary<int, long> scoreList = new Dictionary<int, long>();
            for (int i = 0; i < mHuGolds.Length; i++)
            {
                int score = mHuGolds[i];
                if (score != 0)
                {
                    scoreList[MahjongUtility.GetChair(i)] = score;
                }
            }
            yield return 0.5f;
            GameCenter.EventHandle.Dispatch((int)EventKeys.PlayAddScore, new SetScoreArgs()
            {
                DelayTime = 0f,
                ScoreDic = scoreList,
                Type = (int)SetScoreType.AddScoreAndEffect,
            });
            yield return 0.8f;
            //显示胡UI
            GameCenter.EventHandle.Dispatch((int)EventKeys.SetSingleHuFlag, new HuanAndDqArgs()
            {
                Type = 2,
                HuSeats = mHuSeats,
            });
        }
    }
}
