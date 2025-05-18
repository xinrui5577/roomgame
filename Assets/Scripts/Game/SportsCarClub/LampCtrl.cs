using UnityEngine;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class LampCtrl : MonoBehaviour
    {
        /// <summary>
        /// 灯
        /// </summary>
        public UISprite[] Lamps;
        /// <summary>
        /// 是否播放
        /// </summary>
        public bool IsPlay;

	
        // Update is called once per frame
        void Update () {
            Play();
        }

        private float _timeCount;

        public float TimeSpace;

        /// <summary>
        /// 播放
        /// </summary>
        private void Play()
        {
            if (IsPlay)
            {
                _timeCount += Time.deltaTime;
                if (_timeCount >= TimeSpace)
                {
                    _timeCount = 0f;
                    var oneColor = Lamps[0].spriteName == "red.PNG" ? "yellow.PNG" : "red.PNG";
                    var twoColor = oneColor == "red.PNG" ? "yellow.PNG" : "red.PNG";
                    for (var i = 0; i < Lamps.Length; i++)
                    {
                        Lamps[i].spriteName = i%2 == 0 ? oneColor : twoColor;
                    }
                }

            }
        }

        public void PlayLamp()
        {
            IsPlay = true;
            DisplayLamp(false);
        }

        public void StopLamp()
        {
            IsPlay = false;
            DisplayLamp(false);
        }

        private void DisplayLamp(bool isShow)
        {
            foreach (var lamp in Lamps)
            {
                lamp.gameObject.SetActive(isShow);
            }
        }

    }
}
