using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using Assets.Scripts.Tea;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    public class RankWindow : YxTabPageWindow
    {
        #region LocalKeys
        /// <summary>
        /// Key排行类型
        /// </summary>
        private string _keyType = "type";
        /// <summary>
        /// Key游戏类型
        /// </summary>
        private string _keyGamekey = "game_key";
        #endregion

        /// <summary>
        /// 排行Item
        /// </summary>
        [Tooltip("排行Item")]
        public RankItemView PrefabItem;  
        [Tooltip("排行表格")]
        public UIGrid GridPrefab;
        [Tooltip("自己的排行Item")]
        public RankItemView SelfRankItem;
        [Tooltip("显示特殊背景的个数")]
        public int FirstFew = 3; 
        [Tooltip("标签未点背景")]
        public string PrefixUpStateName;
        [Tooltip("标签点击背景")]
        public string PrefixDownStateName;
        [Tooltip("是否需要标签页")]
        public bool NeedTabs;
        [Tooltip("滚动条")]
        public UIScrollView ScrollView;
        [Tooltip("排行类型，以‘,’分割：coin_a,cash_a")]
        public string RankTypes = "";
        [Tooltip("排行版中用于显示的panel")]
        public UIPanel ShowPanel;
        [Tooltip("多Tab时显示的部分")]
        public GameObject TabsArea;
        [Tooltip("只有一种排行榜时显示的文本")]
        public UILabel SingleLabel;
        [Tooltip("只有一种排行时，是否只显示文本内容")]
        public bool ShowSingleLabel;
        [Tooltip("请求时是否发送茶馆信息")]
        public bool RequestWithTeaInfo;
        [Tooltip("请求时是否有转圈显示")]
        public bool RequestWithHasWait=true;
        #region LocalDatas
        /// <summary>
        /// 临时排行布局
        /// </summary>
        private UIGrid _itemGrid;
        /// <summary>
        /// 排行数据
        /// </summary>
        private RankData _data;
        /// <summary>
        /// 是否为第一次请求
        /// </summary>
        private bool _firstRequest = true;
        /// <summary>
        /// 当前tab的数据
        /// </summary>
        private RankTabData _curTabData;
        #endregion
        protected override void OnAwake()
        {
            RankItemView.FirstFew = FirstFew;
            if (NeedTabs)
            {
                if (TabDatas.Length==0)
                {
                    RankTypes = "";
                    InitViewDict();
                    if (!string.IsNullOrEmpty(TabActionName))
                    {
                        InitStateTotal++;
                        var parm = new Dictionary<string, object>();
                        if (RequestWithTeaInfo)
                        {
                            parm[YxTools.KeyTeaId] = TeaUtil.CurTeaId;
                        }
                        Facade.Instance<TwManager>()
                            .SendAction(TabActionName, parm, UpdateView, false,null, RequestWithHasWait);
                    }
                    else
                    {
                        InitStateTotal++;
                        UpdateView(TabSatate);
                    }
                }
                else
                {
                    CreateTabels();
                }
            }
            else
            {
                RequestWithParm(OnLoadData);
            }
        }
        /// <summary>
        /// 根据游戏gamekey创建Tabs
        /// </summary>
        private void CreateTabels()
        {
            var dict = GameListModel.Instance.GameUnitModels;
            var gamelist = new List<GameUnitModel>();
            foreach (var keyValue in dict)
            {
                var gmodel = keyValue.Value;
                var rk = gmodel.RoomKind;
                if (rk < 1) continue;
                gamelist.Add(gmodel);
            }
            var count = gamelist.Count;
            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var gd = gamelist[i];
                var gk = gd.GameKey;
                var tdata = new TabData
                {
                    Name = gd.GameName,
                    UpStateName = string.Format("{0}{1}", PrefixUpStateName, gk),
                    DownStateName = string.Format("{0}{1}", PrefixDownStateName, gk),
                    Data = gk
                };
                TabDatas[i] = tdata;
            }
            if (TabDatas.Length < 1) return;
            var td = TabDatas[0];
            td.StarttingState = true;
            UpdateView(-1);
        }
        /// <summary>
        /// 根据游戏类型请求排行榜
        /// </summary>
        /// <param name="tableView"></param>
        public override void OnTableClick(YxTabItem tableView)
        {
            YxDebug.LogError("OnTableClick");
            if (!tableView.GetToggle().value) return; 
            var tdata = tableView.GetData<TabData>();
            if (tdata == null) return;
            var parm = new Dictionary<string, object>();
            parm[_keyGamekey] = tdata.Data.ToString();
            Facade.Instance<TwManager>().SendAction(TabActionName, parm, OnLoadData,true,null, RequestWithHasWait); 
        }
        /// <summary>
        /// 预制体挂载对应排行类型请求排行榜数据
        /// </summary>
        /// <param name="toggle"></param>
        public void OnTableClickWithType(UIToggle toggle)
        { 
            if (TabsView != null) TabsView.localScale = new Vector3(0, 1, 1);
            if (!toggle.value) return;
            if (toggle.name.Equals(RankTypes)){return;}
            RankTypes = toggle.name;
            RequestWithParm(OnLoadData);
        }
        /// <summary>
        /// 生成式（后台配置）排行请求
        /// </summary>
        /// <param name="tableView"></param>
        public void OnTabClickWithData(YxTabItem tableView)
        {
            if (!tableView.GetToggle().value) { return;}
            if (tableView.name.Equals(_curTabData.Index.ToString())) { return; }
            var data = tableView.GetData<TabData>().Data;
            _curTabData = data as RankTabData;
            if (_curTabData != null) { RankTypes = _curTabData.Key;}
            base.OnTableClick(tableView);
            RequestWithParm(UpdateView);
        }
        /// <summary>
        /// 发送对应类型请求
        /// </summary>
        public void RequestWithParm(TwCallBack callback)
        {
            var parm = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(RankTypes))
            {
                parm[_keyType] = RankTypes;
                if (RequestWithTeaInfo)
                {
                    parm[YxTools.KeyTeaId] = TeaUtil.CurTeaId;
                }
            }
            Facade.Instance<TwManager>().SendAction(TabActionName, parm, callback);
        }

        public void ReSendRequestWithType()
        {
            RequestWithParm(OnLoadData);
        }

        /// <summary>
        /// 加载排行数据
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoadData(object obj)
        {
            if (ScrollView != null) ScrollView.ResetPosition();
            DealData(obj);
            DealContent();
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null) { return;}
            OnLoadData(Data);
            if (!_firstRequest) { return;}
            ShowView();
            _firstRequest = false;
        }

        protected virtual void DealData(object data)
        {
            _data = new RankData(data);
            if (_curTabData == null)
            {
                if (_data.TabDatas!=null&& _data.TabDatas.Count>0)
                {
                    RankTypes = _data.SelectTab;
                    RankTabData selectTab = _data.TabDatas.Find(item => item.Key.Equals(RankTypes));
                    if (selectTab!=null)
                    {
                        _curTabData = selectTab;
                    }
                    else
                    {
                        YxDebug.LogError("没有选中的页签");
                    }
                }
            }
        }
        /// <summary>
        /// 处理显示 
        /// </summary>
        protected virtual void ShowView()
        {
            bool isSingle = _data.TabDatas.Count == 1&& ShowSingleLabel;
            DealSingle(isSingle);
            DealTabs(isSingle);
        }
        private void DealTabs(bool isSingle)
        {
            if (TabsArea)
            {
                TabsArea.SetActive(!isSingle);
            }
            if (isSingle)
            {
                return;
            }
            List<RankTabData> datas = _data.TabDatas;
            TabDatas=new TabData[datas.Count];
            int index = 0;
            foreach (var itemData in datas)
            {
                TabData data = new TabData()
                {
                    Name = itemData.ShowName,
                    StarttingState = itemData.Key.Equals(_data.SelectTab),
                    Data = itemData,
                    Index = index,
                    UpStateName = string.Format("{0}{1}", PrefixUpStateName, itemData.Key),
                    DownStateName = string.Format("{0}{1}", PrefixDownStateName, itemData.Key),
                };
                TabDatas[index++] = data;
            }
            TabSatate = -1;
            UpdateTabs(TabDatas);
        }
        /// <summary>
        /// 排行榜只有一种数据时，默认显示的Title处理，
        /// </summary>
        /// <param name="isSingle"></param>
        private void DealSingle(bool isSingle)
        {
            if (SingleLabel)
            {
                SingleLabel.gameObject.SetActive(isSingle);
                if (!isSingle)
                {
                    return;
                }
                SingleLabel.text = _curTabData.ShowName;
            } 
        }
        private void DealContent()
        {
            RankItemView.RankType = RankTypes;
            if (_curTabData != null)
            {
                RankItemView.ItemNotice = _curTabData.Notice;
            }
            RankItemView.TotalCount = _data.RankCount;
            if (ShowPanel)
            {
                SpringPanel.Begin(ShowPanel.gameObject, Vector3.zero, int.MaxValue);
            }
            YxWindowUtils.CreateMonoParent(GridPrefab, ref _itemGrid);
            foreach (var rankItemData in _data.RankDatas)
            {
                var item = YxWindowUtils.CreateItem(PrefabItem, _itemGrid.transform);
                item.UpdateView(rankItemData);
            }
            _itemGrid.repositionNow = true;
            if (SelfRankItem == null) return;
            SelfRankItem.UpdateView(_data.SelfRankDatas);
        }

    }
    /// <summary>
    ///排行榜数据
    /// </summary>
    public class RankData
    {
        /// <summary>
        /// Key选项卡
        /// </summary>
        private string _keyOptions = "options";

        /// <summary>
        /// Key默认选中
        /// </summary>
        private string _keyDefault = "default";

        /// <summary>
        /// Key数量
        /// </summary>
        private string _keyCount = "count";

        /// <summary>
        ///Key当前玩家
        /// </summary>
        private string _keySelf = "self";
        /// <summary>
        /// 排行榜数据（RankRequest）
        /// </summary>
        private string _keyRankData = "data";
        /// <summary>
        /// 旧版排行榜数据（TopRank）
        /// </summary>
        private string _keyTop = "top";
        /// <summary>
        /// 排行名次
        /// </summary>
        private string _keyRankNum = "rankNum";
        /// <summary>
        /// 排行切页数据
        /// </summary>
        List<RankTabData> _tabDatas = new List<RankTabData>();
        /// <summary>
        /// 排行数据
        /// </summary>
        private List<RankItemData> _rankDatas = new List<RankItemData>();
        /// <summary>
        /// 默认Tab
        /// </summary>
        private string _selectTab;
        /// <summary>
        /// 排行榜中入榜人数
        /// </summary>
        private int _pageCount;
        /// <summary>
        /// 当前玩家数据
        /// </summary>
        private RankItemData _selfRankItem;
        /// <summary>
        /// 页签数据
        /// </summary>
        public List<RankTabData> TabDatas
        {
            get { return _tabDatas; }
        }

        public List<RankItemData> RankDatas
        {
            get { return _rankDatas; }
        }
        public RankItemData SelfRankDatas
        {
            get { return _selfRankItem; }
        }

        public string SelectTab
        {
            get { return _selectTab; }
        }

        public int RankCount
        {
            get{return _pageCount;}
        }

        public RankData(object data)
        {
            Dictionary<string, object> param = (Dictionary<string, object>)data;
            if (param.ContainsKey(_keyOptions))
            {
                SetTabsData(param[_keyOptions]);
            }
            if (param.ContainsKey(_keyRankData))
            {
                SetItemsData(param[_keyRankData]);
            }
            if (param.ContainsKey(_keyTop))
            {
                SetItemsData(param[_keyTop]);
            }
            if (param.ContainsKey(_keyDefault))
            {
                _selectTab = param[_keyDefault].ToString();
            }
            if (param.ContainsKey(_keyCount))
            {
                _pageCount = int.Parse(param[_keyCount].ToString());
            }
            else
            {
                _pageCount = _rankDatas.Count;
            }
            if(param.ContainsKey(_keySelf))
            {
                SetSelfData(param[_keySelf]);
            }
        }

        /// <summary>
        /// 处理页签数据
        /// </summary>
        /// <param name="tabDatas"></param>
        private void SetTabsData(object tabDatas)
        {
            if (tabDatas == null)
            {
                return;
            }
            _tabDatas.Clear();
            Dictionary<string, object> param = (Dictionary<string, object>)tabDatas;
            int index = 0;
            foreach (KeyValuePair<string, object> keyPair in param)
            {
                RankTabData data = new RankTabData(keyPair.Key, keyPair.Value);
                data.Index = index++;
                _tabDatas.Add(data);
            }
        }
        private void SetItemsData(object rankData)
        {
            _rankDatas.Clear();
            List<object> datas = (List<object>)rankData;
            int index = 1;
            foreach (object itemData in datas)
            {
                RankItemData data = new RankItemData((Dictionary<string, object>)itemData, index++);
                _rankDatas.Add(data);
            }
        }
        private void SetSelfData(object selfRankData)
        {
            var currentRank = 0;
            if (selfRankData is Dictionary<string, object>)
            {
                var selfRank = selfRankData as Dictionary<string, object>;
                if (selfRank.ContainsKey(_keyRankNum))
                {
                    currentRank = int.Parse(selfRank[_keyRankNum].ToString());
                }
                else
                {
                    currentRank = 0;
                }           
            }
            _selfRankItem = new RankItemData((Dictionary<string, object>)selfRankData, currentRank);
        }
    }
    /// <summary>
    /// 排行榜Tab数据
    /// </summary>
    public class RankTabData:TabData
    {
        /// <summary>
        /// Key切页名称
        /// </summary>
        private string _keyName = "Name";

        /// <summary>
        /// Key提示
        /// </summary>
        private string _keyNotice = "Notice";

        /// <summary>
        /// 选项卡显示名称
        /// </summary>
        private string _showName;

        /// <summary>
        /// 选项卡
        /// </summary>
        private int _index;

        /// <summary>
        /// 提示文本
        /// </summary>
        private string _notice;
        /// <summary>
        /// 键值
        /// </summary>
        private string _key;

        public string ShowName
        {
            get { return _showName; }
        }

        public string Notice
        {
            get { return _notice; }
        }

        public string Key
        {
            get { return _key; }
        }

        public RankTabData(string key,object data)
        {
            Dictionary<string, object> param = (Dictionary<string, object>)data;
            if (param.ContainsKey(_keyName))
            {
                _showName = param[_keyName].ToString();
            }
            if (param.ContainsKey(_keyNotice))
            {
                _notice = param[_keyNotice].ToString();
            }
            _key = key;
        }
    }
}