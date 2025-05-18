using Assets.Scripts.Game.sssjp.Tool;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sssjp
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

        public Transform LabelPrefab;

        public float Area = 1120;


        public void InitItem(int round, int[] array)
        {
            RoundLabel.text = round.ToString();     //设置回合数
            gameObject.name = "Round_" + round;    //设置名字

            int count = array.Length;

            //设置label内容
            for (int i = 0; i < count; i++)
            {
                var tran = Instantiate(LabelPrefab);
                tran.parent = LabelPrefab.parent;
                tran.localScale = Vector3.one;
                var label = tran.GetComponent<UILabel>();
                if (label == null)
                    return;
                SetLabel(label, array[i]);
            }

            gameObject.SetActive(true);
            Grid.cellWidth = Area/count;
            Grid.hideInactive = true;
            Grid.repositionNow = true;
            Grid.Reposition();
        }

        void SetLabel(UILabel label, int score)
        {
            label.text = string.Format("{0}{1}", score >= 0 ? "+" : string.Empty, YxUtiles.ReduceNumber(score));
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