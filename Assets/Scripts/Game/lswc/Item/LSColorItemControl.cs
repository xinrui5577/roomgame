using System.Collections;
using Assets.Scripts.Game.lswc.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Item
{
    public class LSColorItemControl : LSItemControlBase
    {
        public void ResetLayout()
        {
            transform.localEulerAngles = Vector3.zero;
            ResetItems();
        }

        public void ResetItems()
        {
            StartCoroutine(TweenColor());
        }

        private int GetLastIndex(LSColorType type)
        {
            int index = Items.FindLastIndex(delegate(LSItemBase Obj)
                {
                    LSColorItem item = (LSColorItem) Obj;
                    return item.IsRightColor(type);
                });
            if (index < 0 || index > Items.Count)
            {
                //Debug.LogError("Not have such item,type is "+type+" index  is "+ index);
                index = -1;
            }
            return index;
        }

        public override void SetRadius()
        {
            _raidus = LSConstant.Num_ColorRadius;
        }

        protected override void InitOther()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                LSColorItem item = (LSColorItem)Items[i];
                LSColorType type = App.GetGameData<LswcGameData>().Colors[i];
                item.SetColorType(type);
                item.name = item.CurColor.ToString();
            }
        }

        /// <summary>
        /// 颜色刷新帧率
        /// </summary>
        private float _tweenFrame = 4;

        private IEnumerator TweenColor()
        {
            for (int i = 0; i <Items.Count; i++)
            {
                LSColorItem item = (LSColorItem)Items[i];
                if (item)
                {
                    item.SetColorType(LSColorType.DEFAULT);
                }
                
                yield return new WaitForSeconds(_tweenFrame*Time.deltaTime);
            }
            for (int i = 0; i < Items.Count; i++)
            {
                LSColorItem item = (LSColorItem)Items[i];

                LSColorType type = App.GetGameData<LswcGameData>().Colors[i];

                item.SetColorType(type);

                yield return new WaitForSeconds(_tweenFrame * Time.deltaTime);
            }
        }

    }
}
