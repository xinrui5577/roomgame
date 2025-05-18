using Assets.Scripts.Game.paijiu.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;


namespace Assets.Scripts.Game.paijiu
{
    public class PaiJiuPlayerSelf : PaiJiuPlayer
    {

        public UIButton ReadyBtn;

        public UIButton StartBtn;

        public UILabel SelfPointLabel;

       
        protected override void ShowUserInfo()
        {
            base.ShowUserInfo();
            var gdata = App.GetGameData<PaiJiuGameData>();
            var userInfo = (PaiJiuUserInfo) Info;
            bool showReady = !userInfo.State && !gdata.IsGameing;

            if(ReadyBtn != null)
            {
                ReadyBtn.gameObject.SetActive(showReady);
            }

        }


        public void OnClickReadyBtn()
        {
            App.GetRServer<PaiJiuGameServer>().ReadyGame();
        }

        public void OnClickStartBtn()
        {
            //Debug.Log(" === 点击了开始按钮 ===");
            App.GetRServer<PaiJiuGameServer>().SendRequest(GameRequestType.AllowStart, null);
        }
        

        public override void OnUserReady()
        {
            base.OnUserReady();
            var gdata = App.GetGameData<PaiJiuGameData>();
            ReadyBtn.gameObject.SetActive(false);

            if (StartBtn != null)
            {
                com.yxixia.utile.YxDebug.YxDebug.Log("IsRoomGame == {0} , IsPlayed  == {1}", "StartBtn", null, gdata.IsRoomGame, !gdata.IsPlayed);
                StartBtn.gameObject.SetActive(gdata.IsRoomGame && Info.Seat == 0 && !gdata.IsPlayed);
            }
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
            StartBtn.gameObject.SetActive(false);
            ReadyBtn.gameObject.SetActive(false);
        }

        internal override void CouldStart()
        {
            if (/*!App.GetGameData < PaiJiuGameData >().IsRoomGame || */StartBtn == null)
                return;

            StartBtn.gameObject.SetActive(true);
            StartBtn.GetComponent<BoxCollider>().enabled = true;
            StartBtn.state = UIButtonColor.State.Normal;
        }

        internal override void FinishSelect()
        {
            UserBetPoker.CleanSelected();
            UserBetPoker.CleanCardsBoxCollider();
            UserBetPoker.SetBetPokerInfo(new int[4]);
            UserBetPoker.HideAllTypeLabel();
            base.FinishSelect();

        }

    }
}