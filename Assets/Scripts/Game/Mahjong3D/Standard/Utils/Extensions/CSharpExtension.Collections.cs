using System.Collections.Generic;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public static partial class CSharpExtension
    {
        /// <summary>
        /// 交换
        /// </summary>
        /// <param name="currIndex">当前索引</param>
        /// <param name="targetIndex">目标所有</param>
        public static void ExeExchange<T>(this IList<T> list, int currIndex, int targetIndex)
        {
            if (currIndex == targetIndex)
            {
                return;
            }
            if (list != null || list.Count > 0)
            {
                var temp = list[currIndex];
                list[currIndex] = list[targetIndex];
                list[targetIndex] = temp;
            }
        }

        public static bool ExIterationAction<T>(this IList<T> list, Action<T> Action)
        {
            int count = list.Count;
            if (0 == count) return false;
            for (int i = 0; i < count; i++)
            {
                if (null != Action)
                {
                    Action(list[i]);
                }
            }
            return true;
        }

        public static bool ExIterationAction<TKey, TValue>(this Dictionary<TKey, TValue> Dic, Action<KeyValuePair<TKey, TValue>> Action)
        {
            if (0 == Dic.Count) return false;
            var container = Dic.GetEnumerator();
            while (container.MoveNext())
            {
                var curr = container.Current;
                if (null != Action)
                {
                    Action(curr);
                }
            }
            return true;
        }

        public static T ExGet<T>(this IList<T> list, int index)
        {
            if (list.Count > index && index >= 0)
            {
                var item = list[index];
                if (null == item)
                {
                    while (null == item && index >= 0 && list.Count > 0)
                    {
                        index--;
                        item = list[index];
                        list.RemoveAt(index);
                    }
                }
                else
                {
                    return item;
                }
            }
            return default(T);
        }
    }
}