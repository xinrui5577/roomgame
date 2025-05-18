using System;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool
{
    public class U3DNum : MonoBehaviour {

        public Material[] Materials;
        public Texture[] Numbers;

        //几位
        public int PlaceCnt;

        private int _maxNum;
        private int _minNum;
        private int _num;

        private int[] _viewNum;
        public int Num
        {
            get { return _num; }
            set
            {
                if (value>_maxNum&&value<_minNum)
                    return;

                _num = value;
                GetViewNum();
                Refresh();
            }
        }

        // Use this for initialization
        void Start ()
        {
            _viewNum = new int[PlaceCnt];
            _maxNum = (int)Math.Pow(10, PlaceCnt) - 1;
            _minNum = 0;
        }

        void GetViewNum()
        {
            _viewNum = new int[PlaceCnt];
            int numTemp = _num;
            int loop = PlaceCnt-1;
            while (numTemp > 0)
            {
                int temp = (int)Math.Pow(10, loop);
                int cnt = numTemp/temp;
                _viewNum[loop] = cnt;
                numTemp -= temp*cnt;
                loop--;
            }
        }

        void Refresh()
        {
            for (int i = 0; i < PlaceCnt; i++)
            {
                Materials[i].mainTexture = Numbers[_viewNum[i]];
            }
        }
    
    }
}
