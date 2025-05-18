using System;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Hall.Controller;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    public class GameListItemView : YxView, IItemView
    {
        public int RoomListStyleIndex;
        public int DeskListStyleIndex;
        public UIWidget Widget;
        public GameObject Tween;
        public StateSprites BtnKindSprites;
        public NguiParticleAdapter[] ParticleAdapters;
        /// <summary>
        /// label样式
        /// </summary>
        public NguiLabelAdapter NameLabel;
        /// <summary>
        /// 移动的时候要隐藏的物体
        /// </summary>
        public GameObject[] HideObjectInMove;
        protected override void OnAwake()
        {
            base.OnAwake();
            if (Widget==null) Widget = GetComponent<UIWidget>();
            AwakAction(false);
        }

        public override void SetOrder(int order)
        {
            order += 1;
            foreach (var adapter in ParticleAdapters)
            {
                if (adapter==null)continue;
                adapter.SortingOrder(order);
            }
        }
          
        public void AwakAction(bool isAction)
        {
            if(Tween!=null)Tween.SetActive(isAction);
        }

        public void SetColor(Color color)
        {
            BtnKindSprites.DefaultColor = color;
            if(Widget!=null) { Widget.color = color;}
            var arr = GetComponentsInChildren<UISprite>();
            foreach (var sp in arr)
            {
                sp.color = color;
            }
        }

        public void SetShowState(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public void MoveAction(bool isMove)
        {
            if (HideObjectInMove != null)
            {
                GameObjectUtile.DisplayComponent(HideObjectInMove,!isMove);
            }
        }

        public void FreshBtnClickBound(UIButton btn, UIWidget defWidget, bool isDeve = false)
        {
            if (btn == null) { return;}
            var boxCollider = btn.GetComponent<BoxCollider>();
            if (Widget == null) { Widget = defWidget;}
            Vector3 size = Widget.localSize;
            size.z = 1;
            boxCollider.size = size;
            boxCollider.center = transform.localPosition;
            btn.tweenTarget = gameObject;
            if (isDeve)
            {
                if (!string.IsNullOrEmpty(BtnKindSprites.SpecialNormalSprite)) btn.normalSprite = BtnKindSprites.SpecialNormalSprite;
                if (!string.IsNullOrEmpty(BtnKindSprites.SpecialHoverSprite)) btn.hoverSprite = BtnKindSprites.SpecialHoverSprite;
                if (!string.IsNullOrEmpty(BtnKindSprites.SpecialPressedSprite)) btn.pressedSprite = BtnKindSprites.SpecialPressedSprite;
                if (!string.IsNullOrEmpty(BtnKindSprites.SpecialDisabledSprite)) btn.disabledSprite = BtnKindSprites.SpecialDisabledSprite;
                btn.hover = BtnKindSprites.SpecialHoverColor;
                btn.pressed = BtnKindSprites.SpecialPressedColor;
                btn.disabledColor = BtnKindSprites.SpecialDisabledColor;
                btn.defaultColor = BtnKindSprites.SpecialDefaultColor;
            }
            else
            {
                if (!string.IsNullOrEmpty(BtnKindSprites.NormalSprite)) btn.normalSprite = BtnKindSprites.NormalSprite;
                if (!string.IsNullOrEmpty(BtnKindSprites.HoverSprite)) btn.hoverSprite = BtnKindSprites.HoverSprite;
                if (!string.IsNullOrEmpty(BtnKindSprites.PressedSprite)) btn.pressedSprite = BtnKindSprites.PressedSprite;
                if (!string.IsNullOrEmpty(BtnKindSprites.DisabledSprite)) btn.disabledSprite = BtnKindSprites.DisabledSprite;
                btn.hover = BtnKindSprites.HoverColor;
                btn.pressed = BtnKindSprites.PressedColor;
                btn.disabledColor = BtnKindSprites.DisabledColor;
                btn.defaultColor = BtnKindSprites.DefaultColor;
            }
        }

        public void OnGameClick()
        {
            var listItem = MainYxView as ListViews.GameListItem;
            if (listItem == null) return;
            var hallController = HallController.Instance;
            hallController.DeskListStyleIndex = DeskListStyleIndex;
            hallController.RoomListStyleIndex = RoomListStyleIndex;
            listItem.OnGameClick(); 
        }
    }
     

    [Serializable]
    public class StateSprites
    {
        /// <summary>
        /// 正常的
        /// </summary>
        public string NormalSprite;
        /// <summary>
        /// 悬停
        /// </summary>
        public string HoverSprite;
        /// <summary>
        /// 按下
        /// </summary>
        public string PressedSprite;
        /// <summary>
        /// 不可用
        /// </summary>
        public string DisabledSprite;
        /// <summary>
        /// 特殊时正常状态
        /// </summary>
        public string SpecialNormalSprite;
        /// <summary>
        /// 特殊时悬停
        /// </summary>
        public string SpecialHoverSprite;
        /// <summary>
        /// 特殊时按下
        /// </summary>
        public string SpecialPressedSprite;
        /// <summary>
        /// 不可用
        /// </summary>
        public string SpecialDisabledSprite;

        public Color HoverColor = new Color(1, 1, 1, 1f);
        public Color PressedColor = new Color(0.7f, 0.6f, 0.5f, 1f);
        public Color DisabledColor = Color.grey;
        public Color DefaultColor = Color.white;
        public Color SpecialHoverColor = new Color(1, 1, 1, 1f);
        public Color SpecialPressedColor = new Color(0.7f, 0.6f, 0.5f, 1f);
        public Color SpecialDisabledColor = Color.grey;
        public Color SpecialDefaultColor = Color.white;
    }
}
