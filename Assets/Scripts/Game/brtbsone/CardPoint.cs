using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;


namespace Assets.Scripts.Game.brtbsone
{
    public class CardPoint : MonoBehaviour
    {
        public GameObject Point;
        public GameObject Effect;
        public GameObject ErBaGang;

        protected List<GameObject> PointGameItems = new List<GameObject>();

        public void ShowPointValue(int type, int value)
        {

            if (type <= 2)
            {
                var pointItem = CreateItem(Point);
                UISprite cardsValue = pointItem.transform.FindChild("value").GetComponent<UISprite>();
                cardsValue.spriteName = value.ToString();//ParticleSystem
                var str = "dian" + value;
                Facade.Instance<MusicManager>().Play(str);
                var tween = pointItem.GetComponent<TweenScale>();
                if (tween != null) tween.PlayForward();
            }
            else if (type == 3)
            {
                var ebgItem = CreateItem(ErBaGang);
                Facade.Instance<MusicManager>().Play("erbagang");
                var tween = ebgItem.GetComponent<TweenScale>();
                if (tween != null) tween.PlayForward();
            }
            else if (type > 3)
            {
                var baoziItem = CreateItem(Effect);
                var effects = baoziItem.GetComponent<ParticleSystem>();
                effects.Play();
                Facade.Instance<MusicManager>().Play("duizi");
            }
        }

        protected GameObject CreateItem(GameObject go)
        {
            var temp = Instantiate(go);
            temp.transform.parent = transform;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localScale = Vector3.one;
            PointGameItems.Add(temp);
            return temp;
        }

        public void Init()
        {
            while (PointGameItems.Count != 0)
            {
                Destroy(PointGameItems[0]);
                PointGameItems.RemoveAt(0);
            }
        }
    }
}