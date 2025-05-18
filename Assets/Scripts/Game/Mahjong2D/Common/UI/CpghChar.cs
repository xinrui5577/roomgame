using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Mahjong2D.Common.UI
{
    /// <summary>
    /// 显示吃碰杠胡的特效，还包括了准备。。。。。好吧各种标记都是它
    /// </summary>
    public class CpghChar : MonoBehaviour
    {
        [Tooltip("跟庄之后玩家的扣分显示")]
        public UILabel DeductMarksShow;
        [SerializeField]
        public ParticleSystem[] cpgh;//0.chi 1.gang 2.hu 3.peng
        [Tooltip("吃碰杠胡物体")]
        public GameObject[] CpghObjs;
        /// <summary>
        ///飘字开始位置
        /// </summary>
        public Transform FromPos;
        [Tooltip("显示效果:true使用CpghObj，false使用cpgh")]
        public bool ShowTweenAni;
        /// <summary>
        /// 飘字结束位置
        /// </summary>
        public Transform ToPos;
        public float MoveTime=1.5f;
        [Tooltip("特效显示时间 ")]
        public float EffectShowTime = 2;
        [Tooltip("tween时间")]
        public float TweenBehaviorTime=2;
        [Tooltip("特效播放事件")]
        public List<EventDelegate> EffectPlay=new List<EventDelegate>();

        private Coroutine _showCor;
        public void SetBehavior(EnumCpgType type)
        {
            int index;
            switch (type)
            {
                case EnumCpgType.Chi:
                    index = 0;
                    break;
                case EnumCpgType.Peng:
                    index = 3;
                    break;
                case EnumCpgType.ZhuaGang:
                case EnumCpgType.PengGang:
                case EnumCpgType.MingGang:
                case EnumCpgType.AnGang:
                case EnumCpgType.CaiGang:
                case EnumCpgType.LaiZiGang:
                    index = 1;
                    break;
                case EnumCpgType.Hu:
                    index = 2;
                    break;
                case EnumCpgType.ZiMo:
                    index = 4;
                    break;
                case EnumCpgType.MoBao:
                    index = 5;
                    break;
                case EnumCpgType.PiaoHu:
                    index = 6;
                    break;
                case EnumCpgType.ChongBao:
                    index = 7;
                    break;
                case EnumCpgType.HuanBao:
                    index = 8;
                    break;
                case EnumCpgType.NiuBiHu:
                    index = 9;
                    break;
                case EnumCpgType.Ting:
                    index = 10;
                    break;
                case EnumCpgType.Xst:
                    index = 11;
                    break;
               case EnumCpgType.QiangGangHu:
                    index = 12;
                    break;
                case EnumCpgType.ThreeFengGang:
                    index = 13;
                    break;
                case EnumCpgType.FourFengGang:
                    index = 14;
                    break;
                case EnumCpgType.LiGang:
                    index = 15;
                    break;
                default:
                    return;
            }
            if (_showCor!=null)
            {
                StopCoroutine(_showCor);
            }
            if (ShowTweenAni)
            {
                if (index >= CpghObjs.Length || index <= -1)
                {
                    return;
                }
                var showObj = CpghObjs[index];
                if (showObj)
                {
                    _showCor= StartCoroutine(OnPlayTweenEffect(showObj, EffectShowTime));
                }
            }
            else
            {
                if (index>=cpgh.Length||index<=-1)
                {
                    return;
                }
                var effect = cpgh[index];
                if (effect)
                {
                    _showCor= StartCoroutine(OnPlayEffect(effect, EffectShowTime));
                }
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(EffectPlay.WaitExcuteCalls());
            }
        }
        #region Effect
        private ParticleSystem _curEffect;
        private IEnumerator OnPlayEffect(ParticleSystem cpghEffect, float waitTime)
        {
            if (_curEffect)
            {
                ShowEffect(_curEffect, false);
            }
            _curEffect = cpghEffect;
            ShowEffect(cpghEffect, true);
            yield return new WaitForSeconds(waitTime);
            ShowEffect(_curEffect, false);
            _curEffect = null;
        }
        private void ShowEffect(ParticleSystem effect, bool show)
        {
            effect.Stop();
            effect.gameObject.TrySetComponentValue(show);
            if (show)
            {
                effect.Play();
            }
        }
        #endregion

        #region TweenEffect

        private GameObject _curBehavior;
        IEnumerator OnPlayTweenEffect(GameObject objeect,float showTime)
        {
            if (_curBehavior)
            {
                _curBehavior.TrySetComponentValue(false);
            }
            _curBehavior = objeect;
            _curBehavior.TrySetComponentValue(true);
            yield return new WaitForSeconds(TweenBehaviorTime);
            _curBehavior.TrySetComponentValue(false);
            _curBehavior = null;
        }

        #endregion


        /// <summary>
        /// 漂提示
        /// </summary>
        [SerializeField]
        private UISprite _piaoSprite;
        /// <summary>
        /// 准备提示
        /// </summary>
        [SerializeField]
        private UISprite _readySprite;

        public void ShowPiao(int piao)
        {
            _piaoSprite.gameObject.SetActive(true);

            switch (piao)
            {
                case 1:
                    switch ((EnumGameKeys)Enum.Parse(typeof(EnumGameKeys), App.GameKey))
                    {
                        case EnumGameKeys.fxmj:
                            int value = App.GetGameData<Mahjong2DGameData>().XiaZhiValue;
                            if (value == 5)
                            {
                                _piaoSprite.spriteName = "piao2";
                            }
                            else
                            {
                                _piaoSprite.spriteName = "piao" + piao;
                            }
                            break;
                        default:
                            _piaoSprite.spriteName = "piao" + piao;
                            break;
                    }
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
            if (gameObject.activeInHierarchy)
            {
                _readySprite.gameObject.SetActive(true);
                _readySprite.MakePixelPerfect();
            }
        }

        public void HideReady()
        {
            _readySprite.gameObject.SetActive(false);
        }

        private int _showing;
        private IEnumerator ShowPiaoAni(UISprite showSprite)
        {
            _showing++;
            int sx = showSprite.width;
            int mx = 0;
            while (sx > mx)
            {
                showSprite.width = mx;
                mx += (int)(600 * Time.deltaTime);
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
        /// <summary>
        /// 飘字移动效果tween
        /// </summary>
        private TweenPosition _pos;
        /// <summary>
        /// 飘字的渐变色效果
        /// </summary>
        private TweenAlpha _alpha;
        /// <summary>
        /// 显示飘字效果
        /// </summary>
        /// <param name="num">飘字显示的字</param>
        public void ShowPiaoLabel(int num)
        {
            DeductMarksShow.gameObject.TrySetComponentValue(true);
            var showNum = YxUtiles.GetShowNumber(num);
            DeductMarksShow.TrySetComponentValue(showNum > 0 ? "+" + showNum : showNum.ToString());
            PosAndAlpChange();
        }
        /// <summary>
        /// 飘字pos和alp的改变
        /// </summary>
        private void PosAndAlpChange()
        {
            DeductMarksShow.alpha = 0f;
            DeductMarksShow.transform.localPosition = FromPos.localPosition;
            _pos = TweenPosition.Begin(DeductMarksShow.gameObject, MoveTime, ToPos.localPosition);
            _alpha=TweenAlpha.Begin(DeductMarksShow.gameObject, MoveTime, 1f);
            Invoke("Finished", MoveTime);
        }

        private void Finished()
        {
            if (_pos)
            {
                DestroyImmediate(_pos);
            }
            if (_alpha)
            {
                DestroyImmediate(_alpha);
            }
            DeductMarksShow.gameObject.SetActive(false);
        }
    }
}
