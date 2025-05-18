using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Fishing.commons
{
    /// <summary>
    /// 新手引导
    /// </summary>
    public class YxGuide : YxView
    {
        /// <summary>
        /// 各随特效
        /// </summary>
        public Transform Effect;
        public Material MainMaterial; 
        public RectTransform RectTransform;
        void Awake()
        {
            Facade.EventCenter.AddEventListeners<EFishingEventType,Vector4>(EFishingEventType.Guide,SetPoint);
        }

        void Start()
        {
            ResizeSize();
        }

        public void ResizeSize()
        {
            var rect = RectTransform.rect;
            MainMaterial.SetInt("_SceneWidth", (int)rect.width);
            MainMaterial.SetInt("_SceneHeight", (int)rect.height);
        }

        public void SetPoint(Vector4 origin)
        {
            Show();
            var pos = Effect.position;
            pos.x = origin.x;
            pos.y = origin.y;
            Effect.position = pos;
            var localPos = Effect.localPosition;
            origin.x = localPos.x;
            origin.y = localPos.y; 
            MainMaterial.SetVector("_Origin", origin);
        }
         
    }
}
