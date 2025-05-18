using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.fxmj
{
    public class ShowSpecialGangInfo : MonoSingleton<ShowSpecialGangInfo>
    {
        [SerializeField]
        private GameObject _showParent;
        [SerializeField]
        private UIGrid _grid;
        [SerializeField]
        private UISprite _paiBg;
        [SerializeField]
        private GameObject _totalBg;
        public Action OnClickResetEvevt;
        public Action<GameObject> OnClickSpecialGangEvent;
     
        public override void Awake()
        {
            base.Awake();
            if (_totalBg!= null)
            {
                UIEventListener.Get(_totalBg).onClick = OnClickReset;
            }
        }
        public void OnClickReset(GameObject obj)
        {
            if (OnClickResetEvevt != null)
            {
                OnClickResetEvevt();
            }
            Show(false);
        }
        public void ShowSpecialGang(List<int[]> data, Action<GameObject> callback)
        {
            OnClickSpecialGangEvent = callback;
            Show(true);
            int childLenth = _grid.transform.childCount;
            while (childLenth > 0)
            {
                DestroyImmediate(_grid.transform.GetChild(0).gameObject);
                childLenth--;
            }
            foreach (int[] t in data)
            {
                var aa = new List<int>(t);
                CreatMahjongData(aa);
            }
            for (int i = 0; i < _grid.GetChildList().Count; i++)
            {
                _grid.GetChild(i).name = i.ToString();
            }
            _grid.transform.localPosition=new Vector3(-110,0,0);
            _grid.cellWidth = 320;
             _grid.repositionNow = true;
            _paiBg.width = (int)(_grid.GetChildList().Count * _grid.cellWidth);
        }
        private void CreatMahjongData(List<int> data)
        {
            var obj=new GameObject();
            var item=new MahjongItem();
            var parent = obj.AddComponent<UIGrid>();
            for (int i = 0, lenth = data.Count; i < lenth; i++)
            {
                item = GameTools.CreateMahjong(data[i], false).GetComponent<MahjongItem>();
                GameTools.DestroyDragObject(item);
                item.SelfData.Action = EnumMahJongAction.StandWith;
                item.SelfData.Direction = EnumMahJongDirection.Vertical;
                item.SelfData.ShowDirection = EnumShowDirection.Self;
                item.SelfData.MahjongLayer = ConstantData.ShowItemLayler;
                GameTools.AddChild(parent.transform, item.transform);
            }
            UIEventListener.Get(parent.gameObject).onClick = OnClickSpecialGangItem;
            var box = parent.gameObject.AddComponent<BoxCollider>();
            box.size = new Vector3(parent.GetChildList().Count * item.BGSprite.width, item.BGSprite.height);
            box.center=new Vector3(110,0,0);
            parent.cellWidth = 72;
            parent.repositionNow = true;
            GameTools.AddChild(_grid.transform, parent.transform);
        }
        public void OnClickSpecialGangItem(GameObject obj)
        {
            if (OnClickSpecialGangEvent != null)
            {
                OnClickSpecialGangEvent(obj.gameObject);
            }
            Show(false);
        }
        public void Show(bool state)
        {
            _showParent.SetActive(state);
        }
    }
    
}
