/** 
 *文件名称:     MahjongPile.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-14 
 *描述:         麻将堆,不存储数据，只负责显示
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    public class MahjongPile : MonoBehaviour
    {
        [SerializeField] private int _baseLayer;

        [SerializeField] private EnumMahJongAction _itemAction;

        [SerializeField] private EnumMahJongDirection _itemDirection;

        [SerializeField] private EnumShowDirection _itemShow;

        [SerializeField] private DefLayout _layout;

        [SerializeField] internal float _localScaleX = 1;

        [SerializeField] internal float _localScaleY = 1;

        [SerializeField] private int _nowLayer;

        [SerializeField] public bool IsLayerAdd = true;

        public bool IsRight = true;

        public EnumMahJongDirection ItemDirection
        {
            set
            {
                if (value != _itemDirection)
                {
                    _itemDirection = value;
                    ChangeDirection(_itemDirection);
                }
            }
            get { return _itemDirection; }
        }

        public EnumMahJongAction ItemAction
        {
            set
            {
                if (value != _itemAction)
                {
                    _itemAction = value;
                    ChangeAction(_itemAction);
                }
            }
            get { return _itemAction; }
        }

        public EnumShowDirection ItemShow
        {
            set
            {
                if (value != _itemShow)
                {
                    _itemShow = value;
                    ChangShow(ItemShow);
                }
            }
            get { return _itemShow; }
        }

        public int BaseLayer
        {
            set
            {
                if (_baseLayer.Equals(value))
                {
                    return;
                }
                _baseLayer = value;
                ChangeLayer(_baseLayer);
            }
            get { return _baseLayer; }
        }

        public float ItemScaleX
        {
            set
            {
                if (_localScaleX.Equals(value))
                {
                    return;
                }
                _localScaleX = value;
                RefreshPile();
            }
            get { return _localScaleX; }
        }

        public virtual int ChildCount
        {
            get { return transform.childCount; }
        }

        public float ItemScaleY
        {
            set
            {
                if (_localScaleY.Equals(value))
                {
                    return;
                }
                _localScaleY = value;
                RefreshPile();
            }
            get { return _localScaleY; }
        }

        public DefLayout Layout
        {
            get
            {
                if (_layout == null)
                {
                    _layout = GetComponentInChildren<DefLayout>();
                }
                return _layout;
            }
            set { _layout = value; }
        }

        public int NowLayer
        {
            get { return _nowLayer += IsLayerAdd ? 4 : -4; }
        }

        protected virtual void Awake()
        {
            InitListener();
            CheckLayout();
            ResetPile();
        }

        protected virtual void InitListener()
        {
            Layout.OnAddItem = OnAddItem;
            Layout.OnDeleteItem = OnDelete;
        }

        public virtual void CheckLayout()
        {
        }

        public virtual void AddItem(Transform item, bool auto = true)
        {
            ParseItemToThis(item);
            Layout.AddItem(item, auto, ItemScaleX, ItemScaleY);
        }

        public virtual void AddItems(List<int> values)
        {
            var list = GameTools.CreatMahjongItems(values);
            for (int i = 0, lenth = list.Count; i < lenth; i++)
            {
                AddItem(list[i], false);
            }
            Layout.ResetPositionNow = true;
        }

        protected void OnAddItem(Transform trans)
        {
        }

        public void RemoveItem(MahjongItem item)
        {
            Layout.RemoveItem(item.transform);
        }


        protected void OnDelete(Transform trans)
        {
        }

        public virtual void ResetPile()
        {
            Layout.ClearAll();
        }

        public void ResetPosition()
        {
            Layout.Reposition();
        }

        public virtual void ChangeAction(EnumMahJongAction action)
        {
            ItemAction = action;
            RefreshPile();
        }

        public virtual void ChangeDirection(EnumMahJongDirection direction)
        {
            ItemDirection = direction;
            RefreshPile();
        }

        public virtual void ChangShow(EnumShowDirection show)
        {
            ItemShow = show;
            RefreshPile();
        }

        public virtual void ChangeLayer(int baseLayer)
        {
            BaseLayer = baseLayer;
            RefreshPile();
        }

        protected virtual void RefreshPile()
        {
            LayerReset();
            var _list = GetList();
            for (int i = 0, lenth = _list.Count; i < lenth; i++)
            {
                var item = _list[i].GetComponent<MahjongItem>();
                item.SelfData.Action = _itemAction;
                item.SelfData.Direction = _itemDirection;
                item.SelfData.MahjongLayer = NowLayer;
                item.SelfData.ShowDirection = _itemShow;
                item.OnDataChange();
                item.transform.localScale = new Vector3(_localScaleX, _localScaleY);
                item.OnLayerChange();
            }
        }

        public void LayerReset()
        {
            _nowLayer = BaseLayer;
        }

        protected virtual void ParseItemToThis(MahjongItem item)
        {
            item.SelfData.ShowDirection = ItemShow;
            item.SelfData.Direction = ItemDirection;
            item.SelfData.Action = ItemAction;
            item.SelfData.MahjongLayer = NowLayer;
            item.JudgeHunTag(-1);
            item.transform.localScale = new Vector3(_localScaleX, _localScaleY);
        }

        protected virtual void ParseItemToThis(Transform trans)
        {
            ParseItemToThis(trans.GetComponent<MahjongItem>());
        }

        public virtual Transform GetLast()
        {
            return Layout.GetLastItem();
        }

        public virtual List<Transform> GetList()
        {
            return Layout.GetChildList();
        }
    }
}