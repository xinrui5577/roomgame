namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionLaiziReplay : AbsCommandAction
    {
        private bool mBaoFlag;

        public void ReplayLaizi(ReplayFrameData data)
        {
            //设置赖子牌
            var laizi = data.Cards[0];
            GameCenter.DataCenter.Game.LaiziCard = laizi;

            //排序手牌
            var group = Game.MahjongGroups;
            for (int i = 0; i < Game.MahjongGroups.MahjongHandWall.Count; i++)
            {
                var handWall = Game.MahjongGroups.MahjongHandWall[i];
                var list = handWall.MahjongList;
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Value == laizi)
                    {
                        list[j].Laizi = true;
                    }
                }

                handWall.SortHandMahjong();
            }
        }

        public void ReplayBao(ReplayFrameData data)
        {
            var bao = data.Cards[0];
            if (mBaoFlag)
            {
                var lastBao = data.LastFrameData.Cards[0];
                var item = Game.MahjongGroups.MahjongThrow[data.OpChair].GetInMahjong(lastBao);
                item.SetOtherSign(Anchor.TopRight, true);
                Game.TableManager.ShowOutcardFlag(item);
            }
            else
            {
                mBaoFlag = true;
            }
            var panel = GameCenter.Hud.GetPanel<PanelReplay>();
            panel.SetLaizi(bao);
        }

        public void ReplayFanPai(ReplayFrameData data)
        {
            var fanpai = data.Cards[0];
            var panel = GameCenter.Hud.GetPanel<PanelReplay>();
            panel.SetLaizi(fanpai);
        }


        public override void OnReset()
        {
            mBaoFlag = false;
        }
    }
}
