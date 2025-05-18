using UnityEngine;
using System.Collections;
using DG.Tweening;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.brnn3d
{
    public class Pai : MonoBehaviour
    {
        private int _area;
        private int _paiIndex;
        private int _paiLun;

        private Animator _paiAni;
        protected void Awake()
        {
            var tf = transform.Find("default");
            if (tf == null)
            {
                YxDebug.LogError("No Such Object");
            }
            else
            {
                _paiAni = tf.GetComponent<Animator>();
            }
            if (_paiAni == null)
            {
                YxDebug.LogError("No Such Component");
            }
        }
        //显示翻牌后的阶段
        public void Show(int index, int iArea, int _paiLun)
        {
            _area = iArea;
            this._paiLun = _paiLun;
            _paiIndex = index;
            StartCoroutine("WaitToShow", index * 0.58f);
        }

        IEnumerator WaitToShow(float s)
        {
            yield return new WaitForSeconds(s);
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            gameMgr.TheCardMachine.CardMachinPlay();
            yield return new WaitForSeconds(0.1f);
            var vor = transform.position;
            var paiMode = gameMgr.ThePaiMode;
            transform.position = paiMode.PaiFirstTf.position;
            transform.localScale = new Vector3(2.3f, 0.5f, 2.6f);
            var te = transform.DOMove(paiMode.PaiSecondTf.position, 0.1f);
            te.OnComplete(delegate ()
                {
                    transform.position = paiMode.PaiSecondTf.position;

                    Facade.Instance<MusicManager>().Play("sendcard");
                    Invoke("StopMusic", 5f);
                    var tw = transform.DOMove(vor, 0.5f);
                    tw.OnComplete(delegate ()
                    {
                        if (_paiIndex > 23)
                            paiMode.FanPaiFun();
                    });
                });
        }
        //音乐停止播放
        private void StopMusic()
        {
            Facade.Instance<MusicManager>().Stop();
        }
        //播放翻牌动画
        public void PlayFanPaiAni()
        {
            _paiAni.Play("fp");
        }

    }
}

