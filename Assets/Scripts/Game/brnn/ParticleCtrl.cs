using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn
{
    public class ParticleCtrl : MonoBehaviour
    {
        public GameObject[] Target;
        public GameObject[] Effect;

        public GameObject SetParticleEffect(int effect, int target = 1)
        {
            return Instantiate(Target[target], Effect[effect]);
        }

        private GameObject Instantiate(GameObject target, GameObject cloned)
        {
            var temp = Instantiate(cloned);
            temp.transform.parent = target.transform;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localScale = new Vector3(1, 1, 1);
            temp.SetActive(true);
            return temp;
        }
    }
}
