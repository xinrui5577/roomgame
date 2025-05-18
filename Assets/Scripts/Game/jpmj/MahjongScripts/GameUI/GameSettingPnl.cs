using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class GameSettingPnl : PopPnlBase
    {
        public Slider MusicSlider;
        public Slider EffectSLider;

        public GameObject DisVolseRoom;
        public GameObject ChangeRoom;

        void Start()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            MusicSlider.value = musicMgr.MusicVolume;
            EffectSLider.value = musicMgr.EffectVolume;
        }

        public void OnExistGame()
        {
            OnClose();
            YxMessageBox.Show("是否要退出游戏？", null, (box, btnName) =>
            {
                if (btnName.Equals(YxMessageBox.BtnLeft))
                {
                    EventDispatch.Dispatch((int)TableDataEventId.OnSendLeaveRoomState, new EventData(1));
                    Application.Quit();
                }
            }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }

        public void OnExistRoom()
        {
            OnClose();
            EventDispatch.Dispatch((int)NetEventId.OnDismissRoom, new EventData(EnDismissFeedBack.ApplyFor));
        }

        //退出游戏到大厅,
        public void OnExistGameToHall()
        {
            OnClose(); 
            if (UtilData.RoomType == EnRoomType.FanKa)
            {
                YxMessageBox.Show("是否要退出游戏？", null, (box, btnName) =>
                {
                    if (btnName.Equals(YxMessageBox.BtnLeft))
                    {
                        EventDispatch.Dispatch((int)TableDataEventId.OnSendLeaveRoomState, new EventData(0));
                        App.QuitGame();
                    }
                }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            }
            else
            {
                if (App.GameData.GStatus != YxEGameStatus.Normal)
                {
                    YxMessageBox.Show("游戏中不能回到大厅！");
                }
                else
                {
                    YxMessageBox.Show("是否要退出游戏？", null, (box, btnName) =>
                    {
                        if (btnName.Equals(YxMessageBox.BtnLeft))
                        {
                            EventDispatch.Dispatch((int)TableDataEventId.OnSendLeaveRoomState, new EventData(0));
                            App.QuitGame();
                        }
                    }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                }              
            }           
        }

        public void OnClose()
        {
            Hide();
        }

        public void SetRoomInfo(RoomInfo info)
        {
            if (DisVolseRoom != null) DisVolseRoom.SetActive(info.RoomType == EnRoomType.FanKa);
            if (ChangeRoom != null) ChangeRoom.SetActive(info.RoomType == EnRoomType.YuLe);
        }


        public void OnChangeRoomClick()
        {
            OnClose();

            if (App.GameData.GStatus != YxEGameStatus.Normal)
            {
                YxMessageBox.Show("游戏中不能换房间！");
            }
            else
            {
                EventDispatch.Dispatch((int)NetEventId.OnChangeRoom);
            }
        }
    }
}
