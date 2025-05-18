using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Models.CreateRoomRules;
using Assets.Scripts.Common.UI;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class RuleRowView : FreshLayoutBaseView
    {
        /// <summary>
        /// 多选预制
        /// </summary>
        [Tooltip("多选预制，如果不设置，将使用RadioPerfab")]
        public NguiCheckBox CheckBoxPerfab;
        /// <summary>
        /// 单选预制
        /// </summary>
        [Tooltip("单选预制，如果不设置，将使用CheckBoxPerfab")]
        public NguiCheckBox RadioPerfab;
        /// <summary>
        /// 按钮预制
        /// </summary>
        [Tooltip("按钮预制，如果不设置，将使用CheckBoxPerfab")]
        public NguiCheckBox ButtonPerfab;
        /// <summary>
        /// 标签页预制
        /// </summary>
        [Tooltip("标签页预制，如果不设置，将使用ButtonPerfab")]
        public NguiCheckBox TabPerfab;
        /// <summary>
        /// label预制
        /// </summary>
        [Tooltip("label预制")]
        public NguiCRLabel LabelPerfab;
        /// <summary>
        /// input预制
        /// </summary>
        [Tooltip("input预制")]
        public NguiCRInput InputPerfab;
        /// <summary>
        /// pop预制
        /// </summary>
        [Tooltip("pop预制")]
        public NguiCRPop PopPerfab;
        /// <summary>
        /// 范围预制
        /// </summary>
        [Tooltip("范围预制")]
        public NguiCRSlider SliderPerfab;

        [Tooltip("行容器")]
        public Transform RowContainer;
        [Tooltip("行间的线")]
        public GameObject Line;
        private Transform _rowContainer;

        protected override void OnAwake()
        {
            InitStateTotal = 2;
            CheckIsStart = true;
            var defaultBox = RadioPerfab;
            if (RadioPerfab == null)
            {
                defaultBox = RadioPerfab = CheckBoxPerfab;
            }else if (CheckBoxPerfab == null)
            {
                defaultBox = CheckBoxPerfab = RadioPerfab;
            } 
            if (ButtonPerfab == null)
            {
                ButtonPerfab = defaultBox;
            }
            if (TabPerfab == null)
            {
                TabPerfab = ButtonPerfab;
            }
        }

        protected override void OnStart()
        {
            var widgetAdapter = GetWidgetAdapter();
            if (widgetAdapter == null)
            {
                gameObject.AddComponent<NguiWidgetAdapter>();
            }
            base.OnStart();
        }

        protected override void InitViews()
        {
            ClearBufferView();
            var rowData = GetData<ItemRowData>();
            if (rowData == null || rowData.Parent.ViewIsHide(rowData.Id))
            {
                CallBack(IdCode);
                return;
            }
            YxWindowUtils.CreateItemParent(RowContainer, ref _rowContainer,transform);
            var rowX = rowData.X;
            var rowY = rowData.Y;
            var containerPos = _rowContainer.localPosition;
            if (!float.IsNaN(rowX))
            {
                containerPos.x = rowX;
            }
            if (!float.IsNaN(rowY))
            {
                containerPos.y = rowY;
            }
            _rowContainer.localPosition = containerPos;
            var rowItems = rowData.Items;
            var dataCount = rowItems.Count;
            CreateNewView(0,dataCount, rowItems, _rowContainer);
            if (BufferViewCount < 1)
            {
                OnFreshLayout(); 
            }
        }

        protected override YxView CreateView<T>(T data, Transform pts, Vector3 pos = default(Vector3))
        {
            var itemData = data as ItemData;
            if (itemData == null) return null;
            var ruleInfo = itemData.Parent; 
            if (ruleInfo.ViewIsHide(itemData.Id))
            { 
                return null;
            }
            var view = CreateRuleItemView(itemData);
            if (view == null) return null;
            GameObjectUtile.ResetTransformInfo(view.transform, pts);
            return view;
        }

        public bool HasChildView()
        {
            return BufferViewCount > 0;
        }

        protected NguiCRComponent CreateRuleItemView(ItemData rowItemInfo)
        {
            NguiCRComponent view = null;
            switch (rowItemInfo.Type)
            {
                case RuleItemType.checkbox:
                    view = CreateBoxComponent(CheckBoxPerfab,rowItemInfo, rowItemInfo.Parent.CurTabId);
                    break;
                case RuleItemType.radio:
                    view = CreateBoxComponent(RadioPerfab, rowItemInfo, rowItemInfo.Parent.CurTabId);
                    break;
                case RuleItemType.button:
                    view = CreateBoxComponent(ButtonPerfab, rowItemInfo, rowItemInfo.Parent.CurTabId);
                    if (rowItemInfo.State)
                    {
                        rowItemInfo.Parent.SetButtonId(rowItemInfo.Group, rowItemInfo.Id);
                    }
                    break;
                case RuleItemType.tab:
                    view = CreateBoxComponent(TabPerfab, rowItemInfo);
                    break;
                case RuleItemType.slider:
                    view = CreateValueComponent(SliderPerfab, rowItemInfo);
                    break; 
                case RuleItemType.label:
                    view = CreateValueComponent(LabelPerfab, rowItemInfo);
                    break; 
                case RuleItemType.input:
                    view = CreateValueComponent(InputPerfab, rowItemInfo);
                    break; 
                case RuleItemType.pop:
                    view = CreateValueComponent(PopPerfab, rowItemInfo);
                    break; 
            }
            if (view == null)
            {
                YxDebug.LogError("View类型：{0}不存在","RuleRowView",null, rowItemInfo.Type);
            }
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static NguiCRComponent CreateBoxComponent(NguiCRComponent prefab, ItemData rowItemInfo,string tabId=null)
        {
            var view = CreateNguiCrComponent(prefab,rowItemInfo);
            if (view == null)
            {
                rowItemInfo.State = false;
                return null;
            }
            var rowInfo = rowItemInfo.Parent;
            rowItemInfo.State = CreateRoomRuleInfo.GetItemState(tabId, rowItemInfo.Id, rowItemInfo.Group, rowInfo.GameKey, rowItemInfo.DefaultState);
            return view;
        }

        private static NguiCRComponent CreateValueComponent(NguiCRComponent prefab, ItemData rowItemInfo)
        {
            var view = CreateNguiCrComponent(prefab, rowItemInfo);
            if (view == null)
            {
                return null;
            }
            var rowInfo = rowItemInfo.Parent;
            rowItemInfo.Value = CreateRoomRuleInfo.GetItemValue(rowInfo.CurTabId, rowItemInfo.Id, rowInfo.GameKey, rowItemInfo.Value);
            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static NguiCRComponent CreateNguiCrComponent(NguiCRComponent prefab, ItemData rowItemInfo)
        {
            if (prefab == null) { return null; }
            var view = Instantiate(prefab);
            rowItemInfo.View = view;
            view.Id = rowItemInfo.Id; 
            return view;
        }

        protected override void OnFreshLayout()
        {
            var rowDatas = GetData<ItemRowData>();
            if (rowDatas == null)
            {
                CallBack(IdCode);
                return;
            }
            var count = BufferViewCount;
            var curlpos = Vector3.zero;
            var bound = Vector2.zero;
            for (var i = 0; i < count; i++)
            {
                var view = GetBufferView(i) as NguiCRComponent;
                if (view == null)
                {
                    continue;
                }
                var ts = view.transform;
                ts.localScale = Vector3.one;
                ts.localPosition = curlpos;
                view.UpdateBoxCollider();
                var size = view.Bounds.size;
                curlpos.x = curlpos.x + size.x + rowDatas.Spacing;
                bound.x = curlpos.x;
//                Debug.LogError("2x: "+curlpos.x);
                if (size.y > bound.y) bound.y = size.y;
            }
            if(bound.x>0) { bound.x -= rowDatas.Spacing;}
            var lineY = rowDatas.Height;
            if (lineY > 0) { bound.y = lineY;}
            lineY = lineY > 0 ? bound.y = lineY : bound.y;
            UpdateWidget(bound.x, lineY);
//            DrawLine(lineY);
            CallBack(IdCode);
        }
        public void DrawLine(float y)
        {
            if (Line == null) { return; }
            Line.SetActive(true);
            var pos = new Vector3(0, -y, 0);
            Line.transform.localPosition = pos;
        }

        public void HideLine()
        {
            if (Line == null) { return; }
            Line.SetActive(false);
        }
    }
}
