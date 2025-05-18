/** 
 *文件名称:     ShowGangPaiInfo.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-30 
 *描述:         杠的显示控制
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class ShowGangPaiInfo : MonoSingleton<ShowGangPaiInfo>
    {
        [SerializeField]
        private GameObject _showParent;

        [SerializeField]
        private UISprite _paiBg;

        [SerializeField]
        private GameObject _totalBg;

        [SerializeField]
        private UIGrid _grid;
        [SerializeField]

        public Action<int> OnClickGangEvent;

        public Action OnClickResetEvevt;

        public Action<List<int>> OnFengSureEvent;

        [Tooltip("风杠选择事件")]
        public List<EventDelegate> OnFengSelectAction = new List<EventDelegate>();

        public override void Awake()
        {
            base.Awake();
            UIEventListener.Get(_totalBg).onClick = OnClickReset;
        }

        public void ShowInfo(List<int> data,bool mulSelect=false)
        {
            Show(true);
            ShowFengBtns(mulSelect);
            _selectList.Clear();
            data = GameTools.SortCardWithOutLaiZi(data, App.GetGameManager<Mahjong2DGameManager>().LaiZiNum);
            int ChildLenth = _grid.transform.childCount;
            while (ChildLenth > 0)
            {
                DestroyImmediate(_grid.transform.GetChild(0).gameObject);
                ChildLenth--;
            }
            for (int i = 0, lenth = data.Count; i < lenth; i++)
            {
                MahjongItem item = GameTools.CreateMahjong(data[i], false).GetComponent<MahjongItem>();
                GameTools.DestroyDragObject(item);
                item.SelfData.Action = EnumMahJongAction.StandWith;
                item.SelfData.Direction = EnumMahJongDirection.Vertical;
                item.SelfData.ShowDirection = EnumShowDirection.Self;
                item.SelfData.MahjongLayer = ConstantData.ShowItemLayler;
                if (mulSelect)
                {
                    UIEventListener.Get(item.gameObject).onClick = OnSelectGangItem;
                }
                else
                {
                    UIEventListener.Get(item.gameObject).onClick = OnClickGangItem;
                }
                GameTools.AddChild(_grid.transform, item.transform);
                BoxCollider box = item.gameObject.AddComponent<BoxCollider>();
                box.size = new Vector3(item.BGSprite.width, item.BGSprite.height);
            }
            _grid.repositionNow = true;
            _paiBg.width = (int)(_grid.GetChildList().Count * _grid.cellWidth);
        }

        public void OnClickGangItem(GameObject obj)
        {
            MahjongItem item = obj.GetComponent<MahjongItem>();
            if (OnClickGangEvent != null)
            {
                OnClickGangEvent(item.Value);
            }
            OnClose();
        }


        List<MahjongItem> _selectList=new List<MahjongItem>(); 

        public void OnSelectGangItem(GameObject obj)
        {
            MahjongItem item = obj.GetComponent<MahjongItem>();
            if (_selectList.Contains(item))
            {
                item.SetColor(Color.white);
                _selectList.Remove(item);
            }
            else
            {
                item.SetColor(Color.gray);
                _selectList.Add(item);
            }
        }
        public bool FengSelectActive { private set; get; }
        public void ShowFengBtns(bool active)
        {
            FengSelectActive = active;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnFengSelectAction.WaitExcuteCalls());
            }
        }

        public void OnFengGangSure()
        {
            var selectCount = _selectList.Count;
            if (selectCount != 3 && selectCount != 4)
            {
                YxMessageTip.Show("风杠选择数量不正确，请重新选择!");
                ResetSelect();
                return;
            }
            var checkList = new List<int>();
            foreach (var mahJongItem in _selectList)
            {
                if (mahJongItem)
                {
                    checkList.Add(mahJongItem.Value);
                }
            }
            checkList.Sort();
            if (FengGangHelper.Instance.CheckFengGangLimit(checkList))
            {
                if (OnFengSureEvent != null)
                {
                    OnFengSureEvent(checkList);
                }
                OnClose();
            }
            else
            {
                YxMessageTip.Show("牌型选择错误，请重新选择!");
                ResetSelect();
            }
        }

        private void ResetSelect()
        {
            if (_selectList!=null&&_selectList.Count>0)
            {
                foreach (var selectItem in _selectList)
                {
                    selectItem.SetColor(Color.white);
                }
                _selectList.Clear();
            }
        }


        public void Show(bool state)
        {
            _showParent.SetActive(state);
            if (!state)
            {
                ShowFengBtns(false);
            }
        }

        public void OnClickReset(GameObject obj)
        {
            if (OnClickResetEvevt != null)
            {
                OnClickResetEvevt();
            }
            OnClose();
        }
        public void OnClose()
        {
            Show(false);
        }
    }
}
