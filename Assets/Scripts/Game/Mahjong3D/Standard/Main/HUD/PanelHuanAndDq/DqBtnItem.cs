using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DqBtnItem : MonoBehaviour
    {
        public ParticleSystem Particle;

        public void ShowEffect()
        { 
            gameObject.SetActive(true);
            Particle.Play();
        }
    }
}