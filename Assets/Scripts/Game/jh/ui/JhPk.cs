using System.Collections;
using UnityEngine;
using Assets.Scripts.Game.jh.Public;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhPk : MonoBehaviour
    {

        public GameObject Pk1;
        public GameObject Pk2;

        public GameObject Animation;

        public GameObject Gzyz;
        public UILabel GzyzName;

        protected Vector3 Pos1;
        protected Vector3 Pos2;

        protected GameObject Pk1Obj;
        protected GameObject Pk2Obj;

        protected int ObjDepth;

        protected Coroutine PkAnm;
        public Vector3 GetPk1Pos()
        {
            return Pk1.transform.position;
        }

        public Vector3 GetPk2Pos()
        {
            return Pk2.transform.position;
        }


        public void OnPk(GameObject pk1Obj,GameObject pk2Obj,bool isWin,EventDelegate delDelegate)
        {
            Pos1 = pk1Obj.transform.position;
            Pos2 = pk2Obj.transform.position;
            Pk1Obj = pk1Obj;
            Pk2Obj = pk2Obj;


            JhFunc.AddDepthForWidget(pk1Obj.GetComponent<UIWidget>(),30);
            JhFunc.AddDepthForWidget(pk2Obj.GetComponent<UIWidget>(), 30);

            PkAnm = StartCoroutine(PkAniamtion(isWin, delDelegate));
        }

        private IEnumerator PkAniamtion(bool isWin, EventDelegate delDelegate)
        {
            yield return MoveTo();
            yield return ShowResult(isWin);
            ResetAnimation();
            if (delDelegate != null)
            {
                delDelegate.Execute();
            }
            PkAnm = null;
        }

        public void ResetAnimation()
        {
            Pk1Obj.transform.position = Pos1;
            Pk2Obj.transform.position = Pos2;
            JhFunc.AddDepthForWidget(Pk1Obj.GetComponent<UIWidget>(), -30);
            JhFunc.AddDepthForWidget(Pk2Obj.GetComponent<UIWidget>(), -30);
            Pk1.SetActive(false);
            Pk2.SetActive(false);
            Animation.SetActive(false);
            Gzyz.SetActive(false);
        }

        public void Reset()
        {
            Pk1.SetActive(false);
            Pk2.SetActive(false);
            Animation.SetActive(false);
            Gzyz.SetActive(false);
            if (PkAnm != null)
            {
                StopCoroutine(PkAnm);
            }
        }

        private IEnumerator ShowResult(bool isWin)
        {
            if (isWin)
            {
                Pk1.SetActive(true);
            }
            else
            {
                Pk2.SetActive(true);
            }
            yield return new WaitForSeconds(1.5f);

        }

        private IEnumerator MoveTo()
        {
            float val = 0;
            float bTime = Time.time;
            var fpos1 = Pk1Obj.transform.position;
            var tpos1 = Pk1.transform.position;
            var fpos2 = Pk2Obj.transform.position;
            var tpos2 = Pk2.transform.position;

            while (val < 1)
            {
                val = Time.time - bTime;
                float smoothval = val ;
                Pk1Obj.transform.position = Vector3.Lerp(fpos1, tpos1, smoothval);
                Pk2Obj.transform.position = Vector3.Lerp(fpos2, tpos2, smoothval);
                yield return 2;
            }

            Animation.SetActive(true);
        }

        public void OnGzyz(string uname, EventDelegate delDelegate)
        {
            StartCoroutine(GzyzAniamtion(uname, delDelegate));
        }

        private IEnumerator GzyzAniamtion(string uname, EventDelegate delDelegate)
        {
            Gzyz.SetActive(true);
            string tt = "玩家 [D7E556FF]" + uname + "[-] 金币不足，将[9C1F0EFF]孤注一掷[-]";
            GzyzName.text = tt;
            yield return  new WaitForSeconds(1);
            Animation.SetActive(true);
            yield return new WaitForSeconds(1);
            Animation.SetActive(false);
            Gzyz.SetActive(false);
            if (delDelegate != null)
            {
                delDelegate.Execute();
            }

        }

        public void On20TrunBiPai(EventDelegate delDelegate,string msg)
        {
            StartCoroutine(TrunBiPaiAniamtion(delDelegate, msg));
        }

        private IEnumerator TrunBiPaiAniamtion(EventDelegate delDelegate, string msg)
        {
            Gzyz.SetActive(true);
//            string tt = "20轮,全场比牌！ ";
            GzyzName.text = msg;
            yield return new WaitForSeconds(1);
            Animation.SetActive(true);
            yield return new WaitForSeconds(1);
            Animation.SetActive(false);
            Gzyz.SetActive(false);
            if (delDelegate != null)
            {
                delDelegate.Execute();
            }
        }
    }
}
