using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class WmbbmjLaozhuang : MonoBehaviour, IGameStartCycle
    {
        public Text[] Texts;

        private void Awake()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnGameStartCycle()
        {
            var datas = GameCenter.DataCenter;
            var bankChair = datas.BankerChair;
            var laozhuang = datas.Game.Laozhuang;

            for (int i = 0; i < Texts.Length; i++)
            {
                var flag = i == bankChair;
                Texts[i].gameObject.SetActive(flag);
                Texts[i].text = flag ? "x" + laozhuang : "";
            }
        }

        private void OnDestroy()
        {
            GameCenter.RemoveRegisterCycle(this);
        }
    }
}