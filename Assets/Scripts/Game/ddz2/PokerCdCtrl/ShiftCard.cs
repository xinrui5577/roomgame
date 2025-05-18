using UnityEngine;

namespace Assets.Scripts.Game.ddz2.PokerCdCtrl
{
    public class ShiftCard : MonoBehaviour 
    {
        #region UI Param


        private GameObject _lastCard;

        private bool _leftMove;

        private bool _move;
      

        /// <summary>
        /// 移动速度
        /// </summary>
        public float MoveSpeed = 1.5f;

        public EventDelegate OnCardLeftMove;


        #endregion

        #region Data Param

        [Tooltip("刷新时间")]
        public float FrameTime = 0.02f;

        #endregion

        #region Local Data

      

        #endregion

        #region Life Cycle

      

        public void SetLastObj(GameObject lastCard)
        {
            _lastCard = lastCard;
            _move = true;
            enabled = true;
            if (lastCard==null)
            {
                _leftMove = true;
                if (OnCardLeftMove!=null)
                {
                    OnCardLeftMove.Execute();
                }
                InvokeRepeating("CardMove", FrameTime, FrameTime);
            }
            else
            {
                _leftMove = false;
                transform.localPosition = _lastCard.transform.localPosition;
                InvokeRepeating("CardMove", 0, FrameTime);
            }
        }

        private void CardMove()
        {
            if (_move)
            {
                if (_leftMove)
                {
                    transform.localPosition -= new Vector3(MoveSpeed, 0, 0);
                }
                else
                {

                    if (_lastCard)
                    {
                        var lastV3 = _lastCard.transform.localPosition;

                        if (transform.localPosition.x + MoveSpeed - lastV3.x >= 64)
                        {
                            _leftMove = true;
                            transform.localPosition = lastV3 + new Vector3(64, 0, 0);

                            if (OnCardLeftMove != null)
                            {
                                OnCardLeftMove.Execute();
                            }
                        }
                        else
                        {
                            transform.localPosition += new Vector3(MoveSpeed, 0, 0);
                        }

                    }
                }
            }
        }



        #endregion

        #region Function

        public void CancelMove()
        {
            enabled = false;
            _move = false;
            CancelInvoke("CardMove");
        }

        #endregion
    }
}
