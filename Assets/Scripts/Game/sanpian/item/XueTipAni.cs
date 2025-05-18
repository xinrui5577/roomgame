using System.Collections;
using Assets.Scripts.Game.sanpian.server;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.item
{
    public class XueTipAni : MonoBehaviour
    {
        public GameObject Xue;

        public GameObject BuXue;

        private TweenAlpha tween;

        [HideInInspector]
        bool XueBool;

        public void StartAni(bool Xue)
        {
            XueBool = Xue;
            tween = gameObject.GetComponent<TweenAlpha>();
            transform.SetParent(App.GetGameManager<SanPianGameManager>().UIButtonCtrl.OpBt.transform.parent);
            transform.localScale=new Vector3(0.1f,1,1);
            StartCoroutine(XueAni());
        }

        IEnumerator XueAni()
        {
            iTween.ScaleTo(gameObject,Vector3.one,1f);
            yield return new WaitForSeconds(0.4f);
            if (XueBool)
            {
                Xue.SetActive(true);
            }
            else
            {
                BuXue.SetActive(true);
            }
            yield return new WaitForSeconds(0.6f);
            tween.Play();
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }
    }
}
