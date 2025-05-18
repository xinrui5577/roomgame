using System;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Ttzkf
{
    public class Clock : MonoBehaviour
    {
        public UISprite LeftSprite;
        public UISprite RightSprite;

        private Action _callBack;
        private int _countTime;
        private Vector3 _v;
        protected void Start()
        {
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }

        public void SetCountDown(int time, Action callBack = null)
        {
            time++;
            CancelInvoke("SetCountDownA");
            _callBack = callBack;
            _countTime = time;
            InvokeRepeating("SetCountDownA", 0, 1);
        }

        protected void SetCountDownA()
        {
            Facade.Instance<MusicManager>().Play(GameSounds.Clock);
            _v = gameObject.transform.localPosition;
            _v.y += 5;
            gameObject.transform.localPosition = _v;
            Invoke("DownClock", 0.05f);
            LeftSprite.spriteName = "s" + _countTime / 10;
            RightSprite.spriteName = "s" + _countTime % 10;
            _countTime--;
            if (_countTime <= -1)
            {
                CancelInvoke("SetCountDownA");
                if (_callBack != null)
                {
                    _callBack();
                }
            }
        }

        protected void DownClock()
        {
            _v.y -= 5;
            gameObject.transform.localPosition = _v;
        }

        public void Stop()
        {
            CancelInvoke("SetCountDownA");
        }
    }
}
