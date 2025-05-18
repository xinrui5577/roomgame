using System;
using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class ShowChiPaiInfo : MonoSingleton<ShowChiPaiInfo>
    {
        /// <summary>
        ///     grid
        /// </summary>
        [SerializeField] private UIGrid _grid;

        /// <summary>
        ///     显示的父级
        /// </summary>
        [SerializeField] private GameObject _showParent;

        [SerializeField] private GameObject _totalBg;

        /// <summary>
        ///     默认背景宽度
        /// </summary>
        public int BackgroundOffsetX = 20;

        [SerializeField] public GameObject[] ChiZone;

        public Action<GameObject> OnClickChiEvent;

        public Action OnClickResetEvevt;

        /// <summary>
        ///     牌组背景框
        /// </summary>
        [SerializeField] private UISprite paiSpBack;

        public override void Awake()
        {
            base.Awake();
            foreach (var chi in ChiZone)
            {
                UIEventListener.Get(chi).onClick = OnClickChiZone;
            }
            UIEventListener.Get(_totalBg).onClick = OnClickReset;
        }

        public void OnClickChiZone(GameObject obj)
        {
            if (OnClickChiEvent != null)
            {
                OnClickChiEvent(obj);
            }
            Show(false);
        }

        public void OnClickReset(GameObject obj)
        {
            if (OnClickResetEvevt != null)
            {
                OnClickResetEvevt();
            }
            Show(false);
        }

        public void Show(bool state)
        {
            _showParent.SetActive(state);
        }

        public void SetChiInfos(List<int[]> chiInf)
        {
            hideAllPaiSps(); //隐藏所有可能吃牌组显示gob
            Show(true);
            for (var i = 0; i < chiInf.Count; i++)
            {
                ChiZone[i].SetActive(true);
                setSpriteNames(chiInf[i], ChiZone[i].transform);
            }
            _grid.repositionNow = true;
            paiSpBack.width = (int) (chiInf.Count*_grid.cellWidth) + BackgroundOffsetX;
        }

        private void hideAllPaiSps()
        {
            for (int i = 0, lenth = ChiZone.Length; i < lenth; i++)
            {
                ChiZone[i].SetActive(false);
            }
        }

        private void setSpriteNames(int[] paiValue, Transform parent)
        {
            if (paiValue.Length == parent.childCount)
            {
                for (var i = 0; i < paiValue.Length; i++)
                {
                    var item = parent.GetChild(i).GetComponent<MahjongItem>();
                    item.Value = paiValue[i];
                    item.SelfData.Action = EnumMahJongAction.StandWith;
                    item.SelfData.Direction = EnumMahJongDirection.Vertical;
                    item.SelfData.ShowDirection = EnumShowDirection.Self;
                }
            }
        }
    }
}