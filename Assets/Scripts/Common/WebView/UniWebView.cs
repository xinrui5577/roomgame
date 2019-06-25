using System;
using System.Collections;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Common.WebView
{
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_EDITOR
    /// <summary>
    /// 网页内嵌组件 
    /// </summary>
    public class UniWebView : MonoBehaviour
    {
        #region Events and Delegate
        /// <summary>
        /// 加载成功监听
        /// </summary>
        public event Action<UniWebView,bool,string> OnLoadComplete;
        /// <summary>
        /// 开始加载监听
        /// </summary>
        public event Action<UniWebView,string> OnLoadBegin;
        /// <summary>
        /// 传递消息监听
        /// </summary>
        public event Action<UniWebView, UniWebViewMessage> OnReceivedMessage;
        /// <summary>
        /// 执行JS监听
        /// </summary>
        public event Action<UniWebView, string> OnEvalJavaScriptFinished;
        /// <summary>
        /// 关闭确认监听
        /// </summary>
        public event Func<UniWebView, bool> OnWebViewShouldClose;
        /// <summary>
        /// 按钮事件监听（安卓）
        /// </summary>
        public event Action<UniWebView,int> OnReceivedKeyCode;
        /// <summary>
        /// 屏幕变化监听
        /// </summary>
        public event Func<UniWebView, UniWebViewOrientation, UniWebViewEdgeInsets> InsetsForScreenOreitation;    
        /// <summary>
        /// 显示页面效果监听
        /// </summary>
        private Action _showTransitionAction;
        /// <summary>
        /// 隐藏页面效果监听
        /// </summary>
        private Action _hideTransitionAction;
        #endregion
        #region  Param Data 
        /// <summary>
        /// URL
        /// </summary>
        public string Url;
        /// <summary>
        /// 启动加载
        /// </summary>
        public bool LoadOnStart=true;
        /// <summary>
        /// IOS 工具调是否显示(不想要显示工具条，请关闭开关)
        /// </summary>
        public bool ToolBarShow = true;
        /// <summary>
        /// 加载完毕自动显示
        /// </summary>
        public bool AutoShowWhenLoadComplete;
        /// <summary>
        /// View关闭时是否清除缓存
        /// </summary>
        [HideInInspector]
        public bool CleanCacheOnFinished = false;
        #endregion
        /// <summary>
        /// 显示范围
        /// </summary>
        [SerializeField]
        private UniWebViewEdgeInsets _insets = new UniWebViewEdgeInsets(0, 0, 0, 0);
        /// <summary>
        /// 返回按钮开关（安卓）：影响keydown回调，input回调
        /// </summary>
        private bool _backButtonEnable = true;
        /// <summary>
        /// 页面Drag out 效果开关
        /// </summary>
        private bool _bouncesEnable;
        /// <summary>
        /// GUID
        /// </summary>
        private string _currentGuid;
        /// <summary>
        /// 最后一次加载时屏幕高度
        /// </summary>
        private int _lastScreenHeight;
        /// <summary>
        /// 浸入模式开关
        /// </summary>
        private bool _immersiveMode = true;
        /// <summary>
        /// 缩放手势
        /// </summary>
        private bool _zoomEnable;

        public UniWebViewEdgeInsets Insets
        {
            get
            {
                return _insets;
            }
            set
            {
                if (_insets != value)
                {
                    ForceUpdateInsetsInternal(value);
                }
            }
        }

        /// <summary>
        /// 同步显示范围
        /// </summary>
        /// <param name="insets"></param>
        private void ForceUpdateInsetsInternal(UniWebViewEdgeInsets insets)
        {
            _insets = insets;
            UniWebViewPlugin.ChangeInsets(gameObject.name,
                                          Insets.top,
                                          Insets.left,
                                          Insets.bottom,
                                          Insets.right);
#if UNITY_EDITOR
            CreateTexture(Insets.left,
                          Insets.bottom,
                          Screen.width - Insets.left - Insets.right,
                          Screen.height - Insets.top - Insets.bottom
                          );
#endif
        }

        /// <summary>
        /// 当前Url
        /// </summary>
        public string CurrentUrl
        {
            get
            {
                return UniWebViewPlugin.GetCurrentUrl(gameObject.name);
            }
        }
        /// <summary>
        /// 返回按钮可用
        /// </summary>
        public bool BackButtonEnable
        {
            get
            {
                return _backButtonEnable;
            }
            set
            {
                if (_backButtonEnable != value)
                {
                    _backButtonEnable = value;
#if (UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR
                UniWebViewPlugin.SetBackButtonEnable(gameObject.name, _backButtonEnable);
#endif
                }
            }
        }
        /// <summary>
        /// 回弹效果：IOS 弹，android 颜色变化
        /// </summary>
        public bool BouncesEnable
        {
            get
            {
                return _bouncesEnable;
            }
            set
            {
                if (_bouncesEnable != value)
                {
                    _bouncesEnable = value;
#if !UNITY_EDITOR
                UniWebViewPlugin.SetBounces(gameObject.name, _bouncesEnable);
#endif
                }
            }
        }
        /// <summary>
        /// 手势缩放(Android：更改生效 IOS:Loading 前设置，或更改后Reload)
        /// </summary>
        public bool ZoomEnable
        {
            get
            {
                return _zoomEnable;
            }
            set
            {
                if (_zoomEnable != value)
                {
                    _zoomEnable = value;
#if !UNITY_EDITOR
                UniWebViewPlugin.SetZoomEnable(gameObject.name, _zoomEnable);
#endif
                }
            }
        }
        /// <summary>
        /// user agent
        /// </summary>
        public string UserAgent
        {
            get
            {
                return UniWebViewPlugin.GetUserAgent(gameObject.name);
            }
        }
        /// <summary>
        /// 透明度 0~1
        /// </summary>
        public float alpha
        {
            get
            {
                return UniWebViewPlugin.GetAlpha(gameObject.name);
            }
            set
            {
                UniWebViewPlugin.SetAlpha(gameObject.name, Mathf.Clamp01(value));
            }
        }

        /// <summary>
        /// 使用外部浏览器打开URl（开启后，白名单回调生效ReceiveMessage）
        /// </summary>
        public bool OpenLinksInExternalBrowser
        {
            get
            {
                return UniWebViewPlugin.GetOpenLinksInExternalBrowser(gameObject.name);
            }
            set
            {
                UniWebViewPlugin.SetOpenLinksInExternalBrowser(gameObject.name, value);
            }
        }
        /// <summary>
        /// 浸入模式开关
        /// </summary>
        public bool ImmersiveMode
        {
            get
            {
                return _immersiveMode;
            }
            set
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            _immersiveMode = value;
            UniWebViewPlugin.SetImmersiveModeEnabled(gameObject.name, _immersiveMode);
#endif
            }
        }

        #region Life Cycle
        void Awake()
        {
            _currentGuid = Guid.NewGuid().ToString();
            gameObject.name = gameObject.name + _currentGuid;
            InitShow();
#if UNITY_EDITOR
            _screenScale = UniWebViewHelper.screenScale;
            CreateTexture(Insets.left,
                          Insets.bottom,
                          Screen.width - Insets.left - Insets.right,
                          Screen.height - Insets.top - Insets.bottom
                          );
#endif
        }
        /// <summary>
        /// 初始化设置参数
        /// </summary>
        public void InitShow()
        {
            UniWebViewPlugin.Init(gameObject.name,
                     Insets.top,
                     Insets.left,
                     Insets.bottom,
                     Insets.right);
            _lastScreenHeight = UniWebViewHelper.screenHeight;
        }

        void Start()
        {
            if (LoadOnStart)
            {
                Load();
            }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            Clean();
#endif
            if(CleanCacheOnFinished)
            {
                CleanCache();
            }
            RemoveAllListeners();
            UniWebViewPlugin.Destroy(gameObject.name);
            gameObject.name = gameObject.name.Replace(_currentGuid, "");
        }

        private void RemoveAllListeners()
        {
            OnLoadBegin = null;
            OnLoadComplete = null;
            OnReceivedMessage = null;
            OnReceivedKeyCode = null;
            OnEvalJavaScriptFinished = null;
            OnWebViewShouldClose = null;
            OnWebViewShouldClose = null;
            InsetsForScreenOreitation = null;
        }

        #endregion
        /// <summary>
        /// 设置User Agent
        /// </summary>
        /// <param name="value"></param>
        public static void SetUserAgent(string value)
        {
            UniWebViewPlugin.SetUserAgent(value);
        }
        /// <summary>
        /// 重置 User Agent
        /// </summary>
        public static void ResetUserAgent()
        {
            SetUserAgent(null);
        }

        /// <summary>
        /// IOS 工具栏完成按钮文本样式
        /// </summary>
        /// <param name="text"></param>
        public static void SetDoneButtonText(string text)
        {
#if UNITY_IOS && !UNITY_EDITOR
        UniWebViewPlugin.SetDoneButtonText(text);
#endif
        }

        /// <summary>
        /// 加载
        /// </summary>
        public void Load()
        {
            string loadUrl = String.IsNullOrEmpty(Url) ? "about:blank" : Url.Trim();
            if (ZoomEnable)
            {
                ZoomEnable=true;
            }
            UniWebViewPlugin.Load(gameObject.name, loadUrl);
        }

        /// <summary>
        /// 有参数加载
        /// </summary>
        /// <param name="aUrl"></param>
        public void Load(string aUrl)
        {
            Url = aUrl;
            Load();
        }
        
        /// <summary>
        /// 加载HtmlString
        /// </summary>
        /// <param name="htmlString"></param>
        /// <param name="baseUrl"></param>
        public void LoadHTMLString(string htmlString, string baseUrl)
        {
            UniWebViewPlugin.LoadHTMLString(gameObject.name, htmlString, baseUrl);
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        public void Reload()
        {
            UniWebViewPlugin.Reload(gameObject.name);
        }

        /// <summary>
        /// 停止加载
        /// </summary>
        public void Stop()
        {
            UniWebViewPlugin.Stop(gameObject.name);
        }

        /// <summary>
        /// 显示页面
        /// </summary>
        /// <param name="fade"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        /// <param name="finishAction"></param>
        public void Show(bool fade = false, UniWebViewTransitionEdge direction = UniWebViewTransitionEdge.None, float duration = 0.4f, Action finishAction = null)
        {
            _lastScreenHeight = UniWebViewHelper.screenHeight;
            YxDebug.Log(string.Format("_lastScreenHeight:{0}", _lastScreenHeight));
            ResizeInternal();

            UniWebViewPlugin.Show(gameObject.name, fade, (int)direction, duration);
            _showTransitionAction = finishAction;

            if (ToolBarShow)
            {
                ShowToolBar(true);
            }
        #if UNITY_ANDROID&&!UNITY_EDITOR
            UniWebViewPlugin.SetZoomEnable(gameObject.name, _zoomEnable);
        #endif

#if UNITY_EDITOR
            _webViewId = UniWebViewPlugin.GetId(gameObject.name);
            _hidden = false;
#endif
        }

        /// <summary>
        /// 隐藏页面
        /// </summary>
        /// <param name="fade"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        /// <param name="finishAction"></param>
        public void Hide(bool fade = false, UniWebViewTransitionEdge direction = UniWebViewTransitionEdge.None, float duration = 0.4f, Action finishAction = null)
        {
#if UNITY_EDITOR
            _hidden = true;
#endif
            UniWebViewPlugin.Hide(gameObject.name, fade, (int)direction, duration);
            _hideTransitionAction = finishAction;
        }


        /// <summary>
        /// 执行JS 脚本
        /// </summary>
        /// <param name="javaScript"></param>
        public void EvaluatingJavaScript(string javaScript)
        {
            UniWebViewPlugin.EvaluatingJavaScript(gameObject.name, javaScript);
        }

        /// <summary>
        /// 添加需要执行JS string
        /// </summary>
        /// <param name="javaScript"></param>
        public void AddJavaScript(string javaScript)
        {
            UniWebViewPlugin.AddJavaScript(gameObject.name, javaScript);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void CleanCache()
        {
            UniWebViewPlugin.CleanCache(gameObject.name);
        }

        /// <summary>
        /// 清除指定cookie（非及时生效）
        /// </summary>
        /// <param name="key"></param>
        public void CleanCookie(string key = null)
        {
            UniWebViewPlugin.CleanCookie(gameObject.name, key);
        }

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetBackgroundColor(Color color)
        {
            UniWebViewPlugin.SetBackgroundColor(gameObject.name, color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// 显示IOS 工具栏+动画
        /// </summary>
        /// <param name="animate"></param>
        public void ShowToolBar(bool animate)
        {
#if UNITY_IOS && !UNITY_EDITOR
        ToolBarShow = true;
        UniWebViewPlugin.ShowToolBar(gameObject.name,animate);
#endif
        }

        /// <summary>
        /// 隐藏IOS工具栏+动画
        /// </summary>
        /// <param name="animate"></param>
        public void HideToolBar(bool animate)
        {
#if UNITY_IOS && !UNITY_EDITOR
        ToolBarShow = false;
        UniWebViewPlugin.HideToolBar(gameObject.name,animate);
#endif
        }

        /// <summary>
        /// 加载显示页面时是否显示原生Loading（默认为True）
        /// </summary>
        /// <param name="show"></param>
        public void SetShowSpinnerWhenLoading(bool show)
        {
            UniWebViewPlugin.SetSpinnerShowWhenLoading(gameObject.name, show);
        }

        /// <summary>
        /// loading 文本
        /// </summary>
        /// <param name="text"></param>
        public void SetSpinnerLabelText(string text)
        {
            UniWebViewPlugin.SetSpinnerText(gameObject.name, text);
        }

        /// <summary>
        /// 加载或显示前是否显示页面（安卓）
        /// </summary>
        /// <param name="use"></param>
        public void SetUseWideViewPort(bool use)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewPlugin.SetUseWideViewPort(gameObject.name, use);
#endif
        }

        /// <summary>
        /// 使用Overview模式来显示网页
        /// </summary>
        /// <param name="overview"></param>
        public void LoadWithOverviewMode(bool overview)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewPlugin.LoadWithOverviewMode(gameObject.name, overview);
#endif
        }
        
        /// <summary>
        /// 是否可以返回
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack()
        {
            return UniWebViewPlugin.CanGoBack(gameObject.name);
        }

        /// <summary>
        /// 是否可以跳转到下层页面
        /// </summary>
        /// <returns></returns>
        public bool CanGoForward()
        {
            return UniWebViewPlugin.CanGoForward(gameObject.name);
        }

        /// <summary>
        /// 返回到上一个页面
        /// </summary>
        public void GoBack()
        {
            UniWebViewPlugin.GoBack(gameObject.name);
        }

        /// <summary>
        /// 跳转到下一页面
        /// </summary>
        public void GoForward()
        {
            UniWebViewPlugin.GoForward(gameObject.name);
        }

        /// <summary>
        /// 添加信任站点（安卓）
        /// </summary>
        /// <param name="url"></param>
        public void AddPermissionRequestTrustSite(string url)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewPlugin.AddPermissionRequestTrustSite(gameObject.name, url);
#endif
        }

        /// <summary>
        /// 添加协议
        /// </summary>
        /// <param name="scheme"></param>
        public void AddUrlScheme(string scheme)
        {
            UniWebViewPlugin.AddUrlScheme(gameObject.name, scheme);
        }

        /// <summary>
        /// 清除协议
        /// </summary>
        /// <param name="scheme"></param>
        public void RemoveUrlScheme(string scheme)
        {
            UniWebViewPlugin.RemoveUrlScheme(gameObject.name, scheme);
        }

        /// <summary>
        /// 设置自定义头文件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetHeaderField(string key, string value)
        {
#if UNITY_WP8
        Debug.LogWarning("Not implemented for Windows Phone 8.");
#else
            UniWebViewPlugin.SetHeaderField(gameObject.name, key, value);
#endif
        }

        /// <summary>
        /// 竖直方向进度条显示设置
        /// </summary>
        /// <param name="show"></param>
        public void SetVerticalScrollBarShow(bool show)
        {
#if UNITY_WP8
        Debug.LogWarning("Not implemented for Windows Phone 8.");        
#else
            UniWebViewPlugin.SetVerticalScrollBarShow(gameObject.name, show);
#endif
        }

        /// <summary>
        /// 水平方向进度条显示设置
        /// </summary>
        /// <param name="show"></param>
        public void SetHorizontalScrollBarShow(bool show)
        {
#if UNITY_WP8
        Debug.LogWarning("Not implemented for Windows Phone 8.");        
#else
            UniWebViewPlugin.SetHorizontalScrollBarShow(gameObject.name, show);
#endif
        }
        /// <summary>
        /// 安卓（Remote 调试）
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetWebContentsDebuggingEnabled(bool enabled)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        UniWebViewPlugin.SetWebContentsDebuggingEnabled(enabled);
#endif
        }

        private bool OrientationChanged()
        {
            int newHeight = UniWebViewHelper.screenHeight;
            if (_lastScreenHeight != newHeight)
            {
                _lastScreenHeight = newHeight;
                return true;
            }
            return false;
        }

        private void ResizeInternal()
        {
            int newHeight = UniWebViewHelper.screenHeight;
            int newWidth = UniWebViewHelper.screenWidth;

            UniWebViewEdgeInsets newInset = Insets;
            if (InsetsForScreenOreitation != null)
            {
                UniWebViewOrientation orientation =
                    newHeight >= newWidth ? UniWebViewOrientation.Portrait : UniWebViewOrientation.LandScape;
                newInset = InsetsForScreenOreitation(this, orientation);
            }
            ForceUpdateInsetsInternal(newInset);
        }

        #region 消息回调
        /// <summary>
        /// 加载完成回调
        /// </summary>
        /// <param name="message">错误信息，如果为空则表示加载成功，不为空则表示加载失败，消息内容为错误消息</param>
        private void LoadComplete(string message)
        {
            YxDebug.Log(string.Format("加载完成回调：{0}", message));
            bool loadSuc = string.IsNullOrEmpty(message);
            bool hasCompleteListener = (OnLoadComplete != null);
            if (loadSuc)
            {
                if (hasCompleteListener)
                {
                    OnLoadComplete(this, true, null);
                }
                if (AutoShowWhenLoadComplete)
                {
                    Show();
                }
            }
            else
            {
                YxDebug.LogWarning("Web page load failed: " + gameObject.name + "; url: " + Url + "; error:" + message);

#if UNITY_EDITOR
                if (message.Contains("App Transport Security"))
                {
                    YxDebug.LogWarning("It seems that ATS is enabled in Editor. You can not load http pages when it is on. Please visit our help center for more information: https://onevcat.zendesk.com/hc/en-us/articles/215527307-I-cannot-open-the-web-page-in-Unity-Editor-");
                }
#endif
                if (hasCompleteListener)
                {
                    OnLoadComplete(this, false, message);
                }
            }
        }
        /// <summary>
        /// 开始加载
        /// </summary>
        /// <param name="url"></param>
        private void LoadBegin(string url)
        {
            YxDebug.Log(string.Format("开始加载回调：{0}", url));
            if (OnLoadBegin != null)
            {
                OnLoadBegin(this, url);
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="rawMessage"></param>
        private void ReceivedMessage(string rawMessage)
        {
            YxDebug.Log(string.Format("接收到执行消息回调：{0}", rawMessage));
            UniWebViewMessage message = new UniWebViewMessage(rawMessage);
            if (OnReceivedMessage != null)
            {
                OnReceivedMessage(this, message);
            }
        }
        /// <summary>
        /// 页面关闭
        /// </summary>
        /// <param name="message"></param>
        private void WebViewDone(string message)
        {
            YxDebug.Log(string.Format("执行页面关闭回调，内容是：{0}", message));
            bool destroy = true;
            if (OnWebViewShouldClose != null)
            {
                destroy = OnWebViewShouldClose(this);
            }
            if (destroy)
            {
                Hide();
                Destroy(this);
            }
        }
        /// <summary>
        /// 按钮事件
        /// </summary>
        /// <param name="message"></param>
        private void WebViewKeyDown(string message)
        {
            YxDebug.Log(string.Format("执行按钮按下回调，内容是：{0}", message));
            int keyCode = Convert.ToInt32(message);
            if (OnReceivedKeyCode != null)
            {
                OnReceivedKeyCode(this, keyCode);
            }
        }
        /// <summary>
        /// 执行JS完成
        /// </summary>
        /// <param name="result"></param>
        private void EvalJavaScriptFinished(string result)
        {
            YxDebug.Log(string.Format("执行JS 回调，内容是：{0}",result));
            if (OnEvalJavaScriptFinished != null)
            {
                OnEvalJavaScriptFinished(this, result);
            }
        }
        /// <summary>
        /// 执行动画完成
        /// </summary>
        /// <param name="identifier"></param>
        private void AnimationFinished(string identifier)
        {

        }
        /// <summary>
        /// 显示转换效果结束
        /// </summary>
        /// <param name="message"></param>
        private void ShowTransitionFinished(string message)
        {
            if (_showTransitionAction != null)
            {
                _showTransitionAction();
                _showTransitionAction = null;
            }
        }
        /// <summary>
        /// 隐藏转换效果结束
        /// </summary>
        /// <param name="message"></param>
        private void HideTransitionFinished(string message)
        {
            if (_hideTransitionAction != null)
            {
                _hideTransitionAction();
                _hideTransitionAction = null;
            }
        }
        /// <summary>
        /// 加载Jar包
        /// </summary>
        /// <param name="jarFilePath"></param>
        /// <returns></returns>
        private IEnumerator LoadFromJarPackage(string jarFilePath)
        {
            WWW stream = new WWW(jarFilePath);
            yield return stream;
            if (stream.error != null)
            {
                if (OnLoadComplete != null)
                {
                    OnLoadComplete(this, false, stream.error);
                }
            }
            else
            {
                LoadHTMLString(stream.text, "");
            }
        }

        #endregion
        private void Update()
        {
#if UNITY_EDITOR
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (_webViewId != 0 && !_hidden)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (UniWebViewPlugin.CanGoBack(gameObject.name))
                        {
                            UniWebViewPlugin.GoBack(gameObject.name);
                        }
                        else
                        {
                            WebViewDone("");
                        }
                    }
                    else
                    {
                        _inputString += Input.inputString;
                    }
                }
            }
#endif

            if (OrientationChanged())
            {
                ResizeInternal();
            }
        }

        #region UnityEditor Debug
