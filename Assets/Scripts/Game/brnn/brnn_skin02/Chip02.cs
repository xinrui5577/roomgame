using UnityEngine;
using System.Collections;
using Assets.Scripts.Common.components;



namespace Assets.Scripts.Game.brnn.brnn_skin02
{
    public class Chip02 : Chip
    {
        [HideInInspector]
        public float DelayTime = 0;

        [SerializeField]
        private TweenPosition _tweenPosition;

   
        public void BeginAnim()
        {
            StartCoroutine(ChipDelay());
        }

        IEnumerator ChipDelay()
        {
            yield return new WaitForSeconds(DelayTime);
            Background.gameObject.SetActive(true);
            _tweenPosition.ResetToBeginning();
            _tweenPosition.PlayForward();
        }

        public void DestoryChip()
        {
            Destroy(gameObject);
        }
    }
}
