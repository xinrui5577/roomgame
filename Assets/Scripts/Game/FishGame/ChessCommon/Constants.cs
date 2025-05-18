using UnityEngine;

namespace Assets.Scripts.Game.FishGame.ChessCommon
{
    public class Constants
    {
        public const string GlobalGameObjectName = "__GLOBAL_GAMEOBJECT__";
        public static float WaitingEscapeTime = 70f; //等待多长时间后自动消失

        public static float WidthHeightRate = (float)Screen.width / Screen.height;


        public const string SWITCH_ON = "1";
        public const string SWITCH_OFF = "0";

        public static Vector3 FULL_SCREEN_VECTOR3 = new Vector3(Screen.width, Screen.height, 0);
        public static Vector3 ZERO_SCREEN_VECTOR3 = new Vector3(0.3f, 0.3f, 1);
        public static Vector3 POPUP_WINDOW_CENTER = new Vector3(0, 0, 0);
        public static Vector3 QUIT_CONFIRM_WINDOW_CENTER = new Vector3(0, 0, 0);

        //for popup
        public static Vector3 POPUP_LOADING_CENTER = new Vector3(0, 0, 0);
        public static Vector3 REWARD_WINDOW_CENTER = new Vector3(0, 0, 0);
        public static Vector3 CONFIRM_WINDOW_CENTER = new Vector3(0, 0, 0);
        public static Vector3 MESSAGE_BOX_VECTOR3 = new Vector3(0, 0, 0);
        public static Vector3 WAITING_CENTER = new Vector3(0, 0, 0);
        public static Vector3 SYSTEM_CONFIRM_WINDOW_CENTER = new Vector3(0, 0, 0);
        public static Vector3 ANIMATION_EFFECT = new Vector3(0, 0, 0);
        public static Vector3 POPUP_USER_GUIDE_VECTOR3 = new Vector3(0, 0, 0);


        public static int SEPDISTANCE = 40;

        public static bool IsLandlords = false;
        public static int SmallJoker = 0x51;
        public static int BigJoker = 0x61;
        public static int MagicKing = 0x71;

        /// <summary>
        /// 可连续性牌的最大值
        ///最大值为A但是可以取到，所以定义为2(15)
        ///</summary>
        public const int MaxSize = 15;
        /// <summary>
        /// 当前的规则
        /// </summary>
        //public static Guize CurTurnRule = Guize.BU_CHU;
        /// <summary>
        /// 是否包含魔法牌
        /// </summary>
        public static bool HasMagic = false;

        public static int RoomSorce = 20;
        public static bool IsInPlay = false;

        public const int ReadyCardStatus = 0;//没有拖拽
        public const int BeginCardStatus = 1;//开始拖拽
        public const int EndCardStatus = 2;//结束拖拽
        /// <summary>
        /// 拖拽卡牌状态值
        /// 0 没有拖拽
        /// 1 开始拖拽
        /// 2 结束拖拽
        /// </summary>
        public static int SelectCardStatus = ReadyCardStatus;
    }
}
