using System.Globalization;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Components;
using com.yxixia.utile.YxDebug;
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
    public class RoomListItem : NguiListItem
    {
        public string AssetPrefix = "roomlist";
        [Tooltip("游戏名称")]
        public UILabel GameNameLabel;
        /// <summary>
        /// 房间标题
        /// </summary>
        public UILabel RoomNameLabel;
        /// <summary>
        /// 金币上限
        /// </summary>
        public UILabel MaxCoinLabel;
        /// <summary>
        /// 图标纹理
        /// </summary>
        public UITexture IconTexture;
        /// <summary>
        /// 金币下线
        /// </summary>
        public UILabel MinCoinLabel;
        /// <summary>
        /// 底注
        /// </summary>
        public UILabel AnteLabel; 
        /// <summary>
        /// 背景父类（方便自定义位置，可以用作背景、图标）
        /// </summary>
        public Transform BackGround;
        [Tooltip("背景名称前缀，后边会自动加上游戏列表分组对应的值")]
        public string BackgroundNamePrefix;
        [Tooltip("背景类型个数")]
        public int BackgroundTypeMaxCount;
        private RoomUnitModel _model;
        public UIWidget BetInfoPanel;

        private GameObject _view;
        private RoomListItemState _stateObj;
        protected override void OnChangeData(IItemData itemData)
        {
            _model = itemData as RoomUnitModel;
            if (_model == null) return;
            CreateBackground(_model); 
            CreateIcon(_model.IconUrl);
            name = _model.TypeId;
            if (RoomNameLabel!=null) RoomNameLabel.text = _model.RoomName;
            int minCoin;
            int.TryParse(_model.MinGold, out minCoin);
            if (BetInfoPanel != null)
            {
                var ts = BetInfoPanel.transform;
                if (App.AppStyle == EAppStyle.Concise || minCoin < 1)
                {
                    ts.localScale = Vector3.zero;
                    return;
                }
                ts.localScale = Vector3.one;
            }
            if (AnteLabel != null) AnteLabel.text = _model.Ante;
            int maxCoin;
            int.TryParse(_model.MaxGold, out maxCoin);
            if (MaxCoinLabel!=null) MaxCoinLabel.text = maxCoin < 1 ? "∞" : _model.MaxGold.ToString(CultureInfo.InvariantCulture);
            if (MinCoinLabel != null) MinCoinLabel.text = minCoin.ToString(CultureInfo.InvariantCulture);
            if (GameNameLabel != null)
            {
                var gameunitM = GameListModel.Instance.GameUnitModels;
                var gkey = _model.GameKey;
                if (gameunitM.ContainsKey(gkey))GameNameLabel.text = gameunitM[gkey].GameName;
            }
        }

        private void CreateIcon(string url)
        {
            if (IconTexture == null) return;
            if (string.IsNullOrEmpty(url)) return; 
            Facade.Instance<AsyncImage>().GetAsyncImage(url, texture =>
                {
                    IconTexture.mainTexture = texture;
                });
        }

        private void CreateBackground(RoomUnitModel roomModel)
        {
            int typeId;
            int.TryParse(roomModel.TypeId, out typeId);
            var gamekey = roomModel.GameKey;
            if (BackGround==null)//没有容器
            {
                ChangeBackGround(transform, typeId);
                return;
            }
            //有子背景
            if (_view != null) Destroy(_view);
            var prefix = App.GameListPath;
            var roomItemName = string.Format("roomlist_{0}", gamekey);
            var namePrefix = string.Format("{0}_{1}", prefix, gamekey);
            _view = ResourceManager.LoadAsset(prefix, namePrefix, roomItemName);
            if (_view == null) return;
            _view = Instantiate(_view);
            var ts = _view.transform;
            ts.parent = BackGround;
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
            var userInfo = UserInfoModel.Instance.UserInfo;
            var userCoin = YxUtiles.GetShowNumber(userInfo.CoinA);
            YxDebug.LogError("当前玩家的金币数量是："+ userCoin);
            var userBankCoin = YxUtiles.GetShowNumber(userInfo.BankCoin);
            YxDebug.LogError("当前玩家的银行金币数量是：" + userBankCoin);
            if (int.Parse(room.MinGold) > userCoin)//判断金币是否达到上限
            {
                YxMessageBox.Show(string.Format("非常抱歉!!!当前房间最低下限需要{0}金，您的金币只有{1}金，小于房间的最低下限不能进入。", room.MinGold, userCoin));
                return;
            }
            var max = long.Parse(room.MaxGold);
            if (max > 0 && max < (userCoin + userBankCoin))
            {
                YxMessageBox.Show(string.Format("非常抱歉!!!当前房间最高上限需要{0}金，您的金币已有{1}金(包含银行金币{2})，大于房间的最高上限将不能进入。", max, userCoin, userBankCoin));
                return;
            }
            RoomListController.Instance.OnDirectGame(room);
        }
         
    }
}
