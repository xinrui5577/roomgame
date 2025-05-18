using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class DeskCtrl : MonoBehaviour
    {

        public UISprite Ship;
        public UISprite Plane;
        public UISprite Tank;
        private int _indexs;
        private int _indexp;
        private int _indext;
 
        public void SetAnimation(int p)
        {
            switch (p)
            {
                case 0:
                    InvokeRepeating("AnimationShip", 0, 0.1f);
                    break;
                case 1:
                    InvokeRepeating("AnimationTank", 0, 0.1f);
                    break;
                case 2:
                    InvokeRepeating("AnimationPlane", 0, 0.1f);
                    break;
            }
        
        }

        private void AnimationShip()
        {

            Ship.color = _indexs % 2 == 0 ? new Color(1, 0.8f, 1, 1) : new Color(1, 1, 1, 1);
            _indexs ++;
            if (_indexs >= 81)
            {
                _indexs = 0;
                CancelInvoke("AnimationShip");
                Ship.color = new Color(1, 1, 1, 1);
            }
        }
        private void AnimationTank()
        {

            Tank.color = _indext % 2 == 0 ? new Color(1, 0.8f, 1, 1) : new Color(1, 1, 1, 1);
            _indext++;
            if (_indext >= 81)
            {
                _indext = 0;
                CancelInvoke("AnimationTank");
                Tank.color = new Color(1,1,1,1);
            }
        }
        private void AnimationPlane()
        {
            Plane.color = _indexp % 2 == 0 ? new Color(1, 0.8f, 1, 1) : new Color(1, 1, 1, 1);
            _indexp++;
            if (_indexp >= 81)
            {
                _indexp = 0;
                CancelInvoke("AnimationPlane");
                Plane.color = new Color(1,1,1,1);
            }
        }
    }
}
