using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class SzwmmjGameInfo : MonoBehaviour, IGameStartCycle, IGameEndCycle
    {
        public Text Text;

        private void Awake()
        {
            GameCenter.RegisterCycle(this);
        }

        private void OnDestroy()
        {
            GameCenter.RemoveRegisterCycle(this);
        }

        public void OnGameEndCycle()
        {
            Text.text = "";
            Text.gameObject.SetActive(false);
        }

        public void OnGameStartCycle()
        {
            SetInfo();
        }

        private void SetInfo()
        {
            var data = GameCenter.DataCenter.Game;
            var diling = data.Diling == 1;
            var baozi = data.Baozi == 1;
            var flag = diling || baozi;

            Text.gameObject.SetActive(flag);
            if (flag)
            {
                var str = "";
                if (diling) str += " 滴零";
                if (baozi) str += " 豹子";
                Text.text = str;
            }
        }
    }
}