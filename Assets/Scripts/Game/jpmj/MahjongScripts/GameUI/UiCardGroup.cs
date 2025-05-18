using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    /// <summary>
    /// 用于创造 单牌，杠牌，碰牌等牌型组
    /// </summary>
    public class UiCardGroup : MonoBehaviour
    {
        [HideInInspector]
        public List<GameObject> ObjList = new List<GameObject>();
        private Vector2 _mjSize;

        public static UiCardGroup create(GameObject[] objList, Vector2 MjSize, Sprite bg, EnGroupType cpgType = EnGroupType.None)
        {



            GameObject obj = new GameObject();
            UiCardGroup ret = obj.AddComponent<UiCardGroup>();
            ret._mjSize = MjSize;
            ret.AddObj(objList);
           
            //根据暗杠的形式摆牌
            if (cpgType == EnGroupType.AnGang)
            {
                ret.SortAnGangGroup();
                return ret;
            }

            ret.Sort();

            if (bg!=null)
            {
                Image imgBg = obj.AddComponent<Image>();
                imgBg.sprite = bg;
                imgBg.type = Image.Type.Sliced;
                //设置底图大小 要比大
                RectTransform rtf = obj.GetComponent<RectTransform>();
                rtf.sizeDelta = new Vector2(ret.ObjList.Count * ret._mjSize.x + 20, ret._mjSize.y + 20);

                obj.AddComponent<Button>();
            }

            return ret;
        }

        public void AddObj(GameObject[] objList, EnGroupType cpgType = EnGroupType.None)
        {
            for (int i = 0; i < objList.Length; i++)
            {
                objList[i].transform.parent = transform;
                ObjList.Add(objList[i]);
            }
        }

        public void SetClickCallFunc(UnityAction onClickCallFunc)
        {
            Button btn = gameObject.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.AddListener(onClickCallFunc);
            }
        }

        public void Sort()
        {
            float width = _mjSize.x * ObjList.Count;
            float posX = -width/2;
            for (int i = 0; i < ObjList.Count; i++)
            {
                posX += _mjSize.x * 0.5f;
                Vector3 pos = new Vector3( posX,0,0 );
                ObjList[i].transform.localPosition = pos;
                posX += _mjSize.x * 0.5f;
            }
        }

        //排序暗杠摆设
        private void SortAnGangGroup()
        {
            var len = ObjList.Count-1;

            float width = _mjSize.x * len;
            float posX = -width / 2;
            for (int i = 0; i < len; i++)
            {
                posX += _mjSize.x * 0.5f;
                Vector3 pos = new Vector3(posX, 0, 0);
                ObjList[i].transform.localPosition = pos;
                posX += _mjSize.x * 0.5f;
            }

            if (ObjList.Count<4)return;

            ObjList[3].transform.localPosition = ObjList[1].transform.localPosition;

            ObjList[3].transform.localPosition +=new Vector3(0,12f,0);
        }


        public float Width
        {
            get { return ObjList.Count * _mjSize.x; }
        }

        public float Height
        {
            get { return _mjSize.y; }
        }
    }
}
