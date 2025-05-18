using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Tbs
{
    public class CheckBankerMgr : MonoBehaviour
    {
        /// <summary>
        /// 庄家姓名
        /// </summary>
        public UILabel BankerName;
        /// <summary>
        /// 得到锅内金币
        /// </summary>
        public UILabel GuoGold;
        /// <summary>
        /// 得到出血金币
        /// </summary>
        public UILabel BooldGold;
        /// <summary>
        /// 内容父节点
        /// </summary>
        public GameObject Content;
        /// <summary>
        /// 下一步cd
        /// </summary>
        private int _cd;

        public void OpenPanel(string bname,int guog,int booldg,int cd)
        {
            _cd = cd;
            BankerName.text = bname;
            GuoGold.text = string.Format("+{0}", guog >= 0 ? guog:0);
            BooldGold.text = string.Format("+{0}", booldg);
            Content.SetActive(true);
            int timeout = (_cd >> 1) < 5 ? 5 : (_cd >> 1);
            StartCoroutine(AutoClosePanel(timeout));
        }

        public void ClosePanel()
        {
            Content.SetActive(false);
            StopCoroutine(AutoClosePanel(_cd >> 1));
        }

        public IEnumerator AutoClosePanel(float time)
        {
            yield return new WaitForSeconds(time);
            ClosePanel();
        }


    }
}
