using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{
    public class CardPoint : MonoBehaviour
    {

        public GameObject MaxPointMark;

        public UILabel CardPointLabel;

        public UILabel DifPointLabel;

        protected int OpenCardsPoint;

        protected int MaxPoint;

        public bool MaxMarkHaveAnim = false;


        /// <summary>
        /// 显示牌差值
        /// </summary>
        /// <param name="maxPoint"></param>
        public void SetMaxPoint(int maxPoint)
        {
            MaxPoint = maxPoint;
        }

        public void ShowDifPointLabel()
        {
            if (DifPointLabel == null) return;
            int openDifCv = OpenCardsPoint - MaxPoint;
            DifPointLabel.text = string.Format("({0}{1})", openDifCv >= 0 ? "+" : string.Empty, openDifCv);

            //最大牌值标记和分差是互斥的
            bool active = openDifCv >= 0;
            SetMaxPointMarkActive(active);
            SetDifPointLabelActive(!active);
        }


        protected virtual void SetMaxPointMarkActive(bool active)
        {
            if (MaxPointMark == null) return;

            if (MaxMarkHaveAnim)
            {
                var tweens = MaxPointMark.GetComponents<UITweener>();
                if (tweens != null && tweens.Length > 0)
                {
                    foreach (var tween in tweens)
                    {
                        if (active)
                            tween.PlayForward();
                        else
                            tween.PlayReverse();
                    }
                    return;
                }
            }
            
            var scale = active ? Vector3.one : new Vector3(0.0001f, 1, 1);
            MaxPointMark.transform.localScale = scale;
        }

        protected virtual void SetDifPointLabelActive(bool active)
        {
            if (DifPointLabel == null) return;
            var scale = active ? Vector3.one : new Vector3(0, 1, 1);
            DifPointLabel.transform.localScale = scale;
        }


        protected virtual void ShowMaxPointMark()
        {
            DifPointLabel.transform.localScale = new Vector3(0, 1, 1);

            if (MaxPointMark == null) return;
            if (MaxMarkHaveAnim)
            {
                var tweens = MaxPointMark.GetComponents<UITweener>();
                if (tweens == null || tweens.Length == 0)
                {
                    MaxPointMark.transform.localScale = Vector3.one;
                }
                else
                {
                    foreach (var tween in tweens)
                    {
                        tween.PlayForward();
                    }
                }
            }
            else
            {
                MaxPointMark.transform.localScale = Vector3.one;
            }
        }


        /// <summary>
        /// 设置明牌的点数
        /// </summary>
        /// <param name="point"></param>
        public void SetOpenCardsPoint(int point)
        {
            OpenCardsPoint = point;
            SetPointLabel(OpenCardsPoint);
        }

        public void SetPointLabel(int point)
        {
            CardPointLabel.text = point.ToString();
        }


        /// <summary>
        /// 设置点数的显示
        /// </summary>
        /// <param name="active"></param>
        public void SetCardPointActive(bool active)
        {
            gameObject.SetActive(active);
            if (active)
                ShowDifPointLabel();
        }
        
        /// <summary>
        /// 设置点数
        /// </summary>
        /// <param name="label"></param>
        /// <param name="point"></param>
        /// <param name="active"></param>
        protected void SetLabel(UILabel label, int point,bool active)
        {
            label.text = string.Format("+ {0}", point);
            label.gameObject.SetActive(active);
        }

    }
}
