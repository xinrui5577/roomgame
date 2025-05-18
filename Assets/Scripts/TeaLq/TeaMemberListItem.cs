using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.TeaLq
{
    public class TeaMemberListItem : YxView
    {
        public NguiTextureAdapter UserHead;
        public NguiLabelAdapter UserName;
        public NguiLabelAdapter UserId;
        public NguiLabelAdapter GameRound;
        public NguiLabelAdapter LastLoginDt;
        public NguiLabelAdapter Desc;
        public UIButton ForbidGameBtn;
        public UIButton UnForbidGameBtn;
        public UIButton TakeOutBtn;
        public string DescInfo;
        public string WindowName;

        private int _userId;
        private List<string> _descList=new List<string>(); 

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as TeaMemberListData;
            if (data == null) return;
            _userId = data.UserId;
            PortraitDb.SetPortrait(data.Avatar, UserHead, data.Sex);
            name = data.UserId.ToString();
            UserId.TrySetComponentValue(data.UserId);
            if (UserName)
            {
                UserName.TrySetComponentValue(data.NickName);
            }
            if (GameRound)
            {
                GameRound.TrySetComponentValue(data.GameRound);
            }

            if (LastLoginDt)
            {
                LastLoginDt.TrySetComponentValue(data.LastLoginDt);
            }
            if (ForbidGameBtn && UnForbidGameBtn)
            {
                if (data.Isforbid)
                {
                    ForbidGameBtn.gameObject.SetActive(false);
                    UnForbidGameBtn.gameObject.SetActive(true);
                }
                else
                {
                    ForbidGameBtn.gameObject.SetActive(true);
                    UnForbidGameBtn.gameObject.SetActive(false);
                }
            }
            if (TakeOutBtn)
            {
                TakeOutBtn.gameObject.SetActive(data.IsShowBtn);
            }
            if (Desc)
            {
                var str = string.Format(DescInfo, data.Detail.Descs.Count);  
                Desc.TrySetComponentValue(str);
                _descList.AddRange(data.Detail.Descs);
            }
        }

        public void OnBtnChange()
        {
            if (ForbidGameBtn.gameObject.activeSelf)
            {
                ForbidGameBtn.gameObject.SetActive(false);
                UnForbidGameBtn.gameObject.SetActive(true);
            }
            else
            {
                ForbidGameBtn.gameObject.SetActive(true);
                UnForbidGameBtn.gameObject.SetActive(false);
            }
        }

        public void OpenChildWinow()
        {
            if (MainYxView)
            {
                var mainWindow=MainYxView as YxWindow;
                if(mainWindow == null)return;
                var win= mainWindow.CreateChildWindow(WindowName);
                win.UpdateView(_descList);
            }
        }

        public void OnTakeOut()
        {
            var dic=new Dictionary<string, object>();
            dic["id"]= TeaMainPanel.CurTeaId;
            dic["forbid_userId"] = _userId;
            Facade.Instance<TwManager>().SendAction("group.takeOutForbid", dic, UpdateView);
        }
    }
}
