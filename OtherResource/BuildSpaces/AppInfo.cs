namespace Assets.Scripts.Common.Utils
{
	public class AppInfo
	{
		/// <summary>
		/// 【starcfg】服务器名称
		/// </summary>
		public const string ServerUrl = "http://121.40.165.241";
		/// <summary>
		/// 激活服务器路径
		/// </summary>
		public const string StartActionUrl = "https://op.yxixia.com/calc/active";
		/// <summary>
		/// 缓存服务器,如果设置"index.php/Md/vs",对startconfig更改ServerUrl有影响
		/// </summary>
		public const string CacheVersionUrl = "";
		/// <summary>
		/// 服务器推广id,打包时自动更改
		/// </summary>
		public const string ServerExtendId = "913";
		/// <summary>
		/// 应用启动配置，打包时会自动更改文件名
		/// </summary>
		public const string StartCfgUrl = "http://cfg.yxixia.com/update/_cfg/yxhall/skin_42/damutong/StartCfgForAndroid1.0.0.cfg";
		/// <summary>
		/// 指定的推广员【starcfg可修改该】
		/// </summary>
		public const string PromoteCode = "";
		/// <summary>
		/// 【starcfg】大厅配置路径
		/// </summary>
		public const string HallCfgUrl = "http://static.18yb.com/cfg/hall/skin_42/_default/4.0.0/201908281735/android";
		/// <summary>
		/// 【starcfg】大厅资源路径
		/// </summary>
		public const string HallResUrl = "http://static.18yb.com/res/hall/skin_42/android";
		/// <summary>
		/// 【starcfg】游戏资源路径，读取游戏配置时先会去HallCfgUrl中查找
		/// </summary>
		public const string GameResCfgUrl = "http://static.18yb.com/cfg/games/android";
		/// <summary>
		/// 【starcfg】游戏配置路径
		/// </summary>
		public const string GameResUrl = "http://static.18yb.com/res/games/android";
		/// <summary>
		/// 【starcfg】公用资源配置路径，读取此配置时先会去HallCfgUrl中查找
		/// </summary>
		public const string ShareCfgUrl = "http://static.18yb.com/cfg/hall/share/share_00/4.0.0/201908221558/android";
		/// <summary>
		/// 【starcfg】公用资源路径
		/// </summary>
		public const string ShareResUrl = "http://static.18yb.com/res/hall/share/android";
		/// <summary>
		/// 屏幕分辨率配置路径
		/// </summary>
		public const string PixelCfgUrl = "http://cfg.yxixia.com/update/_cfg/tools/pixel.cfg";
		/// <summary>
		/// 登录php后缀
		/// </summary>
		public const string LoginUrl = "login_";
		/// <summary>
		/// 是否缓存，默认打包都是true
		/// </summary>
		public const bool HasCache = true;
		/// <summary>
		/// 是否全屏
		/// </summary>
		public const bool IsFullScreen = false;
		/// <summary>
		/// 【starcfg】登陆后是否需要幕布
		/// </summary>
		public const bool NeedCurtainAfterLogin = false;
		/// <summary>
		/// 屏幕大小
		/// </summary>
		public const string ScreenSize = "0,0";
		/// <summary>
		/// 屏幕旋转
		/// </summary>
		public const int ScreenRotate = 12;
		/// <summary>
		/// 【starcfg】是否有本地资源
		/// </summary>
		public const bool HasLoadLocalRes = false;
		/// <summary>
		/// 【starcfg】是否检测网络类型
		/// </summary>
		public const bool NeedCheckNetType = false;
		/// <summary>
		/// 【starcfg】大厅是否支持轮询
		/// </summary>
		public const bool NeedRollNotice = true;
		/// <summary>
		/// 【starcfg】是否需要下载提示框
		/// </summary>
		public const bool NeedDownloadSizeBox = false;
		/// <summary>
		/// 【starcfg】退出时是否返回登录界面
		/// </summary>
		public const bool QuitToLogin = true;
		/// <summary>
		/// 【starcfg】锁定帧率
		/// </summary>
		public const int FrameRate = 60;
		/// <summary>
		/// 【starcfg】是否有微信登录
		/// </summary>
		public const bool HasWechatLogin = false;
		/// <summary>
		/// 【starcfg】微信AppId
		/// </summary>
		public const string WxAppId = "";
		/// <summary>
		/// 【starcfg】是否有QQ登录【starcfg可修改该】
		/// </summary>
		public const bool HasQqLogin = false;
		/// <summary>
		/// 【starcfg】QQ AppId
		/// </summary>
		public const string QqAppId = "";
		/// <summary>
		/// 交互系统弹框样式
		/// </summary>
		public const string TwMsgStyle = "MessageBox";
		/// <summary>
		/// 多服务器模式：  请求地址|数据格式
		/// </summary>
		public const string NetSelectCfg = "";
		/// <summary>
		/// 等待时间
		/// </summary>
		public const int DownLoadWaitTime = 15;
		/// <summary>
		/// 【starcfg】进入大厅前下载全部资源
		/// </summary>
		public const bool DownLoadFull = false;
		/// <summary>
		/// 【starcfg】登录类型
		/// </summary>
		public const int LoginType = 0;
	}
}
