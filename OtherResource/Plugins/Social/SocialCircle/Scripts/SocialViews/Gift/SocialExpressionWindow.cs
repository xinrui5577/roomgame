using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.ChatViews;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Gift
{
    public class SocialExpressionWindow : ExpressionWindow
    {
        private SocialMessageManager _manager;
        protected SocialMessageManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = Facade.Instance<SocialMessageManager>().InitManager();
                }
                return _manager;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            Manager.AddLocalEventListeners<Dictionary<string,object>>(SocialTools.KeyActionSendGift, OnSendGiftSuccess);
        }

        public override void OnDestroy()
        {
            Manager.RemoveLocalEventListener<Dictionary<string, object>>(SocialTools.KeyActionSendGift, OnSendGiftSuccess);
            base.OnDestroy();
        }

        private void OnSendGiftSuccess(Dictionary<string,object> successInfo)
        {
            var dic = successInfo.ParseDefKeyDic();
            string sendName;
            dic.TryGetValueWitheKey(out sendName, SocialTools.KeyName);
            OnSelectedItem(sendName);
            UserController.Instance.SendSimpleUserData(null,false);
        }

        protected override void UpdateItem(YxView viewItem, ChatTextureItemView.ChatSpriteInfo info)
        {
            if (viewItem)
            {
                var dic = new Dictionary<string, object>();
                dic[SocialTools.KeyName] = info.SpriteName;
                dic[SocialTools.KeySourceName] = info.Atlas;
                if (Manager.GiftDataDic.ContainsKey(info.SpriteName))
                {
                    var item = Manager.GiftDataDic[info.SpriteName];
                    if (item!=null)
                    {
                        string itemType;
                        item.TryGetValueWitheKey(out itemType,SocialTools.KeyType);
                        dic[SocialTools.KeyNum] =UserInfoModel.Instance.BackPack.GetItem(itemType);
                        dic[SocialTools.KeyType] = itemType;
                    }
                }
                viewItem.UpdateView(dic);
            }
        }
    }
}
