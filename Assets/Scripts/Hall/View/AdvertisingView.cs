using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using AsyncImage = YxFramwork.Tool.AsyncImage;
using Object = System.Object;

namespace Assets.Scripts.Hall.View
{
    public class AdvertisingView : MonoBehaviour
    {
        public UITexture[] ShowTextures;
        public Transform showPos;
        public Transform preparePos;
        public Transform exchangePos;
        public Texture2D texture2;

        private int spriteNum = 0;
        private float mExchangeTimer;
        private float mMoveTimer;
        private int mIndex = 0;
        private string[] pictureUrl;
        private bool mIsMove = false;
        private Texture2D[] Target;
        private bool isFrist = true;

        //切换图片间隔
        public float mExchangeIntervalTime = 5;
        //图片运动时间
        private float mMoveTotalTime = 2;

        void Awake()
        {
            GetTextures();
        }

        void FixedUpdate()
        {
            if (spriteNum != 0)
            {
                if (mExchangeIntervalTime == 0)
                {
                    mExchangeIntervalTime = 5;
                }
                mExchangeTimer += Time.deltaTime;
                if (mExchangeTimer >= mExchangeIntervalTime)
                {
                    ChangeTextrue();
                    mIsMove = true;
                    mExchangeTimer = 0;
                    isFrist = false;
                }
                if (mIsMove)
                {
                    mMoveTimer += Time.deltaTime;
                    if (mMoveTimer <= mMoveTotalTime)
                    {
                        MoveTexture();
                    }
                    else
                    {
                        mIsMove = false;
                        mMoveTimer = 0;
                        for (int i = 0; i < ShowTextures.Length; i++)
                        {
                            if (ShowTextures[i].transform.localPosition.x >= 0)
                                ShowTextures[i].transform.localPosition = showPos.localPosition;
                            else
                                ShowTextures[i].transform.localPosition = preparePos.localPosition;
                        }

                    }
                }
            }

        }

        private void ChangeTextrue()
        {
            mIndex = (mIndex + 1) % spriteNum;
            if (isFrist) return;
            //用for循环会报数组越界,但是能正常执行,用foreach没有问题
            foreach (var show in ShowTextures)
            {
                if (show.transform.localPosition == preparePos.localPosition)
                    show.mainTexture = Target[mIndex];
            }
        }

        private void MoveTexture()
        {
            Vector3 temp = new Vector3(2.25f, 0, 0);
            foreach (var show in ShowTextures)
            {
                show.transform.localPosition = show.transform.localPosition - temp;
            }
        }

        private void GetTextures()
        {
            //这个发送方法为异步函数,所以初始值要在函数中赋值
            Facade.Instance<TwManger>().SendAction("criclePicture", new Dictionary<string, object>(), (data) =>
            {
                if (data == null) return;
                if (data is Dictionary<string, object>)
                {
                    var dataInfo = data as Dictionary<string, object>;
                    var message = dataInfo.ContainsKey("pictures") ? dataInfo["pictures"] : null;
                    var cricleTime = dataInfo.ContainsKey("cricleTime") ? dataInfo["cricleTime"] : 0;
                    var cricleInfo = float.Parse(cricleTime.ToString());
                    mExchangeIntervalTime = cricleInfo;
                    var textureInfo = message as Dictionary<string, object>;
                    int i = 0;
                    pictureUrl = new string[textureInfo.Keys.Count];
                    foreach (var value in textureInfo.Keys)
                    {
                        pictureUrl[i] = textureInfo[value] as string;
                        i++;
                    }
                    if (pictureUrl != null)
                    {
                        spriteNum = pictureUrl.Length;
                        Target = new Texture2D[spriteNum];
                        ShowTextures[0].transform.localPosition = showPos.localPosition;
                        ShowTextures[1].transform.localPosition = preparePos.localPosition;
                        AsyncImage.Instance.GetAsyncImage(pictureUrl[0], texture2 =>
                        {
                            ShowTextures[0].mainTexture = texture2;
                            Target[0] = texture2;
                        });
                        AsyncImage.Instance.GetAsyncImage(pictureUrl[1], texture2 =>
                        {
                            ShowTextures[1].mainTexture = texture2;
                            Target[1] = texture2;
                        });
                        AsyncImage.Instance.GetAsyncImage(pictureUrl[2], texture2 =>
                        {
                            Target[2] = texture2;
                        });
                    }
                }
            });
        }
    }
}