using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class CoinStackImm : MonoBehaviour {
        //public Transform TsMaskScaler; 
        public tk2dTextMesh Text_CoinNum;
        public tk2dSlicedSprite SliSpr_CoinNumBG;//数字背景
        public GameObject GO_PileCoin;
        public AnimationCurve CoinNumToPileNumCurve;

        private Renderer mRdPileCoin;


        private const float OneCoinHeight = 5.9733333333333504F;
        public void Awake()
        {
            mRdPileCoin = GO_PileCoin.GetComponent<Renderer>();
            if (mRdPileCoin == null)
                mRdPileCoin = GO_PileCoin.GetComponentInChildren<Renderer>();

            mRdPileCoin.sharedMaterial = Instantiate(mRdPileCoin.sharedMaterial) as Material;
        }

        public void OnDestroy()
        {
            Destroy(mRdPileCoin.sharedMaterial);
        }

        public void SetNum(int numScore)
        {
            if (numScore <= 0)
            {
                mRdPileCoin.enabled = false;
                Text_CoinNum.GetComponent<Renderer>().enabled = false;
                return;
            }

            int curStackHeightNum = Mathf.RoundToInt(CoinNumToPileNumCurve.Evaluate((float)numScore /  1000F));

            mRdPileCoin.enabled = true;
            Text_CoinNum.GetComponent<Renderer>().enabled = true;
            Text_CoinNum.text = numScore.ToString();
            Text_CoinNum.Commit();
            SliSpr_CoinNumBG.dimensions = new Vector2(10F + (10F * Text_CoinNum.NumDrawnCharacters()), 20F);

            Text_CoinNum.transform.localPosition = new Vector3(0F, OneCoinHeight * curStackHeightNum + OneCoinHeight * 0.02F, 0F);
            GO_PileCoin.transform.localScale = new Vector3(1F, 0.0333333333333333F * curStackHeightNum, 1F);//贴图有30个币. 所以单币缩放高度是: 1/30 = 0.03333
        
        }
    }
}
