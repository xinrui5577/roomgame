using UnityEngine;
using System;
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local


namespace Assets.Scripts.Game.duifen
{
    public class DelayHide : MonoBehaviour
    {
        public float DelayedTime = 2;

        public Action Onfinish;

        void OnEnable()
        {
            Invoke("HideObj", DelayedTime);
        }

        public void HideObj()
        {
            if(Onfinish != null)
            {
                Onfinish();
            }
            gameObject.SetActive(false);
        }

        void OnDisable()
        {
            CancelInvoke();
        }
        
    }
}