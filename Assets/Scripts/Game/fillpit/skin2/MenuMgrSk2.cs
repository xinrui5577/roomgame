using Assets.Scripts.Game.fillpit.ImgPress.Main;
using Assets.Scripts.Game.fillpit.Mgr;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class MenuMgrSk2 : MenuMgr {



        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void OnClickBackGame()
        {
            var gdata = App.GetGameData<FillpitGameData>();
            var gserver = App.GetRServer<FillpitGameServer>();

            bool isPlayed = gdata.IsPlayed;
            bool roomPlayed = gdata.RoomPlayed;
            bool isRoomOwner = gdata.IsRoomOwner;

            if (!gdata.IsRoomGame)
            {
                if (CouldOut())
                {
                    YxMessageBox.Show(new YxMessageBoxData
                    {
                        Msg = "您确定要退出游戏吗?",
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                        Listener = (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                if (CouldOut())
                                {
                                    App.QuitGame();
                                }
                            }
                        }
                    });
                }
                return;
            }

            if (roomPlayed)
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "确定要发起投票,解散房间么?",
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            gserver.SendHandsUp(2);  //房间游戏已开始,发起投票
                        }
                    }
                });
            }
            else
            {
                if (CouldOut())
                {
                    App.QuitGame();
                }
            }

        }

        /// <summary>
        /// 非房卡模式下,检测是否可以退出房间
        /// </summary>
        /// <returns></returns>
        bool CouldOut()
        {
            var gdata = App.GetGameData<FillpitGameData>();
            var selfPanel = gdata.GetPlayer();
            if (gdata.IsGameStart && selfPanel.ReadyState
                && gdata.GetPlayer<PlayerPanel>().PlayerType != 3)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中,不能更换房间!"
                });
                return false;
            }
            return true;
        }

    }
}
