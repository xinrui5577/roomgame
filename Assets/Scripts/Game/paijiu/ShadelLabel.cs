using UnityEngine;
#pragma warning disable 649


namespace Assets.Scripts.Game.paijiu
{
    public class ShadelLabel : MonoBehaviour
    {

        /// <summary>
        /// 主Label,显示在最上层
        /// </summary>
        [SerializeField]
        private UILabel _mainLabel;

        /// <summary>
        /// 影子Label,由从上到下顺序排列
        /// </summary>
        [SerializeField]
        private UILabel[] _shadelLabels;

        public void ShowLabel(string text)
        {
            SetLabel(text);
            gameObject.SetActive(true);
        }

        private void SetLabel(string text)
        {
            _mainLabel.text = text;
            SetShadelLabel(text);
        }

        private void SetShadelLabel(string text)
        {
            if (_shadelLabels == null || _shadelLabels.Length <= 0)
                return;

            int mainLabelDepth = _mainLabel.depth;
            for (int i = 0; i < _shadelLabels.Length; i++)
            {
                _shadelLabels[i].text = text;
                _shadelLabels[i].depth = mainLabelDepth - i - 1;
            }
        }


        public void HideLabel()
        {
            gameObject.SetActive(false);
        }
    }
}
