using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public static partial class UnityExtension
    {
        /// <summary>
        /// 控制组件显隐
        /// </summary>      
        public static T ExCompSetActive<T>(this T component, bool isOn) where T : Component
        {           
            component.gameObject.SetActive(isOn);
            return component;
        }

        /// <summary>
        /// 组件显示
        /// </summary>   
        public static T ExCompShow<T>(this T component) where T : Component
        {           
            component.gameObject.SetActive(true);
            return component;
        }

        /// <summary>
        /// 组件隐藏
        /// </summary>   
        public static T ExCompHide<T>(this T component) where T : Component
        {
            component.gameObject.SetActive(false);
            return component;
        }

        /// <summary>
        /// 组件层级
        /// </summary>  
        public static T ExSetLayer<T>(this T component, int layer) where T : Component
        {
            component.gameObject.layer = layer;
            return component;
        }

        /// <summary>
        /// 组件层级
        /// </summary>  
        public static T ExSetLayer<T>(this T component, string layerName) where T : Component
        {
            component.gameObject.layer = LayerMask.NameToLayer(layerName);
            return component;
        }
    }
}