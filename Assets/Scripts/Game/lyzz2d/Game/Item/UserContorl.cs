using System;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Item
{
    /// <summary>
    ///     打牌操作组件
    /// </summary>
    internal class UserContorl : MonoBehaviour
    {
        /// <summary>
        ///     同时选中的麻将只能有一个
        /// </summary>
        private static Transform _selectTransform;

        /// <summary>
        ///     拖拽牌增加层级
        /// </summary>
        private readonly int _dragAddLayer = 3000;

        /// <summary>
        ///     拖拽后位置
        /// </summary>
        private Vector3 _dragAfterV3 = Vector3.zero;

        /// <summary>
        ///     拖拽前位置
        /// </summary>
        private Vector3 _dragBeforeV3 = Vector3.zero;

        /// <summary>
        ///     默认背景层级
        /// </summary>
        private int _offsetBgLayer;

        /// <summary>
        ///     默认值层级
        /// </summary>
        private int _offsetValueLayer;

        /// <summary>
        ///     当前操作的麻将
        /// </summary>
        private MahjongItem _self;

        /// <summary>
        ///     出牌的回调
        /// </summary>
        public Action<Transform> OnThrowOut;

        public void Start()
        {
            var c = gameObject.AddComponent<BoxCollider>();
            _self = GetComponent<MahjongItem>();
            c.size = new Vector3(_self.Width, _self.Height);
            _dragBeforeV3 = gameObject.transform.localPosition;
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
            ResetDragLayer();
            ClickThing();
        }

        private void ClickThing()
        {
            Vector3 v3;
            if (_selectTransform != transform)
            {
                TrySelectReset();
                _selectTransform = transform;
                _dragBeforeV3 = transform.localPosition;
                v3 = _selectTransform.localPosition;
                v3.y = 10;
                _selectTransform.localPosition = v3;
                App.GetGameManager<Lyzz2DGameManager>()
                    .FindVisibleCard(_selectTransform.GetComponent<MahjongItem>().Value);
                return;
            }
            App.GetGameManager<Lyzz2DGameManager>().ClearFlagCard();
            if (OnThrowOut != null)
            {
                OnThrowOut(transform);
                _selectTransform = null;
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
                App.GetGameManager<Lyzz2DGameManager>().ClearFlagCard();
            }
        }

        public void OnDragStart()
        {
            TrySelectReset();
            _selectTransform = transform;
            _dragBeforeV3 = gameObject.transform.localPosition;
            ChangeDragIngLayer();
        }

        public void OnDragEnd()
        {
            _dragAfterV3 = gameObject.transform.localPosition;
            var moveDistance = Mathf.Abs(_dragAfterV3.y - _dragBeforeV3.y);
            if (moveDistance < _self.Height*0.7f)
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

        /// <summary>
        ///     拖动时改变层级
        /// </summary>
        private void ChangeDragIngLayer()
        {
            _offsetBgLayer = _self.BGSprite.depth;
            _offsetValueLayer = _self.ValueSprite.depth;
            _self.BGSprite.depth = _offsetBgLayer + _dragAddLayer;
            _self.ValueSprite.depth = _offsetValueLayer + _dragAddLayer;
        }

        /// <summary>
        ///     恢复
        /// </summary>
        public void OnRecoveryPos()
        {
            _selectTransform = null;
            gameObject.transform.localPosition = new Vector3(_dragBeforeV3.x, 0);
            ResetDragLayer();
        }

        /// <summary>
        ///     恢复层级
        /// </summary>
        private void ResetDragLayer()
        {
            _self.BGSprite.depth = _offsetBgLayer;
            _self.ValueSprite.depth = _offsetValueLayer;
        }
    }
}