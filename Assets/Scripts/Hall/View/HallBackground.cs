using System;
using System.Collections;
using MMT;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 大厅背景系统
    /// </summary>
    public class HallBackground : MonoBehaviour
    {
        /// <summary>
        /// 移动端视频插件
        /// </summary>
        public MobileMovieTexture MobileBg;
        /// <summary>
        /// 默认背景
        /// </summary>
        public UITexture DefaultBg;
        /// <summary>
        /// 游戏列表背景
        /// </summary>
        public string GamelistTextureName = "hnhall";
        /// <summary>
        /// 房间列表背景
        /// </summary>
        public string RoomlistTextureName = "hnroom";
        /// <summary>
        /// 默认背景
        /// </summary>
        public string DefaultTextureName = "DefaultBG";
        /// <summary>
        /// 资源bundle名称
        /// </summary>
        public string ResBundleName = "HallBackground";

        // Use this for initialization
        void Awake ()
        {
            SetHideBg();
        }

        /// <summary>
        /// 切换背景
        /// </summary>
        public void Change(HallState state)
        {
            string textureName;
            switch (state)
            {
                case HallState.Gamelist:
                    textureName = GamelistTextureName;
                    break;
                case HallState.Roomlist:
                    textureName = RoomlistTextureName;
                    break;
                default:
                    textureName = DefaultTextureName;
                    break;
            }
            if (MobileBg!=null && Application.isMobilePlatform)
            {
                SetHideBg(2);
                StartCoroutine(ChangeBg(textureName)); 
                return;
            }
            var texture = ResourceManager.LoadAsset<Texture>(App.HallName, "HallBackground", textureName) ??
                          ResourceManager.LoadAsset<Texture>(App.HallName, "HallBackground", DefaultTextureName);
            SetHideBg(1);
            DefaultBg.mainTexture = texture;
#if UNITY_STANDALONE_WIN
            if (texture is MovieTexture)
            {
                var mt = texture as MovieTexture;
                mt.loop = true;
                mt.Play();
            }
#endif
        }

        /// <summary>
        /// 更换背景 //todo 待改
        /// </summary>
        /// <param name="bgName"></param>
        /// <param name="time"></param>
        private IEnumerator ChangeBg(string bgName, float time = 0.5f)
        {
            yield return new WaitForEndOfFrame();
            if (MobileBg == null) yield break;
            MobileBg.Stop();
            MobileBg.AbsolutePath = false;
            MobileBg.Path = bgName;
            MobileBg.CreateMovie();
            MobileBg.Play();
            MobileBg.GetComponent<UITexture>().enabled = false;
            yield return new WaitForSeconds(time);
            MobileBg.GetComponent<UITexture>().enabled = true;
        }

        private void SetHideBg(int hideState=0)
        {
            DefaultBg.gameObject.SetActive((hideState & 1) == 1);
            if (MobileBg != null) MobileBg.gameObject.SetActive((hideState & 2) == 2);
        }
    }
}
