using Assets.Scripts.Game.sss.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;
#pragma warning disable 649

namespace Assets.Scripts.Game.sss
{
    public class SelfPanel : SssPlayer
    {
        /// <summary>
        /// 开始按钮
        /// </summary>
        [SerializeField] private UIButton _startBtn;

        /// <summary>
        /// 准备按钮
        /// </summary>
        [SerializeField] private UIButton _readyBtn;

        public override void OnUserReady()
        {
            base.OnUserReady();
            if (_startBtn == null)
                return;
            var gdata = App.GetGameData<SssGameData>();
            _startBtn.gameObject.SetActive(gdata.IsRoomGame && Info.Seat == 0 && !gdata.IsPlayed);
        }

        /// <summary>
        /// 当可以开始游戏
        /// </summary>
        public override void OnCouldStart()
        {
            if (Info.Seat != 0 || _startBtn == null)
                return;

            _startBtn.gameObject.SetActive(true);
            _startBtn.GetComponent<BoxCollider>().enabled = true;
            _startBtn.state = UIButtonColor.State.Normal;
        }

        /// <summary>
        /// 点击开始(外挂方法)
        /// </summary>
        public void OnClickStartBtn()
        {
            App.GetRServer<SssGameServer>().SendRequest(GameRequestType.CouldStart, null);
        }

        /// <summary>
        /// 设置准备按钮是否显示
        /// </summary>
        /// <param name="active"></param>
        public override void SetReadyBtnActive(bool active)
        {
            if (_readyBtn == null)
                return;

            _readyBtn.gameObject.SetActive(active);
        }

        public override void FinishChoiseCards()
        {
            base.FinishChoiseCards();
            App.GetGameManager<SssGameManager>().ChoiseMgr.HideChoiseView();
        }


        public override void OnGameStart()
        {
            base.OnGameStart();
            _startBtn.gameObject.SetActive(false);  //隐藏开始按钮
        }
    }
}
