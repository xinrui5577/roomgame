/** 
 *文件名称:     HelpInfoControl.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-04-25 
 *描述:         游戏中的相关信息的控制类,就是界面左上角的信息面板
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.Common.UI;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class HelpInfoControl : MonoSingleton<HelpInfoControl>
    {
        /// <summary>
        /// 显示控制开关
        /// </summary>
        [SerializeField]
        private GameObject _showArea;
        /// <summary>
        /// 剩余牌数
        /// </summary>
        [SerializeField]
        private GameObject _leftNumber;
        [Tooltip("房间信息相关")]
        public GameObject RoomInfoObj;
        /// <summary>
        /// 背景区域
        /// </summary>
        [SerializeField]
        private UISprite _bgSprite;
        /// <summary>
        /// 布局
        /// </summary>
        [SerializeField]
        private DefLayout _layout;
        /// <summary>
        /// 网络类型图标
        /// </summary>
        [SerializeField]
        private UISprite _netIcon;
        /// <summary>
        /// wrap
        /// </summary>
        [SerializeField]
        private UIWrapContent _wrap;
        /// <summary>
        /// 滚动距离
        /// </summary>
        private float _moveLenth;
        /// <summary>
        /// 是否需要滚动
        /// </summary>
        private bool _needWrap;
        /// <summary>
        /// 滚动时间
        /// </summary>
        [SerializeField]
        private float _wrapStrengh=3f;
        /// <summary>
        /// 电量
        /// </summary>
        [SerializeField]
        private UIGrid _powerGrid;
        /// <summary>
        /// 刷新频率 
        /// </summary>
        [SerializeField]
        private float _refreshFrame = 20;
        /// <summary>
        /// 一格代表的电量
        /// </summary>
        [SerializeField]
        private int _perItemOfPower = 33;

        [SerializeField]
        private GameObject _showTimeNotice;

        [SerializeField]
        private GameObject _powerObj;
        /// <summary>
        /// 滚动索引
        /// </summary>
        private int _wrapIndex;
        public bool NeedWrap
        {
            set
            {
                _needWrap = value;
                RoomInfoShow(value);
                if (_needWrap)
                {
                    Move();
                }
                else
                {
                    CancelInvoke("RefreshInfo");
                }
            }
            get
            {
                return _needWrap;
            }
        }

        public override void Awake()
        {
            base.Awake();
            if(_wrap!=null)
            {
                _moveLenth = _wrap.itemSize;
            }
            RoomInfoShow(false);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            _showTimeNotice.TrySetComponentValue(true);
            _powerObj.TrySetComponentValue(false);
            if(_netIcon)
            _netIcon.gameObject.TrySetComponentValue(false);
#else
              _showTimeNotice.TrySetComponentValue(false);
              _powerObj.TrySetComponentValue(true);
              if(_netIcon)
              _netIcon.gameObject.TrySetComponentValue(true);
#endif
        }

        public void Move()
        {
            if(_refreshFrame==0)
            {
                InvokeRepeating("RefreshInfo", 0, _refreshFrame);
            }
        }

        public void OnClickChangeInfo()
        {
            if(NeedWrap)
            {
                RefreshInfo();
            }
        }
        public void LayoutResetPosition()
        {
            if (_layout != null)
            {
                _layout.ResetPositionNow = true;
            }
        }
        private void RefreshInfo()
        {
            if (_wrap != null)
            {
                Vector3 vec = new Vector3(0,--_wrapIndex * _moveLenth);
                SpringPanel.Begin(_wrap.gameObject, vec, _wrapStrengh);
#if UNITY_STANDALONE_WIN
#else
                ShowSignType();
                ShowPowerInfo();
#endif
            }

        }

        private void ShowSignType()
        {
            int type=GameTools.JudgeNetworkType();
            _netIcon.spriteName = string.Format("icon_00{0}",type);
            _netIcon.MakePixelPerfect();
        }

        private void ShowPowerInfo()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            return;
#endif
            int getNum = GameTools.GetBatteryLevel();
            //permission.DEVICE_POWER
            if (getNum==-1) //临时处理，
            {
                getNum =67;
            }
            int showNume = (getNum / _perItemOfPower)+1;
            List<Transform> trans = _powerGrid.GetChildList();
            for (int i = 0,showLenth=trans.Count; i <showLenth; i++)
            {
               trans[i].gameObject.SetActive(showNume > i);
            }
        }

        /// <summary>
        /// 房间信息显示
        /// </summary>
        /// <param name="state"></param>
        private void RoomInfoShow(bool state)
        {
            RoomInfoObj.TrySetComponentValue(state);
            LayoutResetPosition();
        }

        public void LeftCardsShow(bool state)
        {
            _leftNumber.SetActive(state);
            if (_bgSprite && _layout)
                _bgSprite.height = (int)(_layout.GetChildList().Count * _layout.Height);
        }
    }
}
