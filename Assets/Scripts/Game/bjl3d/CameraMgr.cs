using UnityEngine;

namespace Assets.Scripts.Game.bjl3d
{
    public class CameraMgr : MonoBehaviour
    {
        public CameraPathBezierAnimator[] PathAnimator;

        private CameraPathBezierAnimator _playAni;

        public void CameraMoveByPath(int pathId)
        {
            if (_playAni != null)
                _playAni.Stop();
            _playAni = PathAnimator[pathId];
            if (_playAni != null)
            {
                _playAni.Play();
            }
        }
    }
}

