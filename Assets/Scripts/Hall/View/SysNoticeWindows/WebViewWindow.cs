/** 
 *文件名称:     WebViewWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-12-26 
 *描述:         网页内嵌窗口
 *              1.获得网址途径有两种：1.本地资源参数Url 2.后台配置Url
 *              
 *历史记录:    
 *          修改一
 *          时间：2018-01-11 10:48:41
 *          描述：1.处理多平台适应，增加缩放开关ZoomEnable
 *                2.加快加载流程（生成后加载）
 *          
 *          修改二
 *          时间：2018-01-12 11:04:32
 *          描述：1.增加自定义协议类型：gamenotice，并增加事件响应接口，实现unity内与webview交互
 *                2.增加 UI同步方法UpdateInsets，实现Unity webview同步与界面移动同步
 *                
 *          修改三
 *          时间：2018-01-12 14:25:34
 *          描述：1.增加全屏开关，开关生效时，网页范围为全屏
 *                2.增加配置属性，请求网页配置时，可以设置网页显示为全屏（后台参数优先级高）
 *                
 *          修改四
 *          时间:2018-01-17 11:40:39
 *          描述：1.处理安卓返回按钮响应处理
 *                2.优化网页加载流程
 *                3.处理网页关闭回调
 *                
 *          修改五
 *          时间:2018-02-08 11:49:21
 *          描述：增加请求解析格式，可以在网页参数中配置单独的Url，非tab式
 *          
 *          修改六
 *          时间：2018-02-10 17:46:41
 *          描述：处理网页缓存，支持后台配置清除网页缓存
 *          
 *          修改七
 *          时间：2018-04-30 23:37:55
 *          描述：处理网页内嵌Pc端跳转：跳转浏览器网页并关闭当前窗口
 *
 */

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.WebView;
using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.SysNoticeWindows
{
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_EDITOR||UNITY_STANDALONE
    public class WebViewWindow : YxTabPageWindow
    {
        #region UI Param
        [Tooltip("网页显示参数，必要参数")]
        public WebViewParam ViewViewParam;
        [Tooltip("页面显示父级，必要参数")]
        public GameObject ViewParent;
        [Tooltip("网页预设，必要参数")]
        public UniWebView ViewPrefab;
        [Tooltip("PC包或者是编辑器模式下用与关闭的按钮,非必要参数,方便调试使用")]
        public GameObject CloseButtonForPc;
        #endregion

        #region Data Param
        [Tooltip("无请求，显示固定Url")]
        public bool NoAction=true;
        [Tooltip("配置项")]
        public string ParamKey= "config_n";
        [Tooltip("请求接口")]
        public string ParamValue = "WebDatasRequest";
        [Tooltip("手势缩放开关")]
        public bool ZoomEnable = false;
        [Tooltip("响应协议")]
        public string[] Schemes=
        {
            "gamenotice",
        };
        [Tooltip("响应协议操作名称:close:关闭")]
        public List<string> ActionName=new List<string>()
        {
            "close"
        };
        [Tooltip("响应协议操作，需要与名称对齐")]
        public List<EventDelegate> Actions=new List<EventDelegate>();
        [Tooltip("是否全屏")]
        public bool FullScreen;
        [Tooltip("页面加载失败提示")]
        public string NoticeWhenLoadFailed = "页面加载失败，请尝试重新加载";
        /// <summary>
        /// js调用安卓接口用的接口名称
        /// </summary>
        public string JsInterFaceName = "android";
        #endregion

        #region Local Data
        /// <summary>
        /// 用于请求的网页参数
        /// </summary>
        private string _url;
        /// <summary>
        /// 当前显示的网页
        /// </summary>
        private UniWebView _curWebView;
        /// <summary>
        /// 当前页签
        /// </summary>
        private YxTabItem _curTabItem;
        /// <summary>
        /// 交互提示标识
        /// </summary>
        private const string KeyGameNotice= "gamenotice";
        /// <summary>
        /// 缓存数据，用于校验是否为旧数据
        /// </summary>
        private object _cacheData;
        /// <summary>
        /// 是否需要清除缓存
        /// </summary>
        private bool _needCleanCache;
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Debug.LogError(JsInterFaceName);
            UniWebViewHelper.JsInterFaceName = JsInterFaceName;
#if UNITY_EDITOR || UNITY_EDITOR_WIN
            if (CloseButtonForPc)
            {
                CloseButtonForPc.SetActive(true);
            }
#else
              if (CloseButtonForPc)
            {
                CloseButtonForPc.SetActive(false);
            }
#endif
            if (!NoAction)
            {
                Facade.Instance<TwManager>().SendAction(TabActionName,
                    new Dictionary<string, object>()
                   {
                       {ParamKey,ParamValue}
                   },
                   UpdateView);
            }
        }

        #endregion
        #region Function

        protected override void ActionCallBack()
        {
            if (Data.Equals(_cacheData))
            {
                return;
            }
            _cacheData = Data;
            if (Data is Dictionary<string, object>)     
            {
                YxDebug.Log("ActionCallBack is dic");
                WebData webData=new WebData(Data);
                List<TabData> datas = webData.Datas;
                FullScreen = webData.FullScreen;
                _needCleanCache = webData.CleanCache;
                TabDatas = new TabData[datas.Count];
                datas.CopyTo(TabDatas);
                TabDatas[0].StarttingState = true;
                TabSatate = -1;
                UpdateTabs(TabDatas);
            }
            else
            {
                if (Data is string)
                {
                    if (!string.IsNullOrEmpty(Data.ToString()))
                    {
                        YxDebug.Log("ActionCallBack is string,value is:"+Data);
                        _url = Data.ToString();
                        InitWebview();
                        ShowView();
                    }
                }
            }
        }

        public override void OnTableClick(YxTabItem tableView)
        {
            if (tableView.GetToggle().value)
            {
                YxDebug.Log("OnTableClick");
                if (tableView.Equals(_curTabItem))
                {
                    return;
                }
                _curTabItem = tableView;   
                YxDebug.Log(string.Format("显示：{0},配置网址是：{1}",_curTabItem.GetData<TabData>().Name, tableView.GetData<TabData>().Data));
                YxDebug.Log(string.Format("当前的Key是:{0}",gameObject.name));
                _url = _curTabItem.GetData<TabData>().Data.ToString();
                InitWebview();
                ShowView();
            }
        }

        /// <summary>
        /// 显示WebView
        /// </summary>
        public void ShowView()
        {
            if (_curWebView)
            {
                LoadWebView();
            }
        }

        private void InitWebview()
        {
            Transform parentTrans = ViewParent.transform;
            while (parentTrans.childCount > 0)
            {
                DestroyImmediate(parentTrans.GetChild(0).gameObject);
            }
            var item = YxWindowUtils.CreateItem(ViewPrefab, parentTrans);
            if (item)
            {
                _curWebView = item;
                _curWebView.CleanCacheOnFinished = _needCleanCache;
            }
        }

        /// <summary>
        /// 加载网页
        /// </summary>
        private void LoadWebView()
        {
#if UNITY_EDITOR||UNITY_STANDALONE_WIN
            Application.OpenURL(_url);
            Close();
            return;
#endif
            YxWindowManager.ShowWaitFor();
            _curWebView.Url = _url;
            _curWebView.OnWebViewShouldClose += OnWebViewShouldClose;
            _curWebView.ZoomEnable = ZoomEnable;
            AddSchemes(_curWebView, Schemes);
            _curWebView.Insets =GetShowParam();
            _curWebView.OnLoadComplete += OnLoadWebViewFinished;
            _curWebView.InsetsForScreenOreitation += OnInsersChange;
            if(!_curWebView.LoadOnStart)
            _curWebView.Load();
        }
        
        /// <summary>
        /// 添加schemes
        /// </summary>
        /// <param name="webview"></param>
        /// <param name="schemes"></param>
        private void AddSchemes(UniWebView webview,string[] schemes)
        {
            if (webview)
            {
                if (schemes != null&& schemes.Length>=0)
                {
                    foreach (var scheme in schemes)
                    {
                        webview.AddUrlScheme(scheme);
                    }
                    webview.OnReceivedMessage +=delegate(UniWebView view,UniWebViewMessage message)
                    {
                        YxDebug.LogError(string.Format("响应指定协议，协议名称是:{0}", message.scheme));
                        switch (message.scheme)
                        {
                            case KeyGameNotice:     
                                var action = message.path;
                                YxDebug.LogError(string.Format("协议操作是:{0}", action));
                                int index = ActionName.IndexOf(action);
                                YxDebug.LogError(string.Format("协议操作索引是:{0}", index));
                                if (index>-1)
                                {
                                    if (Actions.Count>index)
                                    {
                                        YxDebug.LogError("执行动作");
                                        Actions[index].Execute();
                                    }     
                                }
                                break;
                            default:
                                YxDebug.Log(string.Format("协议是{0}，爱干啥干啥,我这没有。。。", message.scheme));
                                break; 
                        }
                    };

                }
            }
        }

        /// <summary>
        /// 删除schemes
        /// </summary>
        /// <param name="webview"></param>
        /// <param name="schemes"></param>
        private void RemoveSchemes(UniWebView webview, string[] schemes)
        {
            if (webview)
            {
                if (schemes != null && schemes.Length >= 0)
                {
                    foreach (var scheme in schemes)
                    {
                        webview.RemoveUrlScheme(scheme);
                    }
                }
            }
        }

        /// <summary>
        /// 页面回退
        /// </summary>
        public void WebViewBack()
        {
            if (_curWebView)
            {
                if (_curWebView.CanGoBack())
                {
                    _curWebView.GoBack();
                }
                else
                {
                    YxDebug.LogError("已经到达最上层了，不能再返回了");
                }
            }
            else
            {
                YxDebug.LogError("CurWebView is null,please try again!");
            }
        }
        /// <summary>
        /// 页面回退
        /// </summary>
        public void WebViewForward()
        {
            if (_curWebView)
            {
                if (_curWebView.CanGoForward())
                {
                    _curWebView.GoForward();
                }
                else
                {
                    YxDebug.LogError("到头了，还走啥！");
                }
            }
            else
            {
                YxDebug.LogError("CurWebView is null,please try again!");
            }
        }
        /// <summary>
        /// 页面加载完毕处理
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        private void OnLoadWebViewFinished(UniWebView webView, bool success, string errorMessage)
        {
            YxWindowManager.HideWaitFor();
            if (success)
            {
                if (_curWebView)
                {
                    _curWebView = webView;
                    _curWebView.Insets = GetShowParam();
                    _curWebView.Show(true, UniWebViewTransitionEdge.Left, 0);
                }
            }
            else
            {
                YxDebug.LogError("加载失败,Error is ：" + errorMessage);
                if (FullScreen)
                {
                    Close();
                    YxMessageBox.Show(NoticeWhenLoadFailed);
                }
            }
        }

        private bool OnWebViewShouldClose(UniWebView view)
        {
            Close();
            return true;
        }

        /// <summary>
        /// 显示参数变化处理
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        private UniWebViewEdgeInsets OnInsersChange(UniWebView webView, UniWebViewOrientation orientation)
        {
            if (_curWebView)
            {
                _curWebView = webView;
                _curWebView.Insets = GetShowParam();
                return _curWebView.Insets;
            }
            return UniWebViewEdgeInsets.Zero;


        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void OnDestroy()
        {
            if (_curWebView)
            {
                _curWebView.Url = "";
                _curWebView.OnLoadComplete -= OnLoadWebViewFinished;
                _curWebView.InsetsForScreenOreitation -= OnInsersChange;            
                _curWebView = null;

            }
            base.OnDestroy();
        }

        /// <summary>
        /// 更新显示范围参数
        /// </summary>
        public void UpdateInsets()
        {
            if (_curWebView!=null)
            {
                _curWebView.Insets = GetShowParam();
            }
        }
        /// <summary>
        /// 获取显示参数
        /// </summary>
        /// <returns></returns>
        private UniWebViewEdgeInsets GetShowParam()
        {
            if (FullScreen)
            {
                return UniWebViewEdgeInsets.Zero;
            }
            else
            {
                return ViewViewParam.GetShowParam();
            }        
        }

            #endregion
    }

    /// <summary>
    /// 网页数据
    /// </summary>
    public class WebData
    {
        private string _keyData="data";
        private string _keyFullScreen="FullScreen";
        private string _keyCleanCache = "CleanCache";
        private string _keyName="Name";
        private string _keyUrl ="Url";
        private List<TabData> _datas=new List<TabData>();
        private bool _fullScreen;
        private bool _cleanCache;

        public List<TabData> Datas
        {
            get
            {
                return _datas;
            }
        }

        public bool FullScreen
        {
            get
            {
                return _fullScreen;
            }
        }

        public bool CleanCache
        {
            get { return _cleanCache; }
        }

        public bool SingleData
        {
            get
            {
                return _datas.Count == 1;
            }
        }

        public WebData(object mes)
        {
            var dic = (Dictionary<string, object>)mes;
            List<object> datas;
            dic.TryGetValueWitheKey(out datas, _keyData);
            dic.TryGetValueWitheKey( out _fullScreen, _keyFullScreen);
            dic.TryGetValueWitheKey( out _cleanCache, _keyCleanCache);
            foreach (var item in datas)
            {
                var itemData = (Dictionary<string, object>) item;
                TabData tabData = new TabData()
                {
                    Name = itemData[_keyName].ToString(),
                    Data= itemData[_keyUrl].ToString()
                };
                _datas.Add(tabData);
            }
        }

    }
#endif
        }
