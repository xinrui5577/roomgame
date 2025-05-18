using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

//最后操作的牌上面的小点指示

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.CardPoint
{
    public class CardPoint :MonoSingleton<CardPoint>
    {
        private Transform _mjItem;
        public int Offset = 20;
        public float Speed = 40f;
        public int Distance = 10;
        private float _len;
        private int _direction = 1;
        public void SetMahjongItem(MahjongItem item)
        {
            if (item)
            {
                _mjItem = item.transform;
                transform.parent = _mjItem;
                transform.localScale = Vector3.one;
                enabled = true;
                gameObject.SetActive(true);
            }
            else
            {
                _mjItem = null;
                enabled = false;
                gameObject.SetActive(false);
               
            }
        }

        public void Update()
        {
            if (_mjItem && _mjItem.gameObject.activeSelf)
            {
                Vector3 vStart = Vector3.zero;
                vStart.y += 5;
                Vector3 v3 = vStart;
                _len += Speed * Time.deltaTime * _direction;
                if (_len > Distance)
                {
                    _direction = -1;
                }
                else if (_len < 0)
                {
                    _direction = 1;
                } 
                v3.y += _len + Offset;
                transform.localPosition = v3;
            }
            else
            {
                enabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}
