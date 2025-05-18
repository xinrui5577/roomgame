using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.ttz
{
    public class MjCard : MonoBehaviour
    {
        public UISprite Target;
        public UISprite Mahjong;
        public ParticleSystem Effect;
        /// <summary>
        /// 牌值
        /// </summary>
        [HideInInspector]
        public int CardValue;


        public void TurnCard(bool isMing = false)
        {
            if (Mahjong == null) return;
            if (isMing)
            {
                ShowMjFront(true);
            }
            else
                StartCoroutine("Turn");
        }

        IEnumerator Turn()
        {
            var wait = new WaitForSeconds(0.05f);
            for (int i = 1; i < 7; i++)
            {
                Mahjong.spriteName = "turn" + i;
                Mahjong.MakePixelPerfect();
                if (i == 5 && Effect != null)
                {
                    Effect.Stop();
                    Effect.Play();
                }
                if (i == 6) ShowMjFront();
                yield return wait;
            }
        }

        /// <summary>
        /// 显示麻将正面
        /// </summary>
        protected void ShowMjFront(bool isMing = false)
        {
            if (!isMing) Facade.Instance<MusicManager>().Play("tuidao");
            var sprite = GetComponent<UISprite>();
            sprite.spriteName = "card";
            sprite.MakePixelPerfect();
            transform.localScale = Vector3.one;

            Target.spriteName = "A_" + CardValue;
            Target.gameObject.SetActive(true);
        }
    }
}