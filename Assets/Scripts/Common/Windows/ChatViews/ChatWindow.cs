using System;
using System.Globalization;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Common.Windows.ChatViews
{
    /// <summary>
    /// 聊天窗口
    /// </summary>
    public class ChatWindow : YxNguiWindow
    {
        /// <summary>
        /// 偏移量
        /// </summary>
        public int OffY;
        /// <summary>
        /// 输入的内容
        /// </summary>
        public YxBaseInputLabelAdapter TalkInput;
        /// <summary>
        /// item预制体
        /// </summary>
        public ChatItem PrefabItemView;
        /// <summary>
        /// 间隔
        /// </summary>
        public int TalkCycle = 1500;

        protected override void OnFreshView()
        {
            var chatMgr = Facade.GetInterimManager<YxChatManager>();
            if (chatMgr == null)
            {
                Close();
                YxDebug.LogError("YxChatManager不存在！","ChatWindow");
                return;
            }
            var db = chatMgr.LoadChatDb();
            if (db == null) return;
            var types = db.ChatType;
            var typeCount = types.Length;
            var index = 0;
            if (typeCount > 1)
            {
                var self = App.GameData.GetPlayerInfo();
                if (self == null)
                {
                    Close();
                    return;
                }
                var sex = self.SexI;
                index = YxBaseUserInfo.GetSexToMW(sex);
            }
            index = Mathf.Min(typeCount,index);
            var type = types[index];
            var itemList = type.Chats;
            var itmeCount = itemList.Length;
            var parentTs = PrefabItemView.transform.parent;
            var curPosY = 0;
            for (var i = 0; i < itmeCount; i++)
            {
                var chatData = itemList[i];
                if(chatData==null) { continue;}
                var itemView = Instantiate(PrefabItemView);
                itemView.name = i.ToString(CultureInfo.InvariantCulture);
                itemView.gameObject.SetActive(true);
                var ts = itemView.transform;
                ts.parent = parentTs; 
                itemView.UpdateView(chatData.Content);
                ts.localPosition = new Vector3(0, curPosY, 0);
                ts.localScale = Vector3.one;
                curPosY -= itemView.Height + OffY;
            }
        }
         
        public void OnSendTalkContent()
        { 
            var content = TalkInput.Value;
            if (string.IsNullOrEmpty(content)) return;
            var characterLimit = TalkInput.GetCharacterLimit();
            if (content.Length > characterLimit)
            {
                content = content.Remove(characterLimit);
            }
            //todo 特殊指令处理 return
            SendContent(content);
            TalkInput.SetValue("");
            Close();
        }

        public void OnSelectedItem(string content)
        {
            SendContent(string.Format("/{0} {1}",YxChatManager.ChatCmd.Shortcut, content));
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
            // todo 类型
            // 世界、队伍、私聊、系统、喇叭
            // [05:55][世界]玩家：内容
            YxChatManager.SendChatContent(content);

            //普通内容格式 
            //     /chat 类型 内容 [from] [to]
              
        }
    } 
}
