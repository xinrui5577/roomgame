/*
 * 时间：2018年7月10日09:54:04
 * 功能：比大小胜利后显示胜利或失败的prefab
 */

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.fruit
{
    public class DisPlayDxResult : MonoBehaviour
    {
        private float currentTime;
        private float resultShakeUpdate;
        private bool shakeOrNot;
        public GameObject leftImg;
        public GameObject rightImg;

        //比倍彩金
        public long BiBeiCaiJin = 0;

        public GameObject NumImgsListPrep;
        public GameObject winPrefab;
        public GameObject losePrefab;
        public Transform trans;

        private static DisPlayDxResult instance;
        public static DisPlayDxResult getInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            eve_CancleDisplayResult += NewLightsBlink.getInstance().onIdleLights;
        }

        // Update is called once per frame
        void Update()
        {
            if (trans.gameObject.GetComponent<Image>().enabled)
            {
                Color color = trans.gameObject.GetComponent<Image>().color;
                if (color.a < .4f)
                {
                    color.a += 0.8f * Time.deltaTime;
                    trans.gameObject.GetComponent<Image>().color = color;
                }
            }

            currentTime = Time.time;

            ResultShake();
        }

        public void onBdxWin(object obj, EventArgs args)
        {
            //Debug.Log("onBdxWin()");
            trans.gameObject.GetComponent<Image>().enabled = true;

            //img
            var temp = (GameObject)Instantiate(winPrefab, trans);
            temp.transform.localScale = new Vector3(1, 1, 1);
            temp.transform.localPosition = new Vector3(-4, 88, 0);

            //label
            var str = YxUtiles.GetShowNumberToString(BiBeiCaiJin);
            //temp.GetComponentInChildren<Text>().text = str;
            var prepNumImgs = (GameObject)Instantiate(NumImgsListPrep, trans);
            prepNumImgs.transform.localScale = new Vector3(1, 1, 1);
            prepNumImgs.transform.localPosition = new Vector3(16.1f, -8.1f, 0);
            this.strToPic(prepNumImgs, str);

            StartCoroutine(this.cancleDisplay(prepNumImgs));
            shakeOrNot = true;
        }

        public void onBdxLose(object obj, EventArgs args)
        {
            //Debug.Log("onBdxLose()");
            trans.gameObject.GetComponent<Image>().enabled = true;

            //img
            var temp = (GameObject)Instantiate(losePrefab, trans);
            temp.transform.localScale = new Vector3(1, 1, 1);
            temp.transform.localPosition = new Vector3(-4, 71, 0);

            Debug.LogError(BiBeiCaiJin);
            //label
            //            var str = BiBeiCaiJin.ToString();
            //            if (str.Length <= 2)
            //                str = str.PadLeft(3, '0');
            //            Debug.LogError(str);
            //            str = str.Insert(str.Length - 2, ".");
            var str = YxUtiles.GetShowNumberToString(BiBeiCaiJin);
            //temp.GetComponentInChildren<Text>().text = "-" + str;
            var prepNumImgs = (GameObject)Instantiate(NumImgsListPrep, trans);
            prepNumImgs.transform.localScale = new Vector3(1, 1, 1);
            prepNumImgs.transform.localPosition = new Vector3(16.1f, -8.1f, 0);
            this.strToPic(prepNumImgs, "-" + str);

            StartCoroutine(this.cancleDisplay(prepNumImgs));
            shakeOrNot = true;
        }

        public event EventHandler eve_CancleDisplayResult;

        //結果取消显示
        private IEnumerator cancleDisplay(GameObject obj)
        {
            yield return new WaitForSeconds(2);
            Destroy(obj);
            trans.gameObject.GetComponent<Image>().color = new Vector4(0, 0, 0, 0);
            trans.gameObject.GetComponent<Image>().enabled = false;
            foreach (var item in trans.gameObject.GetComponentsInChildren<Image>())
            {
                if (item.enabled != false)
                    Destroy(item.gameObject);
            }

            eve_CancleDisplayResult(this, EventArgs.Empty);

            shakeOrNot = false;
        }

        public Sprite[] imgs;  //存放0-9

        //设置数字集合
        public void strToPic(GameObject numList, string str)
        {
            if (str.Length > 10)
                return;

            //删除多余0或者"."
//            while (str[str.Length - 1] == '0')
//                str = str.Remove(str.Length - 1, 1);
//            if (str[str.Length - 1] == '.')
//                str = str.Remove(str.Length - 1, 1);

            char[] tempChar = str.ToCharArray();

            int idx = 0;
            foreach (var item in numList.GetComponentsInChildren<Image>())  //删除多余图片
            {
                if (idx >= str.Length)
                {
                    Destroy(item.gameObject);
                }
                else
                {
                    int temp;
                    switch (tempChar[idx])
                    {
                        case '-':
                            {
                                temp = 10;
                                break;
                            }
                        case '.':
                            {
                                temp = 11;
                                break;
                            }
                        default:
                            {
                                temp = (int)(tempChar[idx] - '0');
                                break;
                            }
                    }

                    numList.GetComponentsInChildren<Image>()[idx].GetComponent<Image>().sprite = imgs[temp];
                }
                idx++;
            }
        }

        //结果闪烁
        public void ResultShake()
        {
            if (currentTime - resultShakeUpdate > 0.2f && shakeOrNot)
            {
                resultShakeUpdate = Time.time;

                leftImg.GetComponent<Image>().enabled = !leftImg.GetComponent<Image>().enabled;
                rightImg.GetComponent<Image>().enabled = !rightImg.GetComponent<Image>().enabled;
            }
            else if (!shakeOrNot)
            {
                leftImg.GetComponent<Image>().enabled = true;
                rightImg.GetComponent<Image>().enabled = true;
            }
        }
    }
}