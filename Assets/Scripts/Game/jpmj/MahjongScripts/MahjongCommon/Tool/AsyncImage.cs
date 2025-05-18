using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool
{
    public enum EnAsyncImgStatus
    {
        EnStart,
        EnFinish,
        EnError,
    }

    /// <summary>
    /// 异步加载网上图片（单例类）
    /// </summary>
    public class AsyncImage : MonoBehaviour
    {
        public static AsyncImage mInstance;

        public delegate void AsyncImgCall(EnAsyncImgStatus status, object obj);

        private void Awake()
        {
            mInstance = this;
        }

        protected Dictionary<string, Texture> TextureDic = new Dictionary<string, Texture>();

        /// <summary>
        /// 构建单例
        /// </summary>
        /// <returns></returns>
        public static AsyncImage GetInstance()
        {
            if (mInstance == null)
            {
                GameObject obj = new GameObject();
                mInstance = obj.AddComponent<AsyncImage>();
            }
            return mInstance;
        }

        public void SetAsyncImage(string url, AsyncImgCall call)
        {
            //图片储存路径
            string path = Application.persistentDataPath + "/ImageCache/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = path + url.GetHashCode();

            if (TextureDic.ContainsKey(fileName))
            {
                call(EnAsyncImgStatus.EnFinish, TextureDic[fileName]);
            }
            else
            {
                if (!File.Exists(path + url.GetHashCode()))
                {
                    call(EnAsyncImgStatus.EnStart, null);
                    StartCoroutine(DownloadImage(url, path, call));
                }
                else
                {
                    StartCoroutine(LoadLocalImage(url, path, call));
                }
            }
        }

        private IEnumerator DownloadImage(string url, string path, AsyncImgCall call)
        {
            WWW www = new WWW(url);
            yield return www;

            if (www.error != null)
            {
                //YxDebug.Log(www.error);
                call(EnAsyncImgStatus.EnError, www.error);
                yield break;
            }

            Texture2D image = www.texture;
            byte[] pngData = image.EncodeToPNG();

            File.WriteAllBytes(path + url.GetHashCode(), pngData);
            call(EnAsyncImgStatus.EnFinish, image);

            string key = path + url.GetHashCode();
            if (!TextureDic.ContainsKey(key))
            {
                TextureDic.Add(key, image);
            }
        }

        private IEnumerator LoadLocalImage(string url, string path, AsyncImgCall call)
        {
            string filepath = "file:///" + path + url.GetHashCode();

            WWW www = new WWW(filepath);
            yield return www;

            if (www.error != null)
            {
                call(EnAsyncImgStatus.EnError, www.error);
                yield break;
            }

            call(EnAsyncImgStatus.EnFinish, www.texture);
            string key = path + url.GetHashCode();
            if (!TextureDic.ContainsKey(key))
            {
                TextureDic.Add(key, www.texture);
            }
        }

        public void SetTextureWithAsyncImage(string url, RawImage texture, Texture define)
        {
            if (texture != null)
            {
                texture.texture = define;
                SetAsyncImage(url, (status, obj) =>
                    {
                        if (status == EnAsyncImgStatus.EnFinish)
                            texture.texture = (Texture) obj;
                    });
            }
        }

        public void SetTextureWithAsyncImage(string url, Image texture, Texture2D define)
        {

            texture.overrideSprite = Sprite.Create(define,
                                                      new Rect(0, 0, define.width, define.height),
                                                      new Vector2(0.5f, 0.5f));
            SetAsyncImage(url, (status, obj) =>
            {
                if (status == EnAsyncImgStatus.EnFinish)
                {

                    var texture2D = (Texture2D)obj;
                    texture.overrideSprite = Sprite.Create(texture2D,
                                                new Rect(0, 0, texture2D.width, texture2D.height),
                                                new Vector2(0.5f, 0.5f));
                }
            });
        }
    }
}
