using UnityEngine;
using System.Collections;


namespace Assets.Scripts.Game.brnn.brnn_skin02
{
    public class MoveCard02 : MonoBehaviour
    {

        [SerializeField]
        private float _moveArea;


        [SerializeField]
        private float _moveSpeed;


        private bool _isMoving = false;

        public void BeginMoveCard()
        {
            if(!_isMoving)
            {
                _isMoving = true;
                StartCoroutine(MoveCard());
            }
        }


        IEnumerator MoveCard()
        {

            float area = 0;
            Vector3 v3 = transform.localPosition;
            do
            {
                area += _moveSpeed;
                transform.localPosition = new Vector3(v3.x + area, v3.y, v3.z);
                yield return null;
            } while (_isMoving && area < _moveArea);
            transform.localPosition = new Vector3(v3.x + _moveArea, v3.y, v3.z);

            do
            {
                area -= _moveSpeed;
                transform.localPosition = new Vector3(v3.x + area, v3.y, v3.z);
                yield return null;
            } while (_isMoving && area > 0);
            transform.localPosition = v3;

            _isMoving = false;
            StopCoroutine(MoveCard());
        }
        
    }
}