using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ClickUISound : MonoBehaviour
    {
        private float bili = 0.159f; //屏幕比例，点击这以下无声音
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.mousePosition.y / Screen.height > bili)
                {
                    MahjongUtility.PlayEnvironmentSound("uiclick");
                }
            }
        }
    }
}
