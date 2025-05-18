using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class  UsersAddGold : MonoBehaviour
    {
        public Sprite[] _addNumSp;
        public Sprite[] _subNumSp;

        [Range(0, 0.5f)]
        public float Duration = 0.2f;           

        private const float _fScaleNum = 0.9f;
        private PlayerOther PlayerOther;
        private Text[] PlayerAddGold;

        void Start()
        {
            PlayerOther = GetComponent<PlayerOther>();

            if (null != PlayerOther)
            {
                PlayerAddGold = PlayerOther.PlayerAddGold;
            }
        }

        public void SetUsersAddGold(int chair, int gold)
        {
            PlayerAddGold[chair].gameObject.SetActive(true);
            PlayerAddGold[chair].text = "";

            GameObject numGroup = new GameObject();
            numGroup.name = "NumGroup";
            numGroup.transform.parent = PlayerAddGold[chair].transform;
            numGroup.layer = PlayerAddGold[chair].gameObject.layer;
            numGroup.transform.localScale = Vector3.one;
            numGroup.transform.localPosition = Vector3.zero;

            // 按位创建Image并加入到列表
            bool bIsAdd = (0 < gold);
            gold = System.Math.Abs(gold);
            List<GameObject> numLis = new List<GameObject>();
            while (0 < gold)
            {
                int iPosNum = gold % 10;
                GameObject numObj = new GameObject();
                numObj.AddComponent<Image>();
                if (bIsAdd)
                {
                    numObj.GetComponent<Image>().overrideSprite = _addNumSp[iPosNum];
                }
                else
                {
                    numObj.GetComponent<Image>().overrideSprite = _subNumSp[iPosNum];
                }
                numObj.GetComponent<Image>().SetNativeSize();
                numObj.transform.parent = numGroup.transform;
                numObj.transform.localScale = Vector3.one;
                numObj.transform.localPosition = Vector3.zero;
                numObj.layer = numGroup.layer;
                gold = gold / 10;
                numLis.Insert(0, numObj);
            }

            Sprite symbolSp = bIsAdd ? _addNumSp[_addNumSp.Length - 1] : _subNumSp[_subNumSp.Length - 1];
            GameObject symbolObj = new GameObject();
            symbolObj.AddComponent<Image>();
            symbolObj.GetComponent<Image>().overrideSprite = symbolSp;
            symbolObj.GetComponent<Image>().SetNativeSize();
            symbolObj.transform.parent = numGroup.transform;
            symbolObj.transform.localScale = Vector3.one;
            symbolObj.transform.localPosition = Vector3.zero;
            symbolObj.layer = numGroup.layer;
            numLis.Insert(0, symbolObj);
            symbolObj.gameObject.AddComponent<TweenScale>();

            // 对列表进行位置调整
            for (int i = 1; i < numLis.Count; i++)
            {
                Vector3 movV3 = Vector3.zero;
                movV3.x = numLis[i - 1].transform.localPosition.x
                    + numLis[i - 1].GetComponent<Image>().overrideSprite.textureRect.width / 2
                    + numLis[i - 1].GetComponent<Image>().overrideSprite.texture.width / 2 - 10f;
                //				movV3.x *= _fScaleNum;
                numLis[i].transform.localPosition += movV3;
                numLis[i].transform.localScale *= _fScaleNum;

                numLis[i].gameObject.AddComponent<TweenScale>();
                numLis[i].gameObject.SetActive(false);
            }

            StartCoroutine(HideObjWithTimeIEnumerator(PlayerAddGold[chair].gameObject, numLis, 2f));
        }

        private IEnumerator HideObjWithTimeIEnumerator(GameObject txt, List<GameObject> numLis, float time)
        {
            for (int i = 0; i < numLis.Count; i++)
            {
                numLis[i].gameObject.SetActive(true);

                TweenScale twScale = numLis[i].gameObject.GetComponent<TweenScale>();
                Vector3 v3From = numLis[i].transform.localScale;
                EventDelegate.Callback twFinish = () =>
                {
                    numLis[i].transform.localScale = v3From;
                };
                twScale.SetOnFinished(twFinish);
                twScale.from = v3From;
                twScale.to = new Vector3(1.8f, 1.8f, 1.8f);//v3From * 1.5f;
                twScale.duration = 0.2f;
                twScale.PlayForward();

                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(time);

            GameObject numGroup = txt.transform.Find("NumGroup").gameObject;
            Destroy(numGroup);
            txt.gameObject.SetActive(false);
        }
    }
}