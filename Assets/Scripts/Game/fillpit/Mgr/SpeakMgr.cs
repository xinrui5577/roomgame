using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using Assets.Scripts.Game.fillpit.Tool;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.fillpit.Mgr
{
    /// <summary>
    /// 玩家说话管理类 说话:德州扑克轮到本玩家决定跟牌与否的简称
    /// </summary>
    public class SpeakMgr : MonoBehaviour
    {

        /// <summary>
        /// 按键数组
        /// </summary>
        public GameObject[] Buttons;

        /// <summary>
        /// 其他标签页 加注,说话,结束选项,自动选项
        /// </summary>
        public GameObject[] Pages;

        public string FirstSpeakPageName = "BetSpeakBtns";


        /// <summary>
        /// 是否有玩家踢牌
        /// </summary>
        [HideInInspector]
        public bool IsKicked;

        /// <summary>
        /// 自己的对象
        /// </summary>
        [HideInInspector]
        public PlayerPanel Self;


        // Use this for initialization
        protected void Start()
        {
            ShowNothing();
            InitOnClick();
        }

        /// <summary>
        /// 显示说话界面
        /// </summary>
        public void ShowSpeak(GameRequestType rt)
        {
            var panel = App.GameData.GetPlayer<PlayerPanel>();
            panel.PlayerType = (int) rt;

            switch (rt)
            {
                case GameRequestType.BetSpeak:
                    Tools.GobShowOnlyOne(Pages, FirstSpeakPageName);
                    _prePageName = "BetSpeakBtns";
                    break;

                case GameRequestType.KickSpeak:
                    Tools.GobShowOnlyOne(Pages, "KickSpeakBtns");
                    _prePageName = "KickSpeakBtns";

                    break;
                case GameRequestType.BackKick:
                    Tools.GobShowOnlyOne(Pages, "BackKickSpeakBtns");
                    _prePageName = "BackKickSpeakBtns";
                    break;
                case GameRequestType.FollowSpeak:
                    var gdata = App.GetGameData<FillpitGameData>();
                    int lastBet = gdata.LastBetValue;
                    if (lastBet > 0 ||
                        lastBet > gdata.GetPlayer<PlayerPanel>().BetMoney)
                    {
                        Tools.GobShowOnlyOne(Pages, "FollowSpeakBtns");
                        _prePageName = "FollowSpeakBtns";
                    }
                    else
                    {
                        Tools.GobShowOnlyOne(Pages, "BetSpeakBtns");
                        _prePageName = "BetSpeakBtns";
                    }
                    break;
                default:
                    ShowNothing();
                    break;
            }
        }

        

        /// <summary>
        /// 弃牌后不显示按键
        /// </summary>
        public void ShowNothing()
        {
            Tools.GobShowOnlyOne(Pages, "");
        }


        /// <summary>
        /// 初始化按键监听
        /// </summary>
        public void InitOnClick()
        {
            foreach (SpeakButton btnid in Enum.GetValues(typeof(SpeakButton)))
            {
                foreach (GameObject btn in Buttons)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int)btnid);
                    }
                }
            }
            /************进度条按键添加事件************/
            //UIEventListener uiel = UIEventListener.Get(AddBtn.gameObject);
            //uiel.onPress = OnBarBtnPress;
            //uiel.parameter = 1;
            //uiel = UIEventListener.Get(SubtractBtn.gameObject);
            //uiel.onPress = OnBarBtnPress;
            //uiel.parameter = 0;
        }

        private string _prePageName = string.Empty;
        protected void OnClickListener(GameObject gob)
        {
            SpeakButton btnid = (SpeakButton)UIEventListener.Get(gob).parameter;
            var selfPanel = App.GetGameData<FillpitGameData>().GetPlayer<PlayerPanel>();
            switch (btnid)
            {
                case SpeakButton.BetBtn:
                    Tools.GobShowOnlyOne(Pages, "AddBetBtns");
                    break;

                case SpeakButton.KickBtn:
                    Tools.GobShowOnlyOne(Pages, "AddBetBtns");
                    break;

                case SpeakButton.FoldBtn:
                    var selftype = selfPanel.PlayerType;
                    
                    switch (selftype)
                    {
                        case 1:
                        case 6:
                            App.GetRServer<FillpitGameServer>().SendRequest(GameRequestType.Fold, null);
                            break;
                        case 4:
                        case 7:
                        case 10:
                            App.GetRServer<FillpitGameServer>().SendRequest(GameRequestType.NotKick, null);
                            break;
                    }
                    break;

                case SpeakButton.FollowBtn:
                    Dictionary<string, object> data = new Dictionary<string, object>() { { "gold", App.GetGameData<FillpitGameData>().LastBetValue } };
                    App.GetRServer<FillpitGameServer>().SendRequest(GameRequestType.Bet, data);
                    break;

                case SpeakButton.NotKickBtn:
                    App.GetRServer<FillpitGameServer>().SendRequest(GameRequestType.NotKick, null);
                    break;

                case SpeakButton.BackKickBtn:
                    Tools.GobShowOnlyOne(Pages, "AddBetBtns");
                    selfPanel.PlayerType = 4;
                    break;

                case SpeakButton.CancelBetBtn:
                    Tools.GobShowOnlyOne(Pages, _prePageName);
                    break;
            }
        }

        public void ShowObj(GameObject obj)
        {
            obj.SetActive(true);
        }

        public void HideObj(GameObject obj)
        {
            obj.SetActive(false);
        }

        /// <summary>
        /// 重置信息
        /// </summary>
        public void Reset()
        {
            //IsKicked = false;
            ShowNothing();
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum SpeakButton
        {
            BetBtn,
            FoldBtn,
            FollowBtn,
            KickBtn,
            NotKickBtn,
            ShowPokerBtn,
            BackKickBtn,
            CancelBetBtn,
        }

    }
}
