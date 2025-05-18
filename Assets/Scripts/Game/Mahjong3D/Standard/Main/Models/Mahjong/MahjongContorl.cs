using UnityEngine;
using System;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongContorl : MonoBehaviour
    {
        /// <summary>
        /// 禁止选牌
        /// </summary>
        public bool mNoSelectFlag;

        public static MahjongContainer SelectTransform;

        /// <summary>
        /// 出牌的回调
        /// </summary>
        public Action<Transform> OnThrowOut;

        protected MahjongContainer mDragClone;

        //麻将上下提示出牌
        public bool AllowOffsetStatus = true;
        //是否允许拖拽
        public bool ForbidDrag { get; set; }
        //记录偏移前的位置
        private Vector3 mRecordMahjongPos;

        private MahjongContainer mContainer;

        private void Awake()
        {
            mContainer = GetComponent<MahjongContainer>();           
        }

        public static void ClearSelectCard()
        {
            SelectTransform = null;
        }

        protected void OnClick()
        {
            if (!GameCenter.Scene.HandMahTouchEnable)
            {
                return;
            }
            if (AllowOffsetStatus)
            {
                var currMahjong = transform.GetComponent<MahjongContainer>();
                if (SelectTransform == null || currMahjong.MahjongIndex != SelectTransform.MahjongIndex || currMahjong.Value != SelectTransform.Value)
                {
                    if (SelectTransform != null && SelectTransform.gameObject.layer == MahjongPlayerHand.PlayerHardLayer)
                    {
                        SelectTransform.RollDown();
                    }
                    //这里必须重新获取对象， 不用currMahjong赋值
                    SelectTransform = transform.GetComponent<MahjongContainer>();
                    SelectTransform.RollUp();
                    return;
                }
            }
            if (OnThrowOut != null)
            {
                OnThrowOut(transform);
            }
        }

        protected virtual void OnPress(bool isPress)
        {
            if (!GameCenter.Scene.HandMahTouchEnable)
            {
                return;
            }
            if (isPress == false)
            {
                //麻将结束拖拽   
                if (mDragClone != null)
                {
                    //判断是否拖拽有效
                    if (mDragClone.transform.position.y - mRecordMahjongPos.y >= 0.4f)
                    {
                        //拖拽有效 出牌
                        if (OnThrowOut != null)
                        {
                            OnThrowOut(transform);
                        }
                    }
                    GameCenter.Scene.MahjongCtrl.PushMahjongToPool(mDragClone);
                    mDragClone = null;
                }
            }
        }

        protected virtual void OnDrag(Vector2 delta)
        {
            if (!GameCenter.Scene.HandMahTouchEnable)
            {
                return;
            }
            if (ForbidDrag)
            {
                return;
            }
            if (mDragClone == null)
            {
                //克隆出一个新的麻将
                mDragClone = GameCenter.Scene.MahjongCtrl.PopMahjong(gameObject.GetComponent<MahjongContainer>().Value);
                mDragClone.transform.SetParent(transform.parent);
                mDragClone.transform.position = transform.position;
                mDragClone.transform.rotation = transform.rotation;
                mDragClone.transform.localScale = transform.localScale;
                GameUtils.ChangeLayer(mDragClone.transform, transform.gameObject.layer);
                mDragClone.GetComponent<MahjongContainer>().RemoveMahjongScript();
                mRecordMahjongPos = transform.position;
            }
            var handCarmera = GameCenter.Scene.MahjongCamera.HandCamera;
            //物体的屏幕坐标
            Vector3 screenPos = handCarmera.WorldToScreenPoint(mDragClone.transform.position);
#if (UNITY_ANDROID||UNITY_IPHONE)&&!UNITY_EDITOR
            Vector3 mousePos = Input.touches[0].position;
#else
            Vector3 mousePos = Input.mousePosition;
#endif
            mousePos.z = screenPos.z;
            Vector3 worldPos = handCarmera.ScreenToWorldPoint(mousePos);
            mDragClone.transform.position = new Vector3(worldPos.x, worldPos.y, worldPos.z);
        }

        public void RollDown()
        {
            if (mContainer.Tweener != null)
            {
                mContainer.Tweener.Down(0.02f);
            }         
            GameCenter.Scene.MahjongGroups.OnClearFlagMahjong();
            GameCenter.EventHandle.Dispatch((int)EventKeys.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }

        public void ResetPos()
        {
            if (mContainer.Tweener != null)
            {
                mContainer.Tweener.Down(0.02f);
            }
            GameCenter.Scene.MahjongGroups.OnClearFlagMahjong();
        }

        protected void OnDestroy()
        {
            if (mDragClone != null) GameCenter.Scene.MahjongCtrl.PushMahjongToPool(mDragClone);
        }
    }
}