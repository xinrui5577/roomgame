using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.Piao
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

        public virtual void ShowGameObject(bool state = true)
        {
            if (App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserInfo.Piao ==ConstantData.DefPiaoForever)
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
            App.GetRServer<Mahjong2DGameServer>().OnSelectPiao(num);
            gameObject.SetActive(false);
        }

    }
}
