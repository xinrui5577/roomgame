using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.sanpian.server;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.sanpian.CommonCode 
{
    public class SettingPanel :YxNguiWindow
    {
        [SerializeField]
        private UISlider _musiSlider;
        [SerializeField]
        private UISlider _effectSlider;
        [SerializeField]
        private UIButton _dessolveButton;
        [SerializeField]
        private UIToggle _chatToggle;
        [SerializeField]
        private GameObject autoToggle;
        void Start()
        {
            _musiSlider.value = Facade.Instance<MusicManager>().MusicVolume;
            _effectSlider.value = Facade.Instance<MusicManager>().EffectVolume;
           // _chatToggle.value = App.GetGameManager<SanPianGameManager>().ChatVioceToggle;

           // UIEventListener.Get(autoToggle).onClick = OnClickCount;
            count = 0;
        }

        private int count;
        private float startTime;
        private float EndTime;
        //void OnClickCount(GameObject obj)
        //{
        //    count++;
        //    if (count == 1)
        //    {
        //        startTime = Time.realtimeSinceStartup;
        //    }
        //    if (count>5)
        //    {
        //        EndTime = Time.realtimeSinceStartup;
        //        var daleTime = EndTime - startTime;
        //        if(daleTime<5)
        //        {
        //            NoticeLayer.Self.Robot.ToggleRobot();
        //        }
        //        count = 0;
        //        startTime = 0;
        //        EndTime = 0;
        //    }
        //}

        public void OnMusicValueChange()
        {
            Facade.Instance<MusicManager>().MusicVolume = _musiSlider.value;
        }

        public void OnEffectValueChange()
        {
            Facade.Instance<MusicManager>().EffectVolume = _effectSlider.value;
        }

        /// <summary>
        /// 在显示时进行处理，控制解散房间按钮的显示状态
        /// </summary>
        protected override void OnShow()
        {
            //bool CreateRoom = App.GetGameData<SanPianGameData>().CurrentGame.GameRoomType == -1;
            //_dessolveButton.gameObject.SetActive(true);
            //if (SanPianGameManager.instance.IsInit())
            //{
            //  _dessolveButton.GetComponent<BoxCollider>().enabled = App.GetGameData<SanPianGameData>().CurrentUser.Seat== 0;
            //}      
        }

        public void OnDissolveClick()
        {
            Close();
            YxMessageBox.Show(
                "确定解散房间吗?",
                null,
                (window, btnname) =>
                {
                    switch (btnname)
                    {
                        case YxMessageBox.BtnLeft:
                           // App.GetRServer<RemoteServer>().StartHandsUp(2);             // -1拒绝 2发起投票 3同意
                            break;
                    }
                },
                true,
                YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
        }

        public void OnClickChatToggle()
        {
            App.GetGameManager<SanPianGameManager>().ChatVioceToggle = _chatToggle.value;           
        }
    }
}
