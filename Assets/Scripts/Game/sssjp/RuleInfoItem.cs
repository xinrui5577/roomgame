using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class RuleInfoItem : MonoBehaviour
    {

        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField] private UILabel _titel;

        /// <summary>
        /// 正文
        /// </summary>
        [SerializeField] private UILabel _content;

        /// <summary>
        /// UIlabel的大小
        /// </summary>
        public Vector2 Size
        {
            get { return _content.localSize; }
        }

        public void SetTitel(string info)
        {
            _titel.text = info + ":";
        }

        public void SetContent(string info)
        {
            _content.text = info.Replace(" ", "\n");
        }
    }
}