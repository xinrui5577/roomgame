using System.Collections;
using System.IO;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.ImageLoad
{

    /// <summary>
    /// 异步加载网上图片（单例类）
    /// </summary>
    public class AsyncImage : MonoSingleton<AsyncImage>
    {
        /// <summary>
        /// 默认男头像
        /// </summary>
        public Texture _defaultMan;
        /// <summary>
        /// 默认女头像
        /// </summary>
        public Texture _defaultWoman;
        public void SetAsyncImage(string url, UITexture texture,int sex)
        {
            //图片储存路径
            string path = Application.persistentDataPath + "/ImageCache/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            switch(sex)
            {
                case 0:
                    texture.mainTexture = _defaultWoman;
                    break;
                case 1:
                    texture.mainTexture = _defaultMan;
                    break;
                    
            }
            if (!File.Exists(path + url.GetHashCode()))
            {
                StartCoroutine(DownloadImage(url, texture, path));
            }
            else
            {
                StartCoroutine(LoadLocalImage(url, texture, path));
            }
        }

        private IEnumerator DownloadImage(string url, UITexture texture, string path)
        {
            WWW www = new WWW(url);
            yield return www;

            if (www.error != null)
            {
                yield break;
            }

            Texture2D image = www.texture;
            byte[] pngData = image.EncodeToPNG();
            File.WriteAllBytes(path + url.GetHashCode(), pngData);
            texture.mainTexture = image;
        }

        private IEnumerator LoadLocalImage(string url, UITexture texture, string path)
        {
            string filepath = "file:///" + path + url.GetHashCode();
            WWW www = new WWW(filepath);
            yield return www;
            if (www.error != null)
            {
                YxDebug.Log(www.error);
                yield break;
            }
            texture.mainTexture = www.texture;
        }
    }
}
