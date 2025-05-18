using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongCpgGroup : MonoBehaviour
    {
        public float CpgInterval;

        public int Chair;

        public List<MahjongCpg> CpgList = new List<MahjongCpg>();
        // Use this for initialization

        void Start()
        {
            if (null == GameAdpaterManager.Singleton) return;
            switch (Chair)
            {
                case 0: transform.localPosition = GameAdpaterManager.Singleton.GetConfig.Cpg0_Pos; break;
                case 1: transform.localPosition = GameAdpaterManager.Singleton.GetConfig.Cpg1_Pos; break;
                case 3: transform.localPosition = GameAdpaterManager.Singleton.GetConfig.Cpg3_Pos; break;               
            }
        }

        public void SetCpg(CpgData data)
        {
            data.Chair = Chair;
            if (data.Type == EnGroupType.ZhuaGang)
            {
                SetZhuaGang(data);
            }
            else
            {
                AddCpg(data);
                SortGpg();
                ////保存到本地
                //MahjongCpg cpg = CpgList[CpgList.Count - 1];
                //CpgLocalSave.Instance.SaveCpg(Chair, new CpgSaveData(cpg.Data, cpg.AcrossIndex));
            }
        }

        public void SetCpgArray(CpgData[] data)
        {
            foreach (CpgData cpgData in data)
            {
                cpgData.Chair = Chair;
                ////重连时在本地保存中读取Index
                //int index = CpgLocalSave.Instance.GetCpgIndex(Chair, cpgData);
                AddCpg(cpgData);
            }
            SortGpg();
            ////保存到本地
            //CpgSaveData[] saveDataArray = new CpgSaveData[data.Length];
            //for(int i = 0;i<CpgList.Count;i++)
            //{
            //    var mjCpg = CpgList[i];
            //    saveDataArray[i] = new CpgSaveData(mjCpg.Data, mjCpg.AcrossIndex);
            //}
            //CpgLocalSave.Instance.SaveCpgArrayAndCleare(Chair, saveDataArray);
        }

        protected virtual void SortGpg()
        {
            float offsetx = 0;
            for (int i = 0; i < CpgList.Count; i++)
            {
                Transform mahjongTf = CpgList[i].transform;

                offsetx -= CpgList[i].OffsetX;

                Vector3 pos = new Vector3(offsetx, 0, 0);

                mahjongTf.localPosition = pos;

                offsetx += CpgInterval;
            }
        }

        protected virtual void AddCpg(CpgData data, int index = UtilDef.DefInt)
        {
            MahjongCpg cpg = CpgMahjongCreater.CreateCpg(data, index);
            CpgList.Add(cpg);
            cpg.transform.parent = transform;
            cpg.transform.localPosition = Vector3.zero;
            cpg.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        protected virtual void SetZhuaGang(CpgData data)
        {
            for (int i = 0;i<CpgList.Count;i++)
            {
                MahjongCpg cpgPeng = CpgList[i];
                int arrowIndex = cpgPeng.ArrowCrossIndex;
                if (cpgPeng.Data.Type == EnGroupType.Peng && cpgPeng.Data.Card == data.Card)
                {
                    //保存碰的位子
                    Vector3 pengPos = cpgPeng.transform.localPosition;
                    cpgPeng.Delete();

                    MahjongCpg cpgZhuaGang = CpgMahjongCreater.CreateCpg(data, cpgPeng.AcrossIndex, arrowIndex);

                    cpgZhuaGang.transform.parent = transform;
                    cpgZhuaGang.transform.localPosition = pengPos;
                    cpgZhuaGang.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    CpgList[i] = cpgZhuaGang;

                    ////保存到本地
                    //CpgLocalSave.Instance.ChangeToZhuaGangType(Chair, data.Card);
                    break;
                }
            }
        }

        public void Reset()
        {
            foreach (MahjongCpg cpg in CpgList)
            {
                Destroy(cpg.gameObject);
            }

            CpgList.Clear();
        }

        public void RemoveAll()
        {
            foreach (MahjongCpg cpg in CpgList)
            {
                cpg.Delete();
            }
            CpgList.Clear();
        }

        public int GetHardMjCnt()
        {
            return CpgList.Count*3;
        }
    }
}
