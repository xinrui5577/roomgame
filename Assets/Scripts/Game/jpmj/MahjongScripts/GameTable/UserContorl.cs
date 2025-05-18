using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class UserContorl : MonoBehaviour
    {
        /// <summary>
        /// 是否允许ui的事件
        /// </summary>
        public static bool EnableUiEvent = true;
        /// <summary>
        /// 是否允许逻辑事件
        /// </summary>
        public static bool EnableEvent = true;

#if UNITY_ANDROID || UNITY_IPHONE
        protected static Transform _selectTransform;
#endif
        /// <summary>
        /// 出牌的回调
        /// </summary>
        public DVoidTransform OnThrowOut;

        protected GameObject _dragClone;
        protected const float FDragMinDis = 0.4f;

        public bool XjfdStatus = false;

        //是否允许拖拽
        public bool ForbidDrag { get; set; }
        //记录偏移前的位置
        private Vector3 mRecordMahjongPos;

        private bool mHideMahjongOnDrag
        {
            get
            {
                return transform.GetComponent<MahjongItem>().HideMahjongOnDrag;
            }
        }

        protected void OnClick()
        {
            if (!UtilData.HandMjTouchEnable)
            {
                return;
            }

#if UNITY_ANDROID || UNITY_IPHONE
            if (!XjfdStatus)
            {
                Vector3 v3;
                if (_selectTransform != transform)
                {
                    if (_selectTransform != null && _selectTransform.gameObject.layer == MahjongPlayerHard.PlayerHardLayer)
                    {
                        _selectTransform.GetComponent<MouseRoll>().RollDown();
                        //v3 = _selectTransform.localPosition;
                        //v3.y = MahjongManager.MagjongSize.y / 2;
                        //_selectTransform.localPosition = v3;
                        //Game.Instance.ClearFlagCard();
                        //EventDispatch.Dispatch(EventDispatch.GameDgt, new EventData(GameEventId.CleareFlagMahjong));

                        if (EnableEvent && EnableSelectEvent && ItemSelect != null)
                        {
                            ItemSelect(gameObject, false);
                        }
                    }

                    _selectTransform = transform;
                    _selectTransform.GetComponent<MouseRoll>().RollUp();
                    //v3 = _selectTransform.localPosition;
                    //v3.y = MahjongManager.MagjongSize.y / 2 + +0.1f;
                    //_selectTransform.localPosition = v3;
                    //Game.Instance.FindVisibleCard(_selectTransform.GetComponent<MahjongItem>().Value); 
                    //EventDispatch.Dispatch(EventDispatch.GameDgt, new EventData(GameEventId.FlagMahjong, _selectTransform.GetComponent<MahjongItem>().Value));

                    if (EnableEvent && EnableSelectEvent && ItemSelect != null)
                    {
                        ItemSelect(gameObject, true);
                    }
                    return;
                }
                if (EnableEvent && EnableSelectEvent && ItemSelect != null)
                {
                    ItemSelect(gameObject, false);
                }
                //Game.Instance.ClearFlagCard();
                EventDispatch.Dispatch((int)GameEventId.CleareFlagMahjong, new EventData());

                //v3 = _selectTransform.localPosition;
                //v3.y = MahjongManager.MagjongSize.y / 2;
                //_selectTransform.localPosition = v3;
                _selectTransform.GetComponent<MouseRoll>().RollDown();
                _selectTransform = null;
            }
#endif

            if (OnThrowOut != null)
            {
                OnThrowOut(transform);

                if (mHideMahjongOnDrag)
                {
                    EventDispatch.Dispatch((int)UIEventId.HideQueryHulistPnl);
                }
            }
            else
            {
                YxDebug.Log("no response call");
            }
        }
        public bool EnableSelectEvent { get; set; }
        public delegate void OnSelect(GameObject target, bool isPress);

        public OnSelect ItemSelect;
        public static OnSelect UiItemSelect;

        protected virtual void OnPress(bool isPress)
        {
            if (!UtilData.HandMjTouchEnable)
            {
                return;
            }
#if UNITY_ANDROID||UNITY_IPHONE

            if (UiItemSelect != null) {
                UiItemSelect(gameObject, isPress);
	        }
            if (EnableEvent && EnableSelectEvent && ItemSelect != null) {
                ItemSelect(gameObject, isPress);
            }
#endif
            if (isPress == false)
            {
                //麻将结束拖拽   
                if (_dragClone != null)
                {
                    //判断是否拖拽有效
                    if (_dragClone.transform.position.y - mRecordMahjongPos.y >= FDragMinDis)
                    {
                        //拖拽有效 出牌
                        if (OnThrowOut != null)
                        {
                            OnThrowOut(transform);
                        }
                        else
                        {
                            YxDebug.Log("no response call");
                        }
                        if (mHideMahjongOnDrag)
                            transform.position = mRecordMahjongPos;
                    }
                    else
                    {
                        if (mHideMahjongOnDrag)
                            transform.position = mRecordMahjongPos;
                    }
                    MahjongManager.Instance.RecycleCloneMahjong(_dragClone);
                    _dragClone = null;
#if UNITY_ANDROID || UNITY_IPHONE
                    if(_selectTransform!=null)_selectTransform.GetComponent<MouseRoll>().RollDown();
                    _selectTransform = null;
#endif
                    EventDispatch.Dispatch((int)GameEventId.OnRefreshHandCard);
                    EventDispatch.Dispatch((int)GameEventId.CleareFlagMahjong, new EventData(GameEventId.CleareFlagMahjong));
                }
            }
        }

        protected virtual void OnDrag(Vector2 delta)
        {
            if (!UtilData.HandMjTouchEnable)
            {
                return;
            }
            if (ForbidDrag)
            {
                return;
            }

            if (_dragClone == null)
            {
                //克隆出一个新的麻将
                _dragClone = MahjongManager.Instance.CreateCloneMajong(gameObject);
                _dragClone.transform.parent = transform.parent;
                _dragClone.transform.position = transform.position;
                _dragClone.transform.rotation = transform.rotation;
                _dragClone.transform.localScale = transform.localScale;
                UtilFunc.ChangeLayer(_dragClone.transform, transform.gameObject.layer);
                _dragClone.GetComponent<MahjongItem>().RemoveMahjongScript();
                mRecordMahjongPos = transform.position;

                //偏移要打出的麻将
                if (mHideMahjongOnDrag)
                    transform.position = new Vector3(100, 100, 100);
            }

            var handCarmera = GameObject.Find("GameTable/handCardCamera").GetComponent<Camera>();

            //物体的屏幕坐标
            Vector3 screenPos = handCarmera.WorldToScreenPoint(_dragClone.transform.position);
#if (UNITY_ANDROID||UNITY_IPHONE)&&!UNITY_EDITOR
            Vector3 mousePos = Input.touches[0].position;
#else
            Vector3 mousePos = Input.mousePosition;
#endif
            mousePos.z = screenPos.z;

            Vector3 worldPos = handCarmera.ScreenToWorldPoint(mousePos);
            _dragClone.transform.position = new Vector3(worldPos.x, worldPos.y, worldPos.z);

            var offsetY = new Vector3(transform.localPosition.x, 100, transform.localPosition.z);
            transform.localPosition = offsetY;
        }


#if UNITY_STANDALONE
        protected void OnHover(bool isOn)
        {
            if (!UtilData.HandMjTouchEnable)
            {
                return;
            }

            if (EnableSelectEvent)
            {
                ItemSelect(gameObject, isOn);
            }
        }
#endif
        protected void OnDestroy()
        {
            if (_dragClone != null) MahjongManager.Instance.RecycleCloneMahjong(_dragClone);
#if UNITY_ANDROID||UNITY_IPHONE
            _selectTransform = null;
#endif
        }
    }
}
