using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Assets.Scripts.Game.brnn3d
{
    public class DownUILeftUIOn_Off : MonoBehaviour
    {
        public Transform Kaitf;
        public Transform Guantf;
         
        public void ShowKaiBtn()
        {
            if (Guantf.gameObject.activeSelf)
                Guantf.gameObject.SetActive(false);
            StartCoroutine("ToShowKaiBtn", 0.5f);
        }

        IEnumerator ToShowKaiBtn(float s)
        {
            yield return new WaitForSeconds(s);
            if (!Kaitf.gameObject.activeSelf)
                Kaitf.gameObject.SetActive(true);
            Kaitf.DOLocalMoveX(-200, 0.3f);
        }

        public void ShowGuanBtn()
        {
            Kaitf.localPosition = new Vector3(-297f, 78.3f, 0);
            if (Kaitf.gameObject.activeSelf)
                Kaitf.gameObject.SetActive(false);
            if (!Guantf.gameObject.activeSelf)
                Guantf.gameObject.SetActive(true);
        }

    }
}

