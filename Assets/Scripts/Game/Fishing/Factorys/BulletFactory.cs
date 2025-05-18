using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Fishing.Factorys
{
    /// <summary>
    /// 子弹工厂
    /// </summary>
    public class BulletFactory : MonoBehaviour
    {

        private Transform _transform;
        //todo 池子
        void Awake()
        {
            _transform = transform;
            Facade.EventCenter.AddEventListener<EFishingEventType,BulletData>(EFishingEventType.CreateBullet, CreateLocalBullet); 
            Facade.EventCenter.AddEventListeners<EFishingEventType, Rect>(EFishingEventType.ResizeFishponBound, ResizeBound);
        }

        /// <summary>
        /// 创建子弹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Bullet CreateBullet(BulletData data)
        {
            var pre = data.PreFabBullet;
            if (pre == null) return null;
            var bulletGo = Instantiate(pre);
            var bulletTs = bulletGo.transform;
            bulletTs.SetParent(_transform);
            data.StartPos.z = _transform.position.z;
            bulletTs.position = data.StartPos;
            bulletTs.rotation = data.StartDirection;
            bulletTs.localScale = Vector3.one;
            bulletGo.SetActive(true);
            var bullet = bulletGo.GetComponent<Bullet>();
            bullet.Data = data;
            if (data.IsPenetrate)
            {
                bullet.CheckOutSide = CheckOutSide;
            }
            else
            {
                bullet.CheckOutSide = CheckRebound;
            }
            return bullet;
        }

        public void CreateLocalBullet(BulletData data)
        {
            CreateBullet(data);
        }
  
        private Rect _rect;
        protected void ResizeBound(Rect rect)
        {
            _rect = rect;
        }

        public void CheckOutSide(Bullet bullet)
        {
            var pos = bullet.transform.localPosition;
            if (pos.x < _rect.xMin || pos.x > _rect.xMax || pos.y < _rect.yMin || pos.y > _rect.yMax)
            {
                //存入缓存
                Destroy(bullet.gameObject);
                //告诉服务器
                var server = App.GetRServer<FishingGameServer>();
                var bulletData = bullet.Data;
                var bulletId = bulletData.Id;
                server.PostHitFishValidity(-1, bulletId, 0);
            }
        }

        public void CheckRebound(Bullet bullet)
        {
            var ts = bullet.transform;
            var pos = ts.localPosition;
            var eulerAngles = ts.localEulerAngles;
            if (pos.x < _rect.xMin)
            {
                eulerAngles.z = -eulerAngles.z;
                ts.localEulerAngles = eulerAngles;
                pos.x = _rect.xMin;
                ts.localPosition = pos;
            }
            if (pos.x > _rect.xMax)
            {
                eulerAngles.z = -eulerAngles.z;
                ts.localEulerAngles = eulerAngles;
                pos.x = _rect.xMax;
                ts.localPosition = pos;
            }
            if (pos.y < _rect.yMin)
            {
                eulerAngles.z = 180 - eulerAngles.z;
                ts.localEulerAngles = eulerAngles;
                pos.y = _rect.yMin;
                ts.localPosition = pos;
            }
            if (pos.y > _rect.yMax)
            {
                eulerAngles.z = 180 - eulerAngles.z;
                ts.localEulerAngles = eulerAngles;
                pos.y = _rect.yMax;
                ts.localPosition = pos;
            }
        }

    }
}
