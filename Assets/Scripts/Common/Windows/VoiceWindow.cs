using System.Collections.Generic;
using com.yx.chatsystem;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows
{
    /// <summary>
    /// 语音窗口
    /// </summary>
    public class VoiceWindow : YxNguiWindow
    {
        /// <summary>
        /// 语音的api
        /// </summary>
        public string ActionKey = "soundApi";
        /// <summary>
        /// 语音系统
        /// </summary>
        public YxVoiceChatSystem ChatSystem;

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="url"></param>
        private void OnUpLoadVoiceSuccess(string url)
        {
            var sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, App.GameData.SelfSeat);
            sfsObj.PutInt("len", ChatSystem.UploadInfoData.LengthSce);
            App.RServer.SendFrameRequest("sound", sfsObj);
        }

        private void OnUpLoadVoiceFaild(string url)
        {
            YxDebug.LogError("上传声音失败 " + url);
        }

        protected override void OnShow()
        {
            ChatSystem.OnStartRecord();
        }

        public override void Hide()
        {
            var uinfo = ChatSystem.UploadInfoData;
            if (string.IsNullOrEmpty(uinfo.Url))
            {
                Facade.Instance<TwManager>().SendAction(ActionKey, new Dictionary<string, object>(),
                         key =>
                             {
                                 var info = ChatSystem.UploadInfoData;
                                 info.Url = key.ToString();
                                 info.OnFinish = OnUpLoadVoiceSuccess;
                                 info.OnFail = OnUpLoadVoiceFaild;
                                 ChatSystem.OnEndRecord();
                             },
                         false,
                         errMsg => YxDebug.LogError("获取语音key失败！{0}", "VoiceWindow", null, errMsg),false);
                return;
            }
            ChatSystem.OnEndRecord();
        } 
    }
}
