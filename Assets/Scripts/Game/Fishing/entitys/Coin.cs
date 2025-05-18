using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 金币
    /// </summary>
    public class Coin : MonoBehaviour
    {
        public float Speed = 800;

        
        private Vector3 _endPos;
        private CoinData _coinData;
        private Transform _transform;

        void Awake()
        {
            Facade.Instance<MusicManager>().Play("sound_coin_1");
            _transform = transform;
        }

        private bool _needSound;
        public void Set(Vector3 endPosition,bool needSound= false)
        {
            _transform = transform;
            _endPos = endPosition;
            _needSound = needSound;
        }

        void Update()
        {
            _transform.position = Vector3.MoveTowards(_transform.position, _endPos, Speed * Time.deltaTime);
            if (_transform.position == _endPos)
            {
                if (_needSound)
                {
                    Facade.Instance<MusicManager>().Play("sound_coin_2");
                    Facade.EventCenter.DispatchEvent(EFishingEventType.FlyCoinFinished, _coinData);
                }
                Destroy(gameObject);
            }
        }

        public void SetData(CoinData coinData)
        {
            _coinData = coinData;
        }
    }
}
