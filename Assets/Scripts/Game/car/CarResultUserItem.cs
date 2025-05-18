using UnityEngine;

namespace Assets.Scripts.Game.car
{
    public class CarResultUserItem : MonoBehaviour
    {
        public UILabel UserName;
        public UILabel UserWinCoin;

        public void SetData(string userName,int userCoin)
        {
            UserName.text = userName;
            UserWinCoin.text = userCoin.ToString();
        }
    }
}
