using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class CardMachine : MonoBehaviour
    {
        public Animator ZhuanAni;

        //发牌器的转动
        public void CardMachinPlay()
        {
            ZhuanAni.Play("fapq");
        }
    }
}

