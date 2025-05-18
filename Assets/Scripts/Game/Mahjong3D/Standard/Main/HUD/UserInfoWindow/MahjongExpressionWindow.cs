using UnityEngine;
using Assets.Scripts.Common.Windows.ChatViews;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;
using YxFramwork.View;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongExpressionWindow : ExpressionWindow
    {
        public bool IsSelf { get; set; }

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
            if (SpriteAtlas == null) { return; }
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
                itemView.UpdateView(new ChatTextureItemView.ChatSpriteInfo
                {
                    Atlas = SpriteAtlas,
                    SpriteName = spriteName
                });
                ts.localScale = Vector3.one;

                //设置item
                SetItem(itemView, IsSelf);
            }
            Grid.Reposition();
        }

        private void SetItem(ChatTextureItemView item, bool isSelf)
        {
            MahjongChatTextureItemView itemView = item as MahjongChatTextureItemView;
            itemView.SetState(isSelf);
        }
    }
}