using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     GangDiShow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-24
 *描述:        	杠底显示
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public  class GangDiShowView: MonoSingleton<GangDiShowView>
    {
        #region UI Param
        [Tooltip("显示父级")]
        public GameObject ShowParent;
        [Tooltip("杠底牌")]
        public MahjongItem GangDiItem;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        public override void Awake()
        {
            base.Awake();
            Facade.EventCenter.AddEventListeners<string,int>(ConstantData.KeyGangDiCard, OnShowGangDiCard);
        }

        #endregion

        #region Function

        private void OnShowGangDiCard(int value)
        {
            if (GangDiItem)
            {
                GangDiItem.Value = value;
            }
        }

        #endregion
    }
}
