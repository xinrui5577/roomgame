using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.UI;

/*===================================================
 *文件名称:     GameNoticeWindow.cs
 *作者:         Andelee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018年10月29日 19:53:48
 *描述:        	
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.jpmj.GameNotice
{
    public class GameNoticeWindow : YxRequestView
    {
        #region UI Param
        [Tooltip("显示父级")]
        public GameObject ShowParent;
        [Tooltip("页签预制体")]
        public GameNoticeToggleView PerfabTableItem;
        [Tooltip("Grid")]
        public GridLayoutGroup TabParent;
        [Tooltip("显示图片")]
        public RawImage ShowImage;
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        /// <summary>
        /// Key 数据主体
        /// </summary>
        private const string KeyData = "data";
        /// <summary>
        ///  Key 显示
        /// </summary>
        private const string KeyVisible = "Visible";
        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            if (dic != null)
            {
                var visible = true;
                if(dic.ContainsKey(KeyVisible))
                {
                    visible = bool.Parse(dic[KeyVisible].ToString());
                }
                if (visible)
                {
                    var list = dic[KeyData] as List<object>;
                    if (list != null)
                    {
                        ShowParent.SetActive(true);
                        var count = list.Count;
                        List<GameNoticeItemData> tabDatas = new List<GameNoticeItemData>();
                        for (int i = 0; i < count; i++)
                        {
                            var item = new GameNoticeItemData(list[i]);
                            tabDatas.Add(item);
                        }
                        UpdateTabs(tabDatas);
                    }
                }
            }
        }

        private void UpdateTabs(IList<GameNoticeItemData> tabDatas)
        {
            if (TabParent == null)
            {
                return;
            }
            var count = tabDatas.Count;
            var tabItemList=new List<GameNoticeToggleView>();
            for (int i = 0; i < count; i++)
            {
                var data = tabDatas[i];
                var item = YxWindowUtils.CreateItem(PerfabTableItem, TabParent.transform);
                item.UpdateView(data);
                tabItemList.Add(item);
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(SelectFirst(tabItemList));
            }
        }

        #endregion

        #region Function

        private IEnumerator SelectFirst(List<GameNoticeToggleView> tabItems)
        {
            yield return new WaitForEndOfFrame();
            if (tabItems.Count>0)
            {
                var first = tabItems[0];
                if (first)
                {
                    first.GetToggle().isOn = true;
                }
            }
        }

        private GameNoticeToggleView _cacheToggle;
        public void Select(GameNoticeToggleView toggleView)
        {
            if (toggleView.Equals(_cacheToggle))
            {
                return;
            }
            _cacheToggle = toggleView;
            if (toggleView.GetToggle().isOn)
            {
                var itemData=toggleView.GetData<GameNoticeItemData>();
                FreshShowView(itemData.ImageUrl);
            }
        }
        
        /// <summary>
        /// 刷新主View
        /// </summary>
        /// <param name="imageUrl"></param>
        public void FreshShowView(string imageUrl)
        {
            if (ShowImage)
            {
                AsyncImage.GetInstance().SetTextureWithAsyncImage(imageUrl, ShowImage, ShowImage.mainTexture);
            }
        }
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public void HideView()
        {
            ShowParent.SetActive(false);
        }

        #endregion
    }

    public class GameNoticeItemData
    {
        /// <summary>
        /// Key Tab名称
        /// </summary>
        private const string KeyName = "name";
        /// <summary>
        /// Key 图片地址
        /// </summary>
        private const string KeyImage = "image";
        /// <summary>
        /// Key 标题
        /// </summary>
        private const string KeyTitle = "title";
        /// <summary>
        /// Key 内容
        /// </summary>
        private const string KeyContent = "content";

        public string TabName { get; private set; }
        public string ImageUrl { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }

        public GameNoticeItemData(object data)
        {
            var dic = data as Dictionary<string, object>;
            if (dic!=null)
            {
                if (dic.ContainsKey(KeyName))
                {
                    TabName = dic[KeyName].ToString();
                }
                if (dic.ContainsKey(KeyImage))
                {
                    ImageUrl = dic[KeyImage].ToString();
                }
                if (dic.ContainsKey(KeyTitle))
                {
                    Title = dic[KeyTitle].ToString();
                }
                if (dic.ContainsKey(KeyContent))
                {
                    Content = dic[KeyContent].ToString();
                }

            }
        }
    }
}
