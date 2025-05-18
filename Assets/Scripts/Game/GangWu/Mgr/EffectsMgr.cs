using System.Collections;
using Assets.Scripts.Game.GangWu.Tool;
using UnityEngine;

namespace Assets.Scripts.Game.GangWu.Mgr
{
    public class EffectsMgr : MonoBehaviour
    {
        /// <summary>
        /// 你赢了动画
        /// </summary>
        public GameObject YouWin;
        /// <summary>
        /// 粒子特效
        /// </summary>
        public GameObject[] ParticleEffects;
        /// <summary>
        /// 半透
        /// </summary>
        public GameObject Alpha;

        /// <summary>
        /// 关闭传入对象
        /// </summary>
        /// <param name="gob"></param>
        public void CloseGob(GameObject gob)
        {
            gob.SetActive(false);
        }
        /// <summary>
        /// 播放你赢了动画
        /// </summary>
        public void PlayYouWin()
        {
            YouWin.SetActive(true);
            TweenScale ts = YouWin.GetComponent<TweenScale>();
            ts.ResetToBeginning();
            ts.PlayForward();
        }

        /// <summary>
        /// 播放粒子特效
        /// </summary>
        /// <param name="gname"></param>
        /// <param name="delay"></param>
        /// <param name="withAlpha"></param>
        public void PlayParticleEffect(string gname,float delay,bool withAlpha)
        {
            GameObject gob = Tools.GobShowOnlyOne(ParticleEffects, gname);
            if (withAlpha)
            {
                Alpha.SetActive(true);
            }
            StartCoroutine(DelayClose(gob, delay));
        }

        /// <summary>
        /// 当牌型高于四条(ct_FollHouse)时,播放特效
        /// </summary>
        /// <param name="gname"></param>
        /// <returns></returns>
        public bool NeedShowEffect(string gname)
        {
            string s = "ct_" + gname;
            switch (s)
            {
                case "ct_Hulu":
                case "ct_SiTiao":
                case "ct_TongHuaShun":
                case "ct_HJTongHuaShun":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 特效延时结束
        /// </summary>
        /// <param name="gob"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator DelayClose(GameObject gob,float delay)
        {
            yield return new WaitForSeconds(delay);
            gob.SetActive(false);
            Alpha.SetActive(false);
        }
    }
}
