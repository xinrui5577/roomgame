using System.Collections;
using System.Collections.Generic;
using DragonBones;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.hg
{
    public class HgTipView : MonoBehaviour
    {
        public EventObject EventObj;

        public GameObject StartBetBg;

        public UnityArmatureComponent StartBrtAni;

        public GameObject StopBetBg;

        public GameObject StopBlackBg;

        public GameObject CompareCard;

        public UnityArmatureComponent PkAni;

        public GameObject PleaseBet;

        public UISprite TimeFirst;
        public UISprite TimeSecond;

        public GameObject Waiting;

        public GameObject Watching;
        public List<string> ResultShowPos = new List<string>();
        public List<TweenAlpha> ResultShowList = new List<TweenAlpha>();

        private int _timeCd;

        private HgGameData _gdata
        {
            get { return App.GetGameData<HgGameData>(); }
        }

        private HgGameManager _gmanager
        {
            get { return App.GetGameManager<HgGameManager>(); }
        }

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "ShowWatch":
                    ShowWatch((bool)data.Data);
                    break;
                case "ShowWait":
                    ShowWaiting();
                    break;
                case "ShowCompare":
                    CompareCardTip();
                    break;
                case "ShowTime":
                    BetTime((int)data.Data);
                    break;
                case "StopBet":
                    StopBetTip();
                    break;
                case "ShowResult":
                    Result((ResultData)data.Data);
                    break;
                case "CloseFlash":
                    CloseFlash();
                    break;
                case "Clear":
                    Clear();
                    break;

            }
        }

        public void StartBetTip()
        {
//            UnityFactory.factory.ReplaceSlotDisplay();
            if (StartBrtAni)
            {
                StartBrtAni.gameObject.SetActive(true);
                StartBrtAni.animation.Play("Animation1");
                return;
            }
            StartBetBg.transform.localPosition = new Vector3(-1450, 27, 0);
            Facade.Instance<MusicManager>().Play("show");
            TweenPosition.Begin(StartBetBg, _gdata.UnitTime*4, new Vector3(0, 27, 0));
            StartBetBg.GetComponent<TweenPosition>().onFinished.Clear();
            Facade.Instance<MusicManager>().Play("start");
            StartBetBg.GetComponent<TweenPosition>().AddOnFinished(() =>
            {
                StartBetBg.GetComponent<TweenPosition>().delay = _gdata.UnitTime*5;
               
                TweenPosition.Begin(StartBetBg, _gdata.UnitTime*4, new Vector3(1450, 27, 0)); 
            });
        }

        public void StopBetTip()
        {
            StopBetBg.transform.localPosition = new Vector3(-1450, 27, 0);
            Facade.Instance<MusicManager>().Play("show");

            if (StopBlackBg.GetComponent<UITexture>())
            {
                StopBlackBg.GetComponent<UITexture>().color = new Color(0, 0, 0, 1);
            }
            if (StopBlackBg.GetComponent<UISprite>())
            {
                StopBlackBg.GetComponent<UISprite>().color = new Color(0, 0, 0, 1);
            }

            TweenPosition.Begin(StopBetBg, _gdata.UnitTime*4, new Vector3(0, 27, 0));

              Facade.Instance<MusicManager>().Play("stop");
            StopBetBg.GetComponent<TweenPosition>().AddOnFinished(() =>
            {
                StopBetBg.GetComponent<TweenPosition>().delay = _gdata.UnitTime*5;
              
                TweenPosition.Begin(StopBetBg, _gdata.UnitTime*4, new Vector3(1450, 27, 0));

                TweenAlpha.Begin(StopBlackBg, _gdata.UnitTime*2, 0);
                StopBlackBg.GetComponent<TweenAlpha>().delay = _gdata.UnitTime * 3;
                StopBlackBg.GetComponent<TweenAlpha>().AddOnFinished(() =>
                {
                    StopBetBg.GetComponent<TweenPosition>().onFinished.Clear();
                });
            });
        }

        IEnumerator StartBetShow()
        {
            yield return new WaitForSeconds(1.5f);
            StartBetTip();
        }

        public void CompareCardTip()
        {
            CompareCard.SetActive(true);
            if (PkAni)
            {
                PkAni.gameObject.SetActive(true);
                PkAni.animation.Play("Animation1");
                StartCoroutine("StartBetShow");
            }
        }

        public void BetTime(int time)
        {
            PleaseBet.SetActive(true);
            _timeCd = time;
            InvokeRepeating("TimeChange", 0, 1);
        }

        private void TimeChange()
        {
            if (_timeCd >= 10)
            {
                var timeS = _timeCd / 10 % 10;
                TimeFirst.gameObject.SetActive(true);
                TimeFirst.spriteName =string.Format("num{0}", timeS);
                TimeFirst.MakePixelPerfect();
                var timeG = _timeCd % 10;
                TimeSecond.gameObject.SetActive(true);
                TimeSecond.spriteName =string.Format("num{0}", timeG);
                TimeSecond.MakePixelPerfect();
            }
            else
            {
                if (_timeCd == 0)
                {
                    PleaseBet.gameObject.SetActive(false);

                    TimeFirst.gameObject.SetActive(false);
                    CancelInvoke("TimeChange");
                    Facade.Instance<MusicManager>().Play("alert");
                }
                else
                {
                    if (_timeCd <= 5)
                    {
                        Facade.Instance<MusicManager>().Play("countdown");
                    }
                    TimeSecond.gameObject.SetActive(false);
                    TimeFirst.spriteName =string.Format("num{0}", _timeCd);
                      TimeFirst.MakePixelPerfect();
                }
            }

            _timeCd--;
        }

        public void ShowWaiting()
        {
            Waiting.SetActive(true);
        }

        public void ShowWatch(bool isShow)
        {
            Watching.SetActive(!isShow);
        }

        public void Result(ResultData resultData)
        {
            _gmanager.LaterSend = true;
            StopAllCoroutines();

            StartCoroutine(StopResultShow(resultData));
        }

        private IEnumerator StopResultShow(ResultData resultData)
        {
            foreach (var t in resultData.ResultShowList)
            {
                var index = GetShowResultPos(t);
                if(index==-1)continue;
                ResultShowList[index].gameObject.SetActive(true);
                ResultShowList[index].enabled = true;
            }

            yield return new WaitForSeconds(_gdata.UnitTime*12);

            foreach (var t in resultData.ResultShowList)
            {
                var index = GetShowResultPos(t);
                if (index == -1) continue;
                ResultShowList[index].enabled = false;
                ResultShowList[index].gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(_gdata.UnitTime *2);

            EventObj.SendEvent("BetViewEvent", "ShowWin", resultData);
            _gmanager.LaterSend = false;
        }

        private int GetShowResultPos(string pos)
        {
            for (int i = 0; i < ResultShowPos.Count; i++)
            {
                if (pos == ResultShowPos[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public void Clear()
        {
            CompareCard.SetActive(false);
            Waiting.SetActive(false);
            PleaseBet.SetActive(false);
        }

        private void CloseFlash()
        {
            //强制关闭输赢的煽动效果
            foreach (var t in ResultShowList)
            {
                t.enabled = false;
                t.gameObject.SetActive(false);
            }
        }
    }
}
