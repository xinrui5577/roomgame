using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionFanbao : AbsCommandAction
    {
        protected bool mFilter;//如果宝牌不变，不操作
        protected bool mIsHuanbao;
        protected int mBaoIndex;
        protected int mLastBao;
        protected int mSaizi;
        protected int mBao;

        protected void SetData(ISFSObject data)
        {
            mIsHuanbao = false;
            DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
            mBao = data.TryGetInt(AnalysisKeys.CardBao);
            mBaoIndex = data.TryGetInt("baoindex");
            mLastBao = data.TryGetInt("lastbao");
            mSaizi = data.TryGetInt("saizi");
            mIsHuanbao = data.ContainsKey("lastbao") && mBaoIndex != mBao;
            mFilter = DataCenter.Game.BaoCard == mBao;
            //确定宝牌
            if (mBao == 0)
            {
                mBao = 17;
            }
            else
            {
                DataCenter.Game.BaoCard = mBao;
            }
        }

        public void FanbaoAction(ISFSObject data)
        {
            SetData(data);
            //设置宝牌      
            var tempCard = Game.TableManager.GetParts<MahjongDisplayCard>(TablePartsType.DisplayCard).DisplayMahjong;
            //宝牌是否显示

            bool isShowCard = !DataCenter.ConfigData.AnBao && DataCenter.OneselfData.IsAuto;
            if (tempCard != null)
            {
                if (mIsHuanbao)
                {
                    FanbaoAnimation();
                    var card = Game.TableManager.SetShowBao(mBao, isShowCard);
                    var obj = GameCenter.Pools.Pop<EffectObject>(PoolObjectType.huanbao);
                    if (null != obj)
                    {
                        obj.transform.position = card.transform.position;
                        obj.Execute();
                    }
                }
                else if (DataCenter.CurrOpSeat == DataCenter.OneselfData.Seat && !mFilter)
                {
                    Game.TableManager.SetShowBao(mBao, isShowCard);
                }
            }
            else
            {
                FanbaoAnimation();
                //第一次翻宝
                Game.TableManager.SetShowBao(mBao, isShowCard);
            }
        }

        private void FanbaoAnimation()
        {
            //延迟接收消息
            GameCenter.GameLogic.SetDelayTime(1.5f);
            DataCenter.LeaveMahjongCnt--;
            Game.TableManager.StopTimer();
            System.Action action = () =>
            {
                //设置计时器               
                Game.TableManager.StartTimer(Config.TimeOutcard);
                Game.MahjongGroups.OnFanbaoRmoveMahjong(mBaoIndex);

                var command = GameCenter.GameLogic.GetGameResponseLogic<CommandCpg>();
                var cpgLogic = command.LogicAction;
                if (mIsHuanbao && mLastBao > 0 && !cpgLogic.IsGangBao)
                {
                    //将旧的宝牌放在出牌的牌墙中    
                    var item = Game.MahjongGroups.MahjongThrow[DataCenter.CurrOpChair].GetInMahjong(mLastBao);
                    item.SetOtherSign(Anchor.TopRight, true);
                    Game.TableManager.ShowOutcardFlag(item);
                }
            };
            //打骰子
            Game.TableManager.PlaySaiziAnimation((byte)mSaizi, action);
        }
    }
}