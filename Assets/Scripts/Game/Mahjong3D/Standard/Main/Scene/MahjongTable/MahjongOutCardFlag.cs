using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongOutCardFlag : MahjongTablePart
    {
        public GameObject JianTou;
        public float StartPosY = 5.5f;
        public float OffectY = 8.5f;
        public float Timer = 0.8f;

        private Tweener mJianTowTweener;

        private void Awake()
        {
            JianTou.transform.ExLocalPosition(new Vector3(0, StartPosY, 0));
            mJianTowTweener = JianTou.transform.DOLocalMoveY(OffectY, Timer);
            mJianTowTweener.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            mJianTowTweener.SetAutoKill(false);
        }

        public void Show(MahjongContainer item)
        {
            transform.position = item.transform.position;
            JianTou.gameObject.SetActive(true);
            mJianTowTweener.Restart();
        }

        public void Hide()
        {
            mJianTowTweener.Pause();
            JianTou.gameObject.SetActive(false);
            transform.localPosition = Vector3.zero;
        }

        public override void OnReset()
        {
            Hide();
        }
    }
}