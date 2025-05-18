using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class TishiInfo : MonoBehaviour
    {
        public Sprite[] Tishi;
        public Transform TurnLeft;
        public Transform TurnRight;


        public void OnClick()
        {
            gameObject.SetActive(true);
        }

        private int _turnNum=0;
        public void TurnBack()
        {
            if (_turnNum == 0)
            {
                TurnLeft.gameObject.SetActive(false);
            }
            else
            {
                TurnLeft.gameObject.SetActive(true);
                _turnNum--;
                gameObject.transform.GetComponent<Image>().sprite = Tishi[_turnNum];
            }
        }

        public void TurnNext()
        {
            _turnNum++;
            if (_turnNum > 3)
            {
                ShowResult.instance.OnClick();
                gameObject.SetActive(false);
                _turnNum = 0;
                gameObject.transform.GetComponent<Image>().sprite = Tishi[_turnNum];
            }
            else
            {
                TurnLeft.gameObject.SetActive(true);
                gameObject.transform.GetComponent<Image>().sprite = Tishi[_turnNum];
            }
        } 
    }
}
