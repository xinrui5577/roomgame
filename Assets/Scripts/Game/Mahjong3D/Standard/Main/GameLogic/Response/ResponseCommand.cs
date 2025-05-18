using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CommandResponseCommon : AbsGameCommand<ActionCommonResponse>
    {
        [S2CResponseHandler(CustomProl.GameOverLogic)]
        public void OnGameOver(ISFSObject data)
        {
            LogicAction.GameOverAction(data);
        }

        [S2CResponseHandler(CustomProl.ResRollDice)]
        public void OnResRollDice(ISFSObject data)
        {
            LogicAction.RollDiceAction(data);
        }

        [FilterOperateMenu]
        [S2CResponseHandler(NetworkProls.GetHuCards)]
        public void OnQueryHuCard(ISFSObject data)
        {
            LogicAction.QueryHuCardAction(data);
        }

        [S2CResponseHandler(NetworkProls.CheckCards)]
        public void CheckCards(ISFSObject data)
        {
            LogicAction.CheckCardsAction(data);
        }

        [S2CResponseHandler(NetworkProls.FenZhang)]
        public void OnFenzhang(ISFSObject data)
        {
            LogicAction.FenzhangAction(data);
        }

        [S2CResponseHandler(CustomProl.CustomLogic)]
        public void OnCustomLogic(ISFSObject data)
        {
            LogicAction.CustomLogicAction(data);
        }
    }

    public class CommandThrowoutCard : AbsGameCommand<ActionThrowoutCard>
    {
        [S2CResponseHandler(NetworkProls.ThrowoutCard)]
        public void OnThrowoutCard(ISFSObject data)
        {
            LogicAction.ThrowoutCardAction(data);
        }
    }

    public class CommandSendCard : AbsGameCommand<ActionSendCard>
    {
        [S2CResponseHandler(NetworkProls.AlloCate)]
        public void OnSendCard(ISFSObject data)
        {
            LogicAction.SendCardAction(data);
        }
    }

    public class CommandCpg : AbsGameCommand<ActionCpg>
    {
        [S2CResponseHandler(NetworkProls.CPG)]
        public void OnResponseCpg(ISFSObject data)
        {
            LogicAction.ResponseCpgAction(data);
        }

        [S2CResponseHandler(NetworkProls.CPGXFG)]
        public void OnResponseCPGXFG(ISFSObject data)
        {
            LogicAction.ResponseXFGAction(data);
        }

        [S2CResponseHandler(NetworkProls.JueGang)]
        public void OnResponseJueGang(ISFSObject data)
        {
            LogicAction.ResponseJueGangAction(data);
        }

        [S2CResponseHandler(NetworkProls.SelfGang)]
        public void OnResponseSelfGang(ISFSObject data)
        {
            LogicAction.ResponseSelfGangAction(data);
        }

        [S2CResponseHandler(NetworkProls.LaiZiGang)]
        public void OnResponseLaiZiGang(ISFSObject data)
        {
            LogicAction.ResponseLaiZiGangAction(data);
        }

        [S2CResponseHandler(NetworkProls.Dan)]
        public void OnYoujinTing(ISFSObject data)
        {
            LogicAction.OnResponseCpgXjfd(data);
        }
    }

    public class CommandGetCard : AbsGameCommand<ActionGetCard>
    {
        [S2CResponseHandler(NetworkProls.GetInCard)]
        public void OnGetCard(ISFSObject data)
        {
            LogicAction.GetCardAction(data);
        }
    }

    public class CommandHu : AbsGameCommand<ActionHu>
    {
        [S2CResponseHandler(NetworkProls.LastCd)]
        public void OnHuLastCd(ISFSObject data)
        {
            LogicAction.LastCdAction(data);
        }

        [S2CResponseHandler(NetworkProls.ZiMo)]
        public void OnHuZimo(ISFSObject data)
        {
            LogicAction.ZimoAction(data);
        }

        [S2CResponseHandler(NetworkProls.Hu)]
        public void OnHuDianpao(ISFSObject data)
        {
            LogicAction.DianpaoAction(data);
        }

        [S2CResponseHandler(NetworkProls.GameResult)]
        public void OnHuGameResult(ISFSObject data)
        {
            LogicAction.GameResultAction(data);
        }
    }

    public class CommandBuzhang : AbsGameCommand<ActionBuzhang>
    {
        [S2CResponseHandler(NetworkProls.BuZhang)]
        public void OnBuZhang(ISFSObject data)
        {
            LogicAction.BuZhangAction(data);
        }

        [S2CResponseHandler(NetworkProls.BuZhangFinish)]
        public void OnBuZhangFinish(ISFSObject data)
        {
            LogicAction.BuZhangFinishAction(data);
        }

        [S2CResponseHandler(NetworkProls.BuZhangGetIn)]
        public void OnGetInBuZhang(ISFSObject data)
        {
            LogicAction.GetInBuZhangAction(data);
        }
    }

    public class CommandOperate : AbsGameCommand<ActionOperate>
    {
        [S2CResponseHandler(NetworkProls.OpreateType)]
        public void OnOperate(ISFSObject data)
        {
            LogicAction.OperateAction(data);
        }
    }

    public class CommandReconnect : AbsGameCommand<ActionReconnect>
    {
        [S2CResponseHandler(CustomProl.ReconnectLogic)]
        public void OnReconnect(ISFSObject data)
        {
            LogicAction.ReconnectAction(data);
        }
    }

    public class CommandTing : AbsGameCommand<ActionTing>
    {
        [S2CResponseHandler(NetworkProls.Ting)]
        public void OnTing(ISFSObject data)
        {
            LogicAction.TingAction(data);
        }

        [S2CResponseHandler(NetworkProls.Youjin)]
        public void OnYoujin(ISFSObject data)
        {
            LogicAction.SpecialTingAction(data);
        }

        [S2CResponseHandler(NetworkProls.DaiGu)]
        public void OnDaiGu(ISFSObject data)
        {
            LogicAction.SpecialTingAction(data);
        }

        [S2CResponseHandler(NetworkProls.LiangDao)]
        public void OnLiangDao(ISFSObject data)
        {
            LogicAction.SpecialTingAction(data);
        }
    }

    public class CommandChangeCard : AbsGameCommand<ActionChangeCard>
    {
        [S2CResponseHandler(NetworkProls.ChangeCards)]
        public void OnChangeCardsStart(ISFSObject data)
        {
            LogicAction.ChangeCardsStartAction(data);
        }

        [S2CResponseHandler(NetworkProls.RotateCards)]
        public void OnChangeCardsEnd(ISFSObject data)
        {
            LogicAction.ChangeCardsEndAction(data);
        }
    }

    public class CommandDingque : AbsGameCommand<ActionDingque>
    {
        [S2CResponseHandler(NetworkProls.SelectColor)]
        public void OnSelectDingqueStart(ISFSObject data)
        {
            LogicAction.SelectDingqueStartAction(data);
        }

        [S2CResponseHandler(NetworkProls.SelColorRst)]
        public void OnSelectDingqueEnd(ISFSObject data)
        {
            LogicAction.SelectDingqueEndAction(data);
        }
    }

    public class CommandFanbao : AbsGameCommand<ActionFanbao>
    {
        [S2CResponseHandler(NetworkProls.Bao)]
        public void OnFanbao(ISFSObject data)
        {
            LogicAction.FanbaoAction(data);
        }
    }

    public class CommandScoreDouble : AbsGameCommand<ActionScoreDouble>
    {
        //加漂
        [S2CResponseHandler(NetworkProls.SelectPiao)]
        public void OnShowBottompour(ISFSObject data)
        {
            LogicAction.ShowBottompourAction(data);
        }

        //显示加漂分数
        [S2CResponseHandler(NetworkProls.ShowPiao)]
        public void OnShowScoreDouble(ISFSObject data)
        {
            LogicAction.ShowScoreDoubleAction(data);
        }
    }

    //lisi--新增新游戏回调--start--
    public class CommandNewGameBegin : AbsGameCommand<ActionNewGameBegin>
    {
        [S2CResponseHandler(NetworkProls.NewGameBegin)]
        public void OnNewGameBegin(ISFSObject data)
        {
            LogicAction.NewGameBeginAction(data);
        }
    }
    //lisi--end--
}