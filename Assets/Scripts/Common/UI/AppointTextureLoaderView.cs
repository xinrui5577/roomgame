using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 指定图片加载器
    /// </summary>
    public class AppointTextureLoaderView : YxView
    {
        public const string TextureResPath = "res/textures";
        /// <summary>
        /// 图片名称
        /// </summary>
        public string TextureName;

        public ELoadType LoadType;
        /// <summary>
        /// 图片
        /// </summary>
        public YxBaseTextureAdapter TextureAdapter;

        protected override void OnStart()
        {
            base.OnStart();
            //加载图片
            LoadTexture(LoadType);
        }

        public void LoadTexture(ELoadType loadType)
        {
            switch (loadType)
            {
                case ELoadType.Res:
                    var path = TextureResPath.CombinePath(TextureName.Trim());
                    var texture = ImageController.LoadLocalImage(path);//Resources.Load<Texture2D>(TextureName);
                    FinishedLoadTexture(texture, 0);
                    break;
                case ELoadType.Url:
                    Debug.LogError(TextureName);
//                    ImageController.LoadImageFromUrl(TextureName, FinishedLoadTexture);
                    AsyncImage.Instance.GetAsyncImage(TextureName, FinishedLoadTexture);
                    break;
                case ELoadType.Request:
                    ImageController.LoadImageFromServerConfig(TextureName, FinishedLoadTexture);
                    break;
            }
        }

        protected void FinishedLoadTexture(Texture texture,int code)
        {
            if (texture == null) return;
            TextureAdapter.SetTexture(texture);
        }

        public enum ELoadType
        {
            /// <summary>
            /// 资源
            /// </summary>
            Res,
            /// <summary>
            /// url
            /// </summary>
            Url,
            /// <summary>
            /// 请求获取url
            /// </summary>
            Request
        }
    }
}
