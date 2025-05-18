using UnityEngine;
using System.Collections;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
#pragma warning disable 649


namespace Assets.Scripts.Game.paijiu
{
    public class Clock : MonoBehaviour
    {
        /// <summary>
        /// 时间显示label
        /// </summary>
        [SerializeField]
        private UILabel _timeLabel;

        /// <summary>
        /// 下注说明
        /// </summary>
        [SerializeField]
        private UISprite _bgSprite;

        private int _timer;

        private bool _isCounting;

        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="time">记录时间</param>
        /// <param name="type">时间类型</param>
        public void BeginCountDown(int time, GameRequestType type = GameRequestType.None)
        {
            if (time <= 0)
                return;
            gameObject.SetActive(time > 0);
            _timer = time;
            _bgSprite.spriteName = GetSprName(type);

            if (_isCounting) return;
            //开始计时
            _isCounting = true;
            _timeLabel.gameObject.SetActive(true);
            StartCoroutine(DoCd());
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        public void StopCountDown()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            _isCounting = false;
        }

        /// <summary>
        /// 获取计时说明图片
        /// </summary>
        /// <param name="type">当前游戏阶段</param>
        /// <returns></returns>
        private string GetSprName(GameRequestType type)
        {
            switch (type)
            {

                case GameRequestType.BeginBet:
                    return "bettime";

                case GameRequestType.SendCard:
                    return "cardtime";

                default:
                    return string.Empty;
            }
        }

        IEnumerator DoCd()
        {
            while (--_timer >= 0)
            {
                _timeLabel.text = (_timer).ToString();
                yield return new WaitForSeconds(1f);
            }
            StopCountDown();
        }



        public void Reset()
        {
            StopCountDown();
        }

    }
}