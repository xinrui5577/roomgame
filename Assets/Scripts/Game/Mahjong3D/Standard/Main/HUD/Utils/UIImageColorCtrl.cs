using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UIImageColorCtrl : MonoBehaviour
    {
        public Image[] Images;

        public void SetImagesColor(Color color)
        {
            for (int i = 0; i < Images.Length; i++)
            {
                Images[i].color = color;
            }
        }
    }
}