using System;
using System.Globalization;
using Assets.Scripts.Common.Components;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 房间列表item
    /// </summary> 
    [Obsolete("Use Assets.Scripts.Hall.View.ListViews.RoomListItem")]
    public class RoomListItem : NguiListItem
    {
        public string AssetPrefix = "roomlist";
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

        private GameObject _view;
        private RoomListItemState _stateObj;
        protected override void OnChangeData(IItemData itemData, string itemType)
        {
            _model = itemData as RoomUnitModel;
            if (_model == null) return;
            CreateBackground(_model,itemType); 
            CreateIcon(_model.IconUrl);
            name = _model.TypeId;
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

        private void CreateBackground(RoomUnitModel roomModel,string itemType)
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
            var roomItemName = string.Format("roomlist_{0}", gamekey);
            var namePrefix = string.Format("{0}_{1}", prefix, gamekey);
            if (!string.IsNullOrEmpty(itemType))
            {
                roomItemName = string.Format("{0}_{1}", roomItemName, itemType);
            }
            var bundleName = string.Format("{0}/{1}", namePrefix,roomItemName);
            _view = ResourceManager.LoadAsset(prefix, bundleName, roomItemName);
            if (_view == null) return;
            _view = Instantiate(_view);
            var ts = _view.transform;
            ts.parent = ViewContainer;
            ts.localPosition = Vector3.zero;
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
            ChangeBackGround(ts, typeId);
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
            sprite.spriteName = string.Format("{0}{1}", BackgroundNamePrefix, index % BackgroundTypeMaxCount);
        }

        public override void AwakAction(bool isAction)
        {
            if (_stateObj == null) return;
            _stateObj.AwakAction(isAction);
        }


        public void OnRoomClick(string roomType)
        {
            var room = RoomListModel.Instance.RoomUnitModel[int.Parse(roomType)];
            YxTools.GoldJoinRoom(room.GameKey, room.TypeId, delegate
            {
                RoomListController.Instance.OpenGameWithCheck(room);
            });
        }
         
    }
}
