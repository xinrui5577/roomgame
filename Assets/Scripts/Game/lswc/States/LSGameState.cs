using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Item;
using Assets.Scripts.Game.lswc.Manager;
using Assets.Scripts.Game.lswc.Scene;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.lswc.States
{
    public class LSGameState
    {
        /// <summary>
        /// 初始化数据状态
        /// </summary>
        public LSInitState InitState = new LSInitState();
        /// <summary>
        /// 下注阶段
        /// </summary>
        public LSBetState BetState = new LSBetState();
        /// <summary>
        /// 等待结果阶段
        /// </summary>
        public LSWaitResultState WaitResultState = new LSWaitResultState();
        /// <summary>
        /// 处理游戏结果
        /// </summary>
        public LSJudgeResultState JudgeResultState = new LSJudgeResultState();
        /// <summary>
        /// 
        /// </summary>
        public LSSendLampShowNumState SendLampShowNumState = new LSSendLampShowNumState();
        /// <summary>
        /// 通用旋转阶段
        /// </summary>
        public LSRotateState RotateState = new LSRotateState();
        /// <summary>
        /// 
        /// </summary>
        public LSShowGameTypeState ShowGameTypeState = new LSShowGameTypeState();
        /// <summary>
        /// 动物展示阶段--看向动物
        /// </summary>
        public LSLookAtAnimalState LookAtAnimalState = new LSLookAtAnimalState();
        /// <summary>
        /// 动物展示阶段--动物移动
        /// </summary>
        public LSAnimalMoveState AnimalMoveState = new LSAnimalMoveState();
        /// <summary>
        /// 动物播放动画阶段
        /// </summary>
        public LSShowAnimationState ShowAnimationState = new LSShowAnimationState();
        /// <summary>
        /// 显示结果阶段
        /// </summary>
        public LSShowResultUIState ShowResultUIState = new LSShowResultUIState();
        /// <summary>
        /// 空状态，这个状态不满足完成一个单独状态，什么也不做，作为游戏结束后等待下一次游戏的开始
        /// </summary>
        public LSEmptyState EmptyState = new LSEmptyState();
    }

    /// <summary>
    /// 初始化数据状态
    /// </summary>
    public class LSInitState : LSState
    {
        public override void Enter()
        {
            base.Enter();
            var gdata = App.GetGameData<LswcGameData>();
            var gameStates = gdata.GameStates;
            switch (gdata.GlobalELswcGameStatu)
            {
                case ELswcGameState.BetState:
                    NextState = gameStates.BetState;//LSBetState.Instance;
                    gameStates.BetState.DuraTime = gdata.ShowTime;//LSBetState.Instance
                    break;
                case ELswcGameState.ResultState:
                    ProcessPlayerInit();
                    break;
                case ELswcGameState.GameOver:
                    App.GetGameManager<LswcGamemanager>().UIManager.SetShowTime(0);
                    ProcessPlayerInit();
                    break;
                case ELswcGameState.WaitState:
                    NextState = gameStates.BetState;//LSBetState.Instance;
                    gameStates.EmptyState.Update();//LSEmptyState.Instance
                    break;
            }
        }

        public override void Excute()
        {
            base.Excute();
            App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
        }

        private void ProcessPlayerInit()
        {
            var gdata = App.GetGameData<LswcGameData>();
            var gstate = gdata.GameStates;
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            if (gdata.LastResult.Reward == LSRewardType.SENDLAMP)
            {
                //送灯模式
                YxDebug.LogError("送灯模式暂时未定数据格式，需要重新处理");
                NextState = gstate.SendLampShowNumState;//LSSendLampShowNumState.Instance;
                NextState.DuraTime = LSConstant.ShowSendLightNumTime;
                gameMgr.TurnTableControl.PlayAnimation();
            }
            else
            {
                var showTime = gdata.ShowTime;
                var onTime = LSConstant.ShowAnimationTime;
                //非送灯模式
                if (showTime <= onTime)
                {
                    NextState = gstate.ShowAnimationState;//LSShowAnimationState.Instance;
                    NextState.DuraTime = LSConstant.ShowAnimationTime;
                    App.GetGameData<LswcGameData>().LastResult.ToAnimal.MoveToCenter(0);
                    QuickRoate();
                    SetShowTime();
                }
                else if (showTime <= (onTime+= LSConstant.AnimoveMoveToCenterTime))//LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime)
                {
                    NextState = gstate.AnimalMoveState; //LSAnimalMoveState.Instance;
                    NextState.DuraTime = LSConstant.AnimoveMoveToCenterTime;
                    QuickRoate();
                    SetShowTime();
                }
                else if (showTime <= (onTime += LSConstant.LookAtAnimalTime))//LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime + LSConstant.LookAtAnimalTime)
                {
                    NextState = gstate.LookAtAnimalState;//LSLookAtAnimalState.Instance;
                    NextState.DuraTime = LSConstant.LookAtAnimalTime;
                    QuickRoate();        
                    SetShowTime();
                }
                else if (showTime <= (onTime += LSConstant.RotationTime)) //LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime + LSConstant.LookAtAnimalTime + LSConstant.RotationTime)
                {
                    NextState = gstate.RotateState;//LSRotateState.Instance;
                    NextState.DuraTime = LSConstant.RotationTime;
                    SetShowTime();
                }
                else if (showTime <= onTime + LSConstant.ShowGameTypeTime)//LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime + LSConstant.LookAtAnimalTime + LSConstant.RotationTime + LSConstant.ShowGameTypeTime)
                {
                    NextState = gstate.ShowGameTypeState;//LSShowGameTypeState.Instance;
                    NextState.DuraTime = LSConstant.ShowGameTypeTime;

                    gameMgr.SystemControl.PlayeGameStateMusic(LSRewardType.NORMAL);//LSConstant.BackgroundMusci_Normal
                    SetShowTime();
                }
                else
                {
                    NextState = gstate.JudgeResultState;//LSJudgeResultState.Instance;
                }
            }
        }

        private void QuickRoate()
        {
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.AnimalItemCtrl.QuickRotate(App.GetGameData<LswcGameData>().AnimalRandomSeed * 15);
            gameMgr.TurnTableControl.QuickRoate(App.GetGameData<LswcGameData>().LastResult.ToAngle.y);
            gameMgr.SystemControl.PlayeGameStateMusic(App.GetGameData<LswcGameData>().LastResult.Reward);
        }

        private void SetShowTime()
        {
            App.GetGameManager<LswcGamemanager>().UIManager.SetShowTime(0);
        }
    }

    /// <summary>
    /// 下注阶段
    /// </summary>
    public class LSBetState : LSState
    {
        public override void Enter()
        {
            base.Enter();

            App.GetGameManager<LswcGamemanager>().UIManager.ShowBetWindow();
            NextState = App.GetGameData<LswcGameData>().GameStates.WaitResultState;//LSWaitResultState.Instance;
        }

        public override void Excute()
        {
            base.Excute();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.SystemControl.PlayeGameStateMusic(6);//LSConstant.BackgroundMusic_WaitBet
            gameMgr.UIManager.SetShowTime(App.GetGameData<LswcGameData>().ShowTime);
            if(_duraTime<=0)
            {
                gameMgr.SystemControl.ChangeState(NextState);
            }
            UpdateState = true;
        }

        private float _frameTime;

        private float _ShowCountDownVoiceTime = 5;

        public override void Update()
        {
            base.Update();
            _frameTime += Time.deltaTime;
            if (_frameTime >= 1)
            {
                _duraTime -= 1;
                _frameTime = 0;
                var sysCtrl = App.GetGameManager<LswcGamemanager>().SystemControl;
                if (_duraTime <= _ShowCountDownVoiceTime && _frameTime >= 0)
                {
                    sysCtrl.PlayVoice(LSConstant.BetCountDownVoice);
                }
                if (_duraTime <= 0)
                {
                    _duraTime = 0;
                    sysCtrl.ChangeState(NextState);
                    return;
                }
                App.GetGameManager<LswcGamemanager>().UIManager.SetShowTime(Mathf.FloorToInt(_duraTime));
            }

        }

        public override void Exit()
        {
            base.Exit();
            var uiMgr = App.GetGameManager<LswcGamemanager>().UIManager;
            uiMgr.SetShowTime(Mathf.FloorToInt(_duraTime));
            App.GetRServer<LswcGameServer>().SendBetRequest();
            uiMgr.HideBetWindow();
            _frameTime = 0;
        }
    }

    /// <summary>
    /// 等待结果阶段
    /// </summary>
    public class LSWaitResultState : LSState
    {
        public override void Enter()
        {
            base.Enter();

            NextState = App.GetGameData<LswcGameData>().GameStates.JudgeResultState;//LSJudgeResultState.Instance;
        }

        public override void Excute()
        {
            base.Excute();
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            if (App.GetGameData<LswcGameData>().ISGetResult)
            {
                App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
            }
        }
    }

    /// <summary>
    /// 处理游戏结果
    /// </summary>
    public class LSJudgeResultState : LSState
    {
        public override void Enter()
        {
            var gdata = App.GetGameData<LswcGameData>();
            base.Enter();
            if (gdata.LastResult.Reward == LSRewardType.SENDLAMP)
            {
                NextState = gdata.GameStates.SendLampShowNumState;//LSSendLampShowNumState.Instance;
                NextState.DuraTime = LSConstant.ShowSendLightNumTime;
            }
            else
            {
                NextState = gdata.GameStates.ShowGameTypeState;//LSShowGameTypeState.Instance;
                NextState.DuraTime = LSConstant.ShowGameTypeTime;
            }
        }

        public override void Excute()
        {
            base.Excute();
            App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
        }
    }

    /// <summary>
    /// 送灯前，显示数字(字数为轮数)阶段
    /// </summary>
    public class LSSendLampShowNumState : LSState
    {
        public override void Enter()
        {
            base.Enter();
            NextState = App.GetGameData<LswcGameData>().GameStates.RotateState;//LSRotateState.Instance;
            NextState.DuraTime = LSConstant.RotationTime;
        }

        public override void Excute()
        {
            base.Excute();
            SetLastHistory();
            App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
        }

        private void SetLastHistory()
        {
            //ToDo 打算在这里处理送灯的每局数据，将每个送灯数据放到历史纪录中，暂时不确定送灯数据结构，等确认后实现
        }
    }
    /// <summary>
    /// 通用旋转阶段
    /// </summary>
    public class LSRotateState : LSState
    {
        /// <summary>
        /// 彩金刷新频率
        /// </summary>
        private float _setBonuFrame = 0.3f;

        /// <summary>
        /// 庄和闲图片变换频率
        /// </summary>
        private float _setBankerFrame = 0.5f;

        private float _animalCache;

        private float _animalFrame = 0.1f;

        private int _randomNum;

        public override void Enter()
        {
            base.Enter();
            var gdata = App.GetGameData<LswcGameData>();
            //需求，如果游戏中，且下注了，就不让退出游戏
            if(gdata.TotalBets>0)
            {
                gdata.GStatus = YxEGameStatus.PlayAndConfine;
            }  

            if (App.GetGameManager<LswcGamemanager>().SystemControl.NeedSignAnimal(gdata.LastResult.Reward))
            {
                NextState = gdata.GameStates.LookAtAnimalState;//LSLookAtAnimalState.Instance;
                NextState.DuraTime = LSConstant.LookAtAnimalTime;
            }
            else
            {
                NextState = gdata.GameStates.ShowAnimationState;//LSShowAnimationState.Instance;
                NextState.DuraTime = LSConstant.ShowAnimationTime;
            }
        }

        public override void Excute()
        {
            base.Excute();
            var gdata = App.GetGameData<LswcGameData>();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.TurnTableControl.Rotate( gdata.LastResult.ToAngle.y, _duraTime);//要求指针先停止旋转，然后动物停止
            gameMgr.AnimalItemCtrl.Rotate(gdata.AnimalRandomSeed * 15, _duraTime);
            gameMgr.UIManager.ChangeBankerTo(gdata.LastResult.Banker, _duraTime, _setBankerFrame);
            gameMgr.UIManager.SetRandomBonus(_duraTime, _setBonuFrame);
            UpdateState = true;
        }



        public override void Update()
        {
            base.Update();

            _duraTime -= Time.deltaTime;

            if (App.GetGameData<LswcGameData>().LastResult.Reward == LSRewardType.BIG_THREE)
            {
                _animalCache += Time.deltaTime;

                if (_animalCache >= _animalFrame)
                {
                    _randomNum++;

                    _animalCache = 0;

                    App.GetGameManager<LswcGamemanager>().BigThreeReward.SetCurrentAnimal((LSAnimalType)(_randomNum % 8));
                }
            }

            if (_duraTime <= 0)
            {
                _duraTime = 0;
                App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
            }

        }
        public override void Exit()
        {
            base.Exit();
            var gdata = App.GetGameData<LswcGameData>();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            switch (App.GetGameData<LswcGameData>().LastResult.Reward)
            {
                case LSRewardType.BIG_THREE:
                    gameMgr.BigThreeReward.SetCurrentAnimal(gdata.LastResult.Ani);
                    break;
                case LSRewardType.BIG_FOUR:
                    //原项目中对应功能不可用，不知道什么表现
                    //LSChangeParticalColor.Instance.OveredChange(GlobalData.LastResult.Cor);
                    break;
            }

            gameMgr.SystemControl.PlayVoice(gdata.LastResult.lastResultVoice);
            gameMgr.UIManager.SetHistoryWindow();
        }
    }


    public class LSShowGameTypeState : LSState
    {
        private LS_Detail_Result detailRes;

        //private float _animalCache = 0;

        //private float _animalFrame = 0.1f;

        //private int _randomNum = 0;

        public override void Enter()
        {
            base.Enter();
            var gdata = App.GetGameData<LswcGameData>();
            detailRes = gdata.LastResult;
            NextState = gdata.GameStates.RotateState;//LSRotateState.Instance;
            NextState.DuraTime = LSConstant.RotationTime;
        }

        public override void Excute()
        {
            base.Excute();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            //根据类型播放对应的状态前置动画
            switch (detailRes.Reward)
            {
                //正常模式，送灯模式，彩金模式无前置动画
                case LSRewardType.NORMAL:
                case LSRewardType.HANDSEL:
                case LSRewardType.SENDLAMP:
                    _duraTime = 0;
                    break;
                case LSRewardType.LIGHTING:
                    App.GetGameManager<LswcGamemanager>().ShowGameTypeManager.ShowGameTypeLighting(detailRes.Multiple);
                    gameMgr.SystemControl.PlayVoice(LSConstant.ShanDianVoice);
                    break;
                case LSRewardType.BIG_THREE:
                    gameMgr.ShowGameTypeManager.ShowBigThree();
                    gameMgr.TurnTableControl.PlayAnimation();
                    break;
                case LSRewardType.BIG_FOUR:
                    gameMgr.ShowGameTypeManager.ShowBigFour(true);
                    gameMgr.TurnTableControl.PlayAnimation();
                    break;
            }
            gameMgr.SystemControl.PlayeGameStateMusic(detailRes.Reward);
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update(); 
            _duraTime -= Time.deltaTime;
            if (_duraTime <= 0)
            {
                App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            _duraTime = 0;
        }


    }

    /// <summary>
    /// 动物展示阶段--看向动物
    /// </summary>
    public class LSLookAtAnimalState : LSState
    {
        public override void Enter()
        {
            base.Enter();
            NextState = App.GetGameData<LswcGameData>().GameStates.AnimalMoveState;//LSAnimalMoveState.Instance;
            NextState.DuraTime = LSConstant.AnimoveMoveToCenterTime;

        }

        public override void Excute()
        {
            base.Excute();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.FireWorksControl.Show();
            gameMgr.CrystalControl.Show(true);
            gameMgr.SystemControl.PlayeGameStateMusic(7);//LSConstant.WaitForShowAnimal
            gameMgr.TurnTableControl.PlayAnimation();
            gameMgr.CameraManager.RotateToPosition(App.GetGameData<LswcGameData>().LastResult.ToAngle.y, _duraTime);
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            _duraTime -= Time.time;
            if (_duraTime <= 0 && !App.GetGameManager<LswcGamemanager>().CameraManager.IsMoving)
            {
                App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
            }
        }
    }

    /// <summary>
    /// 动物展示阶段--动物移动
    /// </summary>
    public class LSAnimalMoveState : LSState
    {
        public override void Enter()
        {
            base.Enter();
            NextState = App.GetGameData<LswcGameData>().GameStates.ShowAnimationState;//LSShowAnimationState.Instance;
            NextState.DuraTime = LSConstant.ShowAnimationTime;
        }

        public override void Excute()
        {
            base.Excute();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.UIManager.SetVFXActive(true);
            var item = App.GetGameData<LswcGameData>().LastResult.ToAnimal;
            item.MoveToCenter(_duraTime);
            gameMgr.CameraManager.MoveDown(_duraTime * 2, _duraTime * 0.5f, item);
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            _duraTime -= Time.deltaTime;
            if (_duraTime < 0 && !gameMgr.CameraManager.IsMoving)
            {
                gameMgr.SystemControl.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            App.GetGameManager<LswcGamemanager>().UIManager.SetVFXActive(false);
        }
    }

    /// <summary>
    /// 动物播放动画阶段
    /// </summary>
    public class LSShowAnimationState : LSState
    {
        private bool needLook;
        public override void Enter()
        {
            base.Enter();

            needLook = App.GetGameManager<LswcGamemanager>().SystemControl.NeedSignAnimal(App.GetGameData<LswcGameData>().LastResult.Reward);
            NextState = App.GetGameData<LswcGameData>().GameStates.ShowResultUIState;//LSShowResultUIState.Instance;
            NextState.DuraTime = LSConstant.ShowResultUITime;
        }

        public override void Excute()
        {
            base.Excute();
            var lastResult = App.GetGameData<LswcGameData>().LastResult;
            //播三次吧
            if (needLook)
            {
                lastResult.ToAnimal.CurAnimation = LSAnimalAnimationType.RAWARD;
                App.GetGameManager<LswcGamemanager>().AnimalItemCtrl.PlayAnimation(3);
            }
            else if (lastResult.Reward == LSRewardType.BIG_THREE)
            {
                App.GetGameManager<LswcGamemanager>().BigThreeReward.PlayAnimation(3);
            }
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            _duraTime -= Time.deltaTime;
            if (_duraTime <= 0)
            {
                _duraTime = 0;
                App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            var gdata = App.GetGameData<LswcGameData>();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gdata.LastResult.ToAnimal.CurAnimation = LSAnimalAnimationType.WATCH;
            if (gameMgr.SystemControl.NeedSignAnimal(gdata.LastResult.Reward))
            {
                gameMgr.CameraManager.ZoomOut(2);
            }
        }

    }


    /// <summary>
    /// 显示结果阶段
    /// </summary>
    public class LSShowResultUIState : LSState
    {
        private float cacheTime;

        public override void Enter()
        {
            base.Enter();
            var gdata = App.GetGameData<LswcGameData>();
            //应该在打开后关闭，不应该在这处理
            gdata.GStatus = YxEGameStatus.Normal;
            NextState = gdata.GameStates.EmptyState;//LSEmptyState.Instance;
            gdata.ShowTime = 0;
            cacheTime = 0;
        }

        public override void Excute()
        {
            base.Excute();
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            gameMgr.FireWorksControl.Hide();
            gameMgr.UIManager.SetVFXActive(false);
            gameMgr.SystemControl.PlayeGameStateMusic(8);//LSConstant.GameEnd
            if (App.GetGameData<LswcGameData>().LastResult.Reward == LSRewardType.SENDLAMP)
            {
                //ToDo 送灯显示历史记录面板
            }
            else
            {
                gameMgr.UIManager.ShowResultPanel();
            }
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            cacheTime += Time.deltaTime;
            if (_duraTime <= cacheTime)
            {
                App.GetGameManager<LswcGamemanager>().SystemControl.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            cacheTime = 0;
        }
    }

    /// <summary>
    /// 空状态，这个状态不满足完成一个单独状态，什么也不做，作为游戏结束后等待下一次游戏的开始
    /// </summary>
    public class LSEmptyState : LSState
    {
        public override void Enter()
        {
            base.Enter();
            var gdata = App.GetGameData<LswcGameData>();
            gdata.ReadyToNext = false;
            gdata.GlobalELswcGameStatu = ELswcGameState.WaitState;
            NextState = gdata.GameStates.BetState;//LSBetState.Instance;
        }

        public override void Excute()
        {
            base.Excute();
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            var gdata = App.GetGameData<LswcGameData>();
            if (!gdata.ReadyToNext) { return;}
            var gameMgr = App.GetGameManager<LswcGamemanager>();
            if (gameMgr.SystemControl.NeedSignAnimal(gdata.LastResult.Reward))
            {
                gdata.LastResult.ToAnimal.MoveBack();
            }
            gdata.ReadyToNext = false;
            App.GetGameManager<LswcGamemanager>().UIManager.HideResultPanel();
            gameMgr.TurnTableControl.ResetAnimation();
            gameMgr.CrystalControl.Show(false);
            gameMgr.BigThreeReward.HideAll();
            gameMgr.ShowGameTypeManager.ShowBigFour(false);
            NextState.DuraTime = gdata.ShowTime;
            gdata.GlobalELswcGameStatu = ELswcGameState.BetState;
            gameMgr.SystemControl.InitEachRound();
            gameMgr.SystemControl.ChangeState(NextState);
            gameMgr.AnimalItemCtrl.InitPosition();
        }
    }

}