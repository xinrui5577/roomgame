using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class DownUIRightBg : MonoBehaviour
    {
        public Transform[] CoinTypeEffs = new Transform[6];
 
        public void ShowCoinTypeEffect(int index)
        {
            for (var i = 0; i < 6; i++)
            {
                CoinTypeEffs[i].gameObject.SetActive(i == index);
            }
        }

    }
}

