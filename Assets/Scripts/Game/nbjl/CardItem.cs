using UnityEngine;

/*===================================================
 *文件名称:     CardItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:        	牌Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class CardItem : MonoBehaviour 
    {
        #region UI Param
        [Tooltip("牌面")]
        public UISprite CardSprite;
        #endregion

        #region Data Param
        [Tooltip("默认名称")]
        public string DefaultName="00";
        #endregion

        #region Local Data

        private string _setName;
        #endregion

        #region Life Cycle

        public void Play(int value)
        {
            gameObject.SetActive(true);
            _setName = value.ToString("x");
            CardSprite.spriteName =DefaultName;
            _right = false;
            _time = 0;
            _finished = false;
            InvokeRepeating("Tween", 0.2f, 0.03f);
        }

        public void QucikPlay(int value)
        {
            Reset();
            gameObject.SetActive(true);
            _setName = value.ToString("x");
            CardSprite.spriteName = _setName;
        }

        public void Reset()
        {
            CancelInvoke("Tween");
            transform.Rotate(Vector3.zero);
            CardSprite.spriteName = DefaultName;
            gameObject.SetActive(false);
        }

        private bool _right;
        private bool _finished;
        private int _time;
        private void Tween()
        {
            if (!_finished)
            {
                if (!_right)
                {
                    if (_time < 5)
                    {
                        transform.Rotate(new Vector3(0,18, 0));
                        _time++;
                    }
                    else
                    {
                        _right = true;
                        CardSprite.spriteName = _setName;
                    }
                }
                else
                {
                    if (_time >0)
                    {
                        transform.Rotate(new Vector3(0, - 18, 0));
                        _time--;
                    }
                    else
                    {
                        _finished = true;
                        CancelInvoke("Tween");
                    }
                }
            }


        }

        #endregion

        #region Function

        #endregion
    }
}
