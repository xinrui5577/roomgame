using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_DestroyDelay : MonoBehaviour {
        public float delay = 1F;
        // Use this for initialization
        IEnumerator Start () {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

    }
}
