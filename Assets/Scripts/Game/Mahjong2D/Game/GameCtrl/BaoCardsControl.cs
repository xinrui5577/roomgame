/** 
 *文件名称:     BaoCardsControl.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-05-03 
 *描述:         宝牌控制类(摸宝，换宝，双宝)
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class BaoCardsControl : MonoSingleton<BaoCardsControl>
    {
        [SerializeField]
        private GameObject _showArea;
        [HideInInspector]
        public List<int> BaoValues=new List<int>();
        [SerializeField]
        private UIGrid _baoGrid;
        [SerializeField]
        private UISprite _baoBg;
        [SerializeField]
        private UILabel _baoLabel;
        [SerializeField]
        private int _offsetWidth=20;
        private int _itemWidth;
        private Mahjong2DGameManager Manager
        {
            get { return App.GetGameManager<Mahjong2DGameManager>(); }
        }
        public override void Awake()
        {
            base.Awake();
            if (_baoGrid!=null)
            {
                _itemWidth = (int)_baoGrid.cellWidth;
            }
            if (_baoLabel!=null)
            {
#if (UNITY_STANDALONE_WIN&&LOCAL_DEBUG)||UNITY_EDITOR
                _baoLabel.gameObject.SetActive(true);
#else
                _baoLabel.gameObject.SetActive(false); 
#endif
            }
        }
        public void SetBaos(List<int> baos, bool showBao)
        {
            if(baos.Count==0)
            {
                return;
            }
            _showArea.SetActive(true);
            BaoValues = baos;
            int baoNum = BaoValues.Count;
            if (_baoLabel!=null)
            {
                _baoLabel.text = "";
            }
            List<Transform> trans = _baoGrid.GetChildList();
            for (int i = 0,lenth= trans.Count; i < lenth; i++)
            {
                if(baoNum>i)
                {
                    MahjongItem item = trans[i].GetComponent<MahjongItem>();
                    item.gameObject.SetActive(true);
                    item.SelfData.Direction = EnumMahJongDirection.Vertical;
                    item.SelfData.ShowDirection = EnumShowDirection.Self;
                    if (showBao)
                    {
                        item.SelfData.Action = EnumMahJongAction.Lie;
                        if(Manager.Data.IsMingBao)
                        {
                            item.SelfData.Value = (EnumMahjongValue) BaoValues[i];
                        }
                        else
                        {
                            item.SelfData.Value = EnumMahjongValue.AnBao;
                        } 
                    }
                    else
                    {
                        item.SelfData.Action = EnumMahJongAction.Push;
                    }
                    if (_baoLabel!=null)
                    {
                        _baoLabel.text += string.Format("{0}:{1};", (EnumMahjongValue)BaoValues[i], BaoValues[i]);
                    }
                }
                else
                {
                    trans[i].gameObject.SetActive(false);
                }
            
            }
            _baoGrid.repositionNow = true;
            _baoBg.width = _offsetWidth + _itemWidth* baoNum;
        }

        public void Reset()
        {
            if (_showArea!=null)
            {
                _showArea.SetActive(false);
                List<Transform> list = _baoGrid.GetChildList();
                foreach (var item in list)
                {
                    item.gameObject.SetActive(true);
                }
            }
        }
    }

}