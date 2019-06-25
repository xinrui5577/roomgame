using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.SysNoticeWindows
{
    public class ActionItemView : YxView
    { 
        public UITexture ActionTexture;
         
        protected override void OnFreshView()
        {
            var adata = GetData<ActionData>();
            if (adata == null) return;
            AsyncImage.Instance.GetAsyncImage(adata.IconUrl, SetIcon);
        }

        private void SetIcon(Texture2D texture)
        {
            ActionTexture.mainTexture = texture;
        }

        public override void SetOrder(int order)
        {
        }

        public void OnClickItem()
        {
            var adata = GetData<ActionData>();
            if (adata == null) return;
            Application.OpenURL(adata.ClickUrl);
        }
    }

    public class ActionData
    {
        public string IconUrl;
        public string ClickUrl;

        public ActionData(IDictionary<string, object> itemDict)
        {
            if (itemDict.ContainsKey("pic_url_x"))
            {
                var temp = itemDict["pic_url_x"];
                if (temp != null)
                {
                    IconUrl = temp.ToString();
                }
            }
            if (itemDict.ContainsKey("detail_url_x"))
            {
                var temp = itemDict["detail_url_x"];
                if (temp != null)
                {
                    ClickUrl = temp.ToString();
                }
            }
        }
    }
}
