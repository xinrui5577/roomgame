using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DlmjActionReconnect : ActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            //更新牌
            UpdateMahjongGroup();
            //更新牌桌
            UpdateMahjongTable();
            MahjongQueryHuTip();
            //开启允许托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
            MahjongUserInfo userInfo;
            var groups = GameCenter.Scene.MahjongGroups;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                userInfo = DataCenter.Players[i];
                var nius = userInfo.HandShowCards;
                if (nius == null || nius.Length < 1) continue;
                groups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.TingAndShowCard, nius);
            }
            //设置手牌状态
            SetReconectCardState();
        }
    }
}
