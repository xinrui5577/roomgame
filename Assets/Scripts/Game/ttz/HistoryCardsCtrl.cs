using System;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.ttz
{
    public class HistoryCardsCtrl : MonoBehaviour
    {
        public UILabel[] MahjongNum;
        public Transform[] MahjongParent;
        public GameObject Bg;
        public Color ColorHui;
        public Color[] LabelColors;
        public GameObject Item;
        public float DesItemTime;
        [HideInInspector]
        public int MaxMahjongNum;

        public void OnMemoryCardClick()
        {
            Bg.SetActive(!Bg.activeInHierarchy);
        }

        public void RefreshData(int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                MahjongNum[i].text = data[i].ToString();
                if (data[i] == 0)
                {
                    MahjongParent[i].GetComponent<UISprite>().color = ColorHui;
                    MahjongNum[i].color = LabelColors[1];
                }
                else if (data[i] == MaxMahjongNum)
                    MahjongNum[i].color = LabelColors[0];
                else
                    MahjongNum[i].color = Color.white;
            }
        }

        public void RefrshDataOnPlay(int num, int target)
        {
            CreateItem(num);
            MahjongNum[num].text = (MaxMahjongNum - target).ToString();
            MahjongNum[num].color = Color.white;
            if (target == MaxMahjongNum)
            {
                MahjongParent[num].GetComponent<UISprite>().color = ColorHui;
                MahjongNum[num].color = LabelColors[1];
            }
        }

        public void InitMahjong()
        {
            foreach (var mj in MahjongParent)
            {
                mj.GetComponent<UISprite>().color = Color.white;
            }
            foreach (var mj in MahjongNum)
            {
                mj.color = LabelColors[0];
            }
        }

        protected void CreateItem(int num)
        {
            var temp = Instantiate(Item);
            temp.SetActive(true);
            temp.transform.parent = MahjongParent[num];
            temp.transform.localScale = Vector3.one;
            temp.transform.localPosition = new Vector3(0, -30, 0);
            var tween = temp.GetComponent<TweenPosition>();
            tween.from = temp.transform.localPosition;
            tween.to = new Vector3(0, 40, 0);
            tween.PlayForward();
            Destroy(temp, DesItemTime);
        }
    }

}