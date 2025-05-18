using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     RecycleWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-30
 *描述:        	
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class RecycleWindow : BaseMono 
    {
        #region UI Param
        [Tooltip("池子")]
        public PoolManager Pool;
        [Tooltip("显示父级")]
        public GameObject ShowParent;

        #endregion

        #region Data Param
        /// <summary>
        /// 是否存在主体数据
        /// </summary>
        public bool DatasExist { get; protected set; }
        [Tooltip("数据监听类型")]
        public LocalRequest DataRequest;
        [Tooltip("子对象添加完毕")]
        public List<EventDelegate> OnChildAdded;
        [Tooltip("界面打开状态变化")]
        public List<EventDelegate> OnShowStateChange;
        [Tooltip("界面打开回调")]
        public List<EventDelegate> OnViewShow;
        /// <summary>
        /// 界面打开状态
        /// </summary>
        public bool OpenState { get; private set; }
        #endregion

        #region Local Data
        /// <summary>
        /// 缓存数据
        /// </summary>
        protected List<IRecycleData> CacheData=new List<IRecycleData>(); 
        /// <summary>
        /// 缓存Views
        /// </summary>
        protected List<YxView> CacheViews=new List<YxView>(); 
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<LocalRequest, List<IRecycleData>>(DataRequest, OnGetData);
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void OnGetData(List<IRecycleData> datas)
        {
            if (CacheData.Equals(datas))
            {
                YxDebug.LogError("数据未变化，不刷新");
                return;
            }
            var list = new List<IRecycleData>();
            foreach (var item in datas)
            {
                if (item == null)
                {
                    YxDebug.LogError("空的，删除掉"+DataRequest);
                }
                else
                {
                    list.Add(item);
                }
            }
            CacheData = list;
            CacheViews.Clear();
            AddChildrenToShow();
        }

        /// <summary>
        /// 添加批量子对象
        /// </summary>
        protected virtual void AddChildrenToShow()
        {
            Pool.StoreAll();
            var count = CacheData.Count;
            DatasExist = count != 0;
            for (int i = 0; i < count; i++)
            {
                var data = CacheData[i];
                CacheViews.Add(AddView(data, i.ToString()));
            }
            StartCoroutine(OnChildAdded.WaitExcuteCalls());
        }

        /// <summary>
        /// 添加单独子对象
        /// </summary>
        protected virtual YxView AddChildToShow(IRecycleData singleData)
        {
            if (CacheViews==null)
            {
                CacheViews=new List<YxView>();
            }
            if (CacheData==null)
            {
                CacheData=new List<IRecycleData>();
            }
            var view = AddView(singleData, CacheData.Count.ToString());
            var count = CacheData.Count;
            DatasExist = count != 0;
            CacheViews.Add(view);
            StartCoroutine(OnChildAdded.WaitExcuteCalls());
            return view;
        }

        /// <summary>
        /// 添加View
        /// </summary>
        /// <param name="singleData"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private YxView AddView(IRecycleData singleData,string index)
        {
            CacheData.Add(singleData);
            var item = Pool.New();
            ShowParent.AddChildToParent(item.gameObject);
            item.Id =index;
            item.UpdateView(singleData);
            return item;
        }

        #endregion

        #region Function

        /// <summary>
        /// 点击打开按钮
        /// </summary>
        public void OnClickOpenBtn()
        {
            OpenState = !OpenState;
            FreshShowState();
        }

        /// <summary>
        /// 刷新显示状态
        /// </summary>
        protected void FreshShowState()
        {
            StartCoroutine(OnShowStateChange.WaitExcuteCalls());
            if (OpenState)
            {
                StartCoroutine(OnViewShow.WaitExcuteCalls());
            }
        }

        #endregion
    }

    /// <summary>
    /// 回收数据
    /// </summary>
    public interface IRecycleData
    {

    }
}
