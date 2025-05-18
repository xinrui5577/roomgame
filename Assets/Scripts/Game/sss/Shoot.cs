using UnityEngine;
using System;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;


namespace Assets.Scripts.Game.sss
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
            _targetUser.Holes[_shootCount++].SetActive(true);
            Facade.Instance<MusicManager>().Play("shoot");
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
            foreach (GameObject hole in _targetUser.Holes)
            {
                hole.SetActive(false);
            }
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            _targetUser = null;
            OnFinish = null;
        }
    }
}

public struct ShootItem
{
    public int ShootSeat;
    public int TargetSeat;
}