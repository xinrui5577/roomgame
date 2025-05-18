using Assets.Scripts.Game.lswc.Data;
using DG.Tweening;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc
{
    public class LSFlyNumber : MonoBehaviour
    {
        public void Reset()
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// NGUI的tween不知道出什么问题了？，还是用dotween吧
        /// </summary>
        public void PlayAnimation()
        {
            var mySequence = DOTween.Sequence();
            gameObject.SetActive(true); 
            var moTo=new Vector3(0,transform.localPosition.y-75,0);
            var t1 = transform.DOLocalMove(moTo, 1);
            t1.SetEase(Ease.InOutElastic);
            t1.OnStart(PlayLabelDownVoice);
            var roTo = new Vector3(0, 180, -30);
            var t2 = transform.DORotate(roTo, 2);
            var cameraMgr = App.GetGameManager<LswcGamemanager>().CameraManager;
            var moTo2 = (cameraMgr.transform.position - transform.position) * Mathf.Cos(15 * Mathf.Deg2Rad) * 1.1f + transform.position;
            var t3 = transform.DOMove(moTo2, 3);
            t3.SetEase(Ease.OutExpo);
            t3.OnStart(PlayLabelOutVoice);
            t3.OnComplete(Reset);
            mySequence.Append(t1);
            mySequence.Append(t2);
            mySequence.AppendInterval(1);
            mySequence.Append(t3);
        }

        private void PlayLabelDownVoice()
        {
            App.GetGameManager<LswcGamemanager>().SystemControl.PlayVoice(LSConstant.LabelDown);
        }

        private void PlayLabelOutVoice()
        {
            App.GetGameManager<LswcGamemanager>().SystemControl.PlayVoice(LSConstant.LabelOut);
        }

    }
}
