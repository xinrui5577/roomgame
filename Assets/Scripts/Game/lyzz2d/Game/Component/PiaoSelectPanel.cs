using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Game.Item.PiaoItem;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class PiaoSelectPanel : MonoSingleton<PiaoSelectPanel>
    {
        public override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
            for (int i = 0, max = transform.childCount; i < max; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            PiaoItem.OnItemSelect += SendPiaoRequest;
        }

        public void ShowGameObject(bool state = true)
        {
            if (App.GetGameManager<Lyzz2DGameManager>().SelfPlayer.UserInfo.Piao == 99)
            {
                return;
            }
            gameObject.SetActive(state);
        }

        public override void OnDestroy()
        {
            if (PiaoItem.OnItemSelect != null) PiaoItem.OnItemSelect -= SendPiaoRequest;
        }

        private void SendPiaoRequest(int num)
        {
            App.GetRServer<Lyzz2DGameServer>().OnSelectPiao(num);
            gameObject.SetActive(false);
        }
    }
}