using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Components;
using Assets.Scripts.Hall.Controller;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;


namespace Assets.Scripts.Hall.View.ListViews
{
    [RequireComponent(typeof(Rigidbody))]
    public class GameListItem : YxNguiListItem
    {
        [Tooltip("背景")]
        public UISprite Background;
        [Tooltip("背景名称前缀，后边会自动加上游戏列表分组对应的值")]
        public string BackgroundNamePrefix;
        [Tooltip("背景样式个数")]
        public int BackgroundCount;
        [Tooltip("游戏名称")]
        public NguiLabelAdapter GameNameLabel;
        [Tooltip("Item默认的box大小，如果GamelistItemView中的Widget为空时，将使用此Widget")]
        public UIWidget DefaultBoxWidget;
        [Tooltip("保持样式，不加载ItemView")]
        public bool KeepStyle = false;
        /// <summary>
        /// 移动的时候要隐藏的物体
        /// </summary>
        public GameObject[] HideObjectInMove;


        private UIButton _btn;
        private GameListItemView _itemView;
        private NguiPanelAdapter _panelAdapter;
        protected GameUnitModel Model;
        
        protected override void OnAwake()
        {
            InitStateTotal = 2;
            CheckIsStart = true;
            _panelAdapter = GetComponent<NguiPanelAdapter>();
            _btn = GetComponent<UIButton>();
//            _btn.state = UIButtonColor.State.Disabled;
        }

