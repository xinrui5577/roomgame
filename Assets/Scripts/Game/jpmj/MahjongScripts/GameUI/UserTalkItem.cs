using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class UserTalkItem : MonoBehaviour {

        public GameObject Content;
        private float Interval = 5;

        private GameObject _content;
        private int _timerIndex;
        private float _minWidth = 125;
        private float _minHeight = 60;
        private void ResetSize(Rect content)
        {
            RectTransform rTf = GetComponent<RectTransform>();
            float width = content.size.x + Interval <= _minWidth + Interval ? _minWidth : content.size.x + Interval;
            float height = content.size.y + Interval <= _minHeight + Interval ? _minHeight : content.size.y + Interval;
            rTf.sizeDelta = new Vector2(width,height);
        }

        public void SetContent(GameObject obj)
        {
            gameObject.SetActive(true);
            if (_content != null)
            {
                Destroy(_content);
                DelayTimer.StopTimer(_timerIndex);
            }
            _content = obj;

            RectTransform rTf = obj.GetComponent<RectTransform>();
             rTf.parent = Content.transform;
            ResetSize(rTf.rect);

            rTf.localPosition = Vector3.zero;//new Vector3(-rTf.rect.size.x / 2 - Interval / 2, rTf.rect.size.y / 2 + Interval/2);
            rTf.localScale = Vector3.one;

            _timerIndex = DelayTimer.StartTimer(GameConfig.ChatShowTime, () =>
            {
                Reset();
            });
        }

        public void Reset()
        {
            Destroy(_content);
            _content = null;
            gameObject.SetActive(false);

        }

    }
}
