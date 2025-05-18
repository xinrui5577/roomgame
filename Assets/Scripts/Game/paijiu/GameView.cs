using UnityEngine;


namespace Assets.Scripts.Game.paijiu
{
    public class GameView : MonoBehaviour
    {
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="view"></param>
        public virtual void CloseView(GameObject view)
        {
            view.gameObject.SetActive(false);
        }

    }
}