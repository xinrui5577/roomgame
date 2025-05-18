using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BankerSign : MonoBehaviour
    {
        public Image Banker;

        public void Set(bool isOn)
        {
            Banker.gameObject.SetActive(isOn);
        }        
    }
}