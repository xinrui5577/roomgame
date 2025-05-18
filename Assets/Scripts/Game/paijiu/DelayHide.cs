using UnityEngine;
using System;

namespace Assets.Scripts.Game.paijiu
{
    public class DelayHide : MonoBehaviour
    {
        public float DelayedTime = 2;

        public Action Onfinish;

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            Invoke("HideObj", DelayedTime);
        }

        public void HideObj()
        {
            if (Onfinish != null)
            {
                Onfinish();
            }
            gameObject.SetActive(false);
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            CancelInvoke();
        }

    }
}