#if UNITY_EDITOR
        private Rect _webViewRect;
        private Texture2D _texture;
        private string _inputString;
        private int _webViewId;
        private bool _hidden;
        private IntPtr _renderCallback;
        private int _screenScale;

        public UniWebView(bool loadOnStart)
        {
            LoadOnStart = loadOnStart;
        }

        private void CreateTexture(int x, int y, int width, int height)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                _webViewRect = new Rect(x, y, width, height);
                _texture = new Texture2D(width * 2, height * 2, TextureFormat.ARGB32, false);
                _texture = new Texture2D(width * _screenScale, height * _screenScale, TextureFormat.ARGB32, false);
            }
        }

        private void Clean()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                Destroy(_texture);
                _webViewId = 0;
                _texture = null;
            }
        }

        private void OnGUI()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (_webViewId != 0 && !_hidden)
                {
                    Vector3 pos = Input.mousePosition;
                    bool down = Input.GetMouseButton(0);
                    bool press = Input.GetMouseButtonDown(0);
                    bool release = Input.GetMouseButtonUp(0);
                    float deltaY = Input.GetAxis("Mouse ScrollWheel");
                    bool keyPress = false;
                    string keyChars = "";
                    short keyCode = 0;
                    if (!string.IsNullOrEmpty(_inputString))
                    {
                        keyPress = true;
                        keyChars = _inputString.Substring(0, 1);
                        keyCode = (short)_inputString[0];
                        _inputString = _inputString.Substring(1);
                    }

                    var id = _texture.GetNativeTexturePtr().ToInt32();
                    UniWebViewPlugin.InputEvent(gameObject.name,
                                                (int)(pos.x - _webViewRect.x), (int)(pos.y - _webViewRect.y), deltaY,
                                                down, press, release, keyPress, keyCode, keyChars,
                                                id);
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2_0
                GL.IssuePluginEvent(_webViewId);
#else

                    GL.IssuePluginEvent(UniWebViewPlugin.GetRenderEventFunc(), _webViewId);
#endif
                    Matrix4x4 m = GUI.matrix;
                    GUI.matrix = Matrix4x4.TRS(new Vector3(0, Screen.height, 0),
                                               Quaternion.identity, new Vector3(1, -1, 1));
                    GUI.DrawTexture(_webViewRect, _texture);
                    GUI.matrix = m;
                }
            }
        }
#endif
        #endregion
    }
#endif
    /// <summary>
    /// 渐变效果方向
    /// </summary>
    public enum UniWebViewTransitionEdge
    {
        /// <summary>
        /// No transition when showing or hiding.
        /// </summary>
        None = 0,
        /// <summary>
        /// Transit the web view from/to top.
        /// </summary>
        Top,
        /// <summary>
        /// Transit the web view from/to left.
        /// </summary>
        Left,
        /// <summary>
        /// Transit the web view from/to bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// Transit the web view from/to right.
        /// </summary>
        Right
    }
}