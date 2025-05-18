using UnityEngine.UI;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class CountDownUI : MonoBehaviour
    {
        public Image SiImg;
        public Image GeImg;
        public Sprite[] NumberSprites;
        public Sprite[] TesuNuSprites;
         
        private int _timeCount;
        private float _time;
        protected void Update()
        {
            _time += Time.deltaTime;
            if (_time > 1)
            {
                if (_timeCount > 0)
                {
                    _timeCount--;
                }
                GetTimeCountNumberToImg(_timeCount);
                _time = 0f;
                if (_timeCount < 4 && App.GetGameData<Brnn3dGameData>().IsBet)
                {
                    Facade.Instance<MusicManager>().Play("Warning");
                }
            }
        }

        //设置时间数量
        public void SetTimeCount(int time)
        {

            _timeCount = time;
            GetTimeCountNumberToImg(_timeCount);
        }

        public void GetTimeCountNumberToImg(int time)
        {
            int siN = time / 10;
            if (siN > 0) ShowSiImg(siN, true);
            else ShowSiImg(0);

            int geN = time % 10;
            if (siN > 0)
            {
                if (geN >= 0) ShowGeImg(geN, true);
                else ShowGeImg(0, true);
            }
            else
            {
                if (geN > 3) ShowGeImg(geN, true);
                else ShowGeImg(geN, false);
            }
        }

        //显示的是十位数
        void ShowSiImg(int number, bool isShow = false)
        {
            if (!isShow)
            {
                if (SiImg.gameObject.activeSelf)
                    SiImg.gameObject.SetActive(false);
                return;
            }

            if (!SiImg.gameObject.activeSelf)
            {
                SiImg.gameObject.SetActive(true);
            }
            SiImg.sprite = NumberSprites[number];
        }

        //显示的是个位数
        void ShowGeImg(int number, bool isShow = false)
        {
            if (!isShow)
            {
                if (!GeImg.gameObject.activeSelf)
                {
                    GeImg.gameObject.SetActive(true);
                }
                GeImg.sprite = TesuNuSprites[number];
                return;
            }

            if (!GeImg.gameObject.activeSelf)
            {
                GeImg.gameObject.SetActive(true);
            }
            GeImg.sprite = NumberSprites[number];
        }

    }
}

