using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class
        GameResult : MonoSingleton<GameResult>
    {
        /// <summary>
        ///     返回大厅按钮
        /// </summary>
        [SerializeField] private GameObject _backToHallBtn;

        /// <summary>
        ///     按钮组
        /// </summary>
        [SerializeField] private UIGrid _btnsGrid;

        /// <summary>
        ///     胡的那张牌
        /// </summary>
        private List<int> _huCards;

        /// <summary>
        ///     下局的庄位置
        /// </summary>
        private int _nextBank;

        /// <summary>
        ///     所有玩家的结果
        /// </summary>
        private PlayerResultInfo[] _panelItems;

        /// <summary>
        ///     结果显示的父级
        /// </summary>
        [SerializeField] private GameObject _showParent;

        /// <summary>
        ///     所有牌
        /// </summary>
        private ISFSArray cardsArray;

        /// <summary>
        ///     继续按钮
        /// </summary>
        public GameObject ContinueBtn;

        /// <summary>
        ///     结果Item
        /// </summary>
        public PlayerResultInfo PanelItem;

        /// <summary>
        ///     所有玩家
        /// </summary>
        private ISFSArray pls;

        /// <summary>
        ///     结果的grid
        /// </summary>
        [SerializeField] private UIGrid ResultGrid;

        public Lyzz2DGameManager Manager
        {
            get { return App.GetGameManager<Lyzz2DGameManager>(); }
        }

        /// <summary>
        ///     显示结果面板
        /// </summary>
        /// <param name="param"></param>
        internal void ShowResultPanel(ISFSObject param)
        {
            _showParent.SetActive(true);
            _backToHallBtn.SetActive(Manager.GameType.GameRoomType >= 0);
            _btnsGrid.repositionNow = true;
            Init(param);
            GameTable.Instance.Reset();
            Manager.ResetReadyState();
        }

        /// <summary>
        ///     计算出面板应该显示的东西
        /// </summary>
        /// <param name="param"></param>
        public void Init(ISFSObject param)
        {
            int _huCard;
            GameTools.TryGetValueWitheKey(param, out pls, RequestKey.KeyPlayerList);
            GameTools.TryGetValueWitheKey(param, out cardsArray, RequestKey.KeyCardsArr);
            GameTools.TryGetValueWitheKey(param, out _huCard, RequestKey.KeyHuCard);
            GameTools.TryGetValueWitheKey(param, out _nextBank, RequestKey.KeyNextBank);
            _huCards = new List<int>();
            if (_huCard != 0)
            {
                _huCards.Add(_huCard);
            }
            InitInfoUI();
        }

        /// <summary>
        ///     显示UI
        /// </summary>
        private void InitInfoUI()
        {
            if (_panelItems == null)
            {
                _panelItems = new PlayerResultInfo[Manager.PlayerNumber];
                for (var i = 0; i < _panelItems.Length; i++)
                {
                    _panelItems[i] = Instantiate(PanelItem.gameObject).GetComponent<PlayerResultInfo>();
                    _panelItems[i].transform.parent = ResultGrid.transform;
                    _panelItems[i].transform.localScale = Vector3.one;
                    _panelItems[i].transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                for (var i = 0; i < _panelItems.Length; i++)
                {
                    _panelItems[i].transform.localPosition = Vector3.zero;
                    _panelItems[i].ResetInfo();
                }
            }
            ResultGrid.cellHeight = _panelItems[0].GetComponent<UIWidget>().localSize.y;
            ResultGrid.repositionNow = true;
            for (var i = 0; pls != null && i < _panelItems.Length; i++)
            {
                _panelItems[i].SetResultInfo(pls.GetSFSObject(i), Manager.Players[i], cardsArray.GetIntArray(i).ToList(),
                    _huCards);
            }
        }

        /// <summary>
        ///     点击继续按钮
        /// </summary>
        public void OnContinueGameClick()
        {
            if (App.GetGameManager<Lyzz2DGameManager>().IsGameOver())
            {
                Manager.ShowGameOver();
            }
            else
            {
                Manager.ContinueGame();
                Manager.StartPosition = _nextBank;
                Manager.CurrentPosition = _nextBank;
            }
            CloseWindow();
        }

        /// <summary>
        ///     关闭界面
        /// </summary>
        public void CloseWindow()
        {
            _showParent.SetActive(false);
        }
    }
}