using Assets.Scripts.Game.sss.Tool;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sss
{
    public class SummaryItem : MonoBehaviour
    {

        /// <summary>
        /// 回合数
        /// </summary>
        public UILabel RoundLabel;

        /// <summary>
        /// Grid
        /// </summary>
        public UIGrid Grid;

        /// <summary>
        /// 总分数据
        /// </summary>
        public UILabel[] ScoreLabel;


        public void InitItem(int round, int[] array)
        {
            RoundLabel.text = round.ToString();     //设置回合数
            gameObject.name = "Round_" + round;    //设置名字

            //设置label内容
            for (int i = 0; i < array.Length; i++)
            {
                SetLabel(ScoreLabel[i], array[i]);
            }

            gameObject.SetActive(true);
            Grid.hideInactive = true;
            Grid.Reposition();
        }

        void SetLabel(UILabel label, int score)
        {
            label.text = YxUtiles.ReduceNumber(score);
            if (score < 0)
            {
                label.gradientTop = Tools.ChangeToColor(0x6FFBF1);
                label.gradientBottom = Tools.ChangeToColor(0x0090FF);
                label.effectColor = Tools.ChangeToColor(0x002EA3);
            }
            else
            {
                label.gradientTop = Tools.ChangeToColor(0xFFFF00);
                label.gradientBottom = Tools.ChangeToColor(0xFF9600);
                label.effectColor = Tools.ChangeToColor(0x831717);
            }

            label.gameObject.SetActive(true);
        }

    }
}