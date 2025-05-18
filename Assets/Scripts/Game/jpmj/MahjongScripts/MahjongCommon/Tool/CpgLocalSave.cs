using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool
{
    public class CpgSaveData
    {
        public EnGroupType type;  //类型
        public int[] value;       //值
        public int index;       //显示横着的位子

        public CpgSaveData() {}

        public CpgSaveData(CpgData data,int index)
        {
            type = data.Type;
            value = data.AllCards().ToArray();
            this.index = index;
        }
    }
    public class CpgLocalSave
    {

        private static CpgLocalSave _instance;
        //保存的Key为 key + chair
        private const string SaveDataKey = "cpg_save_";
        private const string CpgType = "type";
        private const string CpgValue = "value";
        private const string CpgIndex = "index";

        public static CpgLocalSave Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CpgLocalSave();
                    _instance.Init();
                }

                return _instance;
            }
        }

        private List<CpgSaveData>[] _saveList = new List<CpgSaveData>[UtilDef.GamePlayerCnt];
        public void Write(int chair)
        {
            SFSArray array = SFSArray.NewInstance();
            foreach (CpgSaveData saveData in _saveList[chair])
            {
                ISFSObject obj = new SFSObject();
                obj.PutInt(CpgType, (int)saveData.type);
                obj.PutIntArray(CpgValue, saveData.value);
                obj.PutInt(CpgIndex, saveData.index);

                array.AddSFSObject(obj);
            }
            string key = SaveDataKey + chair;
            PlayerPrefs.SetString(key, array.ToJson());
        }

        public List<CpgSaveData> Read(int chair)
        {
            if (_saveList[chair].Count != 0)
            {
                return _saveList[chair];
            }

            string key = SaveDataKey + chair;
            string valueJson = PlayerPrefs.GetString(key,"");
            if (valueJson=="")
            {
                YxDebug.Log("当前本地 无吃碰杠保存数据");
                return null;
            }

            ISFSArray array = SFSArray.NewFromJsonData(valueJson);
            for (int i = 0; i < array.Size(); i++)
            {
                ISFSObject arrayObj = array.GetSFSObject(i);
                CpgSaveData data = new CpgSaveData();
                data.type = (EnGroupType) arrayObj.GetInt(CpgType);
                data.value = arrayObj.GetIntArray(CpgValue);
                data.index = arrayObj.GetInt(CpgIndex);


                _saveList[chair].Add(data);
            }

            return _saveList[chair];
        }

        public void SaveCpg(int chair, CpgSaveData data)
        {
            _saveList[chair].Add(data);
            Write(chair);
        }

        public void SaveCpgArrayAndCleare(int chair, CpgSaveData[] data)
        {
            _saveList[chair].Clear();
            for (int i = 0; i < data.Length; i++)
            {
                _saveList[chair].Add(data[i]);
            }
            Write(chair);
        }

        public void ChangeToZhuaGangType(int chair,int value)
        {
            foreach (CpgSaveData cpgSaveData in _saveList[chair])
            {
                if (cpgSaveData.type == EnGroupType.Peng && cpgSaveData.value[0] == value)
                {
                    cpgSaveData.type = EnGroupType.ZhuaGang;
                }
            }

            Write(chair);
        }

        private void Init()
        {
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                _saveList[i] = new List<CpgSaveData>();
                Read(i);
            }
        }

        public void Cleare()
        {
            for (int i = 0; i < UtilDef.GamePlayerCnt; i++)
            {
                string key = SaveDataKey + i;
                PlayerPrefs.SetString(key, "");
            }
        }

        public int GetCpgIndex(int chair,CpgData data)
        {
            foreach (CpgSaveData cpgSaveData in _saveList[chair])
            {
                if (cpgSaveData.type == data.Type && CompareValue(cpgSaveData.value,data.AllCards().ToArray()))
                {
                    return cpgSaveData.index;
                }
            }

            return UtilDef.DefInt;
        }


        private bool CompareValue(int[] value1, int[] value2)
        {
            if (value1.Length != value2.Length) return false;

            bool isCompare = false;
            for (int i = 0; i < value1.Length; i++)
            {
                isCompare = value1[i] == value2[i];
            }

            return isCompare;
        }

        public void OnDestroy()
        {
            _instance = null;
        }
    }
}
