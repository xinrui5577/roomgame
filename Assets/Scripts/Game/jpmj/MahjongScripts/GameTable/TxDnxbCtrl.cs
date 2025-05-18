using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class TxDnxbCtrl : DnxbCtl
    {
        protected override IEnumerator CurreEnumerator(int newChair, int old)
        {
            SpriteRenderer oldSpr = DnxbBlink[(int)_hasDir[old]];
            oldSpr.color += new Color(0, 0, 0, 1);
            for (int i = 0; i < DnxbBlink.Length; i++)
                DnxbBlink[i].gameObject.SetActive(false);
            SpriteRenderer spr = DnxbBlink[(int)_hasDir[newChair]];
            spr.gameObject.SetActive(true);
            int sign = -1;
            while (true)
            {
                Color changeColor = new Color(0, 0, 0, 0.1f * sign);
                spr.color = spr.color + changeColor;
                if (spr.color.a <= 0.5)
                {
                    sign = 1;
                }
                if (spr.color.a >= 1)
                {
                    sign = -1;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public override void Reset()
        {
            foreach (SpriteRenderer spriteRenderer in DnxbBlink)
            {
                spriteRenderer.gameObject.SetActive(false);
            }
        }
    }
}