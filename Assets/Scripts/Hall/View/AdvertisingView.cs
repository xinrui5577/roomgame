using System;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using AsyncImage = YxFramwork.Tool.AsyncImage;

namespace Assets.Scripts.Hall.View
{
    [Obsolete("优先使用SlideShowView处理,本脚本弃用，具体使用方法可以参考skin23的轮播图")]
    public class AdvertisingView :MonoBehaviour
    {
        public UITexture[] ShowTextures;
        public Transform showPos;
        public Transform preparePos;
        public Transform exchangePos;
        public Texture2D texture2;
        [Tooltip("移动速度")]
        public Vector3 MoveTemp=new Vector3(2f,0);

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
        /// <summary>
        /// 原为privte字段，为保持兼容性，没有更改字段命名，更改为public，默认移动时间不要随意更改，会影响线上游戏运行。
        /// 如要调整,请调整资源参数
        /// </summary>
        public float mMoveTotalTime = 2f;

        private Dictionary<int,int> _textureHasCodes = new Dictionary<int, int>();
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
            foreach (var show in ShowTextures)
            {
                show.transform.localPosition = show.transform.localPosition - MoveTemp;
            }
        }

        private void GetTextures()
        {
            //这个发送方法为异步函数,所以初始值要在函数中赋值
            Facade.Instance<TwManager>().SendAction("criclePicture", new Dictionary<string, object>(), (data) =>
            {
                if (data == null) return;
                var dataInfo = data as Dictionary<string, object>;
                if (dataInfo != null)
                {
                    var message = dataInfo.ContainsKey("pictures") ? dataInfo["pictures"] : null;
                    var cricleTime = dataInfo.ContainsKey("cricleTime") ? dataInfo["cricleTime"] : 0;
                    var cricleInfo = float.Parse(cricleTime.ToString());
                    mExchangeIntervalTime = cricleInfo;
                    var textureInfo = message as Dictionary<string, object>;
                    if (textureInfo == null) { return;}
                    var i = 0;
                    var keys = textureInfo.Keys;
                    pictureUrl = new string[keys.Count];
                    foreach (var value in keys)
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
                        var purl = pictureUrl[0];
                        _textureHasCodes[0] = purl.GetHashCode();
                        AsyncImage.Instance.GetAsyncImage(purl, (t2,urlHasCode) =>
                        {
                            if (urlHasCode != _textureHasCodes[0]) { return;}
                            ShowTextures[0].mainTexture = t2;
                            Target[0] = t2;
                        });

                        purl = pictureUrl[1];
                        _textureHasCodes[1] = purl.GetHashCode();
                        AsyncImage.Instance.GetAsyncImage(purl, (t2,urlHasCode) =>
                        {
                            if (urlHasCode != _textureHasCodes[1]) { return; }
                            ShowTextures[1].mainTexture = t2;
                            Target[1] = t2;
                        });

                        purl = pictureUrl[2];
                        _textureHasCodes[2] = purl.GetHashCode();
                        AsyncImage.Instance.GetAsyncImage(purl, (t2, urlHasCode) =>
                        {
                            if (urlHasCode != _textureHasCodes[2]) { return; }
                            Target[2] = t2;
                        });
                    }
                }
            });
        }
    }
}