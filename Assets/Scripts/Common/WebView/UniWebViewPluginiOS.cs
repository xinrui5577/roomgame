using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Common.WebView
{
#if UNITY_IOS && !UNITY_EDITOR
    
    public class UniWebViewPlugin {

    [DllImport("__Internal")]
    private static extern void _UniWebViewInit(string name, int top, int left, int bottom, int right);
    [DllImport("__Internal")]
    private static extern void _UniWebViewChangeInsets(string name, int top, int left, int bottom, int right);
    [DllImport("__Internal")]
    private static extern void _UniWebViewLoad(string name, string url);
    [DllImport("__Internal")]
    private static extern void _UniWebViewLoadHTMLString(string name, string htmlString, string baseUrl);
    [DllImport("__Internal")]
    private static extern void _UniWebViewReload(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewStop(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewShow(string name, bool fade, int direction, float duration);
    [DllImport("__Internal")]
    private static extern void _UniWebViewEvaluatingJavaScript(string name, string javascript, bool callback);
    [DllImport("__Internal")]
    private static extern void _UniWebViewHide(string name, bool fade, int direction, float duration);
    [DllImport("__Internal")]
    private static extern void _UniWebViewCleanCache(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewCleanCookie(string name, string key);
    [DllImport("__Internal")]
    private static extern void _UniWebViewDestroy(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewTransparentBackground(string name, bool transparent);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetBackgroundColor(string name, float r, float g, float b, float a);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetSpinnerShowWhenLoading(string name, bool show);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetSpinnerText(string name, string text);
    [DllImport("__Internal")]
    private static extern void _UniWebViewShowToolBar(string name, bool animate);
    [DllImport("__Internal")]
    private static extern void _UniWebViewHideToolBar(string name, bool animate);
    [DllImport("__Internal")]
    private static extern bool _UniWebViewCanGoBack(string name);
    [DllImport("__Internal")]
    private static extern bool _UniWebViewCanGoForward(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewGoBack(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewGoForward(string name);
    [DllImport("__Internal")]
    private static extern string _UniWebViewGetCurrentUrl(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetBounces(string name, bool bounces);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetZoomEnable(string name, bool enable);
    [DllImport("__Internal")]
    private static extern void _UniWebViewAddUrlScheme(string name, string scheme);
    [DllImport("__Internal")]
    private static extern void _UniWebViewRemoveUrlScheme(string name, string scheme);
    [DllImport("__Internal")]
    private static extern int _UniWebViewScreenHeight();
    [DllImport("__Internal")]
    private static extern int _UniWebViewScreenWidth();
    [DllImport("__Internal")]
    private static extern int _UniWebViewScreenScale();
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetUserAgent(string userAgent);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetDoneButtonText(string text);
    [DllImport("__Internal")]
    private static extern string _UniWebViewGetUserAgent(string name);
    [DllImport("__Internal")]
    private static extern float _UniWebViewGetAlpha(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetAlpha(string name, float alpha);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetHeaderField(string name, string key, string value);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetVerticalScrollBarShow(string name, bool show);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetHorizontalScrollBarShow(string name, bool show);
    [DllImport("__Internal")]
    private static extern bool _UniWebViewGetOpenLinksInExternalBrowser(string name);
    [DllImport("__Internal")]
    private static extern void _UniWebViewSetOpenLinksInExternalBrowser(string name, bool value);

    public static void Init(string name, int top, int left, int bottom, int right) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewInit(name, top, left, bottom, right);
        }
    }

    public static void ChangeInsets(string name, int top, int left, int bottom, int right) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewChangeInsets(name, top, left, bottom, right);
        }
    }

    public static void Load(string name, string url) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewLoad(name, url);
        }
    }

    public static void LoadHTMLString(string name, string htmlString, string baseUrl) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewLoadHTMLString(name,htmlString,baseUrl);
        }
    }

    public static void Reload(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewReload(name);
        }
    }

    public static void Stop(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewStop(name);
        }
    }

    public static void Show(string name, bool fade, int direction, float duration) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewShow(name, fade, direction, duration);
        }
    }

    public static void EvaluatingJavaScript(string name, string javaScript, bool callback = true) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewEvaluatingJavaScript(name, javaScript, callback);
        }
    }

    public static void AddJavaScript(string name, string javaScript) {
        EvaluatingJavaScript(name, javaScript, false);
    }

    public static void Hide(string name, bool fade, int direction, float duration) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewHide(name, fade, direction, duration);
        }
    }

    public static void CleanCache(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewCleanCache(name);
        }
    }

    public static void CleanCookie(string name, string key) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewCleanCookie(name, key);
        }
    }

    public static void Destroy(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewDestroy(name);
        }
    }

    public static void TransparentBackground(string name, bool transparent) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewTransparentBackground(name, transparent);
        }
    }

    public static void SetBackgroundColor(string name, float r, float g, float b, float a) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetBackgroundColor(name, r, g, b, a);
        }
    }

    public static void SetSpinnerShowWhenLoading(string name, bool show) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetSpinnerShowWhenLoading(name, show);
        }
    }

    public static void SetSpinnerText(string name, string text) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetSpinnerText(name, text);
        }
    }

    public static void ShowToolBar(string name, bool animate) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewShowToolBar(name, animate);
        }
    }

    public static void HideToolBar(string name, bool animate) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewHideToolBar(name, animate);
        }
    }

    public static bool CanGoBack(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewCanGoBack(name);
        }
        return false;
    }
    
    public static bool CanGoForward(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewCanGoForward(name);
        }
        return false;
    }

    public static void GoBack(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewGoBack(name);
        }
    }

    public static void GoForward(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewGoForward(name);
        }
    }

    public static string GetCurrentUrl(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewGetCurrentUrl(name);
        }
        return "";
    }

    public static void SetBounces(string name, bool bounces) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetBounces(name, bounces);
        }
    }

    public static void SetZoomEnable(string name, bool enable) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetZoomEnable(name, enable);
        }
    }

    public static void AddUrlScheme(string name, string scheme) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewAddUrlScheme(name, scheme);
        }
    }

    public static void RemoveUrlScheme(string name, string scheme) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewRemoveUrlScheme(name, scheme);
        }
    }

    public static int ScreenHeight() {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewScreenHeight();
        }
        return 0;
    }

    public static int ScreenWidth() {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewScreenWidth();
        }
        return 0;
    }

    public static int ScreenScale() {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewScreenScale();
        }
        return 1;
    }

    public static void SetUserAgent(string userAgent) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetUserAgent(userAgent);
        }
    }
    
    public static void SetDoneButtonText(string text) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetDoneButtonText(text);
        }
    }

    public static string GetUserAgent(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewGetUserAgent(name);
        }
        return "";
    }

    public static float GetAlpha(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewGetAlpha(name);
        }
        return 0.0f;
    }

    public static void SetAlpha(string name, float alpha) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetAlpha(name, alpha);
        }
    }

    public static void SetHeaderField(string name, string key, string value) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetHeaderField(name, key, value);
        }
    }
    
    public static void SetVerticalScrollBarShow(string name, bool show) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetVerticalScrollBarShow(name, show);
        }
    }
    
    public static void SetHorizontalScrollBarShow(string name, bool show) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetHorizontalScrollBarShow(name, show);
        }
    }
    
    public static bool GetOpenLinksInExternalBrowser(string name) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return _UniWebViewGetOpenLinksInExternalBrowser(name);
        }
        return false;
    }

    public static void SetOpenLinksInExternalBrowser(string name, bool value) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _UniWebViewSetOpenLinksInExternalBrowser(name, value);
        }
    }
}
#endif
}
