using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public static partial class UnityExtension
    {
        /// <summary>
        /// 设置组件父对象,并且设置层级和重置位置
        /// </summary>
        public static T ExSetParent<T>(this T component, Transform parent) where T : Component
        {
            component.transform.SetParent(parent);
            component.gameObject.layer = parent.gameObject.layer;
            component.ExLocalIdentity();
            return component;
        }

        /// <summary>
        /// 相对的父节坐标点重置
        /// </summary>
        public static T ExLocalIdentity<T>(this T component) where T : Component
        {
            component.transform.localRotation = Quaternion.Euler(Vector3.zero);
            component.transform.localPosition = Vector3.zero;
            component.transform.localScale = Vector3.one;
            return component;
        }

        /// <summary>
        ///  相对的世界坐标重置
        /// </summary>
        public static T ExIdentity<T>(this T component) where T : Component
        {
            component.transform.rotation = Quaternion.Euler(Vector3.zero);
            component.transform.localScale = Vector3.one;
            component.transform.position = Vector3.zero;
            return component;
        }

        /// <summary>
        /// 设置本地坐标
        /// </summary>
        public static T ExLocalPosition<T>(this T component, Vector3 localPos) where T : Component
        {
            component.transform.localPosition = localPos;
            return component;
        }

        public static T ExLocalRotation<T>(this T component, Vector3 localPos) where T : Component
        {
            component.transform.localRotation = Quaternion.Euler(localPos);
            return component;
        }

        public static T ExLocalScale<T>(this T component, Vector3 localPos) where T : Component
        {
            component.transform.localScale = localPos;
            return component;
        }

        /// <summary>
        /// 递归找子集
        /// </summary>         
        public static Transform ExFindChildRecursion<T>(this T component, System.Func<Transform, bool> predicate) where T : Component
        {
            if (predicate(component.transform))
            {
                return component.transform;
            }
            foreach (Transform child in component.transform)
            {
                Transform temp = null;
                temp = child.ExFindChildRecursion(predicate);
                if (temp)
                {
                    return temp;
                }
            }
            return null;
        }

        public static string ExGetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }
    }
}