using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// ÔÞÖú´°¿Ú
    /// </summary>
    public class SupportWindow : YxNguiWindow
    {
        public UITexture Background;
        public UILabel ContentLabel;

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var dict = Data as Dictionary<string, object>;
            if (dict == null) return;
            if (dict.ContainsKey("infoImage"))
            {
                var image = dict["infoImage"];
                if (image != null)
                {
                    AsyncImage.Instance.GetAsyncImage(image.ToString(), FreshBackground);
                } 
            }
            if (dict.ContainsKey("infoText"))
            {
                var text = dict["infoText"];
                if (ContentLabel != null)
                {
                    ContentLabel.text = text == null ? "" : text.ToString();
                }
            }
        }

        private void FreshBackground(Texture2D texture,int hashCode)
        {
            if (texture == null) return;
            if (Background == null) return;
            Background.mainTexture = texture;
            Background.MakePixelPerfect();
        }
    }
}
