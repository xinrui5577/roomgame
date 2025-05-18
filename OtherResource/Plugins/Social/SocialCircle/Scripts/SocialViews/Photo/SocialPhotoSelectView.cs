using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using BestHTTP.JSON;
using UnityEngine;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Photo
{
    /// <summary>
    ///亲友圈相册选择面板（选择对应玩家的相册）
    /// </summary>
    public class SocialPhotoSelectView : BaseSocialWrapListView
    {
        protected override void OnFreshView()
        {
            var dic = Data as Dictionary<string, object>;
            if(dic!=null)
            {
                List<string> userIdList;
                dic.TryGetValueWitheKey(out userIdList, SocialTools.KeyData);
                var dataCount = userIdList.Count;
                if (dataCount > 0)
                {
                    var infos=TalkCenter.GetUserInfoByUserIdList(userIdList);
                    var infoCount = infos.Count;
                    if (infoCount>0)
                    {
                        PageIds.Clear();
                        var newDataDic=new Dictionary<string,Dictionary<string,object>>();
                        for (int i = 0; i < infoCount; i++)
                        {
                            var itemData = infos[i];
                            if (itemData!=null)
                            {
                                string itemId;
                                itemData.TryGetValueWitheKey(out itemId, SocialTools.KeyId);
                                if (!string.IsNullOrEmpty(itemId))
                                {
                                    PageIds.Add(itemId);
                                    newDataDic.Add(itemId, itemData);
                                }
                            }
                        }
                        FreshWrapList(newDataDic);
                    }

                }
            }
            else
            {
                Debug.LogError("数据不是字典格式");
            }
        }
    }
}
