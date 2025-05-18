using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.jsys
{

    public class GoldSharkGameUIManager : MonoBehaviour
    {
        public Transform ChipsUI;

        public Transform BottonUI;

        public Transform ReturnToHallBtn;

        public Image OldBgImage;

        public Image NewBgImage;

        public Sprite[] BgSprites;

        public Slider ChangeBgSlider;

        public GameObject Handle;
        //动物寻路随机点
        public int PathNum = 0;

        public bool IsChangeBg = false;
        public bool CanChangeBg = true;
        private int _currentBgIndex = 1;
        private float _effectTime;
        //下注页面sprite
        public Sprite[] BetSprites;
        public Image BetImage;

        public void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

        }
        protected void Start()
        {
            OldBgImage.gameObject.SetActive(false);
        }

        public void ReturnToHall()
        {
            App.QuitGameWithMsgBox();
        }

        protected void Update()
        {
            if (IsChangeBg)
            {
                _effectTime += Time.deltaTime;
                if (_effectTime < 2)
                {
                    if (_currentBgIndex == 1)
                    {
                        ChangeBgSlider.value = 1 - _effectTime / 2;
                    }
                    else
                    {
                        ChangeBgSlider.value = _effectTime / 2;
                    }
                }
                else
                {
                    if (_currentBgIndex == 1)
                    {
                        _currentBgIndex = 0;
                        ChangeBgSlider.value = 0;
                    }
                    else
                    {
                        OldBgImage.gameObject.SetActive(false);
                        _currentBgIndex = 1;
                        ChangeBgSlider.value = 1;
                    }

                    _effectTime = 0f;
                    IsChangeBg = false;
                    CanChangeBg = true;
                    Handle.transform.GetChild(0).gameObject.SetActive(false);
                    Handle.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
        //切换背景图
        public void SetBgSprite(int sceneNum)
        {
            if (sceneNum != _currentBgIndex)
            {
                CanChangeBg = false;
                BetImage.sprite = BetSprites[sceneNum];
                OldBgImage.gameObject.SetActive(true);
                NewBgImage.gameObject.SetActive(true);
                Handle.transform.GetChild(sceneNum).gameObject.SetActive(true);
                IsChangeBg = true;
            }
        }
    }
}