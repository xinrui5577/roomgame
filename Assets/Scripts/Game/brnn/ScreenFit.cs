using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.brnn
{
    public class ScreenFit : MonoBehaviour
    {

        public Vector2 ScreenSize;

        protected void Start()
        {
            //根据平台改scalingStyle
            transform.GetComponent<UIRoot>().scalingStyle = Application.platform == RuntimePlatform.WindowsPlayer ? UIRoot.Scaling.Constrained : UIRoot.Scaling.ConstrainedOnMobiles;

#if UNITY_STANDALONE
            YxDebug.Log("w = " + Screen.currentResolution.width + " h = " + Screen.currentResolution.height);

            if (ScreenSize.x > Screen.currentResolution.width || ScreenSize.y > Screen.currentResolution.height)
            {
                float xRate = Screen.currentResolution.width / ScreenSize.x;
                float yRate = Screen.currentResolution.height / ScreenSize.y;

                ScreenSize.x *= xRate > yRate ? yRate : xRate;
                ScreenSize.y *= xRate > yRate ? yRate : xRate;
            }

            Screen.SetResolution((int)ScreenSize.x, (int)ScreenSize.y, false);
#endif
        }
    }
}
