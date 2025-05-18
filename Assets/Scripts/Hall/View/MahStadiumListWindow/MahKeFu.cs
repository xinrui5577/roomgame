using UnityEngine;
using YxFramwork.Manager;
using YxFramwork.Tool.YxAndroidJNI;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahKeFu : MonoBehaviour
    {
        public void OpenKeFu(string windowName, string url)
        {
            YxWindowManager.OpenWindow(windowName); 
            return;
#if UNITY_IOS
#endif
#if UNITY_ANDROID 
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
#endif
        }
    }
}
