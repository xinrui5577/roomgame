using Assets.Scripts.Game.Fishing.commons;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using Assets.Scripts.Game.Fishing.Factorys;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Fishing.windows
{
    public class FishInfoView : YxView
    {
        public FishDescriptionInfo InfoData;

        public Transform ItemContainer;
        public FishInfoItem ItemPrefable;
        /// <summary>
        /// 模型容器
        /// </summary>
        public Transform ModelContainer;
        /// <summary>
        /// 鱼的名称label
        /// </summary>
        public YxBaseLabelAdapter NameLabel;
        /// <summary>
        /// 倍数label
        /// </summary>
        public YxBaseLabelAdapter BetLabel;
        /// <summary>
        /// 描述label
        /// </summary>
        public YxBaseLabelAdapter DescLabel;

        /// <summary>
        /// 
        /// </summary>
        public ModelDisplayView ModelView;


        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var datas = InfoData.FishDatas;
            var len = datas.Length;
            var maxIndex = len - 1;
            for (var i = maxIndex; i > -1; i--)
            {
                var info = datas[i];
                var item = GameObjectUtile.Instantiate(ItemPrefable, ItemContainer);
                item.SetActive(true);
                item.name = i.ToString();
                item.UpdateView(info);
                item.SetSelected(i == maxIndex);
            }
        }

        public void OnChangeItem(FishInfoItem item)
        {
            var data = item.GetData<FishDescriptionData>();
            if (data == null) return;
            if (NameLabel != null)
            {
                NameLabel.Text(data.Name);
            }
            if (BetLabel != null)
            {
                if (data.MaxBet > 0)
                {
                    BetLabel.Text(string.Format("{0}-{1}", data.MinBet, data.MaxBet));
                }
                else if (data.MinBet > 0)
                {
                    BetLabel.Text(data.MinBet);
                }
                else
                {
                    BetLabel.Text("随机");
                }

            }
            if (DescLabel != null)
            {
                DescLabel.Text(data.Description);
            }
            LoadModel(data);
        }

        private GameObject _lastGameObject;
        private void LoadModel(FishDescriptionData data)
        {
            var dataFishId = data.FishId;
            if (_lastGameObject != null)
            {
                Destroy(_lastGameObject);
            }
            var pre = FishFactory.LoadFishModel(data.FishType, dataFishId);
            if (pre == null) return;
            _lastGameObject = GameObjectUtile.Instantiate(pre, ModelContainer);
            var childs = _lastGameObject.GetComponentsInChildren<Transform>(true);
            foreach (var tran in childs)
            {//遍历当前物体及其所有子物体
                tran.gameObject.layer = gameObject.layer;//更改物体的Layer层
            }
            var swimmer = _lastGameObject.GetComponent<Swimmer>();
            if (swimmer != null)
            {
                swimmer.enabled = false;
                var bodyTs = swimmer.Body.transform;
                bodyTs.eulerAngles = Vector3.zero;
                var pos = ModelContainer.position;
//                pos.z -= swimmer.Radius;
                bodyTs.position = pos;
                Destroy(swimmer);
            }
            var fish = _lastGameObject.GetComponent<Fish>();
            if (fish != null)
            {
                var skinRender = fish.SkinRenderer;
                if (skinRender != null)
                {
                    skinRender.sortingOrder = MainYxView.Order + 1000;
                }
            }
            var ts = _lastGameObject.transform;
            var scale = data.ModelScale;
            ts.localScale = new Vector3(scale, scale, scale);
        }

    } 
}
