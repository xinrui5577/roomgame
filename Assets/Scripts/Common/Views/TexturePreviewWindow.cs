using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.Views
{
    public class TexturePreviewWindow : YxWindow
    {
        public YxBaseTextureAdapter MainTexture;

        public Vector2 ImageMaxSize = new Vector2(1000,700);

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (!(Data is string))
            {
                Close();
                return;
            }
            var path = Data.ToString();
            AsyncImage.Instance.GetAsyncImage(path, ((t2, code) =>
            {
                var width = t2.width;
                var height = t2.height;
                var maxW = ImageMaxSize.x;
                var maxH = ImageMaxSize.y;
                var changW = width > maxW;
                var changH = height > maxH;

                if (changW || changH)
                {
                    if (changW)
                    {
                        var rate = maxW / width;
                        MainTexture.Width = (int)maxW;
                        MainTexture.Height = (int)(height * rate);
                    }
                    if (changH)
                    {
                        var rate = maxH / height;
                        MainTexture.Width = (int)(width * rate);
                        MainTexture.Height = (int)maxH;
                    }
                }
                else
                {
                    MainTexture.Snap = true;
                }

                MainTexture.SetTexture(t2);
            }));
        }


        public override YxEUIType UIType
        {
            get { return MainTexture.UIType; }
        }
    }
}
