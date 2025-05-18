using System.Collections.Generic;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.AddFriend
{
    /// <summary>
    /// 根据ID 添加好友
    /// </summary>
    public class SocialAddFriendByIdView : BaseSocialView
    {
        /// <summary>
        /// /点击确定事件
        /// </summary>
        /// <param name="inputContent"></param>
        public void OnClickSureBtn(string inputContent)
        {
            if (string.IsNullOrEmpty(inputContent))
            {
                YxMessageBox.Show(SocialTools.KeyNoticeInputIdEmpty);
                return;
            }
           
            var dic = new Dictionary<string, object>();
            dic[SocialTools.KeyOtherIds] = new List<string>()
            {
                inputContent
            };
            Manager.SendRequest(SocialTools.KeyActionAddFriend, dic);
        }
    }
}
