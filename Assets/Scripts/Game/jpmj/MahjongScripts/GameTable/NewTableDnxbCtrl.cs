using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class NewTableDnxbCtrl : DnxbCtl
    {
        protected readonly Vector3 _newDongPos = new Vector3(0, -0.0546f,0);
        protected readonly Vector3 _newNanPos = new Vector3(0.052f, -0.0026f,0);
        protected readonly Vector3 _newXiPos = new Vector3(0, 0.0494f,0);
        protected readonly Vector3 _newBeiPos = new Vector3(-0.052f, -0.0026f,0);

        protected readonly Vector3 _newDongRotate = new Vector3(0,0,0);
        protected readonly Vector3 _newNanRotate = new Vector3(0,-90,0);
        protected readonly Vector3 _newXiRotate = new Vector3(0,-180,0);
        protected readonly Vector3 _newBeiRotate = new Vector3(0,-270,0);

        protected Vector3[] PosArr;
        protected Vector3[] RotArr;

        void Start()
        {
            BaseColor=new Color(0.4f,0.4f,0.4f,1);
            PosArr = new[] { _newDongPos, _newNanPos, _newXiPos, _newBeiPos };
            RotArr = new[] { _newDongRotate, _newNanRotate, _newXiRotate, _newBeiRotate };
        }

        protected void RePosition()
        {
            DnxbBlink[0].transform.eulerAngles = new Vector3(90, 0, 180);
            DnxbBlink[1].transform.eulerAngles = new Vector3(90, 0, 90);
            DnxbBlink[2].transform.eulerAngles = new Vector3(90, 0, 0);
            DnxbBlink[3].transform.eulerAngles = new Vector3(90, 0, 270);
        }

        public override void SetPlayerDnxb(EnDnxbDir dir)
        {
            switch ((int)dir)
            {
                case 0:
                    _hasDir = new[] { EnDnxbDir.Dong, EnDnxbDir.Nan, EnDnxbDir.Xi, EnDnxbDir.Bei };
                    break;
                case 1:
                    RePosition();
                    DnxbBlink[0].transform.localPosition = PosArr[3];
                    DnxbBlink[1].transform.localPosition = PosArr[0];
                    DnxbBlink[2].transform.localPosition = PosArr[2];
                    DnxbBlink[3].transform.localPosition = PosArr[1];
                    DnxbBlink[0].transform.Rotate(RotArr[3], Space.World);
                    DnxbBlink[1].transform.Rotate(RotArr[0], Space.World);
                    DnxbBlink[2].transform.Rotate(RotArr[2], Space.World);
                    DnxbBlink[3].transform.Rotate(RotArr[1], Space.World);
                    _hasDir = new[] { EnDnxbDir.Nan, EnDnxbDir.Bei,EnDnxbDir.Xi, EnDnxbDir.Dong };
                    break;
                case 2:
                    RePosition();
                    DnxbBlink[0].transform.localPosition = PosArr[1];
                    DnxbBlink[1].transform.localPosition = PosArr[3];
                    DnxbBlink[2].transform.localPosition = PosArr[2];
                    DnxbBlink[3].transform.localPosition = PosArr[0];
                    DnxbBlink[0].transform.Rotate(RotArr[1], Space.World);
                    DnxbBlink[1].transform.Rotate(RotArr[3], Space.World);
                    DnxbBlink[2].transform.Rotate(RotArr[2], Space.World);
                    DnxbBlink[3].transform.Rotate(RotArr[0], Space.World);
                    _hasDir = new[] { EnDnxbDir.Bei, EnDnxbDir.Dong, EnDnxbDir.Xi, EnDnxbDir.Nan };
                    break;
            }
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
