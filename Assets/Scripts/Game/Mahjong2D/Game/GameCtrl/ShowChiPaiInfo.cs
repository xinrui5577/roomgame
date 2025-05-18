using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class ShowChiPaiInfo : MonoSingleton<ShowChiPaiInfo>
    {
        /// <summary>
        /// 默认背景宽度
        /// </summary>
        public int BackgroundOffsetX = 20;

        /// <summary>
        /// 牌组背景框
        /// </summary>
        [SerializeField]
        private UISprite paiSpBack;

        /// <summary>
        /// grid
        /// </summary>
        [SerializeField]
        private UIGrid _grid;

        [SerializeField]
        public GameObject[] ChiZone;
        /// <summary>
        /// 显示的父级
        /// </summary>
        [SerializeField]
        private GameObject _showParent;

        [SerializeField]
        private GameObject _totalBg;

        public Action<GameObject> OnClickChiEvent;

        public Action OnClickResetEvevt;

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
            if (OnClickChiEvent!=null)
            {
                OnClickChiEvent(obj);
            }
            Show(false);
        }

        public void OnClickReset(GameObject obj)
        {
            if(OnClickResetEvevt!=null)
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
            HideAllPaiSps();//隐藏所有可能吃牌组显示gob
            Show(true);
            for (int i = 0; i < chiInf.Count; i++)
            {
                ChiZone[i].SetActive(true);
                setSpriteNames(chiInf[i], ChiZone[i].transform);
            }
            _grid.repositionNow = true;
            paiSpBack.width =(int)(chiInf.Count*_grid.cellWidth)+BackgroundOffsetX;
        }

        private void HideAllPaiSps()
        {
            for (int i = 0,lenth=ChiZone.Length; i < lenth; i++)
            {
                ChiZone[i].SetActive(false);
            }
        }

        private void setSpriteNames(int[] paiValue, Transform parent)
        {
            if (paiValue.Length == parent.childCount)
            {

                for (int i = 0; i < paiValue.Length; i++)
                {
                    MahjongItem item= parent.GetChild(i).GetComponent<MahjongItem>();
                    item.Value = paiValue[i];
                    item.SelfData.Action=EnumMahJongAction.StandWith;
                    item.SelfData.Direction=EnumMahJongDirection.Vertical;
                    item.SelfData.ShowDirection=EnumShowDirection.Self;
                    item.SelfData.MahjongLayer = ConstantData.ShowItemLayler;
                }
            }
        }



    }
}
