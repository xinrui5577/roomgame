using System;
using System.Collections;
using System.IO;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Texas.ImgPress
{
    /// <summary>
    /// 图片压缩
    /// </summary>
    public class CompressImg : MonoBehaviour
    {
        /// <summary>
        /// 屏幕截图地址
        /// </summary>
        private string _sShotImgpath;

        void Awake()
        {
            _sShotImgpath = Application.persistentDataPath + "/" + "screenName.png";
        }

        public void DoScreenShot(Rect rect,Action<string> onFinish)
        {
            //删除旧的截图
            if (File.Exists(_sShotImgpath))
            {
                File.Delete(_sShotImgpath);
            }

            StartCoroutine(CaptureScreenshotJpg(rect, onFinish));
        }

        IEnumerator CaptureScreenshotJpg(Rect rect,Action<string> onFinish)
        {
            yield return new WaitForEndOfFrame();
            // 先创建一个的空纹理，大小可根据实现需要来设置      
            var screenShot = new Texture2D((int)(rect.width), (int)(rect.height), TextureFormat.RGB24, false);
            // 读取屏幕像素信息并存储为纹理数据，      
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();
            var encoder = new JPGEncoder(screenShot, 20);
            //质量1~100      
            encoder.doEncoding();
            while (!encoder.isDone)
                yield return null;

            File.WriteAllBytes(_sShotImgpath, encoder.GetBytes());

            while (!File.Exists(_sShotImgpath))
            {
                YxDebug.LogError("等待截图完成");
                yield return new WaitForEndOfFrame();
            }

            onFinish(_sShotImgpath);
            //Share.GetInstance().ShowShareImg(tittle, content, "", _sShotImgpath, ContentType.Image);
        }
    }
}
