using System;
using System.Collections.Generic;
using System.Linq;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    /// <summary>
    /// 麻将的管理
    /// </summary>
    public class MahjongManager : MonoBehaviour
    {

        public GameObject Mahjong;
        public GameObject MahjongSign;
        public GameObject MahJongArrow;
        /// <summary>
        /// 杠头角标模型
        /// </summary>
        public GameObject MahjongGangTou;
        //public Material MahjongSignMater;

        public Material MahjongMaterialNomal;
        public Material MahjongMaterialGreen;
        public Material MahjongMaterialGay;
        public GameObject TingIcon;
        public Texture[] MjClothes;

        private static Vector3 _lowMagjongSize = new Vector3(0.2986238f, 0.3964008f, 0.2037924f);
        private static readonly Vector3 HightMagjongSize = new Vector3(0.3000809f, 0.4000001f, 0.2201198f);

        public static Vector3 MagjongSize
        {
            get { return HightMagjongSize; }
        }

        public static MahjongManager Instance;
        //保存所有麻将的引用 用来重置管理
        private List<MahjongItem> MahjongList = new List<MahjongItem>();
        /// 每个麻将Group 在移出麻将时 都先回收到 UnusedMahjongList
        private List<MahjongItem> UnusedMahjongList = new List<MahjongItem>();
        /// 能够交换值的麻将 队列 所有付过值的麻将 不在队列中
        private List<MahjongItem> CanExchangeMahjongList = new List<MahjongItem>();
        // 显示出来的 麻将
        private List<MahjongItem> _visibaleMahjongList = new List<MahjongItem>();
        // 克隆出來的麻將
        private List<MahjongItem> _cloneMahjongList = new List<MahjongItem>(); 

        private int _mahjongCnt;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void InitMahjong(List<int> cards)
        {
            CreateMahjong(cards);
        }

        public void ExchangeByValue(int value, MahjongItem item, int index = UtilDef.DefInt)
        {
            MahjongItem itemFind = FindMjFromList(CanExchangeMahjongList, value, index);
            if (itemFind)
            {
                item.Exchange(itemFind);
            }
        }

        public void RemoveItemFromCanExchangeMahjongList(MahjongItem item)
        {
            CanExchangeMahjongList.Remove(item);
        }

        public List<MahjongItem> GetMahjongList(int cnt)
        {
            List<MahjongItem> Ret = new List<MahjongItem>();
            if (cnt > UnusedMahjongList.Count)
            {
                YxDebug.LogError("没有用到的麻将队列中 没有足够的麻将");
                return null;
            }

            for (int i = 0; i < cnt; i++)
            {
                MahjongItem item = UnusedMahjongList[0];
                UnusedMahjongList.Remove(item);
                item.gameObject.SetActive(true);
                Ret.Add(item);
            }
            YxDebug.Log("剩余的没有用到的牌个数 = " + UnusedMahjongList.Count);
            return Ret;
        }

        public List<MahjongItem> GetMahjongList(int[] value)
        {
            var ret = new List<MahjongItem>();
            for (int i = 0; i < value.Length ; i++)
            {
                MahjongItem item = GetMahjong(value[i]);
                ret.Add(item);
            }

            return ret;
        }

        public MahjongItem GetMahjong(int value,int index)
        {
            MahjongItem ret;

            if (UnusedMahjongList.Count < 1)
            {
                YxDebug.LogEvent("没有用到的麻将队列中 没有足够的麻将");
                var list = new List<int>();
                list.Add(value);
                CreateMahjong(list);
//                return null;
            }

            if (value == 0)
            {
                ret = UnusedMahjongList[0];
                UnusedMahjongList.Remove(ret);
                ret.gameObject.SetActive(true);
                return ret;
            }

            ret = FindMjFromList(UnusedMahjongList, value, index);

            if (ret == null)
            {
                ret = UnusedMahjongList[0];
                UnusedMahjongList.Remove(ret);
                ExchangeByValue(value, ret, index);
            }
            else
            {
                UnusedMahjongList.Remove(ret);
            }
            if (ret.Value != value)
                YxDebug.LogEvent("找到的麻将与给定的直不同！！！");

            int canExchangeIndexRemove = CanExchangeMahjongList.IndexOf(ret);
            if (canExchangeIndexRemove!=-1)
            {
                CanExchangeMahjongList.Remove(ret);
                _visibaleMahjongList.Add(ret);
            }

            ret.gameObject.SetActive(true);

            return ret;
        }

        public MahjongItem GetMahjong(int value)
        {
            MahjongItem ret = GetMahjong(value, UtilDef.DefInt);
            return ret;
        }

        public MahjongItem FindMjFromList(List<MahjongItem> mjList, int value, int index = UtilDef.DefInt)
        {
            MahjongItem ret;
            Predicate<MahjongItem> func;
            if (index == UtilDef.DefInt)
            {
                func = mahjongItem => mahjongItem.Value == value;
            }
            else
            {
                func = (mahjongItem) => mahjongItem.Value == value && mahjongItem.MahjongIndex == index;
            }
            //先在没有用到的列表中查找
            try
            {
                ret = mjList.Find(func);
            }
            catch (Exception)
            {
                ret = null;
            }

            return ret;
        }

        public void CreateMahjong(List<int> cards)
        {
            if (Mahjong == null)
            {
                YxDebug.LogEvent("没有麻将的模型!");
                return;
            }

            //加载麻将预设
            GameObject obj = ResourceManager.LoadAsset("HightMjPrefabs", "HightMjPrefabs");
            var mjPrefabs = obj.GetComponent<MahjongPerfabs>();
            MahjongMaterialNomal = Instantiate(mjPrefabs.MahjongMaterialNomal);
            MahjongMaterialGreen = Instantiate(mjPrefabs.MahjongMaterialGreen);
            MahjongMaterialGay = Instantiate(mjPrefabs.MahjongMaterialGay);
          
            if (mjPrefabs.TingIcon != null)
            {
                TingIcon = Instantiate(mjPrefabs.TingIcon);
                TingIcon.name = TingIcon.name.Replace("(Clone)", "");   
                TingIcon.transform.position=-Vector3.one;
  
            }

            CreateMahjong(mjPrefabs.GetGameObjList(cards), cards);
        }

        void CreateMahjong(List<GameObject> mahjongs,List<int> value)
        {
            for(int j = 0;j<mahjongs.Count;j++)
            {
                GameObject o = mahjongs[j];
                int cnt = value[j] >= 96 ? 1 : 4;

                for (int i = 0; i < cnt; i++)
                {
                    GameObject temp = Instantiate(Mahjong);
                    GameObject mesh = Instantiate(o);

                    var mahjongTemp = temp.GetComponent<MahjongItem>();
                    mesh.transform.parent = mahjongTemp.Model.transform;
                    mahjongTemp.Mesh = mesh;
                    mahjongTemp.MahjongIndex = i;
                    mahjongTemp.Value = value[j];

                    temp.transform.parent = transform;
                    mahjongTemp.Reset();

                    temp.SetActive(false);

                    MahjongList.Add(mahjongTemp);
                    UnusedMahjongList.Add(mahjongTemp);
                    CanExchangeMahjongList.Add(mahjongTemp);
                }
            }
        }

        public void Recycle(MahjongItem item)
        {
            item.gameObject.SetActive(false);
            item.transform.parent = transform;
            item.Reset();

            if (UnusedMahjongList.Find(mj => mj == item)==null)
                UnusedMahjongList.Insert(0, item);
        }

        public void Reset()
        {
            UnusedMahjongList.Clear();
            CanExchangeMahjongList.Clear();
            foreach (MahjongItem item in MahjongList)
            {
                if (item)
                {
                    item.transform.parent = transform;
                    item.Reset();

                    UnusedMahjongList.Add(item);

                    CanExchangeMahjongList.Add(item);
                item.gameObject.SetActive(false);
				}
            }

            foreach (MahjongItem item in _cloneMahjongList)
            {
                Destroy(item.gameObject);
            }
            _cloneMahjongList.Clear();
        }

        public GameObject GetSign()
        {
            GameObject sign = Instantiate(MahjongSign);
            //sign.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(MahjongSignMater);
            return sign;
        }

        /// <summary>
        /// 箭头脚本
        /// </summary>
        /// <returns></returns>
        public GameObject GetArrow()
        {
            GameObject arrow = Instantiate(MahJongArrow);
            return arrow;
        }

        /// <summary>
        /// 获得杠头gameobj
        /// </summary>
        /// <returns></returns>
        public GameObject GetGangTou()
        {
            GameObject gangtou = Instantiate(MahjongGangTou);
            return gangtou;
        }

        public GameObject CreateCloneMajong(int value)
        {
            GameObject ret = null;
            MahjongItem findItem = null;
            foreach (var mahjongItem in MahjongList)
            {
                if (mahjongItem.Value == value)
                {
                    findItem = mahjongItem;
                    break;
                }
            }
            if (findItem != null)
            {
                ret = Instantiate(findItem.gameObject);
                var item = ret.GetComponent<MahjongItem>();
                item.Reset();
                _cloneMahjongList.Add(item);
            }

            return ret;
        }

        public GameObject CreateCloneMajong(MahjongItem item)
        {
            GameObject ret = null;
            if (item != null)
            {
                ret = Instantiate(item.gameObject);
                var itemScp = ret.GetComponent<MahjongItem>();
                itemScp.Reset();
                _cloneMahjongList.Add(itemScp);
            }

            return ret;
        }

        public GameObject CreateCloneMajong(GameObject obj)
        {
            GameObject ret = null;
            if (obj != null)
            {
                ret = Instantiate(obj);
                var itemScp = ret.GetComponent<MahjongItem>();
                itemScp.Reset();
                _cloneMahjongList.Add(itemScp);
            }

            return ret;
        }

        public void RecycleCloneMahjong(MahjongItem item)
        {
            _cloneMahjongList.Remove(item);
            Destroy(item.gameObject);
        }

        public void RecycleCloneMahjong(GameObject obj)
        {
            _cloneMahjongList.Remove(obj.GetComponent<MahjongItem>());
            Destroy(obj);
        }

        public void ChangeMjClothes(int index)
        {
            if (MjClothes==null||MjClothes.Length-1<index)
            {
                return;
            }
            PlayerPrefs.SetInt("MjColor",index);
            MahjongMaterialNomal.mainTexture = MjClothes[index];
            MahjongMaterialGay.mainTexture = MjClothes[index];
            MahjongMaterialGreen.mainTexture = MjClothes[index];
        }


        void OnDestroy()
        {
            Instance = null;
        }
    }
}
