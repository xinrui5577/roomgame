using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.Common.UI;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.GameResult
{
    public class GameResult : MonoSingleton<GameResult>
    {
        /// <summary>
        /// 结果Item
        /// </summary>
        public PlayerResultInfo PanelItem;

        /// <summary>
        /// 所有玩家的结果
        /// </summary>
        private PlayerResultInfo[] _panelItems;

        /// <summary>
        /// 继续按钮
        /// </summary>
        public GameObject ContinueBtn;

        /// <summary>
        /// 显示大结算按钮
        /// </summary>
        public GameObject ShowGameOverBtn;

        /// <summary>
        /// 胡的那张牌
        /// </summary>
        private List<int> _huCards;

        /// <summary>
        /// 结果的grid
        /// </summary>
        [SerializeField] private UIGrid ResultGrid;

        /// <summary>
        /// 结果显示的父级
        /// </summary>
        [SerializeField] private GameObject _showParent;

        /// <summary>
        /// 返回大厅按钮
        /// </summary>
        [SerializeField] private GameObject _backToHallBtn;

        /// <summary>
        /// 按钮组
        /// </summary>
        [SerializeField] private UIGrid _btnsGrid;

        /// <summary>
        /// 显示结束标识
        /// </summary>
        [SerializeField] private UISprite _resultTitle;

        /// <summary>
        /// 显示结束的女孩（输，流局显示为lose，赢为win）
        /// </summary>
        [SerializeField] private UITexture _resultGirl;
        [Tooltip("是否显示平局title")]
        public bool TitleEquleType;
        [Tooltip("是否刷新标题图片")]
        public bool FreshTitleBg;

        /// <summary>
        /// 其它牌的显示处理，如宝牌，癞子牌之类的
        /// </summary>
        [SerializeField] private GameObject _showOtherCards;

        /// <summary>
        /// 其它牌的牌堆
        /// </summary>
        [SerializeField] private DefLayout _otherCards;
        
        /// <summary>
        /// 失败图片
        /// </summary>
        [SerializeField]
        private Texture _loseTex;
        /// <summary>
        /// 胜利图片
        /// </summary>
        [SerializeField]
        private Texture _winTex;
        [Tooltip("平局图片")]
        public Texture EquleTex;
        [Tooltip("胜利玩家设置到首位")]
        public bool WinSetFirst = true;
        /// <summary>
        /// 下局的庄位置
        /// </summary>
        private int _nextBank;
        /// <summary>
        /// 所有玩家
        /// </summary>
        ISFSArray pls;
        /// <summary>
        /// 所有牌
        /// </summary>
        ISFSArray cardsArray;
        /// <summary>
        /// 是否流局
        /// </summary>
        private bool _isRunOut=false;
        /// <summary>
        /// 这局结算是否重宝
        /// </summary>
        private bool _isBaoExist = false;
        /// <summary>
        /// 是否存在清风玩法
        /// </summary>
        private bool _isQingfengExist=false;
        /// <summary>
        /// 结算数据
        /// </summary>
        private List<ResultInfoData> _resultDatas=new List<ResultInfoData>(); 
        /// <summary>
        /// 本局分数结果
        /// </summary>
        private Dictionary<int, int> _resultNowRoundScore = new Dictionary<int, int>();
        [Tooltip("大结算事件")]
        public List<EventDelegate> GameOverAction=new List<EventDelegate>(); 
        private Mahjong2DGameManager Manager
        {
            get { return App.GetGameManager<Mahjong2DGameManager>(); }
        }

        private Mahjong2DGameData Data
        {
            get { return App.GetGameData<Mahjong2DGameData>(); }
        }

        public bool IsGameOver
        {
            get
            {
                if (Manager==null)
                {
                    return false;
                }
                else
                {
                    return Manager.IsGameOver;
                }
            }
        }

        public override void Awake()
        {
            base.Awake();
            Facade.EventCenter.AddEventListeners<string, int>(RequestCmd.GameOver,OnGameOver);
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<string, int>(RequestCmd.GameOver, OnGameOver);
            base.OnDestroy();
        }

        void OnGameOver(int data)
        {
            FreshBtn();
        }

        /// <summary>
        /// 显示结果面板
        /// </summary>
        /// <param name="param"></param>
        internal void ShowResultPanel(ISFSObject param, List<KeyValuePair<int, int>> fenZhangData, bool isRunOut = false)
        {
            _showParent.TrySetComponentValue(true);
            FreshBtn();
            _isRunOut = isRunOut;
            _isBaoExist = param.ContainsKey(RequestKey.KeyChongBao);
            YxDebug.LogError(string.Format("冲宝标识是:{0}",_isBaoExist));
            _isQingfengExist = Data.BootEnv.ContainsKey(ConstantData.RuleQingFeng);
            Init(param, fenZhangData);
            GameTable.GameTable.Instance.Reset();
            Manager.ResetReadyState();
        }

        private void FreshBtn()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(GameOverAction.WaitExcuteCalls());
            }
            _backToHallBtn.TrySetComponentValue(!Manager.GameType.IsCreateRoom);
            _btnsGrid.repositionNow = true;
        }

        /// <summary>
        /// 计算出面板应该显示的东西
        /// </summary>
        /// <param name="param"></param>
        public void Init(ISFSObject param, List<KeyValuePair<int, int>> fenZhangData)
        {
            int _huCard;
            GameTools.TryGetValueWitheKey(param, out pls, RequestKey.KeyPlayerList);
            GameTools.TryGetValueWitheKey(param, out cardsArray, RequestKey.KeyCardsArr);
            GameTools.TryGetValueWitheKey(param, out _huCard, RequestKey.KeyHuCard);
            GameTools.TryGetValueWitheKey(param, out _nextBank, RequestKey.KeyNextBank);
            _huCards = new List<int>();
            _resultNowRoundScore.Clear();
            _resultDatas.Clear();
            if (_huCard != 0)
            {
                _huCards.Add(_huCard);
            }
            InitInfoUI(fenZhangData);
        }

        /// <summary>
        /// 显示UI
        /// </summary>
        private void InitInfoUI(List<KeyValuePair<int, int>> fenZhangData)
        {
            if (_panelItems == null|| _panelItems.Length==0)
            {
                _panelItems = new PlayerResultInfo[Manager.PlayerNumber];
                var gridGameObj = ResultGrid.gameObject;
                var prefabObj = PanelItem.gameObject;
                if (gridGameObj&& prefabObj)
                {
                    for (int i = 0; i < _panelItems.Length; i++)
                    {
                        _panelItems[i]=gridGameObj.AddChild(prefabObj).GetComponent<PlayerResultInfo>();
                    }
                }
            }
            else
            {
                for (int i = 0; i < _panelItems.Length; i++)
                {
                    _panelItems[i].ResetInfo();
                }
            }
            ResultGrid.repositionNow = true;
            int num = 0;
            for (int i = 0; i < pls.Count; i++)
            {
                ResultInfoData infoData=new ResultInfoData(pls.GetSFSObject(i), cardsArray.GetIntArray(i).ToList());
                var pairIndex=fenZhangData.FindIndex(keyPair => keyPair.Key == infoData.UserSeat);
                if (pairIndex>-1)
                {
                    infoData.FenZhangCard = fenZhangData[pairIndex].Value;
                    infoData.HandList.Remove(infoData.FenZhangCard);
                }
                else
                {
                    infoData.FenZhangCard = 0;
                }
                _resultNowRoundScore.Add(infoData.UserSeat, infoData.NowRoundScore);
                if (i.Equals(0))
                {
                    _resultDatas.Add(infoData);
                }
                else
                {
                    if(infoData.HuType>0&& WinSetFirst)
                    {
                        _resultDatas.Insert(0,infoData);
                    }
                    else
                    {
                        _resultDatas.Add(infoData);
                    }
                }
            }
            for (int i = 0; pls != null && i < _panelItems.Length; i++)
            {
                    _panelItems[i].SetResultInfo(
                    _resultDatas[i],
                    _huCards,
                    _isBaoExist,
                    _isQingfengExist
                    );
            }
            DealTitleAndGirl();
            DealOtherCards();
        }

        /// <summary>
        /// 点击继续按钮
        /// </summary>
        public void OnContinueGameClick()
        {
            if (App.GetGameManager<Mahjong2DGameManager>().IsGameOver)
            {
                YxDebug.LogError("点击时游戏状态是大结算");
                Manager.ShowGameOver();
            }
            else
            {
                YxDebug.LogError("点击时游戏状态不是大结算,当前状态是："+ App.GetGameManager<Mahjong2DGameManager>().GameTotalState);
                Manager.ContinueGame();
                Manager.StartPosition = _nextBank;
                Manager.CurrentPosition = _nextBank;
            }
            CloseWindow();
        }
        /// <summary>
        /// 处理title与女孩的显示
        /// </summary>
        private void DealTitleAndGirl()
        {
            if(_resultGirl==null||_resultTitle==null)
            {
                return;
            }
            if (_isRunOut)
            {
                _resultGirl.mainTexture = _loseTex;
                _resultTitle.spriteName = ConstantData.ResultRunOutTitle;
            }
            else
            {
                int selfSeat = Manager.SelfPlayer.UserSeat;
                if (_resultNowRoundScore.ContainsKey(selfSeat))
                {
                    var value = _resultNowRoundScore[selfSeat];
                    if(value>0)
                    {
                        _resultGirl.mainTexture = _winTex;
                        _resultTitle.spriteName = ConstantData.ResultWinTitle;
                    }
                    else
                    {
                        if (TitleEquleType)
                        {
                            if (value == 0)
                            {
                                _resultGirl.mainTexture = EquleTex;
                                _resultTitle.spriteName = ConstantData.ResultEqualTitle;
                            }
                            else
                            {
                                _resultGirl.mainTexture = _loseTex;
                                _resultTitle.spriteName = ConstantData.ResultLoseTitle;
                            }
                            
                        }
                        else
                        {
                            _resultGirl.mainTexture = _loseTex;
                            _resultTitle.spriteName = ConstantData.ResultLoseTitle;
                        }
                    }
                }
            }
            if (FreshTitleBg)
            {
                _resultTitle.MakePixelPerfect();
            }
        }
        /// <summary>
        /// 处理其它牌
        /// </summary>
        private void DealOtherCards()
        {
            if (_showOtherCards != null)
            {
                List<Transform> list;
                if (_otherCards != null)
                {
                    list = _otherCards.GetChildList();
                }
                else
                {
                    YxDebug.LogError("小结算的其它牌的处理那块没弄啊，自己去加上");
                    return;
                }
                List<int> baos=new List<int>();
                if (Manager.LaiZiNum != 0&&Manager.FanNum!=0)
                {
                    baos.Add(Manager.LaiZiNum);
                }
                if (Manager.BaoCards.Count > 0)
                {
                   baos = Manager.BaoCards;
                }
                if(baos.Count>0)
                {
                    _showOtherCards.SetActive(true);
                    for (int i = 0, lenth = list.Count, baoCount = baos.Count; i < lenth; i++)
                    {
                        if ( baoCount>i)
                        {
                            GameTools.SetMahjongValueByTrans(list[i], (EnumMahjongValue)baos[i]);
                        }
                        else
                        {
                            list[i].gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    _showOtherCards.SetActive(false);
                }
   
            }
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        public void CloseWindow()
        {
            _showParent.SetActive(false);
        }
    }
}