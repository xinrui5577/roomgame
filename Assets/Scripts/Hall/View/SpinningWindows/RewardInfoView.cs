/** 
 *文件名称:     RewardInfoView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-12-12 
 *描述:         奖励查看面板
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Hall.View.Agency;
using UnityEngine;

namespace Assets.Scripts.Hall.View.SpinningWindows
{
    public class RewardInfoView : YxNguiWindow
    {
        [Tooltip("对应类型视图")]
        public GameObject[] Views;
        [Tooltip("地址信息")]
        public AgencySingleItem[] AdressItems;
        [Tooltip("充值卡信息")]
        public UILabel[] CardItems;
        [Tooltip("兑换虚拟物品布局Grid")]
        public UIGrid LayoutGrid;
        [Tooltip("兑换虚拟物品Prefab")]
        public GameObject ItemPrefab;
        /// <summary>
        ///兑换消息指令
        /// </summary>
        private string _keyAwardMes = "awardMes";
        /// <summary>
        /// 电话指令
        /// </summary>
        private string _keyPhone = "phone";
        /// <summary>
        /// 地址指令
        /// </summary>
        private string _keyAddress = "address";
        /// <summary>
        /// 名称指令
        /// </summary>
        private string _keyName = "name";
        /// <summary>
        /// 奖励数据指令
        /// </summary>
        private string _keyData = "data";
        /// <summary>
        /// 奖励类型指令
        /// </summary>
        private string _keyType = "type";
        /// <summary>
        /// 数据类型指令
        /// </summary>
        private string _keyNum = "num";
        protected override void OnFreshView()
        {
            if(Data==null)
            {
                return;
            }
            Dictionary<string, object> dics = (Dictionary<string, object>)Data;
            int type = int.Parse(dics[_keyType].ToString());
            Views[type].SetActive(true);
            Dictionary<string, object> awardMes = (Dictionary<string, object>)dics[_keyAwardMes];
            switch (type)
            {
                case 0:
                    ShowAddress(awardMes);
                    break;
                case 1:
                    ShowRewards(awardMes);
                    break;
                case 2:
                    ShowCardInfo(awardMes);
                    break;
            }
            CallBack(Data);
        }
        private void ShowAddress(Dictionary<string, object> dic)
        {
            int index = 0;
            foreach (var itemData in dic)
            {
                AgencySingleItem item = AdressItems[index++];
                item.Refresh(null, itemData.Value.ToString());
            }
        }
        private void ShowRewards(Dictionary<string, object> dic)
        {
            List<object> datas = (List<object>) dic[_keyData];
            while (LayoutGrid.transform.childCount > 0)
            {
                DestroyImmediate(LayoutGrid.transform.GetChild(0).gameObject);
            }
            for (int i = 0,count=datas.Count; i < count; i++)
            {
                Dictionary<string, object> itemData = (Dictionary<string, object>) datas[i];
                GameObject obj = NGUITools.AddChild(LayoutGrid.gameObject, ItemPrefab);
                if (obj)
                {
                    RewardItem item = obj.GetComponent<RewardItem>();
                    item.Init(itemData[_keyType].ToString(), int.Parse(itemData[_keyNum].ToString()));
                }
            }
            LayoutGrid.repositionNow = true;
        }

        private void ShowCardInfo(Dictionary<string, object> dic)
        {
            int index = 0;
            foreach (var itemData in dic)
            {
                UILabel item = CardItems[index++];
                item.text = itemData.Value.ToString();
            }
        }
    }
}
