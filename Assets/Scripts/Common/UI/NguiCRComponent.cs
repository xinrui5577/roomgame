using Assets.Scripts.Common.Models.CreateRoomRules;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class NguiCRComponent : NguiView
    {
        /// <summary>
        /// 提示按钮
        /// </summary>
        public GameObject TipBtn;
        /// <summary>
        /// 是否需要置灰的对象
        /// </summary>
        public UIWidget GrayObject;
        /// <summary>
        /// 工作对象
        /// </summary>
        public BoxCollider EnabelObject;

        protected override void OnAwake()
        {
            InitStateTotal = 2;
            CheckIsStart = true;
            InitWidgetPivot(); 
        }

        /// <summary>
        /// 初始化widget的原点位置
        /// </summary>
        private void InitWidgetPivot()
        {
            var widget = GetWidget;
            if (widget != null)
            {
                widget.pivot = UIWidget.Pivot.TopLeft;
            }
        }

        protected override void OnFreshView()
        {
            var itemData = GetData<ItemData>();
            if (itemData != null)
            {
                if (TipBtn != null)
                {
                    var tip = itemData.Tip;
                    TipBtn.SetActive(!string.IsNullOrEmpty(tip));
                }
                OnFreshCRCView(itemData);
                var dict = itemData.Parent.CreateArgs;
                dict[itemData.Id] = itemData;
                FreshEnable(itemData.Parent.HasServerSaveOption());
                UpdateWidget(itemData.Width, itemData.Height);
            }
            CallBack(IdCode);
        }

        private  void FreshEnable(bool hasServerSaveOption)
        {
           if(GrayObject!=null) { GrayObject.ColorType = hasServerSaveOption ? UIRect.EColorType.Gray : UIRect.EColorType.Normal;}
            OnFreshEnable(!hasServerSaveOption);
        }

        protected virtual void OnFreshEnable(bool isEnable)
        {
            if (EnabelObject != null)
            {
                EnabelObject.enabled = isEnable;
            }
        }

        private UIWidget _widget;
        public UIWidget GetWidget
        {
            get
            {
                if (_widget == null)
                {
                    _widget = GetComponent<UIWidget>();
                }
                return _widget;
            }
        }

        public override Rect UpdateWidget(float width = 0, float height = 0)
        {
            var widget = GetWidgetAdapter();
            if (widget != null)
            {
                var bound = Bounds;
                bound.size = new Vector3(widget.Width, widget.Height);
                Bounds = bound;
            }
            return base.UpdateWidget(width, height);
        }

        // ReSharper disable once InconsistentNaming
        protected virtual void OnFreshCRCView(ItemData itemData)
        {
        }

        public virtual void UpdateBoxCollider()
        { 
            NGUITools.UpdateWidgetCollider(gameObject,true); 
        }

        private static YxView _curView;
        /// <summary>
        /// 
        /// </summary>
        public void ShowTip()
        {
            if (_curView == this)
            {
                UITooltip.Hide();
                _curView = null;
                return;
            }
            _curView = this;
            var itemData = GetData<ItemData>();
            if (itemData == null) return;
            UITooltip.Show(itemData.Tip);
        }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            return true;
        }
    } 
}
