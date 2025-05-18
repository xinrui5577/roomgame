using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class CameraRatioConfig : RatioConfig
    {
        public float CameraFieldofview;
        public Vector3 CameraPosition;
        public Vector3 CameraRotation;
    }

    public class CameraAdaptation : AdaptationBase<CameraRatioConfig>
    {
        protected override void Adptation()
        {
            Camera camera = transform.GetComponent<Camera>();
            switch (camera.transparencySortMode)
            {
                case TransparencySortMode.Perspective:
                    camera.fieldOfView = mCurrConfig.CameraFieldofview;
                    break;
                case TransparencySortMode.Orthographic:
                    camera.orthographicSize = mCurrConfig.CameraFieldofview;
                    break;
            }
            camera.transform.localPosition = mCurrConfig.CameraPosition;
            camera.transform.localRotation = Quaternion.Euler(mCurrConfig.CameraRotation);
        }
    }
}