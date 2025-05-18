using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.Game.Tbs
{
    public class GameOverItem : MonoBehaviour {

        public UILabel UserName;
        public UILabel UserGold;

        public void InitData(string userName, int userGold)
        {
            gameObject.SetActive(true);
            UserName.text = userName;
            UserGold.text = userGold.ToString(CultureInfo.InvariantCulture);
        }
    }
}
