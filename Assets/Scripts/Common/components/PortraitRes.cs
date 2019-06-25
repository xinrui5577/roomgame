using UnityEngine;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.components
{
    public class PortraitRes : MonoBehaviour
    {
        private static PortraitRes _instance; 
        public string DefaultManHead = "";
        public string DefaultLadyHead = "";
        public UIAtlas PortraitAtlas;

        public static bool Init(string resName = "PortraitRes")
        {
            if (_instance != null) return true;
            var asset = ResourceManager.LoadAsset(resName);
            if (asset == null) return false;
            _instance = asset.GetComponent<PortraitRes>();
            return true;
        }

        public static Texture GetPortraitRes(string headName,int sex,string resName = "PortraitRes")
        {
            if (headName==null) return null;
            if(!Init())return null;
            if (_instance == null) return null;
            var atlas = _instance.PortraitAtlas;
            if (atlas == null) return null;
            var spd = atlas.GetSprite(headName) ?? atlas.GetSprite(sex == 1 ? _instance.DefaultManHead : _instance.DefaultLadyHead);
            var sx = spd.x;
            var sy = spd.y;
            var sw = spd.width;
            var sh = spd.height;
            var text = (Texture2D)atlas.texture;
            var f = new Texture2D(sw, sh) { filterMode = text.filterMode, wrapMode = text.wrapMode };
            f.SetPixels(0, 0, sw, sh, text.GetPixels(sx, text.height - sh - sy, sw, sh));
            f.LoadImage(f.EncodeToPNG()); 
            return f;
        }

        public static BetterList<string> HeadNames
        {
            get
            {
                if (!Init()) return new BetterList<string>(); 
                var atlas = _instance.PortraitAtlas;
                return atlas.GetListOfSprites();
            }
        }

        public static UIAtlas GetAtlas()
        {
            return !Init() ? null : _instance.PortraitAtlas;
        }

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="url">头像名称 或url</param>
        /// <param name="portrait">头像texture</param>
        /// <param name="sex">性别</param>
        public static void SetPortrait(string url, UIBasicSprite portrait, int sex)
        {
            if (portrait == null) return;
            if (portrait is UISprite)
            {
                var sp = portrait as UISprite;
                sp.spriteName = url;
                return;
            }
            if (!string.IsNullOrEmpty(url) && url.IndexOf("http://", System.StringComparison.Ordinal) > -1) //是url
            {
                AsyncImage.Instance.GetAsyncImage(url, texture =>
                {
                    portrait.mainTexture = texture;
                });
                return;
            }
            portrait.mainTexture = GetPortraitRes(url, sex);
        }
    }
}