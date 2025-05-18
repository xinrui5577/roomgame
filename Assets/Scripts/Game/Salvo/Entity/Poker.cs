using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Salvo.Entity
{
    public class Poker : MonoBehaviour
    {
        public string NamePrefix = "0x";
        public float TurnSpeed = 0.1f;
        private UISprite _sprite;
        private TweenRotation _ro;
        private bool _isTurned = true;
        public GameObject HeldSign;
        public UIButton ClickTarget;
        private EventDelegate ChangeFEvent;
        private EventDelegate ChangeOEvent;

        protected void Awake()
        {
            _sprite = GetComponent<UISprite>(); 
            _ro = GetComponent<TweenRotation>();
            ClickTarget.isEnabled = false;
            ChangeFEvent = new EventDelegate(Onfinish);
            ChangeOEvent = new EventDelegate(Onfinish1);
        }
         
        /// <summary>
        /// 设置牌面
        /// </summary>
        /// <param name="value">真实值</param>
        /// <param name="nameValue">显示的面值</param>
        public void SetValue(int value, int nameValue = 0)
        {
            Value = value;
            Show(nameValue);
        }

        public void Show(int nameValue = 0)
        {
            _sprite.spriteName = NamePrefix + nameValue.ToString("X");
        }

        public void Turn()
        {
//            Facade.Instance<MusicManager>().Play();//Play2DSound(10);
            _ro.ResetToBeginning();
            _ro.from = new Vector3(0,0,0);
            _ro.to = new Vector3(0,-90,0);
            _ro.AddOnFinished(ChangeFEvent);
            _ro.PlayForward();
        }
         
        private void Onfinish()
        {
            _isTurned = !_isTurned;
            ClickTarget.isEnabled = _isTurned;
            if (!_isTurned) SetHeld();
            var showValue = _isTurned ? Value : 0;
            Show(showValue);
            _ro.RemoveOnFinished(ChangeFEvent);
            _ro.ResetToBeginning();
            _ro.from = new Vector3(0, -90, 0);
            _ro.to = new Vector3(0, 0, 0);
            _ro.AddOnFinished(ChangeOEvent);
            _ro.PlayForward();
        }

        private void Onfinish1()
        { 
            _ro.RemoveOnFinished(ChangeOEvent); 
        }
          
        public void ShowHeld()
        {
            Facade.Instance<MusicManager>().Play("HOLD");//AudioManager.Instance.Play2DSound(8));
            SetHeld(!HeldSign.activeSelf);
        }

        public void SetHeld(bool isShow = false)
        {
            HeldSign.SetActive(isShow); 
            _sprite.color = isShow ? Color.gray : Color.white;
        }

        public void SetHight(bool isHight)
        {
            HeldSign.SetActive(false);
            _sprite.color = isHight ? Color.white : Color.gray; 
        }

        public bool IsHeld()
        {
            return HeldSign.activeSelf;
        }

        public int Value { get; private set; }

        public void Init(int value)
        {
            SetHeld();
            SetValue(value);
        }
    }
}
