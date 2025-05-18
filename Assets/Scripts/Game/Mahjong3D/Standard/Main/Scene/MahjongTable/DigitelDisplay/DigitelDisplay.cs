using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DigitelDisplay : MonoBehaviour
    {
        public MeshRenderer[] MeshRenderers;
        public Texture[] Numbers;

        public int Number
        {
            get; private set;
        }

        public void SetMahjongCounter(int value)
        {
            Number = value;
            if (value < 0)
            {
                Number = -1;
                SetDigitelDisplay(0, 0);
                SetDigitelDisplay(1, 0);
                SetDigitelDisplay(2, 0);
                return;
            }
            int decade = FindNum(value, 2);
            SetDigitelDisplay(2, value >= 100 ? 1 : 0);//百位
            SetDigitelDisplay(1, FindNum(value, 2));//十位
            SetDigitelDisplay(0, value % 10);//个位
        }

        public void SetTimer(int value)
        {
            Number = value;
            if (value > 99 || value < 0)
            {
                Number = -1;
                SetDigitelDisplay(0, 0);
                SetDigitelDisplay(1, 0);
                return;
            }
            SetDigitelDisplay(0, FindNum(value, 2));//十位
            SetDigitelDisplay(1, value % 10);//个位
        }

        /// 求num在n位上的数字,取个位,取十位
        /// </summary>
        /// <param name="num">正整数</param>
        /// <param name="n">所求数字位置(个位 1,十位 2 依此类推)</param>
        public int FindNum(int num, int n)
        {
            int power = (int)System.Math.Pow(10, n);
            return (num - num / power * power) * 10 / power;
        }

        /// <summary>
        /// 设置数字
        /// </summary>
        /// <param name="places">位</param>
        /// <param name="num">数</param>
        private void SetDigitelDisplay(int places, int num)
        {
            var texture = Numbers[num];
            if (!texture.ExIsNullOjbect())
            {
                MeshRenderers[places].material.mainTexture = texture;
            }
        }
    }
}