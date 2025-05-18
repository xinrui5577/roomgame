using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    /// <summary>
    /// 显示相关牌信息（目前暂时用于旋风杠）
    /// </summary>
    public class ShowInfoWithCard : MonoSingleton<ShowInfoWithCard>
    {
        [Tooltip("显示父级")]
        public GameObject ShowParent;
        [Tooltip("牌背景")]
        public UISprite CardsBg;

        public GroupPile GroupPile;

        public MahjongPile singlePile;

        public Vector2 BgBaseBounds=new Vector2(40,40);

        private Action<object> _sureAction;

        private AsyncCallback _cancelAction;

        public void ShowMessageWithInfo(ShowMessageInfo info,Action<object> sureAction=null,AsyncCallback cancelAction=null)
        {
            Show(true);
            _sureAction = sureAction;
            _cancelAction = cancelAction;
            switch (info.ShowType)
            {
                case ShowMessageType.SimpleTypeGroup:
                case ShowMessageType.MulTypeGroup:
                    DealGroupData(info.GroupCardsInfo);
                    break;
                case ShowMessageType.SingleItem:
                    break;
            }

        }

        public void DealGroupData(List<List<int>> groupData) 
        {
            if (GroupPile)
            {
                var count = groupData[0].Count;
                switch (count)
                {
                    case 3:
                        GroupPile.Layout.maxPerLine = 5;
                        GroupPile.Layout.Width = 190;
                        GroupPile.transform.localPosition=new Vector3(70,0,0);
                        break;
                    case 4:
                        GroupPile.Layout.maxPerLine = 4;
                        GroupPile.Layout.Width = 250;
                        GroupPile.transform.localPosition = new Vector3(15, 0, 0);
                        break;
                }
                GroupPile.ResetPile();
                foreach (var group in groupData)
                {
                    MahjongGroupData data = new MahjongGroupData(GroupType.Other);
                    data.values = group.ToArray();
                    var mahJongList = new List<MahjongItem>();
                    for (int i = 0; i < group.Count; i++)
                    {
                        mahJongList.Add(GameTools.CreateMahjong(group[i], false).GetComponent<MahjongItem>());
                    }
                    var groupItem=GroupPile.AddGroup(data, mahJongList, false);
                    BoxCollider box = groupItem.gameObject.AddComponent<BoxCollider>();
                    box.size = new Vector3(GroupPile.Layout.Width, GroupPile.Layout.Height);
                    box.center=new Vector3(GroupPile.Layout.Width/3, 0);
                    UIEventListener.Get(groupItem.gameObject).onClick = OnGroupItemClick;
                }
                var bound=GroupPile.Layout.GetLayoutBounds();
                CardsBg.width = (int)(bound.x+ BgBaseBounds.x);
                CardsBg.height = (int)(bound.y+ BgBaseBounds.y);
            }
        }

        private void OnGroupItemClick(GameObject gourp)
        {
            var groupData = gourp.GetComponent<MahjongGroupItem>();
            if (groupData)
            {
                var list= groupData.Values.ToList();
                if (_sureAction != null)
                {
                    _sureAction(list);
                }
                Show(false);
            }
        }

        public void Show(bool active)
        {
            ShowParent.TrySetComponentValue(active);
        }

        public void HideParent()
        {
            if (_cancelAction != null)
            {
                _cancelAction(null);
            }
            Show(false);
        }
    }

    public class ShowMessageInfo
    {
        /// <summary>
        /// 显示牌类型
        /// </summary>
        public ShowMessageType ShowType;
        /// <summary>
        /// 单牌信息
        /// </summary>
        public List<int> SingleCardsInfo=new List<int>();
        /// <summary>
        /// 组牌信息
        /// </summary>
        public List<List<int>> GroupCardsInfo=new List<List<int>>();
    }

    /// <summary>
    /// 显示信息类型
    /// </summary>
    public enum ShowMessageType
    {
        SimpleTypeGroup,
        MulTypeGroup,
        SingleItem,
    }

    

}
