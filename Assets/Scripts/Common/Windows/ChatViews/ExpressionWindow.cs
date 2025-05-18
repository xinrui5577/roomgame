using System;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.Windows.ChatViews
{
    /// <summary>
    /// 表情界面
    /// </summary>
    public class ExpressionWindow : YxWindow
    {
        /// <summary>
        /// item预制体
        /// </summary>
        public ChatTextureItemView PrefabItemView;
        [Tooltip("包含表情目录的图集，如果null，将从公共资源中获取")]
        public YxBaseAtlasAdapter SpriteAtlas;
        /// <summary>
        /// 表格
        /// </summary>
        public YxBaseGridAdapter Grid;
        public int TalkCycle = 1500;
        [SerializeField]
        protected ExpType Type;
        [HideInInspector]
        public int AttackIndex = -1;
        protected override void OnFreshView()
        {
            var chatMgr = Facade.GetInterimManager<YxChatManager>();
            if (chatMgr == null)
            {
                Close();
                YxDebug.LogError("YxChatManager不存在！", "ChatWindow");
                return;
            }
            var db = chatMgr.LoadExpDb();
            if (db == null) { return; }//没有预制体
            if (SpriteAtlas == null)
            {
                var expInfo = Type == ExpType.Normal ? db.ChatExp : db.HeadExp;
                SpriteAtlas = expInfo.ExpressionAtlas;
            }
            if (SpriteAtlas == null) { return;}
            var textureNames = SpriteAtlas.TextureNames;
            var typeCount = textureNames.Length;
            var parentTs = Grid.transform;
            for (var i = 0; i < typeCount; i++)
            {
                var spriteName = textureNames[i];
                var itemView = Instantiate(PrefabItemView);
                itemView.name = spriteName;
                itemView.gameObject.SetActive(true);
                var ts = itemView.transform;
                ts.parent = parentTs;
                UpdateItem(itemView, new ChatTextureItemView.ChatSpriteInfo
                {
                    Atlas = SpriteAtlas,
                    SpriteName = spriteName
                });
                ts.localScale = Vector3.one;
            }
            Grid.Reposition();
        }

        protected virtual void UpdateItem(YxView viewItem,ChatTextureItemView.ChatSpriteInfo info)
        {
            if (viewItem)
            {
                viewItem.UpdateView(info);
            }
        }


        public void OnSelectedItem(string content)
        {
            SendContent(content);
            Close();
        }

        private DateTime _lastTalkTime;

        private void SendContent(string content)
        {
            var cur = DateTime.Now;
            var c = cur - _lastTalkTime;
            if (c.TotalMilliseconds < TalkCycle)
            {
                YxMessageTip.Show("慢点，频率太快了！！！");
                return;
            }
            _lastTalkTime = cur;
            if (Type == ExpType.Normal)
            {
                YxChatManager.SendChatContent(string.Format("/{0} {1}", YxChatManager.ChatCmd.Exp, content));
                return;
            }
            if (AttackIndex == -1) { return; }
            YxChatManager.SendChatContent(string.Format("/{0} {1} {2}", YxChatManager.ChatCmd.HeadExp, content, AttackIndex));
        }

        protected enum ExpType
        {
            Normal,
            Head
        }

        public override YxEUIType UIType
        {
            get { return Grid==null?YxEUIType.Default:Grid.UIType; }
        }
    }
}
