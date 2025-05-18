using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.Compements
{
    public class GlidePanel : MonoBehaviour
    {
        public Vector3 OutPos;
        public Vector3 BackPos;
        public float Speed;
        public bool IsOn;

        private bool _isGlideing;
        private Vector3 _targetPos;

        public void OnGlide()
        {
            if (IsOn)
            {
                IsOn = false;
                _targetPos = BackPos;
            }
            else
            {
                IsOn = true;
                _targetPos = OutPos;
            }
            if (_isGlideing) return;
            _isGlideing = true;
            StartCoroutine(Glideing());
        }

        private IEnumerator Glideing()
        {
            Vector3 curPos;
            var curTime = 0f;
            do
            {
                curPos = Vector3.MoveTowards(transform.localPosition, _targetPos, curTime/Speed);
                transform.localPosition = curPos;
                curTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (curPos != _targetPos);
            _isGlideing = false;
        }
    }
}
