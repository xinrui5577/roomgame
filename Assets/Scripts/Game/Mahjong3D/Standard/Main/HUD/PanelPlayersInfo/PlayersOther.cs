using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PlayersOther : MonoBehaviour
    {
        [HideInInspector]
        public Transform[] EffectPosGroup;
        public Sprite[] TitleSprites;
        public ObjContainer EffectPosContainer;
        public ObjContainer MiscObjectContainer;
        public ScoreGroup ScoreGroup;

        private List<GameObject> mReadys = new List<GameObject>();
        private List<TitleAniItem> mTitleAnis = new List<TitleAniItem>();

        public void GetContainer(int count)
        {
            EffectPosGroup = EffectPosContainer.GetComponent<Transform>(count);
            var group = MiscObjectContainer.GetComponent<Transform>(count);
            //收集对象
            GameObjectCollections array;
            for (int i = 0; i < group.Length; i++)
            {
                array = group[i].GetComponent<GameObjectCollections>();
                GameObject ready = array.Get("Ready");
                TitleAniItem aniItem = array.Get<TitleAniItem>("AniItem");
                if (null != ready) { mReadys.Add(ready); }
                if (null != aniItem) { mTitleAnis.Add(aniItem); }
            }
        }

        public Transform GetEffectPos(int chair)
        {
            if (chair > EffectPosGroup.Length) return null;
            return EffectPosGroup[chair];
        }

        public void PlayerOut(int chair)
        {
            mReadys[chair].SetActive(false);
            mTitleAnis[chair].Hide();
        }

        public void ReadyState(int chair, bool isOn)
        {
            if (isOn)
            {
                mTitleAnis[chair].Hide();
                mReadys[chair].SetActive(true);
                mReadys[chair].GetComponent<TweenPosition>().Do((cmp) =>
                {
                    cmp.ResetToBeginning();
                    cmp.PlayForward();
                });
            }
            else
            {
                mReadys[chair].SetActive(false);
                //自己不显示准备中动画
                if (chair != 0)
                {
                    mTitleAnis[chair].Show(TitleSprites[(int)PlayerStateFlagType.Readying]);
                }
            }
        }

        public void SetTitleFlag(int chair, int type)
        {
            mTitleAnis[chair].Show(TitleSprites[type]);
        }

        public void SetTitleFlag(int chair, Sprite sprite)
        {
            mTitleAnis[chair].Show(sprite);
        }

        public void HideReadyAni()
        {
            for (int i = 0; i < mReadys.Count; i++)
            {
                mReadys[i].SetActive(false);
            }
        }

        public void HideTitleAni()
        {
            for (int i = 0; i < mTitleAnis.Count; i++)
            {
                mTitleAnis[i].Hide();
            }
        }
    }
}