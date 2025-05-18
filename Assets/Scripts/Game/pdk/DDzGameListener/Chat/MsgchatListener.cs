using System;
using System.Collections.Generic;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.Chat
{
    /// <summary>
    /// 信息聊天系统listener
    /// </summary>
    public class MsgchatListener : ServEvtListener
    {
        /// <summary>
        /// 自己的聊天信息表达
        /// </summary>
        [SerializeField]
        protected ExpressionCtrl SelfExpCtrl;

        /// <summary>
        /// 右侧玩家聊天信息表达
        /// </summary>
        [SerializeField]
        protected ExpressionCtrl LeftExpCtrl;

        /// <summary>
        /// 左侧玩家聊天信息的表达
        /// </summary>
        [SerializeField]
        protected ExpressionCtrl RightExpCtrl;

        /// <summary>
        /// 聊天系统ui
        /// </summary>
        [SerializeField]
        protected MsgChatUiCtrl MsgChatuictrl;


        [SerializeField] protected AudioClip[] ClipSource_Nan;
        [SerializeField] protected AudioClip[] ClipSource_Nv;

        /// <summary>
        /// 某段聊天文字对应的音效男
        /// </summary>
        Dictionary<string,AudioClip> _strtoclipDic_Nan = new Dictionary<string,AudioClip>();

        /// <summary>
        /// 某段聊天文字对应的音效女
        /// </summary>
        Dictionary<string, AudioClip> _strtoclipDic_Nv = new Dictionary<string, AudioClip>();

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnMsgChatEvt(OnUserTalk);
        }

        private void Start()
        {
            //如果声源和文字数组长度对应说明有问题
            if (ClipSource_Nan == null || ClipSource_Nan.Length != MsgChatuictrl.TalkStrs.Length)
            {
                Debug.LogError("快接语音和声源数量不一致");
                return;
            }

            var talkStrs = MsgChatuictrl.TalkStrs;
            var len = ClipSource_Nan.Length;
            for (var i = 0; i < len; i++)
            {
                _strtoclipDic_Nan[talkStrs[i]] = ClipSource_Nan[i];
            }

            len = ClipSource_Nv.Length;
            for (var i = 0; i < len; i++)
            {
                _strtoclipDic_Nv[talkStrs[i]] = ClipSource_Nv[i];
            }
        }


        private void OnUserTalk(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            //排除发送表情移动动画时候
            if(data.ContainsKey(PlayerInfoDetailCtrl.FaceMoveType))return;

            int seat = data.GetInt(RequestKey.KeySeat);
            int exp = data.GetInt(RequestKey.KeyExp);
            string msgtext = data.GetUtfString(RequestKey.KeyText);

            if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                ShowTalk(SelfExpCtrl, exp, msgtext,seat);
            }
            else if (seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                ShowTalk(RightExpCtrl, exp, msgtext,seat);
            }
            else if (seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ShowTalk(LeftExpCtrl, exp, msgtext,seat);
            }
        }

        private void ShowTalk(ExpressionCtrl expctrl,int exp, string msgtext,int seat)
        {
            if (string.IsNullOrEmpty(msgtext))
            {
                expctrl.ShowExp(exp);
                return;
            }

            AudioClip audioClip = null;

            var userInfo = App.GetGameData<GlobalData>().GetUserInfo(seat);

            if (userInfo == null || !userInfo.ContainsKey(NewRequestKey.KeySex)) return;

            if (userInfo.GetShort(NewRequestKey.KeySex) != 0)
            {
                if (_strtoclipDic_Nan.ContainsKey(msgtext)) audioClip = _strtoclipDic_Nan[msgtext];
            }else
                if (_strtoclipDic_Nv.ContainsKey(msgtext)) audioClip = _strtoclipDic_Nv[msgtext];

            expctrl.ShowText(msgtext, 3, audioClip);
        }


        public override void RefreshUiInfo()
        {
                          
        }
    }
}
