using UnityEngine;


namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ZhaniaoEvent : MonoBehaviour
    {
        public GameObject Card;

        public void AnimationEvent()
        {
            Card.gameObject.SetActive(true);
        }
    }
}