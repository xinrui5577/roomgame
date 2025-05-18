using UnityEngine;

namespace Assets.Scripts.Game.sss
{
    public class HandCardType : MonoBehaviour
    {

        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private GameObject[] _marks = new GameObject[3];


        /// <summary>
        /// 显示每行的牌型
        /// </summary>
        /// <param name="line">第几行(0,1,2)</param>
        /// <param name="typeName"></param>
        public void ShowType(int line, string typeName)
        {
            for (int i = 0; i < _marks.Length; i++)
            {
                if (i == line)
                {
                    _marks[i].SetActive(true);
                    UISprite spr = _marks[i].GetComponent<UISprite>();
                    spr.spriteName = typeName;
                    spr.MakePixelPerfect();
                }
                else
                {
                    _marks[i].SetActive(false);
                }
            }
        }

        public void HideType(int line)
        {
            _marks[line].SetActive(false);
        }

        public void Reset()
        {
            foreach (GameObject mark in _marks)
            {
                mark.SetActive(false);
            }
        }
    }
}