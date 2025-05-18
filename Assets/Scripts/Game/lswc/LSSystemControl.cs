using System.Collections.Generic;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Manager;
using Assets.Scripts.Game.lswc.Tools;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.lswc
{
    /// <summary>
    /// 系统控制
    /// </summary>
    public class LSSystemControl : InstanceControl
    { 
        /// <summary>
        /// 是否加载完毕资源
        /// </summary>
        private bool _loadFinished = false;

        private LSState _curState;

        public LSState CurState
        {
            set { _curState = value; }
        }

        private LSState _perState;

        public List<LSTimeSpan> spans;

        private void Awake()
        {
            spans = new List<LSTimeSpan>();
            PlayeGameStateMusic(0);
        }
        

        private void Start()
        {
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.ResourseManager.OnLoadFinished = OnLoadResourseFinshed;
            gameMgr.OperationManager.InitListener();
        }

        #region 测试区域
        /// <summary>
        /// 动物顺序测试临时使用
        /// </summary>
        //public int [] Animals = new int[]
        //    {
        //        4,5,6,7,
        //        0,1,2,3,
        //        0,1,2,3,
        //        0,1,2,3,
        //        0,1,2,3,
        //        0,1,2,3,
        //    };

        /// <summary>
        /// 颜色顺序测试临时使用
        /// </summary>
        //public  int[] Colors = new int[]
        //    {
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //    };
        //private void TestDataInit()
        //{

        //    //初始化信息
        //    user.PutLong(RequestKey.KeyTotalGold, 100000000);
        //    InitInfo.PutSFSObject(RequestKey.KeyUser, user);
        //    InitInfo.PutLong(LSConstant.KeyGameStartTime,1000000000000);
        //    InitInfo.PutLong(RequestCmd.GetServerTime, 1000000001);
        //    InitInfo.PutInt(LSConstant.KeyCDTime,5);
        //    InitInfo.PutInt(LSConstant.KeyGameStatus,2);
        //    int[] arr=new int[10];
        //    InitInfo.PutIntArray(LSConstant.KeyHistoryResult,arr);
        //    InitInfo.PutIntArray(LSConstant.KeyAnimalsPosition,Animals);
        //    InitInfo.PutIntArray(LSConstant.KeyColorPosition,Colors);
        //}


        /// <summary>
        /// 模拟收到返回结果
        /// </summary>
        void TestOnGetResult()
        {
            //Debug.LogError("测试发送接收到游戏消息");
            //int result = 0x002;

            //GlobalData.AddHistory(result);

            //GlobalData.ISGetResult = true;

            //GlobalData.GetLastDetialResult();

        }

        #endregion

        private void OnLoadResourseFinshed()
        {
            _loadFinished = true;
            //TestDataInit();
            //LSServerManager.Instance.TestData(0, InitInfo);
        }

        public void InitState()
        {
            var gdata = App.GetGameData<LswcGameData>();
            YxDebug.Log("当前游戏阶段是： " + gdata.GlobalELswcGameStatu + "剩余时间是 ：" + App.GetGameData<LswcGameData>().ShowTime);

            if (_loadFinished)
            {
                _curState = gdata.GameStates.InitState;//LSInitState.Instance;
                _curState.Enter();
            }
            else
            {
                YxDebug.LogError("Resourse is noe loaded finished");
            }
        }

        public void PlaySuccess(bool success)
        {
            if (success)
            {
                PlayVoice(LSConstant.ButtonSelectVoice);
            }
            else
            {
                PlayVoice(LSConstant.ButtonSelectFailVoice);
            }
        }

        public void QuitGame()
        {
            App.QuitGameWithMsgBox();
            //YxMessageBox.Show("确定要退出游戏吗？", "", (box, btnName) =>
            //    {
            //        if (btnName == YxMessageBox.BtnLeft)
            //        {
            //            App.QuitGame();
            //        }
            //    },false,YxMessageBox.LeftBtnStyle|YxMessageBox.RightBtnStyle,null,5);
        }

        private void Update()
        {
            if (_curState != null)
            {
                if (!_curState.ExcuteState)
                {
                    _curState.Excute();
                }
                if (_curState.UpdateState)
                {
                    _curState.Update();
                }
            }
            if(spans!=null)
            {
                foreach (var span in spans)
                {
                    span.Run();
                }
            }
        }

        public override void OnExit()
        {
        }

        void OnDestroy()
        {
            _curState = null;   
        }

        #region 状态相关

        public void ChangeState(LSState newState)
        {
            _perState = _curState;
            if (_curState != null)
            {
                _curState.Exit();
            }
            _curState = newState;

            _curState.Enter();
        }

        public void RevertToPreviousState()
        {
            ChangeState(_perState);
        }

        #endregion

        /// <summary>
        /// 每局初始化数据
        /// </summary>
        public void InitEachRound()
        {
            App.GetGameData<LswcGameData>().ISGetResult = false; 
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.ColorItemControl.ResetLayout();
            gameMgr.AnimalItemCtrl.ResetLayout();
            gameMgr.TurnTableControl.SetPointPosition(0);
            gameMgr.UIManager.InitUImanager();
            gameMgr.CameraManager.Reset();
        }

        public void SetBonus()
        {
            App.GetGameManager<LswcGamemanager>().UIManager.SetBonus(App.GetGameData<LswcGameData>().GetRandomNum());
        }

        /// <summary>
        /// 播放奖励阶段音乐
        /// </summary>
        /// <param name="type"></param>
        public void  PlayeGameStateMusic(LSRewardType type)
        {
            var musicIndex = (int) type;
            switch (type)
            {
                case LSRewardType.BIG_THREE://LSConstant.BackgroundMusic_BigThree;
                case LSRewardType.BIG_FOUR://LSConstant.BackgroundMusic_BigFour;
                    PlayVoice(LSConstant.ThreeOrFourWaring);
                    break;
                case LSRewardType.NORMAL://LSConstant.BackgroundMusci_Normal;
                case LSRewardType.LIGHTING://LSConstant.BackgroundMusic_SendLamp;
                case LSRewardType.SENDLAMP://LSConstant.BackgroundMusic_SendLamp;
                case LSRewardType.HANDSEL://LSConstant.BackgroundMusic_Handsel;
                    break;
                default:
                    //LSConstant.BackgroundMusic_WaitBet; 
                    musicIndex = 6;
                    YxDebug.LogError("Not exist such type music");
                    break;
            }
            PlayeGameStateMusic(musicIndex);
        }

        /// <summary>
        /// 是否需要上台展示,普通模式，彩金模式，闪电需要上台展示，其他（大四喜，大三元，送灯）不需要上台展示
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool NeedSignAnimal(LSRewardType type)
        {
            switch (type)
            {
                case LSRewardType.NORMAL:
                case LSRewardType.HANDSEL:
                case LSRewardType.SENDLAMP:
                    return true;
                default:
                    return false;
            }
        }

        public void PlayeGameStateMusic(int bgMusicIndex)
        {
            Facade.Instance<MusicManager>().ChangeBackSound(bgMusicIndex);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="voice"></param>
        public void PlayVoice(string voice)
        {
            Facade.Instance<MusicManager>().Play(voice);
        }
    }
}
