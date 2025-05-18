using System.Globalization;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj
{
    public class JpHuManjongGroupInfo : MonoBehaviour
    {
        [SerializeField] private GameObject _infoTextGob;
        [SerializeField] private string _keytai = "tai";
        [SerializeField] private string _keyHua = "hua";
        [SerializeField] private string _keyYu = "yu";

        public float PosX
        {
            get { return _posX; }
        }
        private float _posX;

        [SerializeField] protected float TextinfoWigth = 80;
        [SerializeField] protected float MjSpacingRatio = 0.68f;
        [SerializeField] protected string TittleTaiTextstr = "台:";
        [SerializeField]
        protected string TittleHuaTextstr = "花:";
        [SerializeField]
        protected string TitleYuTextstr = "余:";

        public void AddInfotextGob(int tai,int hua,int cnt)
        {
            var infotextGob = Instantiate(_infoTextGob);
            infotextGob.SetActive(true);
            var taigob = infotextGob.transform.Find(_keytai).gameObject;
            var huagob = infotextGob.transform.Find(_keyHua).gameObject;
            var yugob = infotextGob.transform.Find(_keyYu).gameObject;

            taigob.GetComponent<Text>().text = TittleTaiTextstr + tai.ToString(CultureInfo.InvariantCulture);
            huagob.GetComponent<Text>().text = TittleHuaTextstr + hua.ToString(CultureInfo.InvariantCulture);
            yugob.GetComponent<Text>().text = TitleYuTextstr + cnt.ToString(CultureInfo.InvariantCulture)+"张";

            infotextGob.transform.SetParent(gameObject.transform);
            infotextGob.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            infotextGob.transform.localPosition = new Vector3(_posX, 0, 0);
            _posX += TextinfoWigth;
        }

        public void AddMjGobCell(int cdValue)
        {
            var mj = D2MahjongMng.Instance.GetGroupForJp(new[] { cdValue }, EnD2MjType.Me, false, JpMahjongPlayerHard.CaishenValue);
            mj.transform.SetParent(gameObject.transform);
            mj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            mj.transform.localPosition =new Vector3(_posX,0,0);
            _posX += mj.Width * MjSpacingRatio;
        }



/*        public void SetMahjongInfo(ISFSObject data)
        {
            var cds = data.GetIntArray(_keyCards);
            var tai = data.GetInt(_keytai);
            var hua = data.GetInt(_keyHua);
            var cnt = data.GetInt(_keyYu);
            AddSingleMeCds(cds, tai, hua, cnt);
        }

        private void AddSingleMeCds(int[] cds,int tai,int hua ,int cnt)
        {
            var mj = D2MahjongMng.Instance.GetGroupForJp(cds, EnD2MjType.Me,false,17);
            mj.transform.SetParent(gameObject.transform);
            mj.transform.localPosition = new Vector3(0, 0, 0);
            mj.transform.localScale = new Vector3(0.7f,0.7f,1);

            var taigob = _infoTextGob.transform.Find("tai").gameObject;
            var huagob = _infoTextGob.transform.Find("hua").gameObject;
            var yugob = _infoTextGob.transform.Find("yu").gameObject;

            taigob.GetComponent<Text>().text = "台:"+  tai.ToString(CultureInfo.InvariantCulture);
            huagob.GetComponent<Text>().text = "花:" + hua.ToString(CultureInfo.InvariantCulture);
            yugob.GetComponent<Text>().text = "余:" + cnt.ToString(CultureInfo.InvariantCulture);
            Debug.LogError(mj.transform.childCount);
            var lastMjLocationX = mj.transform.GetChild(mj.transform.childCount-1).localPosition.x;
            _infoTextGob.transform.SetParent(mj.transform);
            _infoTextGob.transform.localPosition = new Vector3(mj.transform.localPosition.x + lastMjLocationX+70, 9, 0);

            mj.transform.localPosition = new Vector3(mj.Width/3,0,0);
        }*/

    }
}
