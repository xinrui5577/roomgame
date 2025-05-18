using System;
using Assets.Scripts.Game.Fishing.Constants;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using Assets.Scripts.Game.Fishing.Factorys;
using Assets.Scripts.Game.Fishing.Managers;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing
{
    public class FishingGameManager : YxGameManager
    {
        /// <summary>
        /// 渔场
        /// </summary>
        public FishFactory TheFishFactory;
        /// <summary>
        /// 子弹工厂
        /// </summary>
        public BulletFactory TheBulletFactory; 
        /// <summary>
        /// 地图管理者
        /// </summary>
        public MapManager TheMapManager;
        /// <summary>
        ///  鱼池
        /// </summary>
        public Fishpond TheFishPond;

        public CoinFactory TheCoinFactory;
        /// <summary>
        /// 菜单界面容器
        /// </summary>
        public Transform MenusUiContainer;
        /// <summary>
        /// 通告容器
        /// </summary>
        public Transform TipContainer;
        /// <summary>
        /// 控制管理器
        /// </summary>
        public FishingOperationManager OperationManager;

        protected override void OnStart()
        {
            base.OnAwake();
            InitMenusUI();
            InitFishTideTip();
        }

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private void InitMenusUI()
        {
            var pre = ResourceManager.LoadAsset("MenusUI");
            if (pre == null) return;
            var go = GameObjectUtile.Instantiate(pre, MenusUiContainer);
            go.SetActive(true);
        }

        private GameObject _fishTideTip;
        private void InitFishTideTip()
        {
            var pre = ResourceManager.LoadAsset("FishTideTip");
            if (pre == null) return;
            _fishTideTip = GameObjectUtile.Instantiate(pre,TipContainer);
            _fishTideTip.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            YxDebug.Log("-----------游戏开始--------------");
            ProcedureType = EProcedureType.ReadyNormal;
            YxWindowManager.OpenWindow("ChangeGunWindow");
            App.GetRServer<FishingGameServer>().SendBuyCoin();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            YxDebug.Log("-----------游戏重连--------------");
            //todo 确定 当前状态 
        }

        public override void GameStatus(int status, ISFSObject info)
        {
           
        }


        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch ((FishRequestType)type)
            {
                case FishRequestType.HitFish: //碰鱼
                    {
                        OnKillFish(response);
                    }
                    break;
                case FishRequestType.BuyCoin: //玩家上分 
                    {
//                        YxDebug.Log("玩家上分！");
//                        var gdata = App.GetGameData<FishGameData>();
//                        gdata.CanBuyCoin = true;
//                        gdata.CanRetrieveCoin = true;
//                        SetUserInfo(response);
                    }
                    break;
                case FishRequestType.Sell:
                    {
//                        YxDebug.Log("玩家下分！");
//                        var gdata = App.GetGameData<FishGameData>();
//                        gdata.CanRetrieveCoin = true;
//                        SetUserInfo(response, false);
                    }
                    break;
                case FishRequestType.Quit: //退出玩家
                    YxDebug.Log("可以断开游戏");
                    App.QuitGame();
                    break;
                case FishRequestType.RobotOut:
//                    JoinRobot(response);
                    break;
                case FishRequestType.Message:
                    //                    var userName = response.GetUtfString(FishRequestKey.Msg);
//                    var userName = response.GetUtfString("UserName");
//                    var fishName = response.GetUtfString("FishName");
//                    var coin = response.GetInt("Coin");
//                    var fishRate = response.GetInt("FishRate");
//                    var msg = string.Format("恭喜玩家 <b><color=#ffff00>{0}</color></b> 捕捉到 <b><color=#ffff00>{1}</color></b> ,以 <b><color=#ff0000>{2}</color></b>倍数获得大奖 <b><color=#ff0000>{3}</color></b>金币", userName, fishName, fishRate, coin);
//                    var noticeMsg = new YxNoticeMessageData()
//                    {
//                        Message = msg,
//                        ShowType = 1
//                    };
//                    Debug.LogError(msg);
//                    YxNoticeMessage.ShowNoticeMsg(noticeMsg);
                    break;
                case FishRequestType.FirePower: //发射请求 
                    { 
                        var blt = response.GetInt(FishRequestKey.Blt);
                        var id = response.GetInt(RequestKey.KeyId);
                        var isLock = response.GetBool(FishRequestKey.LockB);
                        var seat = 0;//todo 通过座位发射
                        var battery = App.GameData.GetPlayer<Battery>();
                        var player = battery.Player;
                        if (player != null)
                        {
                            player.OnFire(blt,id,isLock); 
                        }
                    }
                    break;
                    //case 出鱼:id , 三点
            }
        }

        /// <summary>
        /// 杀死鱼
        /// </summary>
        /// <param name="response"></param>
        private void OnKillFish(ISFSObject response)
        {
            var coin = response.GetInt("coin"); //获得的金币
            var bId = response.GetInt("bid");//子弹id
            var rate = response.GetInt("rate");//倍数
            var fishId = response.GetInt("fid");//fishId
            var blt = response.GetInt("blt");//子弹倍数
            var seat = 0;//todo 位置，某玩家打中鱼
            var gdata = App.GameData;//炮台
            var battery = gdata.GetPlayer<Battery>();
            var player = battery.Player;
            var fish = TheFishFactory.GetFish(fishId);
            var userInfo = gdata.GetPlayerInfo<FishingUserInfo>();
            if (userInfo != null)
            {
                userInfo.CoinA = coin;
            }
            if (fish != null)
            {
                var addCoin = rate * blt;
                var data = new CoinData
                {
                    Player =  player,
                    AddCoin = addCoin,
                    Rate = blt
                };
                //飞金币
                TheCoinFactory.CreateFlyCoin(data, player, fish.transform.position);
                var fishData = fish.Data;
                if (fish.AbilityType != EFishAbilityType.Immortal)
                {
                    TheFishFactory.RemoveFish(fishData.FishId, fishData.Type);
                }
                //鱼死亡
                fish.Die();
                fish.DieSpeak();
            }
        }

        /// <summary>
        /// 流程类型
        /// </summary>
        public EProcedureType ProcedureType { get; private set; }

        private void Update()
        {
            switch (ProcedureType)
            {
                case EProcedureType.ReadyNormal://准备普通模式
                    OnReadyNormal();
                    break;
                case EProcedureType.ReadyChangeMap://流程 : 停止出鱼，清鱼，音乐，背景
                    OnReadyChangeMap();
                    break;
                case EProcedureType.ReadyFormation://准备鱼阵
                    OnReadyFormation();
                    break;
                case EProcedureType.WaitForNormal:
                    OnWaitForNormal();
                    break;
                case EProcedureType.WaitForFormation:
                    OnWaitforFormation();
                    break;
                case EProcedureType.WaitTip:
                    OnWaitForTip();
                    break;
            }
        }

        private float _lastWaitTime;
        private float _waitTime;
        /// <summary>
        /// 准备普通模式
        /// </summary>
        private void OnReadyNormal()
        {
            _lastWaitTime = 0;
            _waitTime = 120;
            if (TheFishFactory == null) return;
            //todo 1、开始出鱼
            TheFishFactory.GeneratorNormal();
            ProcedureType = EProcedureType.WaitForNormal;
        }

        private int _lastMapId;

        /// <summary>
        /// 准备切图
        /// </summary>
        private void OnReadyChangeMap()
        {
//            Debug.LogError("准备切换");
            _lastWaitTime = 0;
            // 1、换音乐     2、换地图     3、停止射击
            var center = Facade.EventCenter;
            center.DispatchEvent(EFishingEventType.SwimmerState, ESwimmerState.RunAway);//鱼逃跑
            ProcedureType = EProcedureType.None;
            //            Random.InitState((int)DateTime.Now.Ticks);
            //            _lastMapId = DateTime.Now.Second % 5;
            _lastMapId++;
            if (_lastMapId > 4)
            {
                _lastMapId = 0;
            }
            TheFishFactory.StopGenerator();
            TheMapManager.ChangeMap(_lastMapId, ChangeReadyFormation);
        }

        private void ChangeReadyFormation()
        {
            ProcedureType = EProcedureType.ReadyFormation;
        }

        /// <summary>
        /// 准备鱼阵
        /// </summary>
        private void OnReadyFormation()
        {
//            Debug.LogError("准备鱼阵");
            _lastWaitTime = 0;
            //todo 1、开始鱼阵 
            _waitTime = TheFishFactory.GeneratorFormation();
            ProcedureType = EProcedureType.WaitForFormation;
        }

        /// <summary>
        /// 普通等待
        /// </summary>
        private void OnWaitForNormal()
        {
            _lastWaitTime += Time.deltaTime;
            if (_lastWaitTime > _waitTime)
            {
//                Debug.LogError("   结束出鱼");
                ProcedureType = EProcedureType.ReadyFormation;
                if (_fishTideTip == null)
                {
                    ProcedureType = EProcedureType.ReadyChangeMap;
                }
                else
                {
                    Facade.Instance<MusicManager>().Play("sound_warning_2");
                    //显示鱼潮tip
                    ProcedureType = EProcedureType.WaitTip;
                    _fishTideTip.SetActive(true);
                    _lastWaitTime = 0;
                    _waitTime = 2;
                }
            }
        }

        private void OnWaitForTip()
        {
            _lastWaitTime += Time.deltaTime;
            if (_lastWaitTime > _waitTime)
            {
                _fishTideTip.SetActive(false);
                //切换地图 ---> 准备鱼阵
                ProcedureType = EProcedureType.ReadyChangeMap;
            }
        }


        /// <summary>
        /// 鱼阵等待
        /// </summary>
        private void OnWaitforFormation()
        {
            _lastWaitTime += Time.deltaTime;

            if (_lastWaitTime > _waitTime)
            {
//                Debug.LogError("   结束鱼阵");
                //结束鱼阵
                ProcedureType = EProcedureType.ReadyNormal;
            }
        }



        public enum EProcedureType
        {
            /// <summary>
            /// 无
            /// </summary>
            None,
            /// <summary>
            /// 准备普通模式
            /// </summary>
            ReadyNormal,
            /// <summary>
            /// 准备阵型模式
            /// </summary>
            ReadyFormation,
            /// <summary>
            /// 准备切图
            /// </summary>
            ReadyChangeMap, 
            /// <summary>
            /// 等待正常结束
            /// </summary>
            WaitForNormal,
            /// <summary>
            /// 等待鱼阵结束
            /// </summary>
            WaitForFormation,
            /// <summary>
            /// 等待tip；
            /// </summary>
            WaitTip
        }

        enum EFishRequestType
        {
            HitFish = 0,
            BuyCoin = 1,
            Sell = 2,
            Quit = 4,
            RobotOut = 6,
            Message = 7,
            FirePower = 8
        }


#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (GameUiCamera == null) return;
            var ca = GameUiCamera;
            var size = ca.orthographicSize;
            var size2 = size * ca.aspect;
            var far = ca.farClipPlane;
            var ts = transform;
            DrawRect(ts, size2, size, 0);
            DrawRect(ts, size2, size, far);

            DrawLine(ts, -size2, -size, 0, far);
            DrawLine(ts, size2, -size, 0, far);
            DrawLine(ts, size2, size, 0, far);
            DrawLine(ts, -size2, size, 0, far);

            //背景区
            if (TheMapManager != null)
            {
                DrawRect(transform, size2, size, TheMapManager.transform.position.z);
            }
            //鱼
            if (TheFishFactory != null)
            {
                DrawRect(transform, size2, size, TheFishFactory.transform.position.z);
            }
            //子弹区域
            if (TheBulletFactory != null)
            {
                DrawRect(ts, size2, size, TheBulletFactory.transform.position.z);
            }
        }


        public static void DrawRect(Transform ts, float w, float h, float f)
        {
            var a = ts.TransformPoint(new Vector3(-w, -h, f)); //a
            var b = ts.TransformPoint(new Vector3(w, -h, f)); //b
            var c = ts.TransformPoint(new Vector3(w, h, f)); //c
            var d = ts.TransformPoint(new Vector3(-w, h, f)); //d

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }

        public static void DrawLine(Transform ts, float w, float h, float f, float f1)
        {
            var a = ts.TransformPoint(new Vector3(w, h, f)); //a
            var aa = ts.TransformPoint(new Vector3(w, h, f1)); //b
            Gizmos.DrawLine(a, aa);
        }
#endif

    }
}
