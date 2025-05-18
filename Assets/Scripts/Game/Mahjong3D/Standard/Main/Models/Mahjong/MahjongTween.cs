using System.Collections;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongTween : MonoBehaviour
    {
        public void Up(float duration = 0.02f)
        {
            var offset = DefaultUtils.MahjongSize.y * 0.5f + 0.1f;
            StartCoroutine(MoveYTo(offset, duration));
        }

        public void Up(float yOffset, float duration, Action action = null)
        {
            StartCoroutine(MoveYTo(yOffset, duration, 0, action));
        }

        public void Down(float duration = 0.02f, Action action = null)
        {
            var offset = DefaultUtils.MahjongSize.y * 0.5f;          
            StartCoroutine(MoveYTo(offset, duration, 0, action));
        }

        public void Transfrom(float xOffset, float duration, Action callback = null)
        {
            StartCoroutine(MoveXTo(xOffset, duration, 0, callback));
        }

        public virtual void Rotate(Vector3 to, float duration)
        {
            StartCoroutine(RotoTo(to, duration));
        }

        public virtual void RotateFrom(Vector3 from, Vector3 to, float duration)
        {
            transform.localRotation = Quaternion.Euler(from);
            StartCoroutine(RotoTo(to, duration));
        }

        public void StopAllTween()
        {
            StopAllCoroutines();            
        }

        private IEnumerator RotoTo(Vector3 to, float time, Action callback = null)
        {
            float val = 0;
            float bTime = Time.time;
            Quaternion fqua = transform.localRotation;
            Quaternion tquat = Quaternion.Euler(to);
            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;

                transform.localRotation = Quaternion.Lerp(fqua, tquat, smoothval);
                yield return 2;
            }
            if (callback != null) callback();
        }

        private IEnumerator MoveXTo(float moveX, float time, float delayTime = 0, Action callback = null)
        {
            yield return new WaitForSeconds(delayTime);

            float val = 0;
            float bTime = Time.time;
            float oldX = transform.localPosition.x;
            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;
                var fpos = new Vector3(oldX, transform.localPosition.y, transform.localPosition.z);
                var tpos = new Vector3(moveX, transform.localPosition.y, transform.localPosition.z);
                transform.localPosition = Vector3.Lerp(fpos, tpos, smoothval);
                yield return 2;
            }           
            if (callback != null) callback();
        }

        private IEnumerator MoveYTo(float moveY, float time, float delayTime = 0, Action callback = null)
        {
            yield return new WaitForSeconds(delayTime);

            float val = 0;
            float bTime = Time.time;
            float oldY = transform.localPosition.y;
            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;
                var fpos = new Vector3(transform.localPosition.x, oldY, transform.localPosition.z);
                var tpos = new Vector3(transform.localPosition.x, moveY, transform.localPosition.z);
                transform.localPosition = Vector3.Lerp(fpos, tpos, smoothval);
                yield return 2;
            }           
            if (callback != null) callback();
        }
    }
}