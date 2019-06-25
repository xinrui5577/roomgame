using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 客服界面
    /// </summary>
    public class CustomerServiceWindow : YxNguiWindow
    {
        /// <summary>
        /// 交互的名字
        /// </summary>
        [Tooltip("交互的名字")]
        public string ActionName = "getkefuInfo";
        /// <summary>
        /// 客服的prefab
        /// </summary>
        [Tooltip("客服的prefab")]
        public KeyValueView KeyValueItemPrefab;
        /// <summary>
        /// items容器
        /// </summary>
        [Tooltip("items容器")]
        public GameObject ShowParent;
        /// <summary>
        /// Item间隙
        /// </summary>
        [Tooltip("Item间隙")]
        public int CellHeight = 40; 
        /// <summary>
        /// 起始位置
        /// </summary>
        [Tooltip("起始位置X")]
        public Vector2 StartPos = new Vector2(20,-50);

        protected override void OnAwake()
        {
            Facade.Instance<TwManger>().SendAction(ActionName, new Dictionary<string, object>(), UpdateView);
        }

        protected override void OnFreshView()
        {
            YxWindowManager.HideWaitFor();
            if (Data==null)
            {
                YxDebug.Log("信息没有配置");
                return;
            }
            if (ShowParent == null) ShowParent = gameObject;
            var dict = Data as Dictionary<string, object>;
            if (dict == null) return;
            var height = StartPos.y;
            foreach (var pair in dict)
            {
                var item = NGUITools.AddChild(ShowParent, KeyValueItemPrefab.gameObject).GetComponent<KeyValueView>();
                item.gameObject.SetActive(true);
                var key = pair.Key;
                var value = pair.Value.ToString();
                if (key.IndexOf("版本", System.StringComparison.Ordinal) > -1)
                {
                    value = Application.version;
                }
                var data = new KeyValueData
                    {
                        KeyString = key,
                        ValueString = value
                    };
                
                item.UpdateView(data);
                item.transform.localPosition = new Vector3(StartPos.x, height);
                height -= (item.ItemHeight + CellHeight);           
            }
        }
    }
}
