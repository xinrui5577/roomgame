using UnityEngine;
using Assets.Scripts.Game.duifen.ImgPress.Main;
using YxFramwork.Common;


namespace Assets.Scripts.Game.duifen
{
    public class SelfPanel : DuifenPlayerPanel
    {

        public UIButton ReadyBtn;

        public UIButton StartBtn;

        public UILabel SelfPointLabel;


        public override void ShowUserInfo()
        {
            base.ShowUserInfo();
            var gdata = App.GetGameData<DuifenGlobalData>();
            bool showReady = !ReadyState && !gdata.IsGameing;

            
            SetGameObjectActive(ReadyBtn.gameObject,showReady);         //设置准备按钮
            
            SetGameObjectActive(StartBtn.gameObject,
                ReadyState && gdata.IsRoomGame && Info.Seat == 0 && !gdata.IsPlayed);   //设置开始按钮
            
        }


        public void OnClickReadyBtn()
        {
            App.GetRServer<DuifenGameServer>().ReadyGame();
        }

        public void OnClickStartBtn()
        {
            //Debug.Log(" === 点击了开始按钮 ===");
            App.GetGameManager<DuifenGameManager>().SendRequest(GameRequestType.CouldStart, null);
        }


        public override void ShowSelfPointLabel()
        {
            ShowPointLabel();
        }
        


        public override void ShowPointLabel(int point = 0)
        {
            if (!ReadyState || PlayerType == 3)
                return;

            if (point > 0)
            {
                base.ShowPointLabel(point);
                SelfPointLabel.gameObject.SetActive(false);
            }
            else
            {
                SelfPointLabel.text = UserBetPoker.HandPokerPoint + "点";
                SetGameObjectActive(SelfPointLabel.gameObject,true);
            }
        }


        public override void PlayerFold()
        {
            base.PlayerFold();
            _pokerPointLabel.gameObject.SetActive(false);
        }

    

        public override void OnUserReady()
        {
            base.OnUserReady();
            var gdata = App.GetGameData<DuifenGlobalData>();
            ReadyBtn.gameObject.SetActive(false);
            
            SetGameObjectActive(StartBtn.gameObject,gdata.IsRoomGame && Info.Seat == 0 && !gdata.IsPlayed);
        }

        public void OnClickCancelAutoBtn()
        {
            App.GetRServer<DuifenGameServer>().SendRequest(GameRequestType.SystemAuto, null);
        }

        public override void Reset()
        {
            base.Reset();
            ReadyBtn.gameObject.SetActive(true);
            SelfPointLabel.gameObject.SetActive(false);
        }

        public override void OnGameStart()
        {
            base.OnGameStart();
            SetGameObjectActive(StartBtn.gameObject,false);
            SetGameObjectActive(ReadyBtn.gameObject, false);
        }

        void SetGameObjectActive(GameObject btn, bool active)
        {
            if (btn != null) btn.SetActive(active);
        }

        internal override void CouldStart()
        {
            if (StartBtn == null)
                return;

            StartBtn.gameObject.SetActive(true);
            StartBtn.GetComponent<BoxCollider>().enabled = true;
            StartBtn.state = UIButtonColor.State.Normal;
        }
    }
}