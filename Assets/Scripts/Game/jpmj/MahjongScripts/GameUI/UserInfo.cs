using System;
using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.UI;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class UserInfo : MonoBehaviour
    {
        public RawImage HeadTxtr;
        public UserDefLable GoldLbl;
        public Image BankerTagSpr;
        public Image OutLine;
        public GameObject CurrParticle;
        public UGUISpriteAnimation SpeakAnimation;
        public Text UserName;
        public Image Ting;
        public Texture[] DirectionTextures;
        public RawImage Direction;

        public Text Id;
        // Use this for initialization
        private bool _isCurr;

        private int _seatID = -1;

        public int Seat
        {
            get { return _seatID; }
        }

        public void RepairPos(int index)
        {
            if (null == GameAdpaterManager.Singleton) return;
            switch (index)
            {
                case 0: GetComponent<RectTransform>().anchoredPosition3D = GameAdpaterManager.Singleton.GetConfig.HeadBgImg0_Pos; break;
                case 1: GetComponent<RectTransform>().anchoredPosition3D = GameAdpaterManager.Singleton.GetConfig.HeadBgImg1_Pos; break;
                case 3: GetComponent<RectTransform>().anchoredPosition3D = GameAdpaterManager.Singleton.GetConfig.HeadBgImg3_Pos; break;
            }
        }

        public void SetGlod(long Gold)
        {
            if (GoldLbl!=null)
                GoldLbl.GlodText = Gold;
        }
        public void AddGlod(int gold)
        {
            if (GoldLbl != null)
                GoldLbl.GlodText += gold;
        }

        public void SetHead(Texture img)
        {
            if (HeadTxtr != null)
                HeadTxtr.texture = img;
        }

        public void SetSeat(int seat)
        {
            _seatID = seat;
        }
        public bool IsBanker
        {
            set {
                if (BankerTagSpr != null)
                    BankerTagSpr.gameObject.SetActive(value);
               }
        }

        public bool IsExsit
        {
            set 
            { 
                gameObject.SetActive(value);
                if (!value) _seatID = -1;
            }
        }

        public bool IsOutLine
        {
            set { if (OutLine != null)OutLine.gameObject.SetActive(value); }
        }

        /// <summary>
        /// 当iscurr值变化时触发
        /// </summary>
        public Action<bool> OnIsCurrValueChange; 

        public bool IsCurr
        {
            get { return _isCurr; }
            set
            {
                if (!_isCurr && value)
                {
                    _isCurr = true;
                    //StartCoroutine(CurreEnumerator());
                    if (CurrParticle != null) CurrParticle.SetActive(true);

                    if (OnIsCurrValueChange != null) OnIsCurrValueChange(true);
                }

                _isCurr = value;
                if (_isCurr == false)
                {
                    if (CurrParticle != null) CurrParticle.SetActive(false);

                    if (OnIsCurrValueChange != null) OnIsCurrValueChange(false);
                }
            }
        }

        public bool IsSpeak
        {
            set
            {
                if (SpeakAnimation != null) SpeakAnimation.gameObject.SetActive(value);
            }
        }

        public string Name
        {
            set { if (UserName != null) UserName.text = value; }
        }

        public string ID
        {
            set { if (Id != null) Id.text = value; }
        }

        public bool IsTing
        {
            set { if(Ting!=null)Ting.gameObject.SetActive(value);}
        }

        protected IEnumerator CurreEnumerator()
        {
            CanvasGroup bg = gameObject.GetComponent<CanvasGroup>();
            int sign = -1;
            while (_isCurr)
            {
                bg.alpha = bg.alpha + 0.1f * sign;
                if (bg.alpha <= 0.5)
                {
                    sign = 1;
                }
                if (bg.alpha >= 1)
                {
                    sign = -1;
                }

                yield return new WaitForSeconds(0.1f);
            }

            bg.alpha = 1;
        }


        /// <summary>
        /// 配置GPS信息显示GPS界面,否则显示人物信息
        /// </summary>
        public void OnUserHeadClick()
        {
            var apiInfo = new Dictionary<string, object>() { { "bundleID", Application.bundleIdentifier } };

            Facade.Instance<TwManager>().SendAction("gpsOpen", apiInfo, msg =>
            {
                if (msg == null) return;

                var dic = (Dictionary<string, object>)msg;

                object flag;
                if (dic.TryGetValue(("gpsOpen"), out flag))
                {
                    if (bool.Parse(flag.ToString()))
                    {
                        //显示GPS
                        EventDispatch.Dispatch((int)UIEventId.ShowGPSInfo, new EventData());
                    }
                    else
                    {
                        //显示表情
                        if (_seatID != -1)
                            EventDispatch.Dispatch((int)NetEventId.OnUserDetail, new EventData(_seatID));
                    }
                }
            });
        }


        /// <summary>
        /// 点击人物头像,直接显示表情礼物动画
        /// </summary>
        public void OnClickUserImageShowAnim()
        {
            var apiInfo = new Dictionary<string, object>() { { "bundleID", Application.bundleIdentifier } };

            Facade.Instance<TwManager>().SendAction("gpsOpen", apiInfo, msg =>
            {
                if (msg == null) return;

                var dic = (Dictionary<string, object>)msg;


                //显示表情
                if (_seatID != -1)
                    EventDispatch.Dispatch((int)NetEventId.OnUserDetail, new EventData(_seatID));
             
            });
        }
        /// <summary>
        /// 设置风位标记
        /// </summary>
        /// <param name="index"></param>
        public void SetDirectionFlag(int index)
        {
            var directionCounts = DirectionTextures.Length;
            if (directionCounts > 0&&Direction)
            {
                Direction.enabled = true;
                Direction.texture = DirectionTextures[index];
                Direction.SetNativeSize();
            }
        }

    }
}