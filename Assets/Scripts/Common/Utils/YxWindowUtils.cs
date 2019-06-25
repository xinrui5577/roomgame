using UnityEngine;

namespace Assets.Scripts.Common.Utils
{
    /// <summary>
    /// 窗口辅助类
    /// </summary>
    public class YxWindowUtils{
        /// <summary>
        /// 创建Grid Item
        /// </summary>
        /// <typeparam name="T">要克隆的预制体的脚本类型</typeparam>
        /// <param name="itemPrefab">要克隆的预制体</param>
        /// <param name="pts">父容器</param>
        /// <param name="localPos">相对父容器的位置</param>
        /// <returns></returns>
        public static T CreateItem<T>(T itemPrefab, Transform pts, Vector3 localPos = default(Vector3)) where T : MonoBehaviour
        {
            var item = Object.Instantiate(itemPrefab);
            var tabelTs = item.transform;
            tabelTs.parent = pts;
            tabelTs.gameObject.SetActive(true);
            tabelTs.localScale = Vector3.one;
            tabelTs.localPosition = localPos;
            tabelTs.localRotation = Quaternion.identity;
            return item;
        }

        /// <summary>
        /// 创建GameObject
        /// </summary>
        /// <param name="gobj">要克隆的预制体</param>
        /// <param name="pts">父容器</param>
        /// <param name="localPos">相对父容器的位置</param>
        /// <returns></returns>
        public static GameObject CreateGameObject(GameObject gobj, Transform pts, Vector3 localPos = default(Vector3))
        {
            var item = Object.Instantiate(gobj);
            var tabelTs = item.transform;
            tabelTs.parent = pts;
            tabelTs.gameObject.SetActive(true);
            tabelTs.localScale = Vector3.one;
            tabelTs.localPosition = localPos;
            tabelTs.localRotation = Quaternion.identity;
            return item;
        }

        /// <summary>
        /// 创建Grid实例
        /// </summary>
        /// <param name="pGridPerfab"></param>
        /// <param name="grid"></param>
        public static void CreateItemGrid(UIGrid pGridPerfab, ref UIGrid grid)
        {
            if (grid != null) Object.Destroy(grid.gameObject);
            var perfabTs = pGridPerfab.transform;
            grid = Object.Instantiate(pGridPerfab);
            var ts = grid.transform;
            ts.parent = perfabTs.parent;
            ts.gameObject.SetActive(true);
            ts.localPosition = new Vector3(0, -2, 0);
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 创建item容器实例
        /// </summary>
        /// <param name="itemPerfab"></param>
        /// <param name="itemAttribute"></param>
        /// <param name="tsParent"></param>
        public static void CreateItemParent<T>(T itemPerfab, ref T itemAttribute, Transform tsParent) where T : Transform
        {
            if (itemAttribute != null) Object.Destroy(itemAttribute.gameObject);
            itemAttribute = Object.Instantiate(itemPerfab);
            var ts = itemAttribute.transform;
            ts.parent = tsParent;
            ts.gameObject.SetActive(true);
            ts.localPosition = new Vector3(0, -2, 0);
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 隐藏指定ui
        /// </summary>
        public static void DisplayUI(GameObject[] array, bool IsDisplay = true)
        {
            var count = array.Length;
            for (var i = 0; i < count; i++)
            {
                var go = array[i];
                if (go == null) continue;
                if (go.activeSelf == IsDisplay) continue;
                go.SetActive(IsDisplay);
            }
        }
    }
}
