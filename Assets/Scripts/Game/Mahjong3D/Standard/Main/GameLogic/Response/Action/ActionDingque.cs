using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionDingque : AbsCommandAction
    {
        /// <summary>
        /// 定缺开始
        /// </summary>
        public void SelectDingqueStartAction(ISFSObject data)
        {        
            if (DataCenter.ConfigData.HasHuanZhang)
            {
                //换张重置
                Game.MahjongGroups.SwitchGorup.OnResetSwitchNodes();
            }
            //开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Close((int)GameSwitchType.PowerAiAgency);  
            var mahHand = Game.MahjongGroups.PlayerHand;
            mahHand.SetHandCardState(HandcardStateTyps.Dingqueing);
            var xzMahHand = mahHand.GetComponent<XzmjMahjongPlayerHand>();
            GameCenter.EventHandle.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                SecletColor = xzMahHand.LeastColor,
                CtrlState = true,
                StateFlagType = (int)PlayerStateFlagType.Selecting
            });
        }

        /// <summary>
        /// 定缺结束
        /// </summary>
        public void SelectDingqueEndAction(ISFSObject data)
        {
            //停止定时任务
            Game.TableManager.StopTimer();
            var eventManger = GameCenter.EventHandle;
            eventManger.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = false
            });
            var colors = data.GetIntArray("color");
            eventManger.Dispatch((int)EventKeys.ShowDingqueFlag, new HuanAndDqArgs()
            {
                Type = 0,
                DingqueColors = colors
            });
            //设置手牌断门
            var color = colors[DataCenter.OneselfData.Seat];
            Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.DingqueOver, color);
            //是庄家，提示打牌
            GameCenter.Controller.ForbbidToken = false;
            if (DataCenter.BankerChair == 0)
            {              
                var list = Game.MahjongGroups.PlayerHand.MahjongList;
                var item = list[list.Count - 1];
                Game.MahjongGroups.PlayerHand.SetLastCardPos(item);
                eventManger.Dispatch((int)EventKeys.TipBankerPutCard);
            }
            //开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
        }
    }
}