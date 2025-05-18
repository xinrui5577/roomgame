using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Utils;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common.Model;
using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.toubao
{
    public class GlobalData : YxGameData
    {
        /// <summary>
        /// 自己
        /// </summary>
        public bool BeginBet = false;
        public int CurrentCanInGold = -1;
        public long ThisCanInGold = 0;
        public long ResultBnakerTotal = 0;
        public int ResultUserTotal = 0;
        public bool Forbiden;
        public Vector2 ScreenSize = new Vector2(1334, 750);
        public bool BankerApplying = false;
        public bool CanBet = false; //是否可以下注
        public int Ante = 1;
        public GameState State;

        public int BankSeat;
        /// <summary>
        /// 游戏是否开始
        /// </summary>
        public bool IsGaming = false;
        /// <summary>
        /// 是否能退出游戏
        /// </summary>
        public bool CouldOut { get { return !IsGaming && BankSeat != SelfSeat; } }

        public void Start()
        {
            //根据平台改scalingStyle
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer ? UIRoot.Scaling.Constrained : UIRoot.Scaling.ConstrainedOnMobiles;

#if UNITY_STANDALONE
            YxDebug.Log("w = " + Screen.currentResolution.width + " h = " + Screen.currentResolution.height);

            if (ScreenSize.x > Screen.currentResolution.width || ScreenSize.y > Screen.currentResolution.height)
            {
                float xRate = Screen.currentResolution.width / ScreenSize.x;
                float yRate = Screen.currentResolution.height / ScreenSize.y;

                ScreenSize.x *= xRate > yRate ? yRate : xRate;
                ScreenSize.y *= xRate > yRate ? yRate : xRate;
            }

            Screen.SetResolution((int)ScreenSize.x, (int)ScreenSize.y, false);
#endif
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            if (gameInfo.ContainsKey("banker"))
            {
                BankSeat = gameInfo.GetInt("banker");
            }
        }

        public void ChangeGameState(GameState state)
        {
            State = state;
            switch (state)
            {
                case GameState.XiaZhu:
                    CanBet = true;
                    break;
                case GameState.Free:
                    CanBet = false;
                    break;
                case GameState.Result:
                    CanBet = false;
                    break;
                case GameState.IsBanker:
                    CanBet = false;
                    break;
            }
        }

        public enum GameState
        {
            Free,
            XiaZhu,
            Result,
            IsBanker
        }
    }
}

