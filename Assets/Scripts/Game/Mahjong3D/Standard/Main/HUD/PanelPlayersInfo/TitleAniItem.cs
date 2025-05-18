using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TitleAniItem : MonoBehaviour
    {
        public Image Title;
        public GameObject[] Points;

        private ContinueTaskContainer mTask;
        private bool mFlag;

        public void Show(Sprite sprite)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i].gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
            Title.sprite = sprite;
            if (null == mTask)
            {
                mTask = ContinueTaskManager.NewTask().AppendFuncTask(() => PlayTitleAni());
            }
            mTask.Start();
            mFlag = true;
        }

        public void Hide()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i].gameObject.SetActive(false);
            }           
            gameObject.SetActive(false);
            mFlag = false;
        }

        private IEnumerator<float> PlayTitleAni()
        {
            while (mFlag)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    Points[i].gameObject.SetActive(false);
                }
                yield return 0.3f;
                Points[0].gameObject.SetActive(true);
                yield return 0.3f;
                Points[1].gameObject.SetActive(true);
                yield return 0.3f;
                Points[2].gameObject.SetActive(true);
                yield return 0.3f;
            }
        }
    }
}