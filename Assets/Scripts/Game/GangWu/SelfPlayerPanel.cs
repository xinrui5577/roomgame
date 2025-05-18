using Assets.Scripts.Game.GangWu.Main;
using YxFramwork.Common;

namespace Assets.Scripts.Game.GangWu
{
    public class SelfPlayerPanel : PlayerPanel {

        public UIButton ReadyBtn;

        protected override void OnStart()
        {
            base.OnStart();
            if (ReadyBtn != null)
            {
                ReadyBtn.onClick.Add(new EventDelegate(() =>
                {
                    App.GetRServer<GangWuGameServer>().ReadyGame();
                }));
            }
        }


        /// <summary>
        /// 设置玩家的准备状态
        /// </summary>
        /// <param name="state">准备状态</param>
        internal override void SetPlayerReadyState(bool state)
        {
            base.SetPlayerReadyState(state);
            ReadyBtn.gameObject.SetActive(!App.GameData.IsGameStart && !state);
        }

        internal void OnGameStart()
        {
            if (ReadyBtn == null) return;
            ReadyBtn.gameObject.SetActive(false);
        }

    }
}
