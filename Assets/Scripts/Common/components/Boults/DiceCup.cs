using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Components.Boults
{
    public class DiceCup : MonoBehaviour
    {
        /// <summary>
        /// 动画组件
        /// </summary>
        public YxBaseAnimationAdapter DiceAnimationAdapter;
        /// <summary>
        /// 摇筛子时间
        /// </summary>
        public float TurnTime = 3;

        public string TurnSound = "sound_gambling_dice";
        /// <summary>
        /// 筛子
        /// </summary>
        public Boult[] Boults;


        public void Init()
        {
            _toTheTime = false;
            _hasNew = false;
        }

        public void Turn()
        {
            if (DiceAnimationAdapter != null)
            {
                DiceAnimationAdapter.Play();
            }
            Init();
            foreach (var boult in Boults)
            {
                boult.Turn();
            }
            Facade.Instance<MusicManager>().Play(TurnSound);
            Invoke("OnFinishTurnTime", TurnTime);
        }

        private bool _toTheTime;
        private void OnFinishTurnTime()
        {
            _toTheTime = true;
            Stop();
        }

        private int[] _dots;
        private bool _hasNew;
        public void Stop(int[] dots)
        {
            _dots = dots;
            _hasNew = true;
            Stop();
        }

        private void Stop()
        {
            if (!_toTheTime || !_hasNew) return;
            var count = Mathf.Min(_dots.Length, Boults.Length);
            for (var i = 0; i < count; i++)
            {
                Boults[i].Stop(_dots[i]);
            }
            _hasNew = false;
        }
    }
}
