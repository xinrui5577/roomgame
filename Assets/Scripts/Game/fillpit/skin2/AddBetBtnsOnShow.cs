using UnityEngine;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class AddBetBtnsOnShow : MonoBehaviour
    {

        public UIGrid Grid;

        public UITweener Tweener;

        public UIWidget Widget;

        protected void OnEnable()
        {
            Resposition();
            PlayTweener();
        }

        /// <summary>
        /// 将显示的筹码重新排序
        /// </summary>
        void Resposition()
        {
            int areaX = Widget.width;
            int childrenCount = Grid.GetChildList().Count;     //只显示显示的子物体个数
            Grid.cellWidth = (float)areaX / childrenCount;
            Grid.Reposition();
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        void PlayTweener()
        {
            Tweener.ResetToBeginning();
            Tweener.PlayForward();
        }
    }
}
