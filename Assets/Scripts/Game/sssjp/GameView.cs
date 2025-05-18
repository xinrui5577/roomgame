using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class GameView : MonoBehaviour
    {

        public virtual void OnClickCloseBtn()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnClickCloseObjBtn(GameObject obj)
        {
            obj.SetActive(false);
        }
    }
}
