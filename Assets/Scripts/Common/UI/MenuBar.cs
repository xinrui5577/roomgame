using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 菜单栏
    /// </summary>
    public class MenuBar : YxView
    {
        public bool HasMoreBtn;
        public YxBaseGridAdapter Grid;
        /// <summary>
        /// 按钮位置
        /// </summary> 
        public GameObject[] MenuBtns;
        /// <summary>
        /// 按钮原来的位置
        /// </summary>
        private Vector3[] _btnPostions;
         
        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            var btnCount = MenuBtns.Length;
            _btnPostions = new Vector3[btnCount];
            for (var i = 0; i < btnCount; i++)
            {
                var btn = MenuBtns[i];
                if(btn == null) { continue;}
                _btnPostions[i] = btn.transform.localPosition;
            }
        }


        [ContextMenu("Execute")]
        protected override void OnFreshView()
        {
            if (Grid != null)
            {
                Grid.Reposition();
            }
        }

        /// <summary>
        /// 获取空位置
        /// </summary>
        public bool AddBtn(GameObject newBtn,int index)
        {
            var len = MenuBtns.Length;
            if (index >= len) { return false; }
            var btn = MenuBtns[index];
            if (btn.activeSelf) { return false; }
            var nts = newBtn.transform;
            var ots = btn.transform;
            nts.parent = ots.parent;
            if (index >= _btnPostions.Length) { return false; }
            nts.localPosition = _btnPostions[index];
            return true;
        }

        public List<int> GetButtonIndexs(bool isActive,bool needMoreBtn)
        {
            var len = MenuBtns.Length;
            len = len > 0 && needMoreBtn && HasMoreBtn ? len - 1 : len;
            var list = new List<int>();
            for (var i = 0; i < len; i++)
            {
                var btn = MenuBtns[i];
                if (btn == null || btn.activeSelf != isActive) { continue; }
                list.Add(i);
            }
            return list;
        }

        public GameObject GetButton(int index)
        {
            var btn = MenuBtns[index];
            return btn == null || !btn.activeSelf ? null : btn;
        }

        public GameObject GetLastButton()
        {
            var len = MenuBtns.Length;
            return len < 1 ? null : GetButton(len - 1);
        }
    }
}
