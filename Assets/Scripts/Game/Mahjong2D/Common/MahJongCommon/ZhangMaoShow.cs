/** 
 *文件名称:     ZhangMaoShow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-16 
 *描述:         沈阳麻将中长毛的效果显示
 *历史记录: 
*/

using System.Collections;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class ZhangMaoShow :MonoSingleton<ZhangMaoShow>
    {
        [SerializeField]
        private GameObject _showParent;
        [SerializeField]
        private ParticleSystem _zhangMaoEffect;

        public override void Awake()
        {
            base.Awake();
            _showParent.SetActive(false);
        }

        public void ShowZhangMaoEffect()
        {
            if (_showParent)
            {
                _showParent.SetActive(true);
                _zhangMaoEffect.Stop();
                _zhangMaoEffect.Play();
                StartCoroutine(OnPlayFinish(2));
            }
        }
        private IEnumerator OnPlayFinish(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (_showParent)
            {
                _zhangMaoEffect.Stop();
                _showParent.SetActive(false);
            }
        }
    }


}
