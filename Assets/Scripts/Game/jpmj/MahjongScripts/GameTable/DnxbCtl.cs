using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{ 
    //东南西北
    public enum EnDnxbDir
    {
        Dong,
        Nan,
        Xi,
        Bei
    }

    public class DnxbCtl : MonoBehaviour
    {
        public SpriteRenderer[] DnxbBlink;

        protected readonly Vector3 _dong = new Vector3(0, 0, 0);
        protected readonly Vector3 _nan = new Vector3(0, 90, 0);
        protected readonly Vector3 _xi = new Vector3(0, 180, 0);
        protected readonly Vector3 _bet = new Vector3(0, -90, 0);


        protected EnDnxbDir _dir;
        protected EnDnxbDir[] _hasDir = new EnDnxbDir[UtilDef.GamePlayerCnt];
        protected int _curr;
        protected Coroutine _currCoroutine;

        protected Color BaseColor = new Color(0.5f, 0.5f, 0.5f, 1);
        public virtual void SetPlayerDnxb(EnDnxbDir dir)
        {
            Vector3[] rotationArray = { _dong, _nan, _xi, _bet };
            transform.localRotation = Quaternion.Euler(rotationArray[(int)dir]);
            _dir = dir;
            ResetDirArray();
        }

        public virtual void SetSaiziDir(int curr)
        {
            SpriteRenderer spr = DnxbBlink[(int)_hasDir[curr]];
            spr.gameObject.SetActive(true);
        }

        public virtual void SetCurr(int newCurr)
        {
            if (_currCoroutine != null)
            {
                StopCoroutine(_currCoroutine);
            }
            _currCoroutine = StartCoroutine(CurreEnumerator(newCurr, _curr));
            _curr = newCurr;
        }

        protected virtual IEnumerator CurreEnumerator(int newChair, int old)
        {
            DnxbBlink[old].color = BaseColor;
            DnxbBlink[newChair].color = BaseColor;
            SpriteRenderer spr = DnxbBlink[(int)_hasDir[newChair]];
            foreach (var sprder in DnxbBlink)
            {
                if (sprder != null && sprder.gameObject != null)
                    sprder.gameObject.SetActive(false);
            }
            if (spr != null && spr.gameObject != null)
                spr.gameObject.SetActive(true);
            int sign = -1;
            while (true)
            {
                Color changeColor = new Color(0.025f * sign, 0.025f * sign, 0.025f * sign, 0);
                spr.color = spr.color + changeColor;
                if (spr.color.r <= 0.5)
                {
                    sign = 1;
                }
                if (spr.color.r >= 1)
                {
                    sign = -1;
                }

                yield return new WaitForSeconds(0.025f);
            }
        }

        public virtual void Reset()
        {
            foreach (SpriteRenderer spriteRenderer in DnxbBlink)
            {
                spriteRenderer.color = BaseColor;
            }
            if (_currCoroutine != null)
            {
                StopCoroutine(_currCoroutine);
            }
        }

        protected void ResetDirArray()
        {
            for (int i = 0; i < _hasDir.Length; i++)
            {
                int temp = ((int)_dir + i) % UtilDef.GamePlayerCnt;

                _hasDir[i] = (EnDnxbDir)temp;
            }
        }
    }
}