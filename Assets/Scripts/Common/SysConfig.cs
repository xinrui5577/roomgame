using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils; 
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Interfaces;
using YxFramwork.Common.Model;
using YxFramwork.Enums;
using YxFramwork.Manager;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Common
{
    public class SysConfig : ISysCfg
    {
        public bool HasCache { get; private set; }

        /// <summary>
        /// 启动配置url
        /// </summary>
        public string StartCfgUrl
        {
            get
            {
                return Application.isEditor && string.IsNullOrEmpty(AppInfo.StartCfgUrl)
                           ? string.Format("file://{0}/../OtherResource/start.cfg", Application.dataPath)
                           : AppInfo.StartCfgUrl;
            }
        }

        public string PixelCfgUrl { get; private set; }

        private string _cacheVersionUrl;
        public string CacheVersionUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_cacheVersionUrl))
                {
                    var cacheVersion = AppInfo.CacheVersionUrl;
                    _cacheVersionUrl = string.IsNullOrEmpty(cacheVersion) ? "index.php/Md/vs" : cacheVersion;
                }
                if (_cacheVersionUrl == "index.php/Md/vs")
                {
                    _cacheVersionUrl = AppInfo.ServerUrl.CombinePath(_cacheVersionUrl);
                }
                return _cacheVersionUrl;
            }
            set { _cacheVersionUrl = value; }
        }

        public string StartActionUrl {
            get { return AppInfo.StartActionUrl; }
        }

        public string ServerExtendId {
            get { return AppInfo.ServerExtendId; }
        }

        /// <summary>
        /// 服务器url
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        ///  Web服务器
        /// </summary>
        public string WebUrl { get; private set; }

         
        /// <summary>
        /// 是否开发模式
        /// </summary>
        public bool IsDeve { get; private set; }

        private string _hallResUrl;
        private string _gameResUrl;
        private string _shareResUrl;
        /// <summary>
        /// 资源服务器
        /// </summary>
        /// <param name="gameName">游戏key</param>
        /// <returns></returns>
        public string ResUrl(string gameName)
        {
            var skin = App.Skin;
            if (gameName == skin.Hall || gameName == skin.GameInfo)
            {
                //                return string.Format("{0}{1}/", _hallResUrl, gameName);
                return _hallResUrl.CombinePath(gameName, "/");
            }
            //            return string.Format("{0}{1}/", gameName == skin.Share ? _shareResUrl : _gameResUrl, gameName);
            return gameName == skin.Share || gameName == skin.Rule ? _shareResUrl.CombinePath(gameName, "/") : _gameResUrl.CombinePath(gameName, "/");
        }

        public string ShareResUrl(string type)
        {
            return _shareResUrl.CombinePath(type);
        }

        private string _hallResCfgUrl; 
        private string _gameResCfgUrl;
        private string _shareResCfgUrl;

        /// <summary>
        /// 资源配置服务器
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="fromHall"></param>
        /// <returns></returns>
        public string ResCfgUrl(string gameName, bool fromHall = true)
        {
            var skin = App.Skin;
            string urlRoot;
            if (gameName == skin.Hall || gameName == skin.GameInfo)
            {
                urlRoot = _hallResCfgUrl;
            }
            else if (gameName == skin.Share || gameName == skin.Rule)
            {
                urlRoot = _shareResCfgUrl;
            }
            else if (fromHall) //如果是大厅指定的资源
            {
                var lastIndex = _gameResCfgUrl.TrimEnd('/').LastIndexOf('/');
                urlRoot = HallCustomUrlPart.CombinePath("games").CombinePath(_gameResCfgUrl.Substring(lastIndex));
            }
            else
            {
                urlRoot = _gameResCfgUrl;
            }
            return urlRoot.CombinePath(gameName, ".cfg");
        }

        /// <summary>
        /// php通用接口
        /// </summary>
        public string GateWay
        {
            get { return ServerUrl.CombinePath("index.php/Client/Api/gateway"); }
        }

        public string WxAppId { get; set; }
        public string QqAppId { get; set; }


        /// <summary>
        /// 是否微信登录
        /// </summary>
        //public static bool HasWechatLogin = false;
        public bool HasWechatLogin { get; private set; }
        public bool HasQqLogin { get; private set; }

        /// <summary>
        /// 调试模式
        /// </summary>
        public int IsDebug { get; private set; }

        /// <summary>
        /// 调试玩家
        /// </summary>
        public string LogUserId { get; private set; }

        /// <summary>
        /// 调试服务器
        /// </summary>
        public string LogUrl { get; private set; }

        /// <summary>
        /// 是否加载本地资源
        /// </summary>
        public bool HasLoadLocalRes { get; private set; }

        /// <summary>
        /// 需要检测网络状态
        /// </summary>
        public bool NeedCheckNetType { get; private set; }

        /// <summary>
        /// 是否需要实时广播
        /// </summary>
        public bool NeedRollNotice { get; private set; }
        /// <summary>
        /// 是否需要下载提示框
        /// </summary>
        public bool NeedDownloadSizeBox { get; private set; }

        /// <summary>
        /// 帧率
        /// </summary>
        public int FrameRate { get; private set; }

        /// <summary>
        /// 退出大厅时是否需要返回登录界面
        /// </summary>
        public bool QuitToLogin { get; private set; }
        /// <summary>
        /// 全屏
        /// </summary>
        public bool IsFullScreen { get { return AppInfo.IsFullScreen; } }

        public Vector2 ScreenSize {
            get
            {
                var screenSize = AppInfo.ScreenSize;
                var size = Vector2.zero;
                if (string.IsNullOrEmpty(screenSize)) return size;
                var arr = screenSize.Split(',');
                if (arr.Length < 2) return size;
                float.TryParse(arr[0], out size.x);
                float.TryParse(arr[1], out size.y);
                return size;
            }
        }

        public ScreenManager.ScreenRotateState ScreenRotate {
            get
            {
                return (ScreenManager.ScreenRotateState)AppInfo.ScreenRotate;
            }
        }

        /// <summary>
        /// 多服务器
        /// </summary>
        public string NetSelectCfg { get { return AppInfo.NetSelectCfg; } }

        public TwManager.TwMessageStyle TwMsgStyle {
            get
            {
                const string style = AppInfo.TwMsgStyle;
                if (string.IsNullOrEmpty(style)) return TwManager.TwMessageStyle.MessageBox;
                return (TwManager.TwMessageStyle)Enum.Parse(typeof(TwManager.TwMessageStyle), style);
            }
        }

        public bool NeedCurtainAfterLogin { get; private set; }
        public int DownLoadWaitTime { get; private set; }
        public bool DownLoadFull { get; set; }
        public YxELoginType LoginType { get; set; }
        public string HallCustomUrlPart { get; private set; }
        public string GameMainUrlPart { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subfield"></param>
        /// <param name="getParameter"></param>
        /// <returns></returns>
        public string GetUrlWithServer(string subfield, string getParameter = "")
        {
            return ServerUrl.CombinePath(string.Format("{0}?mt={1}&ver={2}&token={3}&userid={4}{5}",
                                 subfield,//
                                 App.YxPlatForm,//平台
                                 Application.version,//版本
                                 LoginInfo.Instance.ctoken,
                                 LoginInfo.Instance.user_id,
                                 getParameter));//其他
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetRecharge(string userid, string token)
        {
            return WebUrl.CombinePath(string.Format("index.php/home/Payment/index?uid={0}&token={1}", userid, token));
            //            return WebUrl + "/index.php/home/Payment/index?uid=" + userid + "&token=" + tokend;
        }

        private string _loginParm = "login_";

        public string GetFullLoginUrl()
        {
            return ServerUrl.CombinePath("index.php/Client/User").CombinePath(_loginParm);
            //            return string.Format("{0}/index.php/Client/User/{1}?", ServerUrl, _loginParm);
        }

#if UNITY_EDITOR
        private readonly Dictionary<string,Dictionary<string,string>> _editorAssetDict = new Dictionary<string, Dictionary<string, string>>();
#endif
        public SysConfig(bool isDeve)
        {
            ServerUrl = AppInfo.ServerUrl;
#if UNITY_EDITOR
            IsDeve = isDeve;
            _editorAssetDict.Clear();  
#else
            IsDeve = false;
#endif
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="dict"></param>
        public void LoadConfig(Dictionary<string, string> dict)
        {
            FilterConfig(dict);
            //服务器url
            if (dict.ContainsKey("ServerUrl"))
            {
                ServerUrl = dict["ServerUrl"];
            }
            else
            {
                if (string.IsNullOrEmpty(NetSelectCfg))
                {
                    ServerUrl = AppInfo.ServerUrl;
                }
            }
            //web主url 
            WebUrl = dict.ContainsKey("WebUrl") ? dict["WebUrl"] : ServerUrl;
            //登录参数
            _loginParm = dict.ContainsKey("LoginUrl") ? dict["LoginUrl"] : AppInfo.LoginUrl;
            //大厅资源url
            _hallResUrl = dict.ContainsKey("HallResUrl") ? dict["HallResUrl"] : AppInfo.HallResUrl;
            //游戏资源参数
            _gameResUrl = dict.ContainsKey("GameResUrl") ? dict["GameResUrl"] : AppInfo.GameResUrl;
            //公共资源url
            _shareResUrl = dict.ContainsKey("ShareResUrl") ? dict["ShareResUrl"] : AppInfo.ShareResUrl;
            //大厅资源配置url
            _hallResCfgUrl = dict.ContainsKey("HallCfgUrl") ? dict["HallCfgUrl"] : AppInfo.HallCfgUrl;
            //游戏资源配置url
            _gameResCfgUrl = dict.ContainsKey("GameResCfgUrl") ? dict["GameResCfgUrl"] : AppInfo.GameResCfgUrl;
            //公共资源配置url
            _shareResCfgUrl = dict.ContainsKey("ShareCfgUrl") ? dict["ShareCfgUrl"] : AppInfo.ShareCfgUrl;
            HallCustomUrlPart = PathHelper.GetUrlRoot(_hallResCfgUrl,3);
            GameMainUrlPart = PathHelper.GetUrlRoot(_gameResCfgUrl,1);
            //微信appid
            WxAppId = dict.ContainsKey("WxAppId") ? dict["WxAppId"] : AppInfo.WxAppId;
            QqAppId = dict.ContainsKey("QqAppId") ? dict["QqAppId"] : AppInfo.QqAppId;

            #region 下载等待时间
            if (dict.ContainsKey("DownLoadWaitTime"))
            {
                int downWtime;
                if (int.TryParse(dict["DownLoadWaitTime"], out downWtime))
                {
                    DownLoadWaitTime = downWtime;
                }
            }
            else
            {
                DownLoadWaitTime = AppInfo.DownLoadWaitTime;
            }

            #endregion

            #region MyRegion

            var pixe = string.Empty;
            PixelCfgUrl = DictionaryHelper.Parse(dict, "PixelCfgUrl", ref pixe) ? pixe : AppInfo.PixelCfgUrl;

            #endregion 屏幕配置

            #region 是需要登陆后的幕布

            if (dict.ContainsKey("NeedCurtainAfterLogin"))
            {
                bool needCurtain;
                bool.TryParse(dict["NeedCurtainAfterLogin"], out needCurtain);
                NeedCurtainAfterLogin = needCurtain;
            }
            else NeedCurtainAfterLogin = AppInfo.NeedCurtainAfterLogin;
            #endregion

            #region 是否有缓存

            if (dict.ContainsKey("HasCache"))
            {
                bool hasCache;
                bool.TryParse(dict["HasCache"], out hasCache);
                HasCache = hasCache;
            }
            else if (Application.isEditor)
            {
                HasCache = AppInfo.HasCache;
            }
            else
            {
                HasCache = true;
            }
            #endregion

            #region 是否有微信登录

            if (dict.ContainsKey("HasWechatLogin"))
            {
                bool hswl;
                bool.TryParse(dict["HasWechatLogin"], out hswl);
                HasWechatLogin = hswl;
            }
            else HasWechatLogin = AppInfo.HasWechatLogin;
            #endregion

            #region 是否有qq登录
            if (dict.ContainsKey("HasQqLogin"))
            {
                bool hswl;
                bool.TryParse(dict["HasQqLogin"], out hswl);
                HasQqLogin = hswl;
            }
            else HasQqLogin = AppInfo.HasQqLogin;
            #endregion

            #region 服务器端口

            if (dict.ContainsKey("ServerPort"))
            {
                int port;
                if (int.TryParse(dict["ServerPort"], out port))
                {
                    ServerPort = port;
                }
            }
            else
            {
                ServerPort = 9933;
            }

            #endregion

            #region 是否debug模式
            if (dict.ContainsKey("IsDebug"))
            {
                var debugInfo = dict["IsDebug"];
                var dbInfos = debugInfo.Split('|');
                var debugStr = dbInfos[0];
                int isdebug;
                int.TryParse(debugStr, out isdebug);
                IsDebug = isdebug;
                if (dbInfos.Length > 1)
                {
                    LogUserId = dbInfos[1].Trim();
                }
                if (dbInfos.Length > 2)
                {
                    LogUrl = dbInfos[2].Trim();
                }
            }
            else
            {
#if YX_DEVE
                IsDebug = 48;
#else
                IsDebug = 0;
#endif
            }
            #endregion

            #region 是否加载本地资源
            if (dict.ContainsKey("HasLoadLocalRes"))
            {
                bool hasLoadLocalRes;
                if (bool.TryParse(dict["HasLoadLocalRes"], out hasLoadLocalRes))
                {
                    HasLoadLocalRes = hasLoadLocalRes;
                }
            }
            else
            {
                HasLoadLocalRes = AppInfo.HasLoadLocalRes;
            }
            #endregion

            #region 需要检测网络状态
            if (dict.ContainsKey("NeedCheckNetType"))
            {
                bool needCheckNetType;
                if (bool.TryParse(dict["NeedCheckNetType"], out needCheckNetType))
                {
                    NeedCheckNetType = needCheckNetType;
                }
            }
            else
            {
                NeedCheckNetType = AppInfo.NeedCheckNetType;
            }
            #endregion

            #region 是否需要实时广播
            if (dict.ContainsKey("NeedRollNotice"))
            {
                bool needRollNotice;
                if (bool.TryParse(dict["NeedRollNotice"], out needRollNotice))
                {
                    NeedRollNotice = needRollNotice;
                }
            }
            else
            {
                NeedRollNotice = AppInfo.NeedRollNotice;
            }
            #endregion

            #region 是否需要下载提示框
            if (dict.ContainsKey("NeedDownloadSizeBox"))
            {
                var needDownloadSizeBox = false;
                if (bool.TryParse(dict["NeedDownloadSizeBox"], out needDownloadSizeBox))
                {
                    NeedDownloadSizeBox = needDownloadSizeBox;
                }
            }
            else
            {
                NeedDownloadSizeBox = AppInfo.NeedDownloadSizeBox;
            }
            #endregion

            #region 帧率
            if (dict.ContainsKey("FrameRate"))
            {
                int frameRate;
                if (int.TryParse(dict["FrameRate"], out frameRate))
                {
                    FrameRate = frameRate;
                }
            }
            else
            {
                FrameRate = AppInfo.FrameRate;
            }
            #endregion

            #region 退出大厅时是否需要返回登录界面  
            if (dict.ContainsKey("QuitToLogin"))
            {
                bool quitToLogin;
                if (bool.TryParse(dict["QuitToLogin"], out quitToLogin))
                {
                    QuitToLogin = quitToLogin;
                }
            }
            else
            {
                QuitToLogin = AppInfo.QuitToLogin;
            }
            #endregion

            #region 进入大厅前下载全部资源
            if (dict.ContainsKey("DownLoadFull"))
            {
                bool downLoadFull;
                if (bool.TryParse(dict["DownLoadFull"], out downLoadFull))
                {
                    DownLoadFull = downLoadFull;
                }
            }
            else
            {
                DownLoadFull = AppInfo.DownLoadFull;
            }
            #endregion


            #region 登录类型
            var loginType = 0;
            if (dict.Parse("LoginType", ref loginType))
            {
                LoginType = (YxELoginType)loginType;
            }
            else
            {
                LoginType =(YxELoginType)AppInfo.LoginType;
            }
            #endregion


        }

         
        private static string GetHallCustomUrlPart(string hallCfgUrl)
        {
            const string httpStr = "http://";
            var temp = hallCfgUrl.Replace(httpStr, "").Trim('/');
            var arr = temp.Split('/');
            var arrLen = arr.Length - 3;
            var hallResCfgUrlPart = httpStr;
            for (var i = 0; i < arrLen; i++)
            {
                hallResCfgUrlPart = hallResCfgUrlPart.CombinePath(arr[i]);
            }
            return hallResCfgUrlPart;
        } 

        /// <summary>
        /// 初始化开发资源
        /// </summary>
        public Object DeveLoadAsset(string gameKey, string bundleName, string assetName)
        {
#if UNITY_EDITOR
            if (IsDeve)
            {
                var skins = App.Skin.GetGameKeySkins(gameKey);
                var count = skins.Length;
                for (var i = count - 1; i >= 0; i--)
                {
                    var skinName = skins[i];
                    Dictionary<string, string> assetDict;
                    if (!_editorAssetDict.ContainsKey(skinName))
                    {
                        var path = "Assets/StreamingAssets/" + skinName + ".ini"; 
                        assetDict = new Dictionary<string, string>();
                        com.yxixia.utile.FileTools.PropertyUtile.ReadFile(path, ref assetDict);
                        _editorAssetDict[skinName] = assetDict;
                    }
                    else
                    {
                        assetDict = _editorAssetDict[skinName]; 
                    }
                    if (assetDict.ContainsKey(assetName))
                    {
                        var filePath = assetDict[assetName];
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            return UnityEditor.AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                        }
                    }
                }
            }
#endif 
            return null;
        }


        /// <summary>
        /// 过滤属性
        /// </summary>
        /// <param name="dict"></param>
        private static void FilterConfig(Dictionary<string, string> dict)
        {
            if (_extendConfig == null || dict == null) return;
            foreach (var kv in _extendConfig)
            {
                dict[kv.Key] = kv.Value;
            }
        }

        private static Dictionary<string, string> _extendConfig;
        public static void Extend(Dictionary<string, string> dict)
        {
            _extendConfig = dict;
        }
    }
}
