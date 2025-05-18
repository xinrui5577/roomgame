using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.hg
{
    public class HgBankListView : YxView
    {
        public EventObject EventObj;
        public UILabel TotalBank;
        public UIGrid BankGrid;
        public UILabel BankItem;

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "FreshBankList":
                    FreshView((BankListData) data.Data);
                    break;
            }
        }

        private void FreshView(BankListData bankListData)
        {
            TotalBank.text = string.Format("{0}人", bankListData.BankList.Count);
            List<UILabel> bankerLists = new List<UILabel >();
            foreach (Transform child in BankGrid.transform)
            {
                UILabel item = child.GetComponent<UILabel>();
                if (item)
                {
                    item.gameObject.SetActive(false);
                    bankerLists.Add(item);
                }
            }

            for (int i = 0; i < bankListData.BankList.Count; i++)
            {
                UILabel  bankItem;
                if (bankerLists.Count > 0 && bankerLists[0] != null)
                {
                    bankItem = bankerLists[0];
                    bankItem.gameObject.SetActive(true);
                    bankerLists.RemoveAt(0);
                }
                else
                {
                    bankItem = YxWindowUtils.CreateItem(BankItem, BankGrid.transform);
                }

                bankItem.text = bankListData.BankList[i].UserName;
            }

            BankGrid.Reposition();
            BankGrid.repositionNow = true;
        }

    }
}
