using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongCpgGroup : MonoBehaviour
    {
        public List<MahjongCpgItem> CpgItemList { get; private set; }

        private void Awake()
        {
            CpgItemList = new List<MahjongCpgItem>();
        }

        public void SetCpg(CpgModel model)
        {
            var obj = GameUtils.GetInstanceAssets<GameObject>("MahjongCpgItem");
            var item = obj.GetComponent<MahjongCpgItem>();
            obj.transform.ExSetParent(transform);
            obj.gameObject.SetActive(true);

            item.OnInit(model);
            CpgItemList.Add(item);

            float offsetx = 0;
            for (int i = 0; i < CpgItemList.Count; i++)
            {
                var itemTransform = CpgItemList[i].transform;
                offsetx -= CpgItemList[i].OffsetX;
                itemTransform.localPosition = new Vector3(offsetx, 0, 0);
                offsetx += CpgInterval;
            }
        }

        public virtual void SetZhuaGang(int card)
        {
            for (int i = 0; i < CpgItemList.Count; i++)
            {
                var cpg = CpgItemList[i];
                var flag = cpg.Model.Type == CpgProtocol.Peng && cpg.Model.Cards[0] == card;
                if (flag)
                {
                    cpg.SetZhuaGangLayout(card);
                }
            }
        }

        public void OnReset()
        {
            for (int i = 0; i < CpgItemList.Count; i++)
            {
                var item = CpgItemList[i];
                item.OnReset();
                Destroy(item.gameObject);
            }
            CpgItemList.Clear();

            for (int i = 0; i < CpgList.Count; i++)
            {
                CpgList[i].OnReset();
            }
            CpgList.Clear();
        }

        //-------------------------------------------------------------
        public int Chair;
        public float CpgInterval;
        public List<MahjongCpg> CpgList = new List<MahjongCpg>();

        public void SetCpg(CpgData data)
        {
            if (null == data) return;
            data.Chair = Chair;
            if (data.Type == EnGroupType.ZhuaGang)
            {
                SetZhuaGang(data);
            }
            else
            {
                AddCpg(data);
                SortGpg();
            }
        }

        public void SetCpgArray(CpgData[] datas)
        {
            if (null == datas || datas.Length == 0) return;
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i].Chair = Chair;
                AddCpg(datas[i]);
            }
            SortGpg();
        }

        public virtual void SortGpg()
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

        protected virtual void AddCpg(CpgData data, int index = DefaultUtils.DefInt)
        {
            if (null == data) return;
            var fromInt = data.GetFromChair();
            index = fromInt == -1 ? index : fromInt;
            MahjongCpg cpg = MahjongUtility.CreateCpg(data, index);
            CpgList.Add(cpg);
            cpg.transform.ExSetParent(transform);
        }

        protected virtual void SetZhuaGang(CpgData data)
        {
            if (null == data) return;
            for (int i = 0; i < CpgList.Count; i++)
            {
                MahjongCpg cpgPeng = CpgList[i];
                if (cpgPeng.Data.Type == EnGroupType.Peng && cpgPeng.Data.Card == data.Card)
                {
                    //保存碰的位子
                    Vector3 pengPos = cpgPeng.transform.localPosition;
                    cpgPeng.OnReset();
                    MahjongCpg cpgZhuaGang = MahjongUtility.CreateCpg(data, cpgPeng.AcrossIndex);
                    cpgZhuaGang.transform.SetParent(transform);
                    cpgZhuaGang.transform.localScale = Vector3.one;
                    cpgZhuaGang.transform.localPosition = pengPos;
                    cpgZhuaGang.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    CpgList[i] = cpgZhuaGang;
                    break;
                }
            }
        }

        public int GetHardMjCnt()
        {
            return CpgList.Count * 3;
        }
    }
}