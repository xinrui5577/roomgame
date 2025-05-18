using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.Ttzkf
{
    public class CoinFly : MonoBehaviour
    {
        public static bool AnimoEnable = true;
        public Transform Item;
        public int Num;
        public float MoveTime;
        public float DetaTime;
        public Transform ParenTransform;

        private Transform[] _items;
        private float[] _distincs;
        private int _index;
        private float _detaTime;
        private Vector3 _from;
        private Vector3 _to;
        private Vector3 _speed;
        private int _edx;
        private float _lenSq;//距离的平方

        public void FromTo(Vector3 from, Vector3 to, bool isGobal)
        {
            if (isGobal)
            {
                _from = transform.InverseTransformPoint(from);
                _to = transform.InverseTransformPoint(to);
            }
            else
            {
                _from = from;
                _to = to;
            }
            _items = new Transform[Num];
            _distincs = new float[Num];
            _index = 0;
            _edx = 0;
            Vector3 deta = (_to - _from);
            _speed = deta / MoveTime;
            _lenSq = deta.x * deta.x + deta.y + deta.y;
            enabled = true;
        }

        protected void Awake()
        {
            enabled = false;
        }

        protected void Update()
        {
            //产生新的item
            if (_index < Num)
            {
                _detaTime += Time.deltaTime;
                if (_detaTime >= DetaTime)
                {
                    Transform item = Instantiate(Item);
                    item.parent = ParenTransform;
                    item.localPosition = _from;
                    item.localScale = Item.localScale;
                    item.localRotation = Item.localRotation;
                    item.gameObject.SetActive(true);
                    _items[_index] = item;
                    _distincs[_index] = float.MaxValue;
                    _index++;
                }
            }
            //移动
            for (int i = _edx; i < _index; i++)
            {
                if (_items[i] == null) return;
                Vector3 v = _items[i].localPosition;
                v += _speed * Time.deltaTime;
                float d = (_to.x - v.x) * (_to.x - v.x) + (_to.y - v.y) * (_to.y - v.y);
                if (d > _distincs[i])
                {
                    _items[i].gameObject.SetActive(false);
                    _edx = i + 1;
                }
                else
                {
                    _distincs[i] = d;
                    float rng = 8f * d / _lenSq;
                    if (rng > 5f)
                    {
                        rng = 5f;
                    }
                    v.x += Random.Range(-rng, rng);
                    v.y += Random.Range(-rng, rng);
                    if (float.IsNaN(v.x)) v.x = 0;
                    if (float.IsNaN(v.y)) v.y = 0;
                    _items[i].localPosition = v;
                    //解决在游戏的中心出现一个小的筹码的情况
                    if (v == Vector3.zero)
                    {
                        _items[i].gameObject.SetActive(false);
                    }
                }
            }
            //结束
            if (!AnimoEnable || _edx >= Num)
            {
                Clear();
            }
        }

        public void Clear()
        {
            if (_items != null)
            {
                foreach (var go in _items.Select(item => item.gameObject).Where(go => go != null))
                {
                    Destroy(go);
                }
            }
            enabled = false;
            if (gameObject == null) return;
            Destroy(this);
        }
    }
}
