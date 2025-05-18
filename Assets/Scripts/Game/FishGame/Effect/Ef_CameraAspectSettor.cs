using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    /// <summary>
    /// 根据Defines.OriginWidthUnit 和 Defines.OriginWidthUnit 来调整摄像机长宽
    /// </summary>
    public class Ef_CameraAspectSettor : MonoBehaviour { 
        public bool IsAffectByScreenNum = true;//是否受屏幕数影响
        void Awake () {

            float aspect = (float)Defines.OriginWidthUnit / Defines.OriginHeightUnit;//16 : 9
            if (IsAffectByScreenNum)
                GetComponent<Camera>().aspect = aspect*1;//GameMain.Singleton.ScreenNumUsing;
            else
                GetComponent<Camera>().aspect = aspect;
        }

        void Start()
        {
        }
    }
}
