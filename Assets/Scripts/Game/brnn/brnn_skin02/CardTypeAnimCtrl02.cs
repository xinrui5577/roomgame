using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using YxFramwork.Manager;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.brnn.brnn_skin02
{
    public class CardTypeAnimCtrl02 : MonoBehaviour
    {
        public GameObject[] ShowObjs;

        public UISprite[] TypeSprites;

        private string _sprName = string.Empty;

        private string _soundName = string.Empty;

        public bool HideOnPlay = false;

        public TweenRotation RotateSpr;
        public Animator animtor;


        public void ShowObjects()
        {
            SetObjsActive(true);
        }

        void SetObjsActive(bool active)
        {
            if (ShowObjs == null || ShowObjs.Length <= 0)
                return;

            for (int i = 0; i < ShowObjs.Length; i++)
            {
                var obj = ShowObjs[i];
                obj.SetActive(active);

            }
        }


        public void RotateSprite()
        {
            if (RotateSpr == null)
                return;
            RotateSpr.enabled = true;
            animtor.enabled = false;
        }



        public void SetSpriteName(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
                return;

            _sprName = spriteName;
            if (TypeSprites == null || TypeSprites.Length <= 0)
                return;
            for (int i = 0; i < TypeSprites.Length; i++)
            {
                TypeSprites[i].spriteName = _sprName + i;
                TypeSprites[i].MakePixelPerfect();
            }
        }

        public void SetSoundName(string soundName)
        {
            _soundName = soundName;
        }



        public void PlaySound()
        {
            Facade.Instance<MusicManager>().Play(_soundName);
        }

        void OnDisable()
        {
            if (RotateSpr != null)
            {
                RotateSpr.enabled = false;
                animtor.enabled = true;
            }
        }
       

    }
}