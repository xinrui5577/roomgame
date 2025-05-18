using System;
using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public
{
    public class GameConfig : MonoBehaviour
    {
        public static bool IsShowArrow = true;

        //是否需要cpg横着
        public static bool IsCpgHeng = true;
        //是否需要发牌动画
        public static bool IsNeedSendCardAnimation = true;
        //聊天框中的文本字号
        public static int ChatTxtFontSize = 36;

        //倒计时
        public static int OutCardTime = 10;
        //发牌时间
        public static float SendCardInterval = 0.13f;
        public static float SendCardUpTime = 0.2f;
        public static float SendCardUpWait = 0.15f;
        //抓牌时间
        public static float GetInCardRoteTime = 0.15f;
        public static float GetInCardWaitTime = 0.15f;
        public static float TingPaiWaitTime = 1f;
        //聊天框中文本出现时间
        public static float ChatShowTime = 2.5f;
        //塞子时间
        public static float SiziTime = 1.5f;
        //翻牌摄像机转动时间
        public static float CameraMoveTime = 0.3f;
        //流局 等待时间
        public static float GameEndLiuJuWait = 0.5f;
        //胡牌 等待时间
        public static float GameEndHuWait = 2f;
        //胡牌 推牌时间
        public static float PushCardTime = 0.5f;
        //胡牌 推牌间隔
        public static float PushCardInterval = 1.5f;
        //手牌个数
        public static int UserHandCardCnt = UtilDef.HardMjCnt;
        //牌都推开后出现结算的时间
        public static float ShowResultWaitTime;

        //排序特效
        public static bool GetInEffect;
        public static float PickUpMoreHeight = 0.2f;
        public static float PickUpTime = 0.3f;
        //新桌子动画等待时间
        public static float NewTableAniWaitTime = 5.5f;
        public string JsonConfig;

        public static string GameKey = "";

        //血战换三张时间
        public static int HuanTime = 0;
        //血战断门时间
        public static int DuanTime = 0;
        //血战邀请好友
        public static bool XzInviteFriends;
        //血战继续游戏（创建新房间）
        public static bool XzContinueGame;

        void Awake()
        {
            Init(JsonConfig);
        }

        public void Init(string str)
        {
            if (string.IsNullOrEmpty(str))
                return;
            var json = (Dictionary<string, object>)fastJSON.JSON.Instance.Parse(str);

            if (json == null || json.Count == 0)
            {
                return;
            }

            if (json.ContainsKey("IsCpgHeng"))
            {
                IsCpgHeng = (bool)json["IsCpgHeng"];
            }

            if (json.ContainsKey("IsNeedSendCardAnimation"))
            {
                IsNeedSendCardAnimation = (bool)json["IsNeedSendCardAnimation"];
            }

            if (json.ContainsKey("OutCardTime"))
            {
                OutCardTime = Convert.ToInt32(json["OutCardTime"]);
            }

            if (json.ContainsKey("SendCardInterval"))
            {
                SendCardInterval = Convert.ToSingle(json["SendCardInterval"]);
            }

            if (json.ContainsKey("SendCardUpTime"))
            {
                SendCardUpTime = Convert.ToSingle(json["SendCardUpTime"]);
            }

            if (json.ContainsKey("SendCardUpWait"))
            {
                SendCardUpWait = Convert.ToSingle(json["SendCardUpWait"]);
            }

            if (json.ContainsKey("GetInCardRoteTime"))
            {
                GetInCardRoteTime = Convert.ToSingle(json["GetInCardRoteTime"]);
            }

            if (json.ContainsKey("GetInCardWaitTime"))
            {
                GetInCardWaitTime = Convert.ToSingle(json["GetInCardWaitTime"]);
            }

            if (json.ContainsKey("ChatTxtFontSize"))
            {
                ChatTxtFontSize = Convert.ToInt32(json["ChatTxtFontSize"]);
            }

            if (json.ContainsKey("ChatShowTime"))
            {
                ChatShowTime = Convert.ToSingle(json["ChatShowTime"]);
            }

            if (json.ContainsKey("SiziTime"))
            {
                SiziTime = Convert.ToSingle(json["SiziTime"]);
            }

            if (json.ContainsKey("CameraMoveTime"))
            {
                CameraMoveTime = Convert.ToSingle(json["CameraMoveTime"]);

            }

            if (json.ContainsKey("GameEndLiuJuWait"))
            {
                GameEndLiuJuWait = Convert.ToSingle(json["GameEndLiuJuWait"]);
            }

            if (json.ContainsKey("GameEndHuWait"))
            {
                GameEndHuWait = Convert.ToSingle(json["GameEndHuWait"]);
            }

            if (json.ContainsKey("PushCardTime"))
            {
                PushCardTime = Convert.ToSingle(json["PushCardTime"]);
            }

            if (json.ContainsKey("PushCardInterval"))
            {
                PushCardInterval = Convert.ToSingle(json["PushCardInterval"]);
            }

            if (json.ContainsKey("UserHandCardCnt"))
            {
                UserHandCardCnt = Convert.ToInt32(json["UserHandCardCnt"]);
            }

            if (json.ContainsKey("TingPaiWaitTime"))
            {
                TingPaiWaitTime = Convert.ToSingle(json["TingPaiWaitTime"]);
            }

            if (json.ContainsKey("GameKey"))
            {
                GameKey = (string)json["GameKey"];
            }

            if (json.ContainsKey("GetInEffect"))
            {
                GetInEffect = (bool)json["GetInEffect"];
            }

            if (json.ContainsKey("PickUpMoreHeight"))
            {
                PickUpMoreHeight = Convert.ToSingle(json["PickUpMoreHeight"]);
            }

            if (json.ContainsKey("PickUpTime"))
            {
                PickUpTime = Convert.ToSingle(json["PickUpTime"]);
            }

            if (json.ContainsKey("ShowResultWaitTime"))
            {
                ShowResultWaitTime = Convert.ToSingle(json["ShowResultWaitTime"]);
            }

            if (json.ContainsKey("XzInviteFriends"))
            {
                XzInviteFriends = (bool)json["XzInviteFriends"];
            }
            if (json.ContainsKey("XzContinueGame"))
            {
                XzContinueGame = (bool)json["XzContinueGame"];
            }

        }
    }
}
