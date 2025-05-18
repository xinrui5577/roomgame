using Assets.Scripts.Game.Texas.Main;
using YxFramwork.Common;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.Texas
{
    public class SelfPanel : PlayerPanel
    {
        public UIButton ReadyBtn;

        public void SetReadyBtnActive(bool acitve)
        {
            if (ReadyBtn == null) return;
            ReadyBtn.gameObject.SetActive(acitve);
        }

        protected override void SetReadyState(bool isReady)
        {
            base.SetReadyState(isReady);
            if (ReadyBtn == null) return;
            var isGameStart = App.GameData.IsGameStart;
       
            ReadyBtn.gameObject.SetActive(!isReady && !isGameStart);
        }


        //添加准备按钮的OnClick事件
        public void OnClickReadyBtn()
        {
            var gdata = App.GetGameData<TexasGameData>();
            var selfInfo = gdata.GetPlayerInfo();
            var curGold = gdata.Ante*10;
            if (selfInfo.RoomCoin < curGold)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "无法准备,请先点击右上角添加筹码至" + YxUtiles.ReduceNumber(curGold) + "以上",
                    Delayed = 3,
                });
            }
            else
            {
                //发送准备消息给后台
                App.GetRServer<TexasGameServer>().SendReadyGame();
            }
        }
    }
}


