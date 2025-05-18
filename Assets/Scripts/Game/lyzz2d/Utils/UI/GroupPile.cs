/** 
 *文件名称:     GroupPile.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         组牌牌堆，这个也需要特殊处理，控制显示组牌布局
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game.Data;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    public class GroupPile : MahjongPile
    {
        private readonly List<MahjongGroupItem> _pileList = new List<MahjongGroupItem>();

        [SerializeField] public int FourOffsetY = 15;

        [SerializeField] public float GroupItemHeight = 112;

        [SerializeField] public float GroupItemWidth = 78;

        public virtual MahjongGroupItem AddGroup(MahjongGroupData groupData, List<MahjongItem> items, bool isOther,
            bool changeNumber = true)
        {
            var Item = MahjongGroupItem.CreateGroup();
            Item.ParentPile = this;
            Item.SetGroup(groupData, items, GroupItemWidth, GroupItemHeight, FourOffsetY, isOther, Layout.directon);
            _pileList.Add(Item);
            AddItem(Item.transform);
            ParseItemToThis(Item.transform);
            return Item;
        }

        /// <summary>
        ///     组牌变化,这里基本只有抓杠才有的情况，组牌位置不变，只刷新组数据
        /// </summary>
        /// <param name="groupData"></param>
        /// <param name="isOther"></param>
        public virtual void ChangeGroup(int value, MahjongItem addItem)
        {
            var isHorizontal = Layout.directon.Equals(DefLayout.Directon.Horizontal);
            var pos = Vector3.zero;
            if (isHorizontal)
            {
                pos.x = 1*GroupItemWidth;
                pos.y = pos.y + FourOffsetY;
            }
            else
            {
                pos.y = 1*GroupItemHeight + FourOffsetY;
            }
            for (int i = 0, lenth = _pileList.Count; i < lenth; i++)
            {
                var group = _pileList[i];
                if (group.Values[0].Equals(value) && group.CurrentType.Equals(GroupType.Peng))
                {
                    group.Values = new int[4];
                    for (int j = 0, groupLenth = group.Values.Length; j < groupLenth; j++)
                    {
                        group.Values[j] = value;
                    }
                    group.SetNewItem(addItem, value, EnumMahJongAction.Lie);
                    group.RefreshGroup(addItem);
                    addItem.transform.localPosition = pos;
                    ParseItemToThis(group.transform);
                    return;
                }
            }
        }

        public MahjongGroupItem GetGroupItem(int value)
        {
            var returnGroup = _pileList.Find(item => item.Values[0].Equals(value));
            if (returnGroup != null)
            {
                return returnGroup;
            }
            YxDebug.LogError("没有找到对应的组牌" + value);
            return null;
        }

        public void TryRefreshGroups()
        {
            var itemVec = new Vector3(_localScaleX, _localScaleY);
            foreach (var group in _pileList)
            {
                foreach (var item in group.ItemList)
                {
                    if (!item.transform.localScale.Equals(itemVec))
                    {
                        item.transform.localScale = itemVec;
                    }
                }
            }
        }

        protected override void ParseItemToThis(Transform trans)
        {
            var item = trans.GetComponent<MahjongGroupItem>();
            var list = item.ItemList;
            var groupMaxLayer = 0;
            for (int i = 0, lenth = list.Count; i < lenth; i++)
            {
                var mJItem = list[i];
                mJItem.SelfData.ShowDirection = ItemShow;
                mJItem.SelfData.Direction = ItemDirection;
                mJItem.SelfData.MahjongLayer = NowLayer;
                if (groupMaxLayer < NowLayer)
                {
                    groupMaxLayer = NowLayer;
                }
                mJItem.transform.localScale = new Vector3(_localScaleX, _localScaleY);
                mJItem.gameObject.SetActive(false);
                mJItem.gameObject.SetActive(true);
            }
            if (list.Count == 4)
            {
                list[3].SelfData.MahjongLayer = groupMaxLayer + 4; //将第四张麻将层级集体拉高
            }
        }

        /// <summary>
        ///     组牌的添加处理
        /// </summary>
        /// <param name="item"></param>
        /// <param name="auto"></param>
        public override void AddItem(Transform item, bool auto = true)
        {
            Layout.AddItem(item, auto);
        }

        public override void ResetPile()
        {
            base.ResetPile();
            _pileList.Clear();
        }
    }
}