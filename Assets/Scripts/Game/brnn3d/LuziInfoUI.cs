using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.brnn3d
{
    public class LuziInfoUI : MonoBehaviour
    {
        public Image[] EastImgs = new Image[10];
        public Image[] SouthImgs = new Image[10];
        public Image[] WestImgs = new Image[10];
        public Image[] NorthImgs = new Image[10];

        //路子信息图片0：勾 1：差
        public Sprite[] LuziImgs;

        public void InitImg()
        {
            foreach (var eastImg in EastImgs)
            {
                eastImg.gameObject.SetActive(false);
            }
            foreach (var southImg in SouthImgs)
            {
                southImg.gameObject.SetActive(false);
            }
            foreach (var westImg in WestImgs)
            {
                westImg.gameObject.SetActive(false);
            }
            foreach (var northImg in NorthImgs)
            {
                northImg.gameObject.SetActive(false);
            }
        }

        public void SetEastImg(int indexImg, bool iGou)
        {
            if (iGou)
            {
                if (!EastImgs[indexImg].gameObject.activeSelf)
                    EastImgs[indexImg].gameObject.SetActive(true);
                EastImgs[indexImg].sprite = LuziImgs[0];
            }
            else
            {
                if (!EastImgs[indexImg].gameObject.activeSelf)
                    EastImgs[indexImg].gameObject.SetActive(true);
                EastImgs[indexImg].sprite = LuziImgs[1];
            }
        }

        public void SetSouthImg(int indexImg, bool iGou)
        {
            if (iGou)
            {
                if (!SouthImgs[indexImg].gameObject.activeSelf)
                    SouthImgs[indexImg].gameObject.SetActive(true);
                SouthImgs[indexImg].sprite = LuziImgs[0];
            }
            else
            {
                if (!SouthImgs[indexImg].gameObject.activeSelf)
                    SouthImgs[indexImg].gameObject.SetActive(true);
                SouthImgs[indexImg].sprite = LuziImgs[1];
            }
        }

        public void SetWestImg(int indexImg, bool iGou)
        {
            if (iGou)
            {
                if (!WestImgs[indexImg].gameObject.activeSelf)
                    WestImgs[indexImg].gameObject.SetActive(true);
                WestImgs[indexImg].sprite = LuziImgs[0];
            }
            else
            {
                if (!WestImgs[indexImg].gameObject.activeSelf)
                    WestImgs[indexImg].gameObject.SetActive(true);
                WestImgs[indexImg].sprite = LuziImgs[1];
            }
        }

        public void SetNorthImg(int indexImg, bool iGou)
        {
            if (iGou)
            {
                if (!NorthImgs[indexImg].gameObject.activeSelf)
                    NorthImgs[indexImg].gameObject.SetActive(true);
                NorthImgs[indexImg].sprite = LuziImgs[0];
            }
            else
            {
                if (!NorthImgs[indexImg].gameObject.activeSelf)
                    NorthImgs[indexImg].gameObject.SetActive(true);
                NorthImgs[indexImg].sprite = LuziImgs[1];
            }
        }


    }
}
