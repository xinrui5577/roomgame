using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class HdCdsZone : MonoBehaviour
    {

        public float Interval;
        public Vector2 Size;

        private float _startPos;

        private List<UiCardGroup> _groups = new List<UiCardGroup>();

        private float GroupsLength;
        public void AddUiCdGroup(UiCardGroup cardGroup)
        {
            cardGroup.transform.parent = transform;
            cardGroup.transform.localScale = Vector3.one;

            _groups.Add(cardGroup);
        }

        //0 以左对齐，1 中心对齐
        public void Sort(int sortType)
        {
            _startPos = 0;
            GroupsLength = 0;   

            for (int i = 0; i < _groups.Count; i++)
            {
                UiCardGroup cardGroup = _groups[i];
                _startPos = _startPos + cardGroup.Width / 2;
                cardGroup.transform.localPosition = new Vector3(_startPos, 0);
                _startPos = _startPos + cardGroup.Width / 2;
                _startPos += Interval;

                GroupsLength += cardGroup.Width;
            }

            GroupsLength += (_groups.Count - 1)*Interval;

            float addDis = 0;
            if (sortType==0)
                addDis = -Size.x / 2;
            else if (sortType == 1)
                addDis = -GroupsLength/2;

            for (int i = 0; i < _groups.Count; i++)
            {
                UiCardGroup cardGroup = _groups[i];
                cardGroup.transform.localPosition += new Vector3(addDis, 0);
            }
        }

        public void Clear()
        {
            foreach (UiCardGroup uiCdGroup in _groups)
            {
                Destroy(uiCdGroup.gameObject);
            }
            _groups.Clear();
        }

        /// <summary>
        /// 立刻清除旧的信息
        /// </summary>
        public void ClearImmediate()
        {
            foreach (UiCardGroup uiCdGroup in _groups)
            {
                DestroyImmediate(uiCdGroup.gameObject);
            }
            _groups.Clear();
        }
    }
}
