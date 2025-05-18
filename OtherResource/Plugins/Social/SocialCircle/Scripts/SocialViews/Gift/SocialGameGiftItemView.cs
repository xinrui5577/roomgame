using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Gift
{
    public class SocialGameGiftItemView : ChatTextureItemView
    {
        [Tooltip("礼物数量")]
        public YxBaseLabelAdapter GiftNumAdapter;
        /// <summary>
        /// 名称
        /// </summary>
        private string _spriteName;
        /// <summary>
        /// type
        /// </summary>
        private string _spriteType;

        public string SpriteName
        {
            get { return _spriteName; }
        }

        public string SpriteType
        {
            get { return _spriteType; }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.AddEventListeners<YxESysEventType, Dictionary<string, object>>(
                YxESysEventType.SysSimpleFresh, OnDataValueChange);
        }
        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<YxESysEventType, Dictionary<string, object>>(
                YxESysEventType.SysSimpleFresh, OnDataValueChange);
            base.OnDestroy();
        }


        private void OnDataValueChange(Dictionary<string,object> data)
        {
            var getData = Data as Dictionary<string, object>;
            if (getData != null)
            {
                getData.TryGetValueWitheKey(out _spriteType, SocialTools.KeyType);
                if (!string.IsNullOrEmpty(_spriteType))
                {
                    GiftNumAdapter.TrySetComponentValue(UserInfoModel.Instance.BackPack.GetItem(_spriteType));
                }
            }
        }

        protected override void OnFreshView()
        {
            var getData = Data as Dictionary<string, object>;
            if (getData!=null)
            {
                int giftNum;
                getData.TryGetValueWitheKey(out giftNum,SocialTools.KeyNum);
                getData.TryGetValueWitheKey(out _spriteName, SocialTools.KeyName);
                getData.TryGetValueWitheKey(out _spriteType, SocialTools.KeyType);
                GiftNumAdapter.TrySetComponentValue(giftNum);
                YxBaseSpriteAdapter content = Content as YxBaseSpriteAdapter;
                if (content !=null)
                {
                    content.SetAtlas(getData[SocialTools.KeySourceName] as YxBaseAtlasAdapter);
                    content.TrySetComponentValue(_spriteName);
                }
            }
        }
    }
}
