/** 
 *文件名称:     MahJongData.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-02 
 *描述:         麻将数据。嗯，数据就是数据
 *历史记录: 
*/

using System;
using Assets.Scripts.Game.lyzz2d.Utils;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Data
{
    [Serializable]
    public class MahJongData
    {
        [SerializeField] private EnumMahJongAction _action = EnumMahJongAction.StandNo;

        [SerializeField] private EnumMahJongDirection _direction = EnumMahJongDirection.Horizontal;

        /// <summary>
        ///     当前麻将的基础层级
        /// </summary>
        private int _mahjongLayer;

        [SerializeField] private EnumShowDirection _showDirection;

        [SerializeField] private EnumMahjongValue _value = EnumMahjongValue.None;

        /// <summary>
        ///     当麻将牌是对面玩家时，是否需要反转麻将值的显示
        /// </summary>
        public bool NeedOppsetValue;

        public Action OnActionChange;
        public Action OnDirectonChange;
        public Action OnLayerChange;
        public Action OnShowDirectionChange;
        public Action OnValueChange;

        public MahJongData(int value, EnumMahJongAction action, EnumMahJongDirection direction)
        {
            _value = 0;
            _action = action;
            _direction = direction;
        }

        [SerializeField]
        public EnumMahjongValue Value
        {
            set
            {
                if (_value.Equals(value))
                {
                    return;
                }
                _value = value;
                if (OnValueChange != null)
                {
                    OnValueChange();
                }
            }
            get { return _value; }
        }

        [SerializeField]
        public EnumMahJongAction Action
        {
            set
            {
                if (_action.Equals(value))
                {
                    return;
                }
                _action = value;
                if (OnActionChange != null)
                {
                    OnActionChange();
                }
            }
            get { return _action; }
        }

        [SerializeField]
        public EnumMahJongDirection Direction
        {
            set
            {
                if (_direction.Equals(value))
                {
                    return;
                }

                _direction = value;
                if (OnDirectonChange != null)
                {
                    OnDirectonChange();
                }
            }
            get { return _direction; }
        }

        public EnumShowDirection ShowDirection
        {
            set
            {
                if (_showDirection.Equals(value))
                {
                    return;
                }
                _showDirection = value;
                if (OnShowDirectionChange != null)
                {
                    OnShowDirectionChange();
                }
            }
            get { return _showDirection; }
        }

        public int MahjongLayer
        {
            set
            {
                if (_mahjongLayer.Equals(value))
                {
                    return;
                }
                _mahjongLayer = value;
                if (OnLayerChange != null)
                {
                    OnLayerChange();
                }
            }
            get { return _mahjongLayer; }
        }
    }
}