/** 
 *文件名称:     MahjongGroupItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-23 
 *描述:         组牌单位
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game.Data;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.UI;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Item
{
    public class MahjongGroupItem : MonoBehaviour
    {
        /// <summary>
        ///     组牌数据
        /// </summary>
        private MahjongGroupData _data;

        /// <summary>
        ///     组牌所在堆，组堆....
        /// </summary>
        private GroupPile _parentPile;

        public List<MahjongItem> ItemList= new List<MahjongItem>();

        /// <summary>
        ///     类型
        /// </summary>
        public GroupType CurrentType
        {
            get { return _data.type; }
        }

        /// <summary>
        ///     值
        /// </summary>
        public int[] Values
        {
            get { return _data.values; }
            set { _data.values = value; }
        }

        /// <summary>
        ///     所在堆
        /// </summary>
        public GroupPile ParentPile
        {
            get
            {
                if (_parentPile == null)
                {
                    _parentPile = GetComponentInParent<GroupPile>();
                }
                return _parentPile;
            }
            set { _parentPile = value; }
        }

        /// <summary>
        ///     设置组牌中的UI显示
        /// </summary>
        /// <param name="data"></param>
        /// <param name="IsOther"></param>
        /// <returns></returns>
        public virtual Transform SetGroup(MahjongGroupData data, List<MahjongItem> items, float groupWidth,
            float groupHeight, float fourOffsetY, bool IsOther, DefLayout.Directon direction, bool changeNum = true)
        {
            _data = data;
            var values = data.values;
            var pos = Vector3.zero;
            if (items == null)
            {
                items = new List<MahjongItem>();
                foreach (var value in values)
                {
                    items.Add(GameTools.CreateMahjong(value, changeNum).GetComponent<MahjongItem>());
                }
            }
            var isHorizontal = direction.Equals(DefLayout.Directon.Horizontal);
            for (int i = 0, lenth = values.Length; i < lenth; i++)
            {
                var item = items[i];
                if (item == null)
                {
                    YxDebug.LogError("找不到一张牌了？");
                }
                var newTran = item.transform;
                switch (data.type)
                {
                    case GroupType.Chi:
                    case GroupType.Peng:
                    case GroupType.MingGang:
                    case GroupType.ZhuaGang:
                    case GroupType.PengGang:
                        SetNewItem(item, values[i], EnumMahJongAction.Lie);
                        break;
                    case GroupType.AnGang:
                    case GroupType.FengGang:
                        if (IsOther)
                        {
                            SetNewItem(item, values[i], EnumMahJongAction.Push);
                        }
                        else
                        {
                            if (i != 3)
                            {
                                SetNewItem(item, values[i], EnumMahJongAction.Push);
                            }
                            else
                            {
                                SetNewItem(item, values[i], EnumMahJongAction.Lie);
                            }
                        }
                        break;
                    case GroupType.JueGang:
                        if (IsOther)
                        {
                            SetNewItem(item, values[i], EnumMahJongAction.Push);
                        }
                        else
                        {
                            if (i != 1)
                            {
                                SetNewItem(item, values[i], EnumMahJongAction.Push);
                            }
                            else
                            {
                                SetNewItem(item, values[i], EnumMahJongAction.Lie);
                            }
                        }
                        break;
                    default:
                        newTran = null;
                        YxDebug.LogError("There is not exist such GroupType " + data.type);
                        break;
                }
                if (i == 3)
                {
                    if (isHorizontal)
                    {
                        pos.x = 1*groupWidth;
                        pos.y = pos.y + fourOffsetY;
                    }
                    else
                    {
                        pos.y = 1*groupHeight + fourOffsetY;
                    }
                }
                else
                {
                    if (isHorizontal)
                    {
                        pos.x = i*groupWidth;
                    }
                    else
                    {
                        pos.y = i*groupHeight;
                    }
                }
                newTran.localPosition = pos;
            }
            return transform;
        }

        public void RefreshGroup(MahjongItem addItem)
        {
            addItem.SelfData.Action = EnumMahJongAction.Lie;
        }

        public static MahjongGroupItem CreateGroup()
        {
            var GroupItem = new GameObject();
            var group = GroupItem.AddComponent<MahjongGroupItem>();
            return group;
        }

        public void SetNewItem(MahjongItem item, int value, EnumMahJongAction action)
        {
            item.Value = value;
            item.SelfData.Action = action;
            item.SetColor(Color.white);
            GameTools.AddChild(transform, item.transform);
            ItemList.Add(item);
        }
    }
}