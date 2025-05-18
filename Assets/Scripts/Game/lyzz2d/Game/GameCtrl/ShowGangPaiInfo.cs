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
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class ShowGangPaiInfo : MonoSingleton<ShowGangPaiInfo>
    {
        [SerializeField] private UIGrid _grid;

        [SerializeField] private UISprite _paiBg;

        [SerializeField] private GameObject _shwoParent;

        [SerializeField] private GameObject _totalBg;

        public Action<int> OnClickGangEvent;

        public Action OnClickResetEvevt;

        public override void Awake()
        {
            base.Awake();
            UIEventListener.Get(_totalBg).onClick = OnClickReset;
        }

        public void ShowInfo(List<int> data)
        {
            Show(true);
            data = SortGangWithOutLaiZi(data);
            var ChildLenth = _grid.transform.childCount;
            while (ChildLenth > 0)
            {
                DestroyImmediate(_grid.transform.GetChild(0).gameObject);
                ChildLenth--;
            }
            for (int i = 0, lenth = data.Count; i < lenth; i++)
            {
                var item = GameTools.CreateMahjong(data[i], false).GetComponent<MahjongItem>();
                item.SelfData.Action = EnumMahJongAction.StandWith;
                item.SelfData.Direction = EnumMahJongDirection.Vertical;
                item.SelfData.ShowDirection = EnumShowDirection.Self;
                UIEventListener.Get(item.gameObject).onClick = OnClickGangItem;
                GameTools.AddChild(_grid.transform, item.transform);
                var box = item.gameObject.AddComponent<BoxCollider>();
                box.size = new Vector3(item.BGSprite.width, item.BGSprite.height);
            }
            _grid.repositionNow = true;
            _paiBg.width = (int) (_grid.GetChildList().Count*_grid.cellWidth);
        }

        private List<int> SortGangWithOutLaiZi(List<int> datas)
        {
            var lenth = datas.Count;
            var laiZiNum = App.GetGameManager<Lyzz2DGameManager>().LaiZiNum;
            for (var i = 0; i < lenth - 1; i++)
            {
                for (var j = i + 1; j < lenth; j++)
                {
                    if (datas[i] > datas[j])
                    {
                        var temp = datas[i];
                        datas[i] = datas[j];
                        datas[j] = temp;
                    }
                }
            }
            datas.Remove(laiZiNum);
            return datas;
        }

        public void OnClickGangItem(GameObject obj)
        {
            var item = obj.GetComponent<MahjongItem>();
            if (OnClickGangEvent != null)
            {
                OnClickGangEvent(item.Value);
            }
            OnClose();
        }

        public void Show(bool state)
        {
            _shwoParent.SetActive(state);
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