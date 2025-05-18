using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.brnn3d
{
    public class PaiMode : MonoBehaviour
    {
        public Transform[] PaiEasts = new Transform[5];
        public Transform[] PaiSouths = new Transform[5];
        public Transform[] PaiWests = new Transform[5];
        public Transform[] PaiNorths = new Transform[5];
        public Transform[] PaiZhuangs = new Transform[5];

        public Transform PaiFirstTf;
        public Transform PaiSecondTf;

        private int[] niuNum = new int[5];
        private bool[] Result = new bool[5];
 
        public int JiSuan;

        public PaiModeMgr ThePaiModeMgr;

        public void BeginGiveCards()
        {
            StartCoroutine("GiveCards");
        }

        IEnumerator GiveCards()
        {
            yield return new WaitForSeconds(2f);
            var res = 0;
            for (int i = 0; i < niuNum.Length; i++)
            {
                niuNum[i] = App.GetGameData<Brnn3dGameData>().Nn.GetSFSObject(i).GetInt("niu");
                Result[i] = App.GetGameData<Brnn3dGameData>().Nn.GetSFSObject(i).GetBool("win");
                if (App.GetGameData<Brnn3dGameData>().Nn.GetSFSObject(i).GetBool("win"))
                {
                    res |= (1 << i);
                    //res = res |(1 << i);
                }
            }
            Store.Add(res);
            if (Replace == JiSuan)
            {
                Replace++;
            }
            JiSuan++;
            ThePaiModeMgr.SetPaiModeDataEx(); //设置发牌的数据
        }

        public int Replace;
        public int Pos = 0;
        public List<int> Store = new List<int>();
        public void History()
        {
            if (Replace == 0)
            {
                return;
            }
            var gameMgr = App.GetGameManager<Brnn3DGameManager>();
            var downUIleft = gameMgr.TheDownUICtrl.TheDownUILeftBg;
            var luziInfoMgr = downUIleft.TheLuziInfoUIMgr;
            luziInfoMgr.TheLuziInfoUI.InitImg();

            var pos = Replace > 10 ? Replace - 10 : 0;
            for (var i = pos; i < Replace; i++)
            {
                var cur = Store[i];
                for (var j = 0; j < 4; j++)
                {
                    luziInfoMgr.SetLuziInfoUIDataEy(j, i - pos, ((cur >> j + 1) & 1) == 1);
                }
            }
        }
        public void SetLuziInfoUIData(int index)
        {
            if (index > 0)
            {
                if (Replace < JiSuan)
                {
                    Replace++;
                }
            }
            else
            {
                if (Replace > 10)
                    Replace--;

            }
            History();
        }

        public void InstancePai(int paiIndex, int iArea,/* int paiPoint,*/ int pai25Indxe)
        {
            int[] cards = App.GetGameData<Brnn3dGameData>().Cards.GetIntArray(iArea);
            Transform areaTf = null;
            switch (iArea)
            {
                case 0:
                    areaTf = PaiZhuangs[paiIndex];
                    break;
                case 1:
                    areaTf = PaiEasts[paiIndex];
                    break;
                case 2:
                    areaTf = PaiSouths[paiIndex];
                    break;
                case 3:
                    areaTf = PaiWests[paiIndex];
                    break;
                case 4:
                    areaTf = PaiNorths[paiIndex];
                    break;
            }
            if (areaTf == null) YxDebug.LogError("No Such Object" + iArea);

            var paiPrefabName = "Pai_0" + cards[paiIndex].ToString("X");
            var go = ResourceManager.LoadAsset(paiPrefabName, string.Format("brnnpai/{0}", paiPrefabName));
            var go1 = Instantiate(go);
            Transform obj = go1.transform;
            obj.gameObject.SetActive(true);
            if (areaTf != null) obj.transform.parent = areaTf.parent;
            obj.transform.localScale = new Vector3(0, 0, 0);
            if (paiIndex == 3)
            {
                obj.localEulerAngles = new Vector3(0, PaiFirstTf.localEulerAngles.y, 180);
                if (App.GetGameData<Brnn3dGameData>().PaiAllShow.ContainsKey(iArea))
                {
                    YxDebug.LogError("Error Here");
                }
                else
                {
                    App.GetGameData<Brnn3dGameData>().PaiAllShow.Add(iArea, obj);
                }
            }
            else
                obj.localEulerAngles = new Vector3(0, PaiFirstTf.localEulerAngles.y, 0);

            if (areaTf != null) obj.localPosition = areaTf.localPosition;

            var pai = obj.GetComponent<Pai>();
            if (pai == null)
            {
                YxDebug.LogError("No Such Component");
            }
            else
            {
                pai.Show(pai25Indxe, iArea, paiIndex); 
            }
        }

        public struct Pp
        {
            public int AreaId;
            public Transform Tf;
            public float S;
        }
        //翻牌阶段显示中奖区域
        public void FanPaiFun()
        {
            var tmp = App.GetGameData<Brnn3dGameData>().SendCardPosition;
            StartCoroutine("ToShwoZhongJiangArea", 7.5f);
            for (int i = 0; i < 5; i++)
            {
                var pP = new Pp
                {
                    AreaId = tmp,
                    Tf = App.GetGameData<Brnn3dGameData>().PaiAllShow[tmp],
                    S = i * 1.2f
                };

                StartCoroutine("ToFanPai", pP);
                tmp += 1;
                if (tmp > 4)
                    tmp = 0;
            }
        }

        private IEnumerator ToFanPai(Pp p)
        {
            yield return new WaitForSeconds(p.S);
            p.Tf.localEulerAngles = new Vector3(0, 0, 0);
            Pai pai = p.Tf.GetComponent<Pai>();
            if (pai != null) pai.PlayFanPaiAni();
            yield return new WaitForSeconds(1.2f);

            var niuNumberUI = App.GetGameManager<Brnn3DGameManager>().TheMidUICtrl.TheNiuNumberUI;
            niuNumberUI.ShowNumberUI(niuNum);
            niuNumberUI.ShowAreaNiu(p.AreaId);
            niuNumberUI.PlayAudioNiuJi(p.AreaId, niuNum);
        }

        private IEnumerator ToShwoZhongJiangArea(float s)
        {
            yield return new WaitForSeconds(s);
            var zhongJiangMode = App.GetGameManager<Brnn3DGameManager>().TheZhongJiangMode;
            for (var i = 0; i < 4; i++)
            {
                zhongJiangMode.ShowZhongJiangEffect(i, Result);
            }
        }
        //删除牌的列表
        public void DeletPaiList()
        {
            DeletePaiItemListFromParent(PaiEasts[0].parent);
            DeletePaiItemListFromParent(PaiSouths[0].parent);
            DeletePaiItemListFromParent(PaiWests[0].parent);
            DeletePaiItemListFromParent(PaiNorths[0].parent);
            DeletePaiItemListFromParent(PaiZhuangs[0].parent);
        }
        //从牌组的父物体下删除牌
        private void DeletePaiItemListFromParent(Transform parent)
        {
            foreach (Transform tf in parent)
            {
                if (tf.name.Contains("Pai1") || tf.name.Contains("Pai2") || tf.name.Contains("Pai3") ||
                    tf.name.Contains("Pai4") || tf.name.Contains("Pai5")) continue;
                Destroy(tf.gameObject);
            }
        }
    }
}


