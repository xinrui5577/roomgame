using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class PokerUnitScale : MonoBehaviour
    {
        public Vector3 ScaleValue;

        public void SetScalue()
        {
            transform.localScale = ScaleValue;
        }
    }
}