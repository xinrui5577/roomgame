using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XlmjActionHu : XzmjActionHu
    {
        protected override void OnHuSingle(ISFSObject data)
        {
            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            GameCenter.GameLogic.SetDelayTime(1f);
            SetData(data);
            if (null == mHuTask)
            {
                mHuTask = ContinueTaskManager.NewTask().AppendFuncTask(() => XlmjSingleHuTask());
            }
            mHuTask.Start();
        }

        private IEnumerator<float> XlmjSingleHuTask()
        {
            MahjongUserInfo userInfo;
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
                userInfo = DataCenter.Players[chair];
                SetHuCard(chair, mHuCard);
                userInfo.IsHu = true;
                userInfo.IsAuto = true;
                userInfo.HuCardsList.Add(mHuCard);
                Game.MahjongGroups.MahjongHandWall[chair].SetHandCardState(HandcardStateTyps.SingleHu);
            }

            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            var currChair = DataCenter.CurrOpChair;
            //移除牌，播放特效
            if (mHuType != NetworkProls.ZiMo)
            {

                var effect = MahjongUtility.PlayMahjongEffectAndAudio(PoolObjectType.shandian);
                effect.transform.position = Game.MahjongGroups.MahjongThrow[currChair].GetLastMjPos();
                effect.Execute();
                yield return 0.8f;
                Game.MahjongGroups.MahjongThrow[currChair].PopMahjong();
            }
            else
            {
                if (currChair == 0)
                {
                    //胡的牌从手牌中移除
                    DataCenter.OneselfData.HardCards.Remove(mHuCard);
                }
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
        }
    }
}
