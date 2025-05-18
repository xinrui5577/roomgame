/** 
 *文件名称:     MahjongGroupItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-23 
 *描述:         组牌单位
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.Common.UI;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    public class MahjongGroupItem : MonoBehaviour
    {
        List<MahjongItem> _itemList = new List<MahjongItem>();

        public List<MahjongItem> ItemList
        {
            get { return _itemList; }
        }
        /// <summary>
        /// 组牌数据 
        /// </summary>
        private MahjongGroupData _data;
        /// <summary>
        /// 组牌所在堆，组堆....
        /// </summary>
        private GroupPile _parentPile;
        /// <summary>
        /// 类型
        /// </summary>
        public GroupType CurrentType
        {
            get { return _data.type; }
        }
        /// <summary>
        /// 值
        /// </summary>
        public int[] Values
        {
            get { return _data.values; }
            set { _data.values = value; }
        }
        /// <summary>
        /// 所在堆
        /// </summary>
        public GroupPile ParentPile
        {
            get
            {
                if (_parentPile==null)
                {
                    _parentPile = GetComponentInParent<GroupPile>();
                }
                return _parentPile;
            }
            set { _parentPile = value; }
        }
        /// <summary>
        /// 设置组牌中的UI显示
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isOther"></param>
        /// <returns></returns>
        public virtual  List<MahjongItem> SetGroup(MahjongGroupData data, List<MahjongItem> items, float groupWidth,float groupHeight ,float fourOffsetY,bool isOther,Directon direction,bool changeNum=true)
        {
            _data = data;
            int[] values = data.values;
            Vector3 pos = Vector3.zero;
            if(items==null)
            {
                items=new List<MahjongItem>();
                foreach (var value in values)
                {
                    items.Add(GameTools.CreateMahjong(value,changeNum).GetComponent<MahjongItem>());
                }
            }
            bool isHorizontal = direction.Equals(Directon.Horizontal);
            for (int i = 0, lenth = values.Length; i < lenth; i++)
            {
                MahjongItem item = items[i];
                if (item == null)
                {
                    YxDebug.LogError("找不到一张牌了？");
                }
                Transform newTran = item.transform;
                switch (data.type)
                {
                    case GroupType.Chi:
                    case GroupType.Peng:
                    case GroupType.MingGang:
                    case GroupType.ZhuaGang:
                    case GroupType.PengGang:
                    case GroupType.CaiGang:
                    case GroupType.FengGang:
                        SetNewItem(item, values[i], EnumMahJongAction.Lie);
                        break;
                    case GroupType.AnGang:
                        if (isOther)
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
                        if (isOther)
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
                        SetNewItem(item, values[i], EnumMahJongAction.Lie);
                        break;
                }
                if (i == 3&& data.type!= GroupType.Other)
                {
                    if (isHorizontal)
                    {
                        pos.x = 1 * groupWidth;
                        pos.y = pos.y + fourOffsetY;
                    }
                    else
                    {
                        pos.y = 1 * groupHeight+fourOffsetY;
                    }
                }
                else
                {
                    if(isHorizontal)
                    {
                        pos.x = i * groupWidth;
                    }
                    else
                    {
                        pos.y = i * groupHeight;
                    }
                
                }
                newTran.localPosition = pos;
            }
            return items;
        }

        /// <summary>
        /// 设置乱风显示
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        public void SetLuanFengShow(MahjongGroupData data, List<MahjongItem> items)
        {
            if (data.type.Equals(GroupType.FengGang))//乱风显示处理
            {
                var count = data.values.Length;
                if (count == 4 && data.TopLandScape)
                {
                    var item = items[3];
                    var nowDirection = item.SelfData.Direction;
                    var nowShowDirection = item.SelfData.ShowDirection;
                    if (nowDirection == EnumMahJongDirection.Horizontal)
                    {
                        nowDirection = EnumMahJongDirection.Vertical;
                    }
                    else
                    {
                        nowDirection = EnumMahJongDirection.Horizontal;
                    }

                    switch (nowShowDirection)
                    {
                        case EnumShowDirection.Self:
                        case EnumShowDirection.Oppset:
                            nowShowDirection = EnumShowDirection.Left;
                            break;
                        case EnumShowDirection.Right:
                        case EnumShowDirection.Left:
                            nowShowDirection = EnumShowDirection.Self;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    item.SelfData.Direction = nowDirection;
                    item.SelfData.ShowDirection = nowShowDirection;
                }
            }
        }

        public void RefreshGroup(MahjongItem addItem)
        {
            addItem.SelfData.Action = EnumMahJongAction.Lie;
        }

        public static MahjongGroupItem CreateGroup()
        {
            GameObject GroupItem = new GameObject();
            MahjongGroupItem group=GroupItem.AddComponent<MahjongGroupItem>();
            return group;
        }

        public void SetNewItem(MahjongItem item,int value, EnumMahJongAction action)
        {
            item.Value = value;
            item.SelfData.Action = action;
            item.SetColor(Color.white);
            GameTools.AddChild(transform, item.transform);
            _itemList.Add(item);
        }
    }
}
