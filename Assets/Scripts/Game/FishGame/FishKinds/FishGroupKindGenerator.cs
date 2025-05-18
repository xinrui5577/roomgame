using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.FishKinds
{
    public class FishGroupKindGenerator : KindGenerator
    {
        /// <summary>
        /// 数据
        /// </summary>
        public GroupGenerateData[] Datas; 

        public override void Init()
        {
            WeightTotal = 0;
            Count = BeginId;
            var count = Datas.Length;
            for (var i = 0; i < count; i++)
            {
                var data = Datas[i];
                data.Id = Count++;
                WeightTotal += data.Weight;
                data.HittedWeight = WeightTotal;
            }
        }
        /// <summary>
        /// 生成鱼后限制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool BehindLimit(GroupGenerateData data)
        {
            return false;
        }

        /// <summary>
        /// 随机生成
        /// </summary>
        /// <returns></returns> 
        protected virtual GroupGenerateData RandomData()
        {
            var curWeight = Random.Range(0, WeightTotal);
            return Datas.FirstOrDefault(data => curWeight < data.HittedWeight);
        } 

        //创建
        public override IEnumerator OnGenerate()
        { 
            while (IsRun)
            {
                var prefab = Limit();
                if (prefab != null)
                {
                    prefab.KindGenerator = this;
                    Generate(prefab); 
                }
                yield return new WaitForSeconds(Interval.Value);//cd时间 
            }
        }

        public virtual void Generate(GroupGenerateData data)
        {
            var groupData = Instantiate(data);
            groupData.KindGenerator = this;
            groupData.transform.parent = Generator.transform; 
            groupData.Generate();
        }

        /// <summary>
        /// 限制条件
        /// </summary>
        /// <returns></returns>
        protected GroupGenerateData Limit()
        {
            if (Generator.IsOverstepFishAliveMax()) return null;
            if (FrontLimit()) return null;
            var data = RandomData();
            if (data == null) return null;
            return BehindLimit(data) ? null : data;
        } 
         
    }
}
