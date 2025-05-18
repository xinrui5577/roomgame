using System;
using UnityEngine;

namespace Assets.Scripts.Game.slyz.GameScene
{ 
    public class CardAni : MonoBehaviour {

        public  Action<int> Complete = null;

        // Use this for initialization
        private UISprite _sprite;
        private int _width;
        private float _time;
        
        void Start () {
            _sprite  = GetComponent<UISprite>();
            _width = _sprite.width;
        }

        public string CardName;
        public string CardBg;


        public void SetCompleteFun(Action<int> fun)
        {
            Complete = fun;
        }
   
        public float Speed;
        // Update is called once per frame
        protected void Update ()
        {
            if (!_isPlay) return;
            var dt = Time.deltaTime;
            _time += dt;

            if (_time * Speed > Mathf.PI / 2 && CardName != null) 
            {
                // 换图
                _sprite.spriteName = CardName;            
            }

            _sprite.width = (int)(_width * Mathf.Abs(Mathf.Cos(_time * Speed)));

            if (Mathf.PI <= _time * Speed)
            {
                // 结束
                _sprite.width = _width;
                enabled = false;
                Destroy(this);

                if (Complete != null)
                    Complete(1);
            }
        }

        private bool _isPlay;
        public void Play()
        {
            _isPlay = true;
        }

        public void Stop()
        {
            _isPlay = false;
        }
    }
}
