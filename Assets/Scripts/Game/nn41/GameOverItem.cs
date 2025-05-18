using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.nn41
{
    public class GameOverItem : MonoBehaviour
    {
        public GameObject BigWinner;
        public UILabel UserName;
        public UILabel UserGold;

        public void InitData(string userName,int userGold)
        {
            gameObject.SetActive(true);
            UserName.text = userName;
            UserGold.text = YxUtiles.ReduceNumber(userGold);
        }

    }
}
