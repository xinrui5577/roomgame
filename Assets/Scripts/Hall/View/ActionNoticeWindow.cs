using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 活动公告窗口
    /// </summary>
    public class ActionNoticeWindow : YxNguiWindow
    {
        /// <summary>
        /// 背景
        /// </summary>
        public UITexture BackGround;
        /// <summary>
        /// 是否显示原始大小
        /// </summary>
        public bool NeedSnap = false;
        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            InitStateTotal = 1;
        }

        protected override void OnFreshView()
        {
            if (BackGround.mainTexture == null)
            {
                BackGround.gameObject.SetActive(false);
                Hide();
            }
            base.OnFreshView();
            if (Data == null)
            {
                Close();
                return;
            }
            var data = (KeyValuePair<string, object>)Data;
            Facade.Instance<AsyncImage>().GetAsyncImage(data.Value.ToString(), (texture, code) =>
            { 
                BackGround.mainTexture = texture;
                if(NeedSnap) { BackGround.MakePixelPerfect();}
                BackGround.gameObject.SetActive(true);
                Show();
            });
        }

        public override void Close()
        {
            Destroy(gameObject);
            base.Close();
        }
    }
}
