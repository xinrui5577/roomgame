using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameTable;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts
{
    public class NewDnxbCtrl : DnxbCtl
    {
        protected readonly Vector3 NewDongPos = new Vector3(0.003f, 0, 0.331f);
        protected readonly Vector3 NewNanPos = new Vector3(-0.331f, 0, 0);
        protected readonly Vector3 NewXiPos = new Vector3(-0.004f, 0, -0.333f);
        protected readonly Vector3 NewBeiPos = new Vector3(0.337f, -0.009f, -0.017f);

        protected readonly Vector3 NewDongRotate = new Vector3(0, 0, 0);
        protected readonly Vector3 NewNanRotate = new Vector3(0, -90, 0);
        protected readonly Vector3 NewXiRotate = new Vector3(0, -180, 0);
        protected readonly Vector3 NewBeiRotate = new Vector3(0, -270, 0);

        protected Vector3[] PosArr;
        protected Vector3[] RotArr;

        void Start()
        {
            BaseColor = new Color(0.4f, 0.4f, 0.4f, 1);
            PosArr = new[] { NewDongPos, NewNanPos, NewXiPos, NewBeiPos };
            RotArr = new[] { NewDongRotate, NewNanRotate, NewXiRotate, NewBeiRotate };
        }

        protected void RePosition()
        {
            DnxbBlink[0].transform.eulerAngles = new Vector3(90, -90, 0);
            DnxbBlink[1].transform.eulerAngles = new Vector3(90, 0, 0);
            DnxbBlink[2].transform.eulerAngles = new Vector3(90, 90, 0);
            DnxbBlink[3].transform.eulerAngles = new Vector3(90, 180, 0);
        }

        public override void SetPlayerDnxb(EnDnxbDir dir)
        {
            RePosition();
            int offset = 0;

            EnDnxbDir[] tempArr = new[] { EnDnxbDir.Dong, EnDnxbDir.Nan, EnDnxbDir.Xi, EnDnxbDir.Bei };
            if (UtilData.CurrGamePalyerCnt == 2)
            {
                offset = (int)dir;
            }
            if (UtilData.CurrGamePalyerCnt == 3)
            {
                tempArr = new[] { EnDnxbDir.Dong, EnDnxbDir.Nan, EnDnxbDir.Bei };
            }

            int offsetIndex = (int)dir + offset;
            for (int i = 0; i < 4; i++)
            {
                if (UtilData.CurrGamePalyerCnt != 3)
                {
                    offsetIndex = offsetIndex % tempArr.Length;
                    DnxbBlink[(int)tempArr[offsetIndex]].transform.localPosition = PosArr[i];
                    DnxbBlink[(int)tempArr[offsetIndex]].transform.Rotate(RotArr[i], Space.World);
                    _hasDir[i] = tempArr[offsetIndex];
                    offsetIndex++;
                }
                else
                {
                    if (i != 2)
                    {
                        offsetIndex = (offsetIndex) % tempArr.Length;
                        DnxbBlink[(int)tempArr[offsetIndex]].transform.localPosition = PosArr[i];
                        DnxbBlink[(int)tempArr[offsetIndex]].transform.Rotate(RotArr[i], Space.World);
                        _hasDir[i] = tempArr[offsetIndex];
                        offsetIndex++;
                    }
                    else
                    {
                        DnxbBlink[2].transform.localPosition = PosArr[2];
                        DnxbBlink[2].transform.Rotate(RotArr[2], Space.World);
                        _hasDir[i] = EnDnxbDir.Xi;
                    }
                }
            }
        }

        public override void SetSaiziDir(int curr)
        {
            SetCurr(curr);
        }

        protected override IEnumerator CurreEnumerator(int newChair, int old)
        {
            foreach (SpriteRenderer sprite in DnxbBlink)
            {
                sprite.color = BaseColor;
            }
            SpriteRenderer spr = DnxbBlink[(int)_hasDir[newChair]];
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
    }
}
