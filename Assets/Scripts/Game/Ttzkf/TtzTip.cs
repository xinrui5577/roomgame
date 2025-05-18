using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Ttzkf
{
    public class TtzTip : MonoBehaviour
    {
        public delegate void DelegateFun();

        private UILabel _textLabel;
        private string _text;
        private int _cdTime;
        private DelegateFun _callback;

        protected void Awake()
        {
            _textLabel = gameObject.GetComponentInChildren<UILabel>();
        }
        public void T(string text)
        {
            gameObject.SetActive(true);
            _textLabel.text = text;
            CancelInvoke("SetTimeText");
        }

        public void T(string text, float time)
        {
            _cdTime = Mathf.FloorToInt(time - Time.time);
            T(text + ":" + _cdTime);
            _cdTime -= 1;
            _text = text;
            if (_cdTime < 2)
            {
                return;
            }
            InvokeRepeating("SetTimeText", 1, 1);
        }

        public void T(string text, float time, DelegateFun callBack)
        {
            T(text, time);
            _callback = callBack;
        }

        protected void SetTimeText()
        {
            if (_cdTime < 0)
            {
                Close();
                if (_callback != null) _callback();
                return;
            }
            _textLabel.text = _text + ":" + _cdTime;
            _cdTime--;
            Facade.Instance<MusicManager>().Play(GameSounds.Clock);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            CancelInvoke("SetTimeText");
        }
    }
}
