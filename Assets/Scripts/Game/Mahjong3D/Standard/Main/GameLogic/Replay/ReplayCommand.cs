namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CommandCommonReplay : AbsGameCommand<ActionCommonReplay>
    {
        [ReplayHandlerAttrubute(ReplayProls.GetIn)]
        public void ReplayGetCard(ReplayFrameData data)
        {
            LogicAction.ReplayGetCard(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.Allowcate)]
        public void ReplaySendCard(ReplayFrameData data)
        {
            LogicAction.ReplaySendCard(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.ThrowOut)]
        public void ReplayThrowoutCard(ReplayFrameData data)
        {
            LogicAction.ReplayThrowoutCard(data);
        }
    }

    public class CommandCpgReplay : AbsGameCommand<ActionCpgReplay>
    {
        [ReplayHandlerAttrubute(ReplayProls.Chi)]
        public void ReplayChi(ReplayFrameData data)
        {
            LogicAction.ReplayChi(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.Peng)]
        public void ReplayPeng(ReplayFrameData data)
        {
            LogicAction.ReplayPeng(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.GangZhua)]
        public void ReplayGangZhua(ReplayFrameData data)
        {
            LogicAction.ReplayZhuaGang(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.GangMing)]
        public void ReplayGangMing(ReplayFrameData data)
        {
            LogicAction.ReplayMingGang(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.GangAn)]
        public void ReplayAnGang(ReplayFrameData data)
        {
            LogicAction.ReplayAnGang(data);
        }
    }

    public class CommandHuReplay : AbsGameCommand<ActionHuReplay>
    {
        [ReplayHandlerAttrubute(ReplayProls.Hu)]
        public void ReplayHu(ReplayFrameData data)
        {
            LogicAction.ReplayHu(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.Zimo)]
        public void ReplayZimo(ReplayFrameData data)
        {
            LogicAction.ReplayZimo(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.ZhaNiao)]
        public void ReplayZhaNiao(ReplayFrameData data)
        {
            LogicAction.ReplayZhaNiao(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.GameOver)]
        public void ReplayGameOver(ReplayFrameData data)
        {
            LogicAction.ReplayGameOver(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.LiuJu)]
        public void ReplayLiuJu(ReplayFrameData data)
        {
            LogicAction.ReplayLiuju(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.Ting)]
        public void ReplayTing(ReplayFrameData data)
        {
            LogicAction.ReplayTing(data);
        }
    }

    public class CommandLaiziReplay : AbsGameCommand<ActionLaiziReplay>
    {
        [ReplayHandlerAttrubute(ReplayProls.Laizi)]
        public void ReplayLaizi(ReplayFrameData data)
        {
            LogicAction.ReplayLaizi(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.HuanBao)]
        public void ReplayHuanBao(ReplayFrameData data)
        {
            LogicAction.ReplayBao(data);
        }

        [ReplayHandlerAttrubute(ReplayProls.FanPai)]
        public void ReplayFanPai(ReplayFrameData data)
        {
            LogicAction.ReplayFanPai(data);
        }
    }
}
