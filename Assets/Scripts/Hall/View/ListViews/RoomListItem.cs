using System.Globalization;
using Assets.Scripts.Common.Components;
using Assets.Scripts.Common.Enums;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.ListViews
{
    /// <summary>
    /// 房间列表item
    /// </summary> 
    public class RoomListItem : YxNguiListItem
    {
        [Tooltip("游戏名称")]
        public UILabel GameNameLabel;
        /// <summary>
        /// 房间标题
        /// </summary>
        [Tooltip("房间标题")]
        public UILabel RoomNameLabel;
        /// <summary>
        /// 金币上限
        /// </summary>
        [Tooltip("金币上限")]
        public UILabel MaxCoinLabel;
        /// <summary>
        /// 图标纹理
        /// </summary>
        [Tooltip("图标纹理")]
        public UITexture IconTexture;
        /// <summary>
        /// 金币下线
        /// </summary>
        [Tooltip("金币下线")]
        public UILabel MinCoinLabel;
        /// <summary>
        /// 底注
        /// </summary>
        [Tooltip("底注")]
        public UILabel AnteLabel;
        
        [Tooltip("背景")]
        public Transform BackGround;
        /// <summary>
        /// 背景父类（方便自定义位置，可以用作背景、图标）
        /// </summary>
        [Tooltip("背景名称前缀，后边会自动加上游戏列表分组对应的值")]
        public string BackgroundNamePrefix;
        [Tooltip("背景类型个数")]
        public int BackgroundTypeMaxCount;
        [Tooltip("RoomListItemView的容器")]
        public Transform ViewContainer;
        private RoomUnitModel _model;
        [Tooltip("游戏名称")]
        public UIWidget BetInfoPanel;
        [Tooltip("加载类型")]
        public GameInfoAssetType AssetType = GameInfoAssetType.Roomlist;

        private GameObject _view;
        private RoomListItemState _stateObj;
        protected override void FreshData()
        {
            base.FreshData();
            _model = Data as RoomUnitModel;
            if (_model == null) return;
            CreateBackground(_model);//itemType 
            CreateIcon(_model.IconUrl);
            name = _model.Index.ToString();
            if (RoomNameLabel!=null) RoomNameLabel.text = _model.RoomName;
            UpdateGameLabel();
            UpdateBetInfo();
        }

        protected void UpdateGameLabel()
        {
            if (GameNameLabel == null) { return;}
            var gameunitM = GameListModel.Instance.GameUnitModels;
            var gkey = _model.GameKey;
            if (gameunitM.ContainsKey(gkey)) GameNameLabel.text = gameunitM[gkey].GameName;
        }

        /// <summary>
        /// 刷新房间门槛信息
        /// </summary>
        protected void UpdateBetInfo()
        {
            long minCoin;
            long.TryParse(_model.MinGold, out minCoin);
            if (BetInfoPanel != null)
            {
                var ts = BetInfoPanel.transform;
                if (App.AppStyle == YxEAppStyle.Concise || minCoin < 1)
                {
                    ts.localScale = Vector3.zero;
                    return;
                }
                ts.localScale = Vector3.one;
            }
            long ante;
            long.TryParse(_model.Ante, out ante);
            AnteLabel.TrySetComponentValue(YxUtiles.GetShowNumber(ante).ToString(CultureInfo.InvariantCulture));
            long maxCoin;
            long.TryParse(_model.MaxGold, out maxCoin);
            MaxCoinLabel.TrySetComponentValue(maxCoin < 1 ? "∞" : YxUtiles.GetShowNumber(maxCoin).ToString(CultureInfo.InvariantCulture));
            MinCoinLabel.TrySetComponentValue(YxUtiles.GetShowNumber(minCoin).ToString(CultureInfo.InvariantCulture));
        }

        private void CreateIcon(string url)
        {
            if (IconTexture == null) return;
            if (string.IsNullOrEmpty(url)) return; 
            Facade.Instance<AsyncImage>().GetAsyncImage(url, (texture,hashCode)=>
                {
                    IconTexture.mainTexture = texture;
                });
        }

        private void CreateBackground(RoomUnitModel roomModel)
        {
            int typeId;
            int.TryParse(roomModel.TypeId, out typeId);
            var gamekey = roomModel.GameKey;
            if (ViewContainer == null)//没有容器
            {
                ChangeBackGround(BackGround, typeId);
                return;
            }
            //有子背景
            if (_view != null) Destroy(_view);
            var prefix = App.Skin.GameInfo; 
            var namePrefix = string.Format("{0}_{1}", prefix, gamekey);//gameinfo_gamekey
            var roomItemName = string.Format("{0}_{1}", AssetType.ToString().ToLower(), gamekey);//roomlist_gamekey
            var styleName = string.Format("{0}_{1}", roomItemName, typeId);//先找带typeId样式
            var bundleName = string.Format("{0}/{1}", namePrefix, styleName);//gameinfo_gamekey/roomlist_gamekey
            _view = ResourceManager.LoadAsset(prefix, bundleName, styleName);
            if (_view == null)//如果没有带typeId样式，加载默认的样式
            {
                bundleName = string.Format("{0}/{1}", namePrefix, roomItemName);//默认样式
                _view = ResourceManager.LoadAsset(prefix, bundleName, roomItemName);
                if (_view == null)
                {
                    return;
                }
            }
            _view = Instantiate(_view);
            var ts = _view.transform;
            ts.parent = ViewContainer;
            ts.localPosition = Vector3.zero;
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
            var rlView = _view.GetComponent<RoomListItemBaseView>();
            if (rlView != null)
            {
                rlView.MainYxView = this;
                rlView.Init(roomModel);
            }
            ChangeBackGround(BackGround==null ? ts : BackGround, typeId);
            _stateObj = _view.GetComponent<RoomListItemState>();
            if (_stateObj == null) return;
            _stateObj.StartChangeRoom();
            _stateObj.UpAnchor();
        }

        private void ChangeBackGround(Component background,int index)
        {
            if (string.IsNullOrEmpty(BackgroundNamePrefix) || BackgroundTypeMaxCount <= 0) return;
            var sprite = background.GetComponent<UISprite>();
            if (sprite == null) return; 
            var normalSprite = string.Format("{0}{1}", BackgroundNamePrefix, index % BackgroundTypeMaxCount);
            sprite.spriteName = normalSprite;
            var btn = GetComponent<UIButton>();
            if (btn == null) { return ; }
            btn.normalSprite = normalSprite;
        }

        public override void AwakAction(bool isAction)
        {
            if (_stateObj == null) return;
            _stateObj.AwakAction(isAction);
        }


        public void OnRoomClick(string index)
        {
            var room = RoomListModel.Instance.RoomUnitModel[int.Parse(index)];
            YxTools.GoldJoinRoom(room.GameKey, room.TypeId, delegate
            {
                RoomListController.Instance.OpenGameWithCheck(room);
            });
        } 
    }
}
