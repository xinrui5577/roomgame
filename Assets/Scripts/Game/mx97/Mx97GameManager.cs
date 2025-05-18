using System.Collections;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.mx97
{
    /// <summary>
    /// 明星97
    /// </summary>
    public class Mx97GameManager : YxGameManager
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        protected MoveMgr[] MoveItems;
        private bool _mIsSendedStopMsg = true;                                          // 记录是否已经发送了停止消息 初始化时不用发送 故为true

        protected override void OnStart()
        {
            base.OnStart();
            var eventCenter = Facade.EventCenter;
            eventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.StartScrollAni, OnStartScrollAni);
            eventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.StopScrollAni, OnStopScrollAni);
        }
 
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            App.GetGameData<Mx97GlobalData>().ChangeAnteRate();
            var eventCenter = Facade.EventCenter;
            // 显示彩金
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshJackpot);
            // 刷新押注分数
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshBetScore);


            // 刷新每个得分行对应的分数
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.ChangeLineScore);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int type, ISFSObject gameInfo)
        { 
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            YxDebug.Log("------> RemoteServer: OnServerResponse() CMDID = rqst and type = " + type + "" + "! \n");
            var eventCenter = Facade.EventCenter;
            switch ((Mx97RequestCmd)type)
            {
                case Mx97RequestCmd.CmdIdStartGame:
                {
                    App.GetGameData<Mx97GlobalData>().SetStartData(response);

                    eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.SwitchBtnWhenStart);
                    eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.StartScrollAni);
                }
                    break;
                case Mx97RequestCmd.CmdIdJackpotChange:
                {
                    // 更新奖池
                    App.GetGameData<Mx97GlobalData>().Caichi = response.GetLong("caichi");
                    eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshJackpot);
                }
                    break;
                    /*   //目前不需要Notice
                case Mx97RequestCmd.CmdIdGetMessage:
                {
                    Facade.Instance<MusicManager>().Play("Winning");
                    var data = response.GetUtfString("data");
                    StartCoroutine(ShowNotice(data));
                }
                    break;
                    */
            }
        }

        private IEnumerator ShowNotice(string data)
        {
            //18-01-08 16:50:56,,2000,235,游客_225891
            yield return new WaitForSeconds(5f);
            var infos = data.Split(',');
            if (infos.Length < 5) yield break;
            var msg = string.Format("恭喜玩家：{0} 获得{1}分,并获得{2}彩金", infos[4], infos[2], infos[3]);
            var noticeData = new YxNoticeMessageData { Message = msg, ShowType = 1000 };
            YxNoticeMessage.ShowNoticeMsg(noticeData);
        }


        public override void UserOut(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
        }

        public override void OnQuitGameClick()
        {
            Facade.Instance<MusicManager>().Play("button");
            YxMessageBox.Show("确定要退出游戏吗？", "", (box, btnName) =>
            {
                if (btnName == YxMessageBox.BtnLeft)
                {
                    App.QuitGame();
                }
            }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle, null, 5);
        }



        protected void Update()
        {
            if (_mIsSendedStopMsg) { return;}
            for (var i = 0; i < 9; i++)
            {
                var mv = MoveItems[i];
                if (mv == null) { continue; }
                if (!mv.IsBreakUpdate){return;}
            }
            _mIsSendedStopMsg = true;
            var eventCenter = Facade.EventCenter;
            // 显示中奖红线
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.ShowGameResult);
            // 刷新押注分数
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshBetScore);
            // 刷新当前分数
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshCurScore);
            // 重置按钮
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.SwitchBtnWhenStop);
            Facade.Instance<MusicManager>().Stop();
        }

        void OnStartScrollAni(object obj)
        {
            // 播放滚动声音
            Facade.Instance<MusicManager>().Play("CardScroll");
            var gdata = App.GetGameData<Mx97GlobalData>();
            for (var i = 0; i < 9; i++)
            {
                var mv = MoveItems[i];
                if (mv == null) { continue; }
                mv.ArgA = 1f;
                mv.ArgB = 60.0f;
                mv.ArgC = 1.0f;
                mv.ArgD = 1.0f;
                mv.HoverTime = 1.0f;
                mv.SwitchCountToStop = 20;

                var fruitId = gdata.StartData.MFruitList[i];
                mv.StopSpriteName = gdata.GetNameById(fruitId);
                mv.StartScroll();

                //将最终停止的水果放置容器
                if (BigWin.getInstance().finalFruits.Count >= 9)
                    BigWin.getInstance().finalFruits.Clear();

                BigWin.getInstance().finalFruits.Add(mv.StopSpriteName);
            }

            _mIsSendedStopMsg = false;
        }

        void OnStopScrollAni(object obj)
        {
            for (var i = 0; i < 9; i++)
            {
                var mv = MoveItems[i];
                if (mv == null) { continue;}
                mv.StopScroll();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Mx97RequestCmd
    {
        /// <summary>
        /// 彩池金额变化
        /// </summary>
        CmdIdJackpotChange = 0x00,
        /// <summary>
        /// 开始游戏
        /// </summary>
        CmdIdStartGame = 0x01,
        /// <summary>
        /// 中彩通知
        /// </summary>
        CmdIdGetMessage = 0x02,
    }
}
