using System;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    /// <summary>
    /// 打牌操作组件
    /// </summary>
    internal class UserContorl : MonoBehaviour
    {
        /// <summary>
        /// 同时选中的麻将只能有一个
        /// </summary>
        private static Transform _selectTransform;
        /// <summary>
        /// 拖拽前位置
        /// </summary>
        private Vector3 _dragBeforeV3 = Vector3.zero;
        /// <summary>
        /// 拖拽后位置
        /// </summary>
        private Vector3 _dragAfterV3 = Vector3.zero;
        /// <summary>
        /// 默认背景层级
        /// </summary>
        private int _offsetBgLayer;
        /// <summary>
        /// 默认值层级
        /// </summary>
        private int _offsetValueLayer;
        /// <summary>
        /// 默认混标记层级
        /// </summary>
        private int _offsetHunTagLayer;
        /// <summary>
        /// 默认胡牌标记层级
        /// </summary>
        private int _offsetHuTagLayer;
        /// <summary>
        /// 当前操作的麻将
        /// </summary>
        private MahjongItem _self;
        /// <summary>
        /// 拖拽牌增加层级
        /// </summary>
        private int _dragAddLayer=3000;
        /// <summary>
        /// 出牌的回调
        /// </summary>
        public Action<Transform> OnThrowOut;
        /// <summary>
        /// 默认移动距离
        /// </summary>
        private float MoveDistance = 0.1f;
        /// <summary>
        /// Box
        /// </summary>
        private BoxCollider _boxCollider;
        /// <summary>
        /// 是否锁定玩家打牌
        /// </summary>
        public static bool BlockUserControl;

        private MahjongItem Self
        {
            get
            {
                if(_self == null)
                {
                    _self = GetComponent<MahjongItem>();
                }
                return _self;
            }
        }

        public void Start()
        {
            _boxCollider = gameObject.AddComponent<BoxCollider>();
            MoveDistance = App.GetGameManager<Mahjong2DGameManager>().ItemDragDistance;
            _boxCollider.size = new Vector3(Self.Width,150);
            _dragBeforeV3 = gameObject.transform.localPosition;
            if(GetComponent<UIDragObject>())
            {
                DestroyImmediate(GetComponent<UIDragObject>());
            }
        }

        public void OnDestroy()
        {   
            if (GetComponent<Collider>())
            {
                Destroy(GetComponent<Collider>());
            }
        }

        protected void OnClick()
        {
           ClickThing();
        }

        private void ClickThing()
        {
            Vector3 v3;
            if (!BlockUserControl)
            {
                if (_selectTransform != transform)
                {
                    TrySelectReset();
                    _selectTransform = transform;
                    v3 = _selectTransform.localPosition;
                    v3.y = 20;
                    _selectTransform.localPosition = v3;
                    Facade.Instance<MusicManager>().Play(ConstantData.VoiceSelect);
                    App.GetGameManager<Mahjong2DGameManager>().FindVisibleCard(_selectTransform.GetComponent<MahjongItem>().Value);
                    ChangeDragIngLayer();
                    _dragBeforeV3 = transform.localPosition;
                    return;
                }
                App.GetGameManager<Mahjong2DGameManager>().ClearFlagCard();
                if (OnThrowOut != null)
                {
                    OnThrowOut(transform);
                    _selectTransform = null;
                }
            }
        }

        private void TrySelectReset()
        {
            if (_selectTransform != null)
            {
                Vector3 v3;
                v3 = _selectTransform.localPosition;
                v3.y = 0;
                _selectTransform.localPosition = v3;
                App.GetGameManager<Mahjong2DGameManager>().ClearFlagCard();
            }
        }

        private void OnDragStart()
        {
            if (!BlockUserControl)
            {
                TrySelectReset();
                _selectTransform = transform;
                _dragBeforeV3 = gameObject.transform.localPosition;
                ChangeDragIngLayer();
            }
        }

        private void OnDrag(Vector2 delta)
        {
            if (!BlockUserControl)
            {
                Vector2 nowPos = gameObject.transform.localPosition;
                gameObject.transform.localPosition = delta + nowPos;
            }
        }

        private void OnDragEnd()
        {
            if (!BlockUserControl)
            {
                _dragAfterV3 = gameObject.transform.localPosition;
                float moveDistance = Mathf.Abs(_dragAfterV3.y - _dragBeforeV3.y);
                if (moveDistance < Self.Height * MoveDistance)
                {
                    gameObject.transform.localPosition = _dragBeforeV3;
                    ResetDragLayer();
                    _selectTransform = null;
                }
                else if (OnThrowOut != null)
                {
                    OnThrowOut(transform);
                    _selectTransform = null;
                }
            }
        }

        /// <summary>
        /// 拖动时改变层级
        /// </summary>
        private void ChangeDragIngLayer()
        {
            _offsetBgLayer = Self.BGSprite.depth;
            _offsetValueLayer = Self.ValueSprite.depth;
            _offsetHunTagLayer = Self.HuiTagSprite.depth;
            _offsetHuTagLayer = Self.HuTagSprite.depth;
            Self.BGSprite.depth = _offsetBgLayer + _dragAddLayer;
            Self.ValueSprite.depth = _offsetValueLayer + _dragAddLayer;
            Self.HuTagSprite.depth = _offsetHuTagLayer + _dragAddLayer;
            Self.HuiTagSprite.depth = _offsetHunTagLayer + _dragAddLayer;
        }
        /// <summary>
        /// 恢复
        /// </summary>
        public void OnRecoveryPos()
        {
            _selectTransform = null;
            gameObject.transform.localPosition = new Vector3(_dragBeforeV3.x,0);
            ResetDragLayer();
        }
        /// <summary>
        /// 恢复层级
        /// </summary>
        private void ResetDragLayer()
        {
            Self.BGSprite.depth = _offsetBgLayer;
            Self.ValueSprite.depth = _offsetValueLayer;
            Self.HuiTagSprite.depth = _offsetHunTagLayer;
            Self.HuTagSprite.depth = _offsetHuTagLayer;
        }

        public void SetClickEnable(bool enable)
        {
            if (_boxCollider)
            {
                _boxCollider.enabled = enable;
            }
        }
    }
}
