using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.bjlb
{
    public class Girls : MonoBehaviour
    {
        public Animator GirlAn;
        public void Play()
        {
            if (App.GameData.GetPlayerInfo().CoinA < 100)
            {
                YxMessageBox.DynamicShow(new YxMessageBoxData { Msg = "金币不够,不能打赏." });
                return;
            }
            GirlAn.Play(0);
            App.GetRServer<BjlGameServer>().Reward();
            int a = Random.Range(0, 100) % 3;
            switch (a )
            {
                case 0:
                    Facade.Instance<MusicManager>().Play("aoman");
                    break;
                case 1:
                    Facade.Instance<MusicManager>().Play("dese");
                    break;
                case 2:
                    Facade.Instance<MusicManager>().Play("maimeng");
                    break;
            }
        

        }

    }
}
