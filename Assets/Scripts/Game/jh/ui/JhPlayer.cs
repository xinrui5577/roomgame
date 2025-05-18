using System;
using System.Collections;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhPlayer : YxBaseGamePlayer
    {
        public int Chair;

        public GameObject BankerIcon;

        public JhCardGroup CardGroup;

        public GameObject TimerBg;

        public UISprite Timer;
       
        public UISprite TimePoint;

        public GameObject HeadBg;

        public EventDelegate OnTimerFinish;

        public GameObject MoveGold;

        public GameObject ChooseHeadIcon;

        protected EventDelegate OnHeadClick;
        public EventDelegate OnHeadDetailClick;
        public EventDelegate OnHeadChooseCompareClick;

        protected bool IsStart;

        protected double ToTalTime;

        protected double PassTime;

        protected Vector3 HeadBgVec;

        protected int HeadBgDepth;
        protected int HeadDepth;

        protected override void OnStart()
        {
            base.OnStart();
            HeadBgVec = HeadBg.transform.localPosition;
            if (HeadBg.GetComponent<UIWidget>() != null)
            {
                HeadBgDepth = HeadBg.GetComponent<UIWidget>().depth;
                HeadDepth = HeadPortrait.GetComponent<UIWidget>().depth;
            }
            else
            {
                HeadBgDepth = -1;
                HeadDepth = -1;
            }
        }

        protected void Update()
        {

            if (IsStart)
            {
                PassTime += Time.deltaTime;
                Timer.fillAmount = (float)(1-PassTime / ToTalTime);
                TimePoint.transform.localRotation = Quaternion.Euler(0, 0, (float)(-360 * PassTime / ToTalTime));
                if (Timer.fillAmount <= 0)
                {
                    Timer.fillAmount = 1;
                }
                if (PassTime >= ToTalTime)
                {
                    TimerBg.SetActive(false);
                    IsStart = false;
                    PassTime = 0;
                    Timer.fillAmount = 1;
                    TimePoint.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    if (OnTimerFinish != null)
                    {
                        OnTimerFinish.Execute();
                    }
                }
            }
        }

        public void OnPlayerTrun(double time, double pastTime, EventDelegate timeFinish = null)
        {
            if (Math.Abs(pastTime) > 0)
            {
                PassTime += pastTime;

            }

            ToTalTime = time;

            if (PassTime < ToTalTime)
            {
                IsStart = true;
                TimerBg.SetActive(true);
                OnTimerFinish = timeFinish;
            }


            TweenRotation rotation = HeadBg.GetComponent<TweenRotation>();
            if (rotation != null)
            {
                rotation.ResetToBeginning();
                rotation.enabled = true;
                rotation.PlayForward();
            }

            
        }

        public void SetPlayerCards(int[] cards)
        {
            CardGroup.SetCardsValue(cards);
            ShowCardsFront();
        }

        public void SetPlayerCardsStatus(JhCardGroup.GroupStatus status)
        {
            CardGroup.SetCardsStatus(status);
        }

        public void ShowCardsFront()
        {

            CardGroup.gameObject.SetActive(true);
            foreach (JhCard card in CardGroup.Cards)
            {
                card.ShowFront();
            }
        }

        public void ShowCardsBack()
        {
            CardGroup.gameObject.SetActive(true);
            foreach (JhCard card in CardGroup.Cards)
            {
                card.ShowBack();
            }
        }

        public void ShowCardsGray()
        {
            CardGroup.gameObject.SetActive(true);
            foreach (JhCard card in CardGroup.Cards)
            {
                card.ShowGray();
            }
        }

        public void ShowLiangPai()
        {
            CardGroup.gameObject.SetActive(true);
            foreach (JhCard card in CardGroup.Cards)
            {
                card.ShowLiangPai();
            }
        }

        public void SetBanker(bool isBanker)
        {
            BankerIcon.SetActive(isBanker);
        }

        public void ResetTimer()
        {
            TimerBg.SetActive(false);
            IsStart = false;
            PassTime = 0;
            Timer.fillAmount = 0;
            TimePoint.transform.localRotation = Quaternion.Euler(0, 0, 0);

            TweenRotation rotation = HeadBg.GetComponent<TweenRotation>();
            rotation.ResetToBeginning();
            rotation.enabled = false;
        }

        public void Reset()
        {
            ResetTimer();
            CardGroup.Reset();
            BankerIcon.SetActive(false);
            CardGroup.gameObject.SetActive(false); 
            MoveGold.SetActive(false);
            HeadBg.transform.localPosition = HeadBgVec;
            if (HeadBgDepth != -1)
            {
                HeadBg.GetComponent<UIWidget>().depth = HeadBgDepth;
            }
            if (HeadDepth != -1)
            {
                HeadPortrait.GetComponent<UIWidget>().depth = HeadBgDepth;
            }
            ResetHeadClick();
        }

        public void OnHeadClickCall()
        {
            if (OnHeadClick != null)
            {
                OnHeadClick.parameters[0] = new EventDelegate.Parameter(Chair);
                OnHeadClick.Execute();
            }

        }

        public void ResetHeadClick()
        {
            ChooseHeadIcon.SetActive(false);

            TweenPosition pos = ChooseHeadIcon.GetComponent<TweenPosition>();
            if (pos != null)
            {
                pos.ResetToBeginning();
            }
            TweenRotation rotation = ChooseHeadIcon.GetComponent<TweenRotation>();
            if (rotation != null)
            {
                rotation.ResetToBeginning();
                rotation.enabled = false;
            }
            TweenScale scale = ChooseHeadIcon.GetComponent<TweenScale>();
            if (scale != null)
            {
                scale.ResetToBeginning();
                scale.enabled = false;
            }

            if (!Equals(OnHeadClick, OnHeadDetailClick))
            {
                OnHeadClick = OnHeadDetailClick;
            }
        }

        public void SetHeadChooseCompare(Vector3 wordPos)
        {
            ChooseHeadIcon.SetActive(true);

            TweenPosition pos = ChooseHeadIcon.GetComponent<TweenPosition>();
            if (pos != null)
            {
                pos.worldSpace = true;
                pos.from = wordPos;
                pos.to = transform.position;
                pos.PlayForward();
                pos.onFinished.Add(new EventDelegate(() => {
                    TweenRotation rotation = ChooseHeadIcon.GetComponent<TweenRotation>();
                    if (rotation != null)
                    {
                        rotation.enabled = true;
                    }
                    TweenScale scale = ChooseHeadIcon.GetComponent<TweenScale>();
                    if(scale!=null)
                    {
                        scale.enabled = true;
                    }
                
                
                }));
            }

            OnHeadClick = OnHeadChooseCompareClick;
        }

        public void RefreshCoin()
        {
            SetCoin(Coin);
        }


        public void SetReady(bool state)
        {
            ReadyStateFlag.SetActive(state);
        }
        public void ShowMoveGold(int value)
        {
            if (MoveGold != null)
            {
                UILabel label = MoveGold.GetComponent<UILabel>();
                TweenPosition twPos = MoveGold.GetComponent<TweenPosition>();
                string tt = "";
                Color fColor = Color.white;
                if (value < 0)
                {
                    tt += YxUtiles.ReduceNumber(value);
                    fColor = Color.red;
                }
                else
                {
                    tt += "+" + YxUtiles.ReduceNumber(value);
                    fColor = Color.green;
                }

                label.text = tt;
                label.color = fColor;

                twPos.ResetToBeginning();
                MoveGold.SetActive(true);
                twPos.PlayForward();
                twPos.onFinished.Clear();
                twPos.onFinished.Add(new EventDelegate(() =>
                {
                    StartCoroutine(DelayTime(0.5f, new EventDelegate(() =>
                    {
                        MoveGold.SetActive(false);
                    })));
                }));
            }
        }

        protected IEnumerator DelayTime(float time,EventDelegate eDelegate)
        {
            yield return new WaitForSeconds(time);
            if (eDelegate != null)
            {
                eDelegate.Execute();
            }
        }
    }
}
