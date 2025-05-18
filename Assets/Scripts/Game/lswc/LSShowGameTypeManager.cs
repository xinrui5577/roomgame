using Assets.Scripts.Game.lswc.Core;
using UnityEngine;
namespace Assets.Scripts.Game.lswc
{
    public class LSShowGameTypeManager : InstanceControl
    { 
        private LSFlyNumber _light2;

        private LSFlyNumber _light3;

        private LSFlyNumber _bigThree;

        private LSBigFourType _bigFour;

        private GameObject _lighting;

        private void Awake()
        {
            Find();
        }

        private void Find()
        {
            _light2 = transform.GetChild(0).GetComponent<LSFlyNumber>();
            _light3 = transform.GetChild(1).GetComponent<LSFlyNumber>();
            _bigThree = transform.GetChild(2).GetComponent<LSFlyNumber>();
            _bigFour =transform.GetChild(3).GetComponent<LSBigFourType>();
            _bigFour.Init();
            _lighting=transform.GetChild(4).gameObject;
        }

        public void ShowGameTypeLighting(int multiple)
        {

            _lighting.SetActive(true);
            if (multiple==2)
            {
                _light2.PlayAnimation();
            }
            else
            {
                _light3.PlayAnimation();
            }
            Invoke("HideLight",multiple);
        }

        private void HideLight()
        {
            _lighting.SetActive(false);
        }

        public void ShowBigThree()
        {
            _bigThree.PlayAnimation();
        }

        public void ShowBigFour(bool show)
        {
            if (show)
            {
                _bigFour.PlayAnimation();
            }
            else
            {
                _bigFour.HideBigFour();
            }
        
        }

        public override void OnExit()
        {
        }
    }
}
