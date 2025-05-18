using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 图片压缩
    /// </summary>
    public class CompressImg
    {
        /// <summary>
        /// 屏幕截图地址
        /// </summary>
        public string SShotImgpath { get; private set; }
        /// <summary>
        /// 截图缓存
        /// </summary>
        public Texture2D ScreenShot { get; private set; }
        /// <summary>
        /// 截图图片名字
        /// </summary>
        public const string ImageName = "screenName.png";

        public CompressImg()
        {
            SShotImgpath = Application.persistentDataPath + "/" + ImageName;
            if (Application.platform == RuntimePlatform.Android)
            {
                SShotImgpath = "file://" + SShotImgpath;
            }
        }

        public void DoScreenShot(MonoBehaviour mono, Rect rect, Action<string> onFinish = null)
        {
            //删除旧的截图
            if (File.Exists(SShotImgpath))
            {
                File.Delete(SShotImgpath);
            }
            mono.StartCoroutine(CaptureScreenshotJpg(rect, onFinish));
        }

        IEnumerator CaptureScreenshotJpg(Rect rect, Action<string> onFinish)
        {
            yield return new WaitForEndOfFrame();
            // 先创建一个的空纹理，大小可根据实现需要来设置      
            ScreenShot = new Texture2D((int)(rect.width), (int)(rect.height), TextureFormat.RGB24, false);
            // 读取屏幕像素信息并存储为纹理数据，      
            ScreenShot.ReadPixels(rect, 0, 0);
            ScreenShot.Apply();
            var encoder = new JPGEncoder(ScreenShot, 20);
            //质量1~100      
            encoder.doEncoding();
            while (!encoder.isDone)
                yield return null;
            File.WriteAllBytes(SShotImgpath, encoder.GetBytes());
            while (!File.Exists(SShotImgpath))
            {
                yield return new WaitForEndOfFrame();
            }
            if (onFinish != null)
            {
                onFinish(SShotImgpath);
            }
        }
    }
}
