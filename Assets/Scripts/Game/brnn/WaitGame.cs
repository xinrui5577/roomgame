using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class WaitGame : MonoBehaviour
    {
        public int CountTime = 0;
        public UILabel TextLabel;

        protected void Start()
        {
            InvokeRepeating("_waitTime", 0, 1);
        }

        protected void _waitTime()
        {
            if (CountTime <= 0)
            {
                CancelInvoke("_waitTime");
                gameObject.SetActive(false);
            }
            CountTime--;
            TextLabel.text = "本局游戏正在进行中，你最长需要等待" + CountTime + "秒...";

        }
    }
}
