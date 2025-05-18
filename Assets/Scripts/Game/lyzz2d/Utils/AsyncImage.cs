using System.Collections;
using System.IO;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils
{
    /// <summary>
    ///     异步加载网上图片（单例类）
    /// </summary>
    public class AsyncImage : MonoSingleton<AsyncImage>
    {
        /// <summary>
        ///     默认男头像
        /// </summary>
        public Texture _defaultMan;

        /// <summary>
        ///     默认女头像
        /// </summary>
        public Texture _defaultWoman;

        public void SetAsyncImage(string url, UITexture texture, int sex)
        {
            //图片储存路径
            var path = Application.persistentDataPath + "/ImageCache/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            switch (sex)
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
            var www = new WWW(url);
            yield return www;

            if (www.error != null)
            {
                yield break;
            }

            var image = www.texture;
            var pngData = image.EncodeToPNG();
            File.WriteAllBytes(path + url.GetHashCode(), pngData);
            texture.mainTexture = image;
        }

        private IEnumerator LoadLocalImage(string url, UITexture texture, string path)
        {
            var filepath = "file:///" + path + url.GetHashCode();
            var www = new WWW(filepath);
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