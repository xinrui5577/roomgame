using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

/*===================================================
 *文件名称:     PoolManager.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-28
 *描述:        	池子，游啊游啊游。以GameObject作为基本子对象，用来进行UI 物体回收处理
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class PoolManager : MonoBehaviour 
    {
        #region UI Param
        [Tooltip("池对象预设")]
        public YxView ObjectPrefab;

        [Tooltip("池容器，用来放置闲置对象")]
        public GameObject PoolContainer;

        #endregion

        #region Data Param
        [Tooltip("启动时初始化")]
        public bool InitOnAwake = true;
        [Tooltip("初始化数量")]
        public int InitCount = 5;
        [Tooltip("初始化操作")]
        public Action<YxView> OnInitAction;
        [Tooltip("重置操作")]
        public Action<YxView> OnResetAction;

        #endregion

        #region Local Data
        /// <summary>
        /// 对象集合
        /// </summary>
        private readonly List<YxView> _objects=new List<YxView>();
        /// <summary>
        /// 下个有效索引
        /// </summary>
        private int _nextValidIndex;

        public int NextValidIndex {
            get
            {
                return _nextValidIndex;
            }
        }

        #endregion

        #region Life Cycle

        void Awake()
        {
            if (InitOnAwake)
            {
                Init();
            }
        }

        public void Init()
        {
            while (_objects.Count < InitCount)
            {
                New();
            }
        }

        public YxView New()
        {
            YxView newObj;
            if (_objects.Count>_nextValidIndex)
            {
                newObj = _objects[_nextValidIndex];
                if (OnResetAction!=null)
                {
                    OnResetAction(newObj);
                }
                _nextValidIndex++;
            }
            else
            {
                newObj = PoolContainer.AddChild(ObjectPrefab.gameObject).GetComponent<YxView>();
                if (newObj!=null)
                {
                    _objects.Add(newObj);
                    if (OnInitAction != null)
                    {
                        OnInitAction(newObj);
                    }
                    _nextValidIndex++;
                }
            }
            return newObj;
        }

        public void StoreAll()
        {
            for (int i = 0; i < _nextValidIndex; i++)
            {
                PoolContainer.AddChildToParent(_objects[i].gameObject);
            }
            _nextValidIndex = 0;
        }



        #endregion

        #region Function

        #endregion
    }
}
