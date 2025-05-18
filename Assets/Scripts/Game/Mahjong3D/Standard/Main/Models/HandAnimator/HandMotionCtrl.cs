using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class MotionPositionItem
    {
        public HandMotionType AniType;
        public Vector3 Position;
    }

    public class HandMotionCtrl : MonoBehaviour
    {
        public HandMotionFsm PlayerHandFsm { get; set; }

        public Animator Animator { get; set; }

        public List<MotionPositionItem> Positions;

        private void Awake()
        {
            PlayerHandFsm = new HandMotionFsm();
        }

        private void Update()
        {
            if (PlayerHandFsm != null)
            {
                PlayerHandFsm.Update(Time.deltaTime, Time.time);
            }
        }

        public void PlayMotion(int chair, HandMotionType type)
        {
            if (Animator == null)
            {
                var userInfo = GameCenter.DataCenter.Players[chair];
                //0女, 其他男
                var assetsName = userInfo.SexI == 0 ? "HandGirl" : "HandBoy";
                var hand = GameUtils.GetInstanceAssets<GameObject>(assetsName);
                SetHand(hand);
            }
            SetHandPosition(type);
            PlayMotion(type);
        }

        private void SetHandPosition(HandMotionType type)
        {
            for (int i = 0; i < Positions.Count; i++)
            {
                var iten = Positions[i];
                if (type == iten.AniType)
                {
                    Animator.transform.localPosition = iten.Position;
                }
            }
        }

        private void PlayMotion(HandMotionType type)
        {
            var allMotions = PlayerHandFsm.GetAllStates();
            for (int i = 0; i < allMotions.Length; i++)
            {
                var fsm = allMotions[i] as BaseMotion;
                if (fsm.MotionType == type)
                {
                    var fsmType = fsm.GetType().Name.ToString();
                    PlayerHandFsm.ChangeState<BaseMotion>(fsmType);
                }
            }
        }

        private void SetHand(GameObject hand)
        {
            if (hand != null)
            {
                Animator = hand.GetComponent<Animator>();
                hand.transform.ExSetParent(transform);
                PlayerHandFsm.OnInit(Animator);
            }
        }
    }
}