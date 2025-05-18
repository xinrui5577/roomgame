using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using BestHTTP;
using BestHTTP.JSON;
using BestHTTP.WebSocket;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager
{
    /// <summary>
    /// WebSocket 管理
    /// </summary>
    public class WebSocketManager : BaseMono
    {
        private WebSocket _webSocekt;

        private string _address;
        /// <summary>
        /// 连接url格式
        /// </summary>
        private const string ConnectUrlFormat = "client/{0}/{1}/{2}";
        /// <summary>
        /// 发送数据格式
        /// </summary>
        private const string SendDataFormat = "发送消息事件{0},内容为{1}";
        /// <summary>
        /// 关闭提示格式
        /// </summary>
        private const string CloseFormat = "Socket OnClosed,code:{0},message:{1}";
        /// <summary>
        /// 接收数据格式
        /// </summary>
        private const string GetDataFormat = "收到Action【{0}】数据,data is【{1}】";
        /// <summary>
        /// 连接成功
        /// </summary>
        private const string ConnectSuccess = "连接成功";
        /// <summary>
        /// 连接成功
        /// </summary>
        private const string ConnectError = "信息异常：{0}";
        /// <summary>
        /// 数据错误
        /// </summary>
        private const string DataError = "信息异常：{0}";
        /// <summary>
        /// 消息错误
        /// </summary>
        private const string DataFatal = "数据异常：{0}";
        protected SocialMessageManager SocialManager
        {
            get
            {
                return Facade.Instance<SocialMessageManager>();
            }
        }

        public void ConnectSocket(string address = SocialSetting.Host)
        {
            if (HTTPUpdateDelegator.Instance)
            {
                HTTPUpdateDelegator.Instance.enabled = true;
            }
            var loginInfo = LoginInfo.Instance;
            var userInfo = UserInfoModel.Instance.UserInfo;
            if (loginInfo!=null&& userInfo!=null)
            {
                string userName = userInfo.LoginName;
                string userId = userInfo.UserId;
                string cTocken = loginInfo.ctoken;
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(cTocken))
                {
                    return;
                }
                _address = address.CombinePath(string.Format(ConnectUrlFormat, userName, userId, cTocken));
                YxDebug.LogError("_address:"+ _address);
                _webSocekt = new WebSocket(new Uri(_address));
                _webSocekt.OnOpen = OnOpen;
                _webSocekt.OnMessage = OnMessageReceived;
                _webSocekt.OnClosed = OnClosed;
                _webSocekt.OnError = OnError;
                _webSocekt.OnErrorDesc = OnErrorDesc;
                _webSocekt.Open();
            }
        }

        public void CloseSocekt()
        {
            IsOpen = false;
            if (_webSocekt!=null)
            {
                _webSocekt.OnOpen = null;
                _webSocekt.OnMessage = null;
                _webSocekt.OnClosed = null;
                _webSocekt.OnError = null;
                _webSocekt.OnErrorDesc = null;
                _webSocekt.Close();
                _webSocekt = null;
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="action">请求事件</param>
        /// <param name="param">请求参数</param>
        public bool SendSendRequest(string action, Dictionary<string, object> param=null)
        {
            if (_webSocekt != null)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(SocialTools.KeyAction, action);
                if (param != null)
                {
                    dic.Add(SocialTools.KeyData, param);
                }
                if (!IsOpen)
                {
                   SocialManager.ConnectSocket();
                }
                else
                {
                    var sendMessage = Json.Encode(dic);
                    YxDebug.Log(string.Format(SendDataFormat, action, sendMessage));
                    _webSocekt.Send(sendMessage);
                    return true;
                }
            }
            else
            {
                SocialManager.ConnectSocket();
            }
            return false;
        }
        /// <summary>
        /// 是否为打开状态
        /// </summary>
        public bool IsOpen;
        private void OnOpen(WebSocket socket)
        {
            IsOpen = true;
            YxDebug.LogError(ConnectSuccess);
        }
        /// <summary>
        /// Called when we received a text message from the server
        /// </summary>
        void OnMessageReceived(WebSocket ws, string message)
        {
            ReciveData recive = new ReciveData(message);
            if (recive.MessageValid)
            {
                if (recive.MessageSuccess) 
                {
                    YxDebug.Log(string.Format(GetDataFormat, recive.Action,Json.Encode(recive.VaildData)));
                    SocialManager.DispatchEvent(recive.Action,recive.VaildData);
                }
            }
            else
            {
                YxDebug.LogEvent(string.Format(DataFatal, recive.ErrorMsg));
            }
        }
        /// <summary>
        /// Called when the web socket closed
        /// </summary>
        void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            YxDebug.LogEvent(string.Format(CloseFormat,code,message));
            YxDebug.LogError("当前连接状态是:"+IsOpen);
            if(Facade.HasInstance<SocialMessageManager>())
            {
                SocialManager.CloseSocket();
            }
        }

        /// <summary>
        /// Called when an error occured on client side
        /// </summary>
        void OnError(WebSocket ws, Exception ex)
        {
            YxDebug.LogEvent(ConnectError + Json.Encode(ex));
        }

        void OnErrorDesc(WebSocket webSocket, string reason)
        {

        }

        void OnApplicationQuit()
        {
            if (_webSocekt != null)
                CloseSocekt();
        }

        public override void OnDestroy()
        {
            if (_webSocekt != null)
            {
              CloseSocekt();
            }
            base.OnDestroy();
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        private class ReciveData
        {
            /// <summary>
            /// 事件
            /// </summary>
            public string Action;
            /// <summary>
            /// Key Action error
            /// </summary>
            private const string KeyActionErr = "err";
            /// <summary>
            /// Key NoAction error
            /// </summary>
            private const string KeyNoActionErr = "无Action 参数";
            /// <summary>
            /// 数据异常格式
            /// </summary>
            private const string KeyDataErrorFormat = "错误码:{0},Action is{1} msg is:{2}";
            /// <summary>
            /// 消息成功交互值
            /// </summary>
            private const int SuccessCode = 0;
            /// <summary>
            /// 有效数据
            /// </summary>
            public Dictionary<string, object> VaildData;
            /// <summary>
            /// Code 
            /// </summary>
            public int Code;
            /// <summary>
            /// 消息是否有效
            /// </summary>
            public bool MessageValid = true;
            /// <summary>
            /// 消息是否成功
            /// </summary>
            public bool MessageSuccess = true;
            /// <summary>
            /// 错误消息
            /// </summary>
            public string ErrorMsg;

            public ReciveData(string data)
            {
                object parseObj = Json.Decode(data);
                var parseDic = parseObj as Dictionary<string, object>;
                if (parseDic != null)
                {
                    parseDic.TryGetValueWitheKey(out Action, SocialTools.KeyAction);
                    parseDic.TryGetValueWitheKey(out VaildData, SocialTools.KeyData);
                    switch (Action)
                    {
                        case "":
                            MessageValid = false;
                            ErrorMsg = KeyNoActionErr;
                            return;
                        case KeyActionErr:
                            MessageValid = false;
                            VaildData.TryGetValueWitheKey(out ErrorMsg,SocialTools.KeyMessage);
                            return;
                    }
                    VaildData.TryGetValueWitheKey(out Code,SocialTools.KeyCode,int.MinValue);
                    switch (Code)
                    {
                        case SuccessCode:
                            MessageSuccess = true;
                            VaildData.TryGetValueWitheKey(out VaildData, SocialTools.KeyData);
                            break;
                        default:
                            MessageSuccess = false;
                            VaildData.TryGetValueWitheKey(out ErrorMsg,SocialTools.KeyMessage);
                            if (!string.IsNullOrEmpty(ErrorMsg))
                            {
                                YxMessageBox.Show(ErrorMsg);
                            }
                            YxDebug.LogEvent(string.Format(DataError, string.Format(KeyDataErrorFormat, Code, Action, ErrorMsg)));
                            break;
                    }
                }
            }
        }
    }
}