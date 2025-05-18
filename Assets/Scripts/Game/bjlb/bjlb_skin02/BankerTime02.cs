using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    
    public class BankerTime02 : MonoBehaviour
    {
        /// <summary>
        /// 数字图片数组
        /// </summary>
        [SerializeField]
        private UISprite[] _numSprites;

        
        [SerializeField]
        private UIGrid _numsGrid;

        public int Time;

        /// <summary>
        /// 设置和显示当庄的次数
        /// </summary>
        /// <param name="time">当庄的次数,time > 0会正确显示</param>
        public void SetBankerTime(int time)
        {
            if (time >= 0) return;
            if (_numSprites == null || _numSprites.Length <= 0) return;
            SetNumSprite(time, 0);

            _numsGrid.gameObject.SetActive(true);
            _numsGrid.repositionNow = true;
            _numsGrid.Reposition();

            var Tween = GetComponent<TweenAlpha>();
            if (Tween == null) return;
            Tween.ResetToBeginning();
            Tween.PlayForward();
        }

        public void HideBankerTime()
        {
            _numsGrid.gameObject.SetActive(false);
            foreach (var item in _numSprites)
            {
                item.gameObject.SetActive(false);
            }
        }

        void SetNumSprite(int time, int index)
        {
            if (index >= _numSprites.Length) return;
            var spr = _numSprites[index];
            spr.spriteName = (time % 10).ToString();
            bool haveTime = time > 0;
            spr.gameObject.SetActive(haveTime);

            if (haveTime)
                SetNumSprite(time / 10, index + 1);
        }
    }
}