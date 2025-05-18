using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class LuanFengGroupPile : GroupPile
    {
        public override MahjongGroupItem AddGroup(MahjongGroupData groupData, List<MahjongItem> items, bool isOther)
        {
            if (groupData.type.Equals(GroupType.FengGang))
            {
                groupData.SortDataByLuanFengData();
            }
            MahjongGroupItem group = base.AddGroup(groupData, items, isOther);
            return group;
        }
    }
}
