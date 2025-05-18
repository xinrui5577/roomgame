using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{
    public class LaddyAnim : MonoBehaviour {


        public UISprite Sprite;

        public string CommonName = "anim{0}_";

        public string PlayerIdleName = "anim1_";

        public string NormalName = "anim0_0";

        public int MaxCount = 1;

        private bool _isPlaying;

        protected void Start()
        {
            StartCoroutine(PlayIdleAnim());
        }

        private IEnumerator PlayIdleAnim()
        {
            while (gameObject.activeSelf)
            {
                PlayAnim(PlayerIdleName);

                yield return new WaitForSeconds(Random.Range(5, 15));
            }
        }

        public void PlayClickAnim()
        {
            int max = MaxCount < 1 ? 1 : MaxCount;
            var index = Random.Range(0, max);
            string listName = string.Format(CommonName, index);
            PlayAnim(listName);
        }

        void PlayAnim(string listName)
        {
            if (_isPlaying) return;
            _isPlaying = true;
            var list = Sprite.atlas.GetListOfSprites(listName);
            if (list == null) return;
            StartCoroutine(PlayLaddyAnim(listName, list.size));
        }

        public float SpaceTime = 0.08f;

        IEnumerator PlayLaddyAnim(string listName, int len)
        {
            for (int i = 0; i < len; i++)
            {
                Sprite.spriteName = listName + i;
                Sprite.MakePixelPerfect();
                yield return new WaitForSeconds(SpaceTime);
            }
            Sprite.spriteName = NormalName;
            Sprite.MakePixelPerfect();
            _isPlaying = false;
        }

        protected void OnEnable()
        {
            StopAllCoroutines();
        }
    }
}
