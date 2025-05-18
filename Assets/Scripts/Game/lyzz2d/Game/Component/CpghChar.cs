using System.Collections;
using Assets.Scripts.Game.lyzz2d.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    /// <summary>
    ///     显示吃碰杠胡的特效，还包括了准备。。。。。好吧各种标记都是它
    /// </summary>
    public class CpghChar : MonoBehaviour
    {
        /// <summary>
        ///     漂提示
        /// </summary>
        [SerializeField] private UISprite _piaoSprite;

        /// <summary>
        ///     准备提示
        /// </summary>
        [SerializeField] private UISprite _readySprite;

        private int _showing;
        protected float BigScale = 3;
        protected float BigSpeed = 6;

        [SerializeField] public ParticleSystem[] cpgh; //0.chi 1.gang 2.hu 3.peng

        protected float Scale = 2f;
        protected float Speed = 0.01f;
        protected float Wait2 = 0.2f;

        public void SetBehavior(Enum_CPGType type)
        {
            var index = 0;
            switch (type)
            {
                case Enum_CPGType.Chi:
                    index = 0;
                    break;
                case Enum_CPGType.Peng:
                    index = 3;
                    break;
                case Enum_CPGType.ZhuaGang:
                case Enum_CPGType.PengGang:
                case Enum_CPGType.MingGang:
                case Enum_CPGType.AnGang:
                case Enum_CPGType.LaiZiGang:
                    index = 1;
                    break;
                case Enum_CPGType.Hu:
                    index = 2;
                    break;
                case Enum_CPGType.ZiMo:
                    index = 4;
                    break;
                case Enum_CPGType.MoBao:
                    index = 5;
                    break;
                case Enum_CPGType.PiaoHu:
                    index = 6;
                    break;
                case Enum_CPGType.ChongBao:
                    index = 7;
                    break;
                case Enum_CPGType.HuanBao:
                    index = 8;
                    break;
                case Enum_CPGType.NiuBiHu:
                    index = 9;
                    break;
                case Enum_CPGType.Ting:
                    index = 10;
                    break;
                case Enum_CPGType.Xst:
                    index = 11;
                    break;
                default:
                    return;
            }
            YxDebug.Log("播放特效是：" + cpgh[index].name);
            cpgh[index].gameObject.SetActive(true);
            cpgh[index].Stop();
            cpgh[index].Play();
            StartCoroutine(OnPlayFinish(index, 2));
        }

        private IEnumerator OnPlayFinish(int index, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            cpgh[index].Stop();
            cpgh[index].gameObject.SetActive(false);
        }

        public void ShowPiao(int piao)
        {
            _piaoSprite.gameObject.SetActive(true);

            switch (piao)
            {
                case 1:
                    _piaoSprite.spriteName = "piao" + piao;
                    break;
                case 99:
                    _piaoSprite.spriteName = "piao2";
                    break;
                case 0:
                case -1:
                    _piaoSprite.spriteName = "nopiao";
                    break;
            }
            _piaoSprite.MakePixelPerfect();
            StartCoroutine(ShowPiaoAni(_piaoSprite));
        }


        public void ShowReady()
        {
            if (gameObject.activeSelf)
            {
                _readySprite.gameObject.SetActive(true);
                _readySprite.spriteName = "ready";
                _readySprite.MakePixelPerfect();
                StartCoroutine(ShowPiaoAni(_readySprite));
            }
        }

        public void HideReady()
        {
            _readySprite.gameObject.SetActive(false);
        }

        private IEnumerator ShowPiaoAni(UISprite showSprite)
        {
            _showing++;
            var sx = showSprite.width;
            var mx = 0;
            while (sx > mx)
            {
                showSprite.width = mx;
                mx += (int) (600*Time.deltaTime);
                yield return 1;
            }
            showSprite.MakePixelPerfect();
            yield return new WaitForSeconds(3);
            _showing--;
            if (_showing < 1)
            {
                showSprite.gameObject.SetActive(false);
            }
        }
    }
}