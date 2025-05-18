using Assets.Scripts.Game.biji.EventII;
using UnityEngine;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjTipView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject StartShow;
        public GameObject CompareShow;
        public UITexture TipShow;
        public UILabel TipLabel;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "StartGame":
                    OnStateShow(StartShow);
                    break;
                case "Compare":
                    OnStateShow(CompareShow);
                    break;
                case "Remind":
                    string desc = (string)data.Data;
                    OnTipShow(TipShow, desc);
                    break;
            }
        }
        private void OnStateShow(GameObject obj)
        {
            obj.SetActive(true);
            obj.transform.localScale = Vector3.zero;
            var item = TweenScale.Begin(obj, 0.8f, Vector3.one);
            if (item.onFinished.Count == 0)
            {
                item.AddOnFinished(() =>
                {
                    obj.SetActive(false);
                });
            }
        }
        private void OnTipShow(UITexture obj, string desc = "")
        {
            TipLabel.text = desc;
            obj.gameObject.SetActive(true);
            obj.color = new Color(1, 1, 1, 1);

            var item = TweenAlpha.Begin(obj.gameObject, 0.4f, 0);
            item.delay = 1.5f;
            if (item.onFinished.Count == 0)
            {
                item.AddOnFinished(() =>
                {
                    obj.gameObject.SetActive(false);
                });
            }
        }

    }
}