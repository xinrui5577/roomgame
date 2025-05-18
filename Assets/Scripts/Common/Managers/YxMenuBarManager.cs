using Assets.Scripts.Common.UI;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Common.Managers
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    public class YxMenuBarManager : InterimMono
    {
        /// <summary>
        /// 更多按钮
        /// </summary>
        public Transform MoreBtn;
        /// <summary>
        /// 更多视图
        /// </summary>
        public MenuMoreView MoreView;
        /// <summary>
        /// 菜单按钮（受开关管理）
        /// </summary>
        public GameObject[] MenuBtns;

        public MenuBar[] MenuBars;
        [Tooltip("启动时是否要刷新")]
        public bool NeedFresh;

        private MenuBar _hasMoreBtnBar;

        protected override void OnAwake()
        {
            base.OnAwake();
            SetBtnsActive(1);
            if (MoreView != null)
            {
                MoreView.Hide();
            }
            if (MoreBtn != null)
            {
                ShowMoreBtn(false);
            } 
            var len = MenuBars.Length;
            for (var i = 0; i < len; i++)
            {
                var bar = MenuBars[i];
                if (!bar.HasMoreBtn) { continue; }
                _hasMoreBtnBar = bar;
                break;
            }
            AddListeners("HallWindow_hallMenuChange", OnFreshMenu);
            if (NeedFresh)
            {
                OnFreshMenu(null);
            }
        }
        

        public void ShowMoreBtn(bool show)
        {
            if (MoreBtn == null) { return; }
            if (show)
            {
                MoreBtn.transform.localScale = Vector3.one;
                if (_hasMoreBtnBar != null)
                {
                    var btn = _hasMoreBtnBar.GetLastButton();
                    if (btn != null)
                    {
                        MoreView.AddMenu(btn.transform);
                    }
                }
                return;
            }
            MoreBtn.transform.localScale = Vector3.zero;
            MoreView.Hide();
        }

        private void OnFreshMenu(object msg)
        {
            var menustate = App.AppStyle == YxEAppStyle.Concise ? 1 : HallModel.Instance.OptionSwitch.HallMenue;
            
            //隐藏按钮
            SetBtnsActive(menustate);
            if (MoreBtn == null)
            {
                FreshBars();
            }
            else
            {
                FreshAll();
            } 
        }

        private void FreshAll()
        {
            //是否隐藏显示按钮
            var hasMoreBtn = GetAllBtns() > GetAllBtnSeatCount();
            ShowMoreBtn(hasMoreBtn);
            //分配more
            var moreMenus = MoreView.GetMenus();//更多按钮
            var moreMenuCount = moreMenus.Count;
            var moreIndex = 0;
            var barCount = MenuBars.Length;
            for (var i = 0; i < barCount; i++)
            {
                var bar = MenuBars[i];
                if (moreIndex < moreMenuCount)
                {
                    var list = bar.GetButtonIndexs(false, hasMoreBtn);//隐藏的索引 
                    var listCount = list.Count;
                    var residue = moreMenuCount - moreIndex;//剩余的
                    var addCount = Mathf.Min(listCount, residue);
                    for (var j = 0; j < addCount; j++)
                    {
                        var index = list[j];
                        var btn = moreMenus[moreIndex++].gameObject;
                        bar.AddBtn(btn, index);
                    }
                }
                bar.UpdateView();
            }
        }

        private void FreshBars()
        {
            var barCount = MenuBars.Length;
            for (var i = 0; i < barCount; i++)
            {
                var bar = MenuBars[i];
                bar.UpdateView();
            }
        }

        protected int SetBtnsActive(int menustate)
        {
            var count = MenuBtns.Length;
            var btnsCount = 0;
            for (var i = 0; i < count; i++)
            {
                var btn = MenuBtns[i];
                if (btn == null) continue;
                var show = 1 << i;
                var isShow = (menustate & show) == show;
                btn.SetActive(isShow);
                if (isShow)
                {
                    btnsCount++;
                }
            }
            return btnsCount;
        }
        public void OnMoreClick()
        {
            if (MoreView == null) return;
            if (MoreView.IsShow())
            {
                MoreView.Hide();
            }
            else
            {
                MoreView.Show();
            }
        }

        public void OnHideMoreClick()
        {
            if (MoreView == null) return;
            MoreView.Hide();
        }


        /// <summary>
        /// 获取所有按钮位置
        /// </summary>
        /// <returns></returns>
        public int GetAllBtnSeatCount()
        {
            var count = 0;
            var barCount = MenuBars.Length;
            for (var i = 0; i < barCount; i++)
            {
                var bar = MenuBars[i];
                count += bar.MenuBtns.Length;
            }
            return count;
        }

        public int GetAllBtns()
        {
            var len = MenuBtns.Length;
            var count = 0;
            for (var i = 0; i < len; i++)
            {
                var btn = MenuBtns[i];
                if(btn == null || !btn.activeSelf)continue;
                count++;
            }
            return count;
        }
    }
}
