using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     RuleInfoView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-16
 *描述:        	规则信息
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class RuleInfoView : PludoFreshView 
    {
        #region UI Param
        [Tooltip("Item父级")]
        public Transform ItemParent;
        [Tooltip("item")]
        public RuleInfoItemView PrefabView;
        [Tooltip("背景")]
        public UISprite Bg;
        #endregion

        #region Data Param
        [Tooltip("分隔符")]
        public char SpliteFlag = ';';
        [Tooltip("背景默认Y轴偏移")]
        public float BgOffsetY = 30;
        [Tooltip("确定背景高度延迟时间")]
        public float SureBgHeightTime = 0.5f;
        #endregion

        #region Local Data
        private List<RuleInfoItemView> _views=new List<RuleInfoItemView>();
        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<LoaclRequest, string>(LoaclRequest.RuleInfoInit, OnSetRuleInfo);
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, string>(LoaclRequest.RuleInfoInit, OnSetRuleInfo);
            base.OnDestroy();
        }

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            if (Data is string)
            {
                _views.Clear();
                string[] items = Data.ToString().Split(SpliteFlag);
                var count = items.Length;
                int itemIndex = 0;
                for (int i = 0; i < count; i++)
                {
                    var info = items[i];
                    if(string.IsNullOrEmpty(info))
                    {
                        continue;
                    }
                    var view=ItemParent.GetChildView(itemIndex, PrefabView);
                    if (view)
                    {
                        view.UpdateView(info);
                        _views.Add(view.GetComponent<RuleInfoItemView>());
                        itemIndex++;
                    }
                }
            }
        }

        #endregion

        #region Life Cycle
        #endregion

        #region Function
        /// <summary>
        /// 设置房间规则
        /// </summary>
        private void OnSetRuleInfo(string rule)
        {
            UpdateView(rule);
        }

        private Coroutine _waitSure;
        public void GetTransBounds()
        {
            if (_waitSure!=null)
            {
                StopCoroutine(_waitSure);
            }
            _waitSure=StartCoroutine(SureBgWidth(SureBgHeightTime));
        }

        IEnumerator SureBgWidth(float time)
        {
            yield return new WaitForSeconds(time);
            if (_views.Count > ConstantData.IntValue)
            {
                var lastView = _views.Last();
                if (lastView)
                {
                    var height = Mathf.Abs(lastView.GetLineVector3().y);
                    Bg.height = (int)(height + BgOffsetY);
                }
            }
        }
        #endregion
    }
}
