using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ScoreGroup : MonoBehaviour
    {
        public List<ScoreEffectItem> ScoreEffectList = new List<ScoreEffectItem>();

        public float ShowTime = 0;
        private float mTimer;
        private bool mFlag;

        public ScoreEffectItem this[int index]
        {
            get { return GetItemByChair(ScoreEffectList, index); }
        }

        public T GetItemByChair<T>(IList<T> list, int chair) where T : class
        {
            T item = null;
            if (chair >= list.Count) return null;
            int playerCount = GameCenter.DataCenter.MaxPlayerCount;
            switch (playerCount)
            {
                case 2:
                    {
                        if (chair == 0) { item = list[chair]; }
                        else { item = list[chair + 1]; }
                    };
                    break;
                case 3:
                    {
                        if (chair == 2) { item = list[chair + 1]; }
                        else { item = list[chair]; }
                    }
                    break;
                case 4: item = list[chair]; break;
            }
            return item;
        }

        private void Update()
        {
            if (mFlag)
            {
                mTimer += Time.deltaTime;
                if (mTimer >= ShowTime)
                {
                    Hide();
                }
            }
        }

        public void Play()
        {
            var cmp = GetComponent<TweenPosition>();
            cmp.ResetToBeginning();
            cmp.PlayForward();
            mFlag = true;
        }

        public void Hide()
        {
            mTimer = 0;
            mFlag = false;
            for (int i = 0; i < ScoreEffectList.Count; i++)
            {
                ScoreEffectList[i].Hide();
            }
        }
    }
}