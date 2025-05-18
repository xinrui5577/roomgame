using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.car
{
    public class CarTrendView : YxView
    {
        public EventObject EventObj;
        public UIScrollBar ScrollBar;
        public UIGrid RecordGrid;
        public UISprite RecordItem;

        private CarGameData _gdata
        {
            get
            {
                return App.GetGameData<CarGameData>();
            }
        }

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Fresh":
                    FreshRecordView(_gdata.RecordDatas);
                    break;
            }
        }

        private void FreshRecordView(List<int> record)
        {
            var recordCount = record.Count;

            if (recordCount == 0 || RecordGrid == null) return;

            var recordEnough = false;

            if (RecordGrid.transform.childCount == 20)
            {
                recordEnough = true;
            }
            else
            {
                while (RecordGrid.transform.childCount > 0)
                {
                    DestroyImmediate(RecordGrid.transform.GetChild(0).gameObject);
                }
            }

            var index = 0;

            if (recordCount > 20)
            {
                recordCount -= 20;
                for (int i = recordCount; i < record.Count; i++)
                {
                    UISprite item;
                    if (recordEnough)
                    {
                        item = RecordGrid.transform.GetChild(index).GetComponent<UISprite>();
                        index++;
                    }
                    else
                    {
                        item = YxWindowUtils.CreateItem(RecordItem, RecordGrid.transform);

                    }
                    item.spriteName = string.Format("trend{0}", record[i]);
                    item.name = i.ToString();
                    item.MakePixelPerfect();
                    item.transform.GetChild(0).gameObject.SetActive(i == record.Count - 1);
                }
            }
            else
            {
                for (int i = 0; i < recordCount; i++)
                {
                    var item = YxWindowUtils.CreateItem(RecordItem, RecordGrid.transform);
                    item.spriteName = string.Format("trend{0}", record[i]);
                    item.name = i.ToString();
                    item.MakePixelPerfect();
                    item.transform.GetChild(0).gameObject.SetActive(i == record.Count - 1);
                }
            }
         
            RecordGrid.repositionNow = true;
            Invoke("FreshScrollBar",1);
        }

        private void FreshScrollBar()
        {
            ScrollBar.value = 1;
        }
    }
}
