using UnityEngine;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 火花
    /// </summary>
    public class Spark:MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
            Invoke("Hide",0.1f);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
