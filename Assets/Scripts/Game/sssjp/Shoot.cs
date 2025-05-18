using UnityEngine;
using System;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;


namespace Assets.Scripts.Game.sssjp
{
    public class Shoot : MonoBehaviour
    {
        private int _shootCount;

        public Action OnFinish;

        SssPlayer _targetUser;

        public void DoShoot(SssPlayer someone)
        {
            _targetUser = someone;
            gameObject.SetActive(true);
        }

        public void ShowHole()
        {
            if (_targetUser&& _targetUser.Holes&& _targetUser.Holes.childCount>_shootCount)
            {
                _targetUser.Holes.GetChild(_shootCount++).gameObject.SetActive(true);
                Facade.Instance<MusicManager>().Play("shoot");
            }
        }


        /// <summary>
        /// 在打枪动画中调用的方法
        /// </summary>
        public void CheckShoot()
        {
            if (_shootCount > 2)
            {
                HideHoles();
                if (OnFinish != null)
                    OnFinish();
                _shootCount = 0;
                gameObject.SetActive(false);
            }
        }

        void HideHoles()
        {
            foreach (Transform hole in _targetUser.Holes)
            {
                hole.gameObject.SetActive(false);
            }
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            _targetUser = null;
            OnFinish = null;
        }
    }

    public struct ShootItem
    {
        public int ShootSeat;
        public int TargetSeat;
    }
}