        private string _bundleName;
        protected override void FreshData()
        {
            if (Data is string)
            {
                Model = GameListModel.Instance.GameUnitModels[Data.ToString()];
            }
            else
            {
                Model = Data as GameUnitModel;
            }
            if (Model == null)
            { 
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            if (Background != null)
            {
                var gm = GameListModel.Instance;
                var group = gm.GetGroup(gm.CurGroup);
                var index = group == null || KeepStyle ? Model.Index % BackgroundCount :group.Type % BackgroundCount;
                var normalSprite = string.Format("{0}{1}", BackgroundNamePrefix, index);
                Background.spriteName = normalSprite;
                if(_btn!=null) { _btn.normalSprite = normalSprite;}
            }
            if (GameNameLabel != null)
            {
                GameNameLabel.SetActive(KeepStyle);
                GameNameLabel.Text(Model.GameName);
            }
            name = Model.GameKey;
            if (_itemView != null) { Destroy(_itemView.gameObject); }
            if (KeepStyle) { return; }
            var assetname = string.Format("gamelist_{0}",name);
            if (!string.IsNullOrEmpty(ViewType))
            {
                assetname = string.Format("{0}_{1}", assetname, ViewType);
            }
            var bundlePrefix = string.Format("{0}_{1}", App.Skin.GameInfo,name);
            _bundleName = string.Format("{0}/{1}", bundlePrefix, assetname);

            var asInfo = new AssetBundleInfo
            {
                Name = _bundleName,
                Attach = App.Skin.GameInfo,
                AssetName = assetname
            };
            if (KeepStyle)
            {
                
            }
            else
            {
                ResourceManager.LoadAssesAsync<GameObject>(asInfo,false, CreateItem);
            }
            #region 注释 同步加载
            //            var go = ResourceManager.LoadAsset(App.Skin.GameInfo, bundleName, assetname);//App.HallName
            //            if (go == null) return;
            //            go = Instantiate(go);
            //            var ts = go.transform;
            //            var lcScale = ts.localScale;
            //            var lcPos = ts.localPosition;
            //            var lcRot = ts.localRotation;
            //            ts.parent = transform;
            //            ts.localPosition = lcPos;
            //            ts.localRotation = lcRot;
            //            ts.localScale = lcScale;
            //            _itemView = go.GetComponent<GameListItemView>();
            //            if (_itemView!=null)
            //            {
            //                _itemView.FreshBtnClickBound(_btn, DefaultBoxWidget, Model.GameState == GameState.Developing);
            //            }
            //            else
            //            {
            //                YxDebug.LogError("没有GameListItemView","GameListItem");
            //            }
            #endregion
        }

        private void CreateItem(AssetBundleInfo info)
        {
            if (!info.Name.Equals(_bundleName)) { return; }
            if (_itemView != null) { Destroy(_itemView.gameObject); }
            var go = info.GetAsset<GameObject>();
            if (go == null) { return; }
            go = Instantiate(go);
            var ts = go.transform;
            var lcScale = ts.localScale;
            var lcPos = ts.localPosition;
            var lcRot = ts.localRotation;
            ts.parent = transform;
            ts.localPosition = lcPos;
            ts.localRotation = lcRot;
            ts.localScale = lcScale;
            _itemView = go.GetComponent<GameListItemView>();
            if (_itemView != null)
            {
                _itemView.MainYxView = this;
                _itemView.FreshBtnClickBound(_btn, DefaultBoxWidget, Model.GameState == GameState.Developing);
                if (_itemView.NameLabel != null)
                {
                    GameNameLabel = _itemView.NameLabel;
                }
            }
            else
            {
                YxDebug.LogError("没有GameListItemView", "GameListItem");
            }
            if (GameNameLabel != null)
            {
                GameNameLabel.SetActive(true);
                GameNameLabel.Text(Model.GameName);
            }
        }


        /// <summary>
        /// 点击游戏列表，进入房间列表
        /// </summary>
        public void OnGameClick()
        {
            //todo 判断是否已下
            if (Model == null) return;
            switch (Model.GameState)
            {
                case GameState.Developing://开发中
                    YxMessageBox.Show("游戏在努力开发中，敬请期待!!");
                    return;
                case GameState.CreateMode://创建模式
                    OnOpenCreateWindow();
                    break;
                case GameState.Match://比赛模式
                    //todo 比赛报名界面
                    OnOpenGame();
                    break;
                case GameState.Desk://走势
                    HallController.Instance.ShowDeskListPanel(Model.GameKey);
                    break;
                default:// 1个房间时，直接进入游戏，>1时，显示房间列表
                    var roomlistCtrl = RoomListController.Instance;
                    roomlistCtrl.GetRoomlistAndShow(Model);
                    break;
            }
        }
         
        /// <summary>
        /// 点击游戏列表，打开创建窗口
        /// </summary>
        public void OnOpenCreateWindow()
        {
            if (Model != null && Model.GameState == GameState.Developing)
            {
                YxMessageBox.Show("游戏在努力开发中，\r\n敬请期待!!!!");
                return;
            }
            var win = CreateOhterWindowWithT<CreateRoomWindow>("CreateRoomWindow");
            win.GameKey = name;
        }

        /// <summary>
        /// 点击游戏列表，打开指定创建窗口//todo 临时处理
        /// </summary>
        public void OnOpenAssuredCreateWindow()
        {
            if (Model != null && Model.GameState == GameState.Developing)
            {
                YxMessageBox.Show("游戏在努力开发中，\r\n敬请期待!!!!");
                return;
            }
            var win = CreateOhterWindowWithT<CreateRoomWindow>("CreateRoomWindow");
            win.GameKey = name;
            win.IsDesignated = true;
        }

        /// <summary>
        /// 点击游戏列表，打开游戏
        /// </summary>
        public void OnOpenGame()
        {
            if (Model == null) return;
            RoomListController.Instance.QuickGame(Model.GameKey);
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="isAction"></param>
        public override void AwakAction(bool isAction)
        {
            if (_itemView == null) return;
            _itemView.AwakAction(isAction);
            if (isAction) _itemView.SetOrder(Order);
        }

        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="color"></param>
        public override void SetColor(Color color)
        {
            var widgetAdapter = GetWidgetAdapter();
            if (widgetAdapter != null)
            {
                widgetAdapter.Color = color;
            }
            if (Background != null)
            {
                Background.color = color;
            }
            if (_btn != null)
            {
                _btn.defaultColor = color;
            }
            if (_itemView != null)
            {
                _itemView.SetColor(color);
            }
        }

        public override void SetOrder(int order)
        {
            Order = order;
            if (_panelAdapter != null)
            {
                _panelAdapter.SortingOrder(order);
            }
        }

        protected override void OnMoveAction(bool isMove)
        {
            if (HideObjectInMove != null)
            {
                GameObjectUtile.DisplayComponent(HideObjectInMove, !isMove);
            }
            if (_itemView == null) return;
            _itemView.MoveAction(isMove);
        }
    }
}
