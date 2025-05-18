using System.Collections;
using UnityEngine;

namespace Assets.Scripts.MobileMovieTexture
{
    /// <inheritdoc />
    /// <summary>
    /// 视频图片助手
    /// </summary>
    public class MovieTextureHelper : MonoBehaviour
    {
        void Start()
        {
//#if UNITY_STANDALONE_WIN
//            if (texture is MovieTexture)
//            {
//                var mt = texture as MovieTexture;
//                mt.loop = true;
//                mt.Play();
//            }
//#else
            var mmt = GetComponent<MMT.MobileMovieTexture>();
            StartCoroutine(ChangeBg(mmt));
        }

        /// <summary>
        /// 更换背景 //todo 待改
        /// </summary>
        /// <param name="mmt"></param>
        /// <param name="time"></param>
        private IEnumerator ChangeBg(MMT.MobileMovieTexture mmt,float time = 0.2f)
        {
            if (mmt == null) yield break;
            mmt.Stop();
            mmt.AbsolutePath = false;
            mmt.CreateMovie();
            mmt.Play();
            mmt.GetComponent<UITexture>().enabled = false;
            yield return new WaitForSeconds(time);
            mmt.GetComponent<UITexture>().enabled = true;
        }
    }
}
