using System.Linq;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.FishKinds;
using Assets.Scripts.Game.FishGame.Fishs;
using Sfs2X.Entities.Data;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

//using DictFish = System.Collections.Generic.Dictionary<int,Fish>;
namespace Assets.Scripts.Game.FishGame.FishGenereate
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>不要将go作为该go的child,因为清鱼时会清除</remarks>
    public class FishGenerator : MonoBehaviour
    {
        private struct Segment
        {
            public Segment(Vector2 p1, Vector2 p2)
            {
                P1 = p1;
                P2 = p2;
            }

            public Vector2 P1;
            public Vector2 P2;

            public Vector2 Direct()
            {
                return P2 - P1;
            }
        }

        public FishGeneratConfig Config;
        private int _initState = 0;
        private bool _isStartGenerat = false;
        //public FloatRnd Interval_FishBomb;
        //最大鱼数限制
        public int MaxFishAtUnitWorld = 100; //单一屏幕出鱼数
        [System.NonSerialized] public int MaxFishAtWorld = 100;

        public bool IsGenerate = true;
        private Rect mWorldDim; //来至于GameMain.WorldDimension; 

        [System.NonSerialized] public Dictionary<int, Dictionary<int, Fish>> FishTypeIndexMap;
                                                                             //Fish.TypeIndex为数组索引,为同类鱼服务

        [System.NonSerialized] public List<Fish> FishLockable; //可被锁定鱼

        public Dictionary<int, Swimmer> LeadersAll; //所有领队(主要用于定身炸弹)

        public Queue<Fish> mUniqueFishToGenerate; //唯一鱼生成队列
        public Dictionary<int, Fish> SameTypeBombs; //同类炸弹
        private RandomFloat[] _randomFloat;

        public void Init(ISFSObject gameInfo)
        {
            //interval,0.2:0.3#2:4#2:4#3:5#8:20
            if (!gameInfo.ContainsKey("interval")) return;
            var info = gameInfo.GetUtfString("interval");
            var intervals = info.Split('#');
            var len = intervals.Length;
            _randomFloat = new RandomFloat[len];
            for (var i = 0; i < len; i++)
            {
                var interval = intervals[i];
                var minMax = interval.Split(':');
                if(minMax.Length<2)continue;
                var rf = new RandomFloat();
                float.TryParse(minMax[0], out rf.Min);
                float.TryParse(minMax[1], out rf.Max);
                _randomFloat[i] = rf;
            }
            SetInterVal();
        }

        public void SetInterVal()
        {
            if (Config == null) return;
            if(_randomFloat == null || _randomFloat.Length<1)return;
            var generators = Config.Generators;
            var count = Mathf.Min(generators.Length,_randomFloat.Length);
            for (var i = 0; i < count; i++)
            {
                var gtor = generators[i];
                if(gtor==null)continue;
                gtor.Interval = _randomFloat[i];
            }
        }

        public void Handle_LeaderInstance(Swimmer s)
        {
            //Debug.Log(s.name + " instance",s.gameObject);
            //Debug.Log("LeadersAll.count = " + LeadersAll.Count);
            LeadersAll.Add(s.GetInstanceID(), s);
            s.EvtSwimOutLiveArea += () => LeadersAll.Remove(s.GetInstanceID());
        }

        public void Handle_InstanceFish(Fish f)
        {
            if (!FishTypeIndexMap.ContainsKey(f.TypeIndex))
                FishTypeIndexMap[f.TypeIndex] = new Dictionary<int, Fish>();
            FishTypeIndexMap[f.TypeIndex].Add(f.GetInstanceID(), f);

            if (f.IsLockable)
                FishLockable.Add(f);

            //if (f.HittableTypeS == "SameTypeBomb")
            if (f.HittableType == HittableType.SameTypeBomb)
            {
                SameTypeBombs.Add(f.GetInstanceID(), f);
            }
        }

        /// <summary>
        /// 清除小鱼
        /// </summary>
        /// <param name="f"></param>
        private void Handle_ClearFish(Fish f)
        {
            var type = f.TypeIndex;
            if (FishTypeIndexMap.ContainsKey(type))
            {
                FishTypeIndexMap[type].Remove(f.GetInstanceID());
            }


            if (f.IsLockable)
                FishLockable.Remove(f);

            //if (f.HittableTypeS == "SameTypeBomb")
            if (f.HittableType == HittableType.SameTypeBomb)
            {
                SameTypeBombs.Remove(f.GetInstanceID());
            }
        }

        /// <summary>
        /// 线段相交
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="interectPot"></param>
        /// <returns></returns>
        /// <remarks>

        #region 参考1

        //    已知一条线段的两个端点为A(x1,y1),B(x2,y2),另一条线段的两个端点为C(x3,y3),D(x4,y4),要判断AB与CD是否有交点，若有求出. 
        //        首先判断d   =   (y2-y1)(x4-x3)-(y4-y3)(x2-x1)， 
        //  若d=0，则直线AB与CD平行或重合， 
        //  若d!=0，则直线AB与CD有交点，且交点(x0,y0)为： 
        //        x0   =   [(x2-x1)*(x4-x3)*(y3-y1)+(y2-y1)*(x4-x3)*x1-(y4-y3)*(x2-x1)*x3]/d 
        //        x0   =   [(y2-y1)*(y4-y3)*(x3-x1)+(x2-x1)*(y4-y3)*y1-(x4-x3)*(y2-y1)*y3]/(-d) 
        //求出交点后在判断交点是否在线段上，即判断以下的式子： 
        //        (x0-x1)*(x0-x2) <=0 
        //        (x0-x3)*(x0-x4) <=0 
        //        (y0-y1)*(y0-y2) <=0 
        //        (y0-y3)*(y0-y4) <=0 
        //只有上面的四个式子都成立才可判定(x0,y0)是线段AB与CD的交点，否则两线段无交点

        #endregion

        #region 参考2

        //第一条直线   
        //double x1 = 10, y1 = 20, x2 = 100, y2 = 200;    
        //double a = (y1 - y2) / (x1 - x2);   
        //double b = (x1 * y2 - x2 * y1) / (x1 - x2);   
        //System.out.println("求出该直线方程为: y=" + a + "x + " + b);   

        ////第二条   
        //double x3 = 50, y3 = 20, x4 = 20, y4 = 100;   
        //double c = (y3 - y4) / (x3 - x4);   
        //double d = (x3 * y4 - x4 * y3) / (x3 - x4);   
        //System.out.println("求出该直线方程为: y=" + c + "x + " + d);   

        //double x = ((x1 - x2) * (x3 * y4 - x4 * y3) - (x3 - x4) * (x1 * y2 - x2 * y1))   
        //    / ((x3 - x4) * (y1 - y2) - (x1 - x2) * (y3 - y4));   

        //double y = ((y1 - y2) * (x3 * y4 - x4 * y3) - (x1 * y2 - x2 * y1) * (y3 - y4))   
        //    / ((y1 - y2) * (x3 - x4) - (x1 - x2) * (y3 - y4));   

        //System.out.println("他们的交点为: (" + x + "," + y + ")");  

        #endregion

        /// </remarks>
        private bool InterectSegment(Segment s1, Segment s2, out Vector2 interectPot)
        {
            Vector3 crossResult = Vector3.Cross(s1.Direct(), s2.Direct());
            if (crossResult.z == 0F)
            {
                interectPot = Vector2.zero;
                return false;
            }

            float x1 = s1.P1.x, x2 = s1.P2.x, x3 = s2.P1.x, x4 = s2.P2.x;
            float y1 = s1.P1.y, y2 = s1.P2.y, y3 = s2.P1.y, y4 = s2.P2.y;
            interectPot.x = ((x1 - x2)*(x3*y4 - x4*y3) - (x3 - x4)*(x1*y2 - x2*y1))/
                            ((x3 - x4)*(y1 - y2) - (x1 - x2)*(y3 - y4));
            interectPot.y = ((y1 - y2)*(x3*y4 - x4*y3) - (x1*y2 - x2*y1)*(y3 - y4))/
                            ((y1 - y2)*(x3 - x4) - (x1 - x2)*(y3 - y4));


            if ((interectPot.x - s1.P1.x)*(interectPot.x - s1.P2.x) <= 0
                && (interectPot.y - s1.P1.y)*(interectPot.y - s1.P2.y) <= 0
                && (interectPot.x - s2.P1.x)*(interectPot.x - s2.P2.x) <= 0
                && (interectPot.y - s2.P1.y)*(interectPot.y - s2.P2.y) <= 0
                )
            {
                return true;
            }
            return false;
        }

        public Vector2 RndPosInWorldRect()
        {
            return new Vector2(Random.Range(mWorldDim.xMin, mWorldDim.xMax),
                               Random.Range(mWorldDim.yMin, mWorldDim.yMax));
        }

        /// <summary>
        /// 从原点发出射线与世界边框相交
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="interectPot"></param>
        /// <returns></returns>
        public bool InterectWorldRectWithOriginRay(Vector2 rayDir, out Vector2 interectPot)
        {
            //Segment s2 = new Segment(Vector2.zero, rayDir * 10000F);
            if (rayDir.y > 0)
            {
                if (rayDir.x < 0)
                    return Interect2WorldRect(0, 1, rayDir, out interectPot); //左上
                else
                    return Interect2WorldRect(1, 2, rayDir, out interectPot); //右上
            }
            else
            {
                if (rayDir.x < 0)
                    return Interect2WorldRect(0, 3, rayDir, out interectPot); //左下
                else
                    return Interect2WorldRect(2, 3, rayDir, out interectPot); //向下
            }
        }

        /// <summary>
        /// 与指定的两条线段相交测试
        /// </summary>
        /// <param name="idx1"></param>
        /// <param name="idx2"></param>
        /// <param name="rayDir"></param>
        /// <param name="interectPot"></param>
        /// <returns></returns>
        private bool Interect2WorldRect(int idx1, int idx2, Vector2 rayDir, out Vector2 interectPot)
        {
            Segment s2 = new Segment(Vector2.zero, rayDir*10000F);
            if (InterectSegment(mWorldRects[idx1], s2, out interectPot))
                return true;
            if (InterectSegment(mWorldRects[idx2], s2, out interectPot))
                return true;
            return false;
        }

        private Segment[] mWorldRects;


        private void Awake()
        {
            //Rect worldRect 
            mWorldDim = GameMain.Singleton.WorldDimension;
            mWorldRects = new Segment[4];
            mWorldRects[0] = new Segment(new Vector2(mWorldDim.x, mWorldDim.yMax)
                                         , new Vector2(mWorldDim.x, mWorldDim.y)); //左

            mWorldRects[1] = new Segment(new Vector2(mWorldDim.x, mWorldDim.yMax)
                                         , new Vector2(mWorldDim.xMax, mWorldDim.yMax)); //上

            mWorldRects[2] = new Segment(new Vector2(mWorldDim.xMax, mWorldDim.yMax)
                                         , new Vector2(mWorldDim.xMax, mWorldDim.y)); //右

            mWorldRects[3] = new Segment(new Vector2(mWorldDim.xMax, mWorldDim.y)
                                         , new Vector2(mWorldDim.x, mWorldDim.y)); //下

            

            FishTypeIndexMap = new Dictionary<int, Dictionary<int, Fish>>();
            FishLockable = new List<Fish>();
            LeadersAll = new Dictionary<int, Swimmer>();
            mUniqueFishToGenerate = new Queue<Fish>();
            SameTypeBombs = new Dictionary<int, Fish>(); 
        }

        public void Start()
        {
            var numScreen = GameMain.Singleton.ScreenNumUsing;
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtFishInstance += Handle_InstanceFish;
            gdata.EvtFishClear += Handle_ClearFish;
            gdata.EvtLeaderInstance += Handle_LeaderInstance;
            LoadConfig();
            MaxFishAtWorld = MaxFishAtUnitWorld*numScreen;
            SetInterVal();
        }

        public void LoadConfig()
        {
            if (Config != null) return;
            var asset = ResourceManager.LoadAsset("FishGeneratConfig");
            InitConfig(asset);
        }

        private void InitConfig(Object asset)
        {
            var go = asset as GameObject;
            if (go == null)
            {
                YxDebug.LogError("FishGenerator no config");
                return;
            }
            YxDebug.Log("Load Finished FishGenerator config");
            Config = go.GetComponent<FishGeneratConfig>();
            var generators = Config.Generators;
            var count = generators.Length;
            for (var i = 0; i < count; i++)
            {
                generators[i].Init(); //保证normal鱼最先初始化，可以保证鱼有typeindext
            }
            MaxFishAtUnitWorld = Config.MaxFishAtUnitWorld; 
            OnGenerate();
        }

        /// <summary>
        /// 开始出鱼
        /// </summary>
        public void StartFishGenerate()
        { 
            if (!IsGenerate)
                return;
            _isStartGenerat = true; 
            OnGenerate();
        }

        private void OnGenerate()
        {
            _initState++;
            if (_initState < 2) return;
            if (!_isStartGenerat) return;
            var generators = Config.Generators;
            var count = generators.Length; 
            foreach (var generator in generators)
            {
                generator.StartGenerate(this);
                StartCoroutine(generator.OnGenerate());
            }
        }

        /// <summary>
        /// 停止出鱼
        /// </summary>
        public void StopFishGenerate()
        {
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtStopGenerateFish != null)
                gdata.EvtStopGenerateFish();
            _isStartGenerat = false; 
            if (Config == null) return;
            var generators = Config.Generators;
            var count = generators.Length;
            foreach (var generator in generators)
            {
                generator.Stop();
            }
        }

        /// <summary>
        /// 杀死所有鱼
        /// </summary>
        public void KillAllImmediate()
        {
            var values = FishTypeIndexMap.Values;
            var fishToClear =
                (from dictFishType in values
                 where dictFishType != null
                 from f in dictFishType.Values
                 where f != null
                 select f).ToList();
            foreach (var f in fishToClear.Where(f => f.Attackable))
            {
                f.Clear();
            }
        }

        /// <summary>
        /// 是否超出鱼的最大数量
        /// </summary>
        /// <returns></returns>
        public bool IsOverstepFishAliveMax()
        {
            var curNum = GameMain.Singleton.NumFishAlive;
            return curNum >= MaxFishAtWorld;
        }
         
        public Fish GetFishPrefab(int index)
        {
            if (Config == null || Config.NormalGenerator == null) return null;
            return Config.NormalGenerator.Datas[index].FishPerfab;
        }
#if UNITY_EDITOR
       /* private void OnGUI()
        {
            if (GUILayout.Button("interatect"))
            {
                Vector2 interectPot;
                Vector3 rndPos = Random.onUnitSphere;
                if (InterectWorldRectWithOriginRay(rndPos, out interectPot))
                {
                    GameObject goNew = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    goNew.transform.localScale = Vector3.one*0.4F;
                    rndPos.z = 0F;
                    rndPos.Normalize();
                    goNew.transform.position = new Vector3(rndPos.x*3F, rndPos.y*3F, -10F);

                    GameObject goInterect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    goInterect.transform.localScale = Vector3.one*0.2F;
                    goInterect.transform.position = new Vector3(interectPot.x, interectPot.y, -10F);

                }
            }
            if (GUILayout.Button("清鱼"))
            {
                KillAllImmediate();
            } 
        }*/
         
    /*    private bool _isChange;
        private void Update()
        { 
            if (Config == null) return;
            if(_isChange == IsGenerate)return;
            if (IsGenerate)
            {
                Debug.Log("出鱼 Update ");
                StartFishGenerate();
            }
            else
            {
                StopFishGenerate();
            }
            _isChange = IsGenerate;
        }*/
#endif 
    }
}
