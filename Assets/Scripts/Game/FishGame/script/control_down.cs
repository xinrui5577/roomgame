using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class control_down : MonoBehaviour
    {
        public static control_down Instance;
        private bool ON = true;
        public Move_UP[] move;
        public Chain_zoom[] zoom;
        private float IntervalTime = 0.7f;
        private float ClickTime = 0;
        private bool record = true;

        void Awake()
        {
            Instance = this;
        }

        public void OnMouseCtlDown()
        { 
            if (record == false)
            {
                if (ClickTime + IntervalTime > Time.time)
                {
                    return;
                }
                record = true;
            }
            if (ON)
            {
                record = false ;
                ClickTime = Time.time;
                //Move_UP.Singlton.Play();
                var count = move.Length;
                for (var i = 0; i < count; i++)
                {
                    move[i].Play();
                }
                count = zoom.Length;
                for (var i = 0; i < count; i++)
                {
                    zoom[i].Play();
                }
                ON = false;
            }
            else
            {
                record = false;
                ClickTime = Time.time;
                var count = move.Length;
                for (var i = 0; i < count; i++)
                {
                    move[i].rePlay();
                }
                count = zoom.Length;
                for (var i = 0; i < count; i++)
                {
                    zoom[i].rePlay();
                }
                ON = true;
            }
            // Move_UP.
        } 
     
        /// <summary>
        /// 缩回
        /// </summary>
        public void Retract()
        {
            if (ON) return; 
            OnMouseCtlDown();
        }
    }
}
