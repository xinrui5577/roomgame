﻿using UnityEngine;

namespace Assets.Scripts.Game.sanpian.DataStore
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
        public static Vector3 POPUP_WINDOW_CENTER = new Vector3(0, 0, -250);
        public static Vector3 QUIT_CONFIRM_WINDOW_CENTER = new Vector3(0, 0, -320);

        //for popup
        public static Vector3 POPUP_LOADING_CENTER = new Vector3(0, 0, -240);
        public static Vector3 REWARD_WINDOW_CENTER = new Vector3(0, 0, -250);
        public static Vector3 CONFIRM_WINDOW_CENTER = new Vector3(0, 0, -260);
        public static Vector3 MESSAGE_BOX_VECTOR3 = new Vector3(0, 0, -270);
        public static Vector3 WAITING_CENTER = new Vector3(0, 0, -280);
        public static Vector3 SYSTEM_CONFIRM_WINDOW_CENTER = new Vector3(0, 0, -300);
        public static Vector3 ANIMATION_EFFECT = new Vector3(0, 0, -1000);
        public static Vector3 POPUP_USER_GUIDE_VECTOR3 = new Vector3(0, 0, -290);

        public static int  CardOffsetY = 40;
        public static int SEPDISTANCE = 32;
        /// <summary>
        /// 自己手牌的间距
        /// </summary>
        public static int SepaceDistanceSelf = 45;
        /// <summary>
        /// 自己手牌默认Y
        /// </summary>
        public static int SelfDefaultY = 0;
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

        public static int GameType = 1;
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
        /// <summary>
        /// Key玩法连片
        /// </summary>
        public static string KeyLianPian = "islp";

        /// <summary>
        /// Key 片分
        /// </summary>
        public static string KeyBetGold = "betGold";

        /// <summary>
        /// Key 下注分数
        /// </summary>
        public static string KeyBetScores = "score";
    }

    public class HupData
    {
        public int ID;
        public string Name;
        public int Operation;//-1:拒绝 2.发起 3同意。
    }
}
