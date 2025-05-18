using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.car
{
    public class CarTipView : MonoBehaviour
    {
        public EventObject EventObj;

        public GameObject StartBetBg;
        public GameObject StartBetText;

        public GameObject StopBetBg;
        public GameObject StopBetText;

        public UILabel Clock;

        public UISprite TimeBg;

        public UILabel TimeState;

        public GameObject Waiting;

        public GameObject Watching;
        public List<string> ResultShowPos = new List<string>();

        private int _timeCd;
        private bool _changeTimeBg;

        private CarGameData _gdata
        {
            get { return App.GetGameData<CarGameData>(); }
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
                case "ShowTime":
                    BetTime((SFSObject)data.Data);
                    break;
                case "StopBet":
                    ShowBetTip(StopBetBg, StopBetText);
                    break;
                case "Clear":
                    Clear();
                    break;
            }
        }

        private void ShowBetTip(GameObject bg, GameObject state)
        {
            bg.transform.localPosition = new Vector3(-1100, 70, 0);
            state.transform.localPosition = new Vector3(920, 47, 0);
            Facade.Instance<MusicManager>().Play("show");
            TweenPosition.Begin(bg, _gdata.UnitTime * 3, new Vector3(0, 70, 0));
            TweenPosition.Begin(state, _gdata.UnitTime * 3, new Vector3(0, 47, 0));
            state.GetComponent<TweenPosition>().onFinished.Clear();
            Facade.Instance<MusicManager>().Play("start");
            state.GetComponent<TweenPosition>().AddOnFinished(() =>
            {
                bg.GetComponent<TweenPosition>().delay = _gdata.UnitTime * 5;
                TweenPosition.Begin(bg, _gdata.UnitTime * 2, new Vector3(-1100, 70, 0));
                state.GetComponent<TweenPosition>().delay = _gdata.UnitTime * 5;
                TweenPosition.Begin(state, _gdata.UnitTime * 2, new Vector3(920, 47, 0));
            });
        }

        IEnumerator StartBetShow()
        {
            yield return new WaitForSeconds(1.5f);
            ShowBetTip(StartBetBg, StartBetText);
        }
        private void BetTime(SFSObject data)
        {
            if (data.ContainsKey("cd"))
            {
                _timeCd = data.GetInt("cd");
            }

            if (data.ContainsKey("tip"))
            {
                TimeState.text = data.GetUtfString("tip");
            }

            _changeTimeBg = true;
            InvokeRepeating("TimeChange", 0, 1);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShowBetTip(StartBetBg, StartBetText);
                //                BetTime(20);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                ShowBetTip(StopBetBg, StopBetText);
            }

            ChangeTimeBg();
        }


        private void ChangeTimeBg()
        {
            if(_changeTimeBg==false)return;
            TimeBg.fillAmount -= Time.deltaTime;
            if (TimeBg.fillAmount <= 0)
            {
                TimeBg.fillAmount = 1;
            }
        }

        private void TimeChange()
        {
            Clock.text = _timeCd.ToString();
            if (_timeCd == 0)
            {
                _changeTimeBg = false;
                TimeBg.fillAmount = 0;
                CancelInvoke("TimeChange");
            }
            _timeCd--;
        }

        public void ShowWaiting()
        {
            if (Waiting)
            {
                Waiting.SetActive(true);
            }
        }

        public void ShowWatch(bool isShow)
        {
            if (Watching)
            {
                Watching.SetActive(!isShow);
            }
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
            if (Waiting) Waiting.SetActive(false);
        }
    }
}
