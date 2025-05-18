using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Ttzkf
{
    public class Result : MonoBehaviour
    {
        public GameObject BgParent;
        public EntiretyItem EntiretyItem;
        public AResM AResM;
        public GameObject[] Button;
        public GameObject CloseBtn;
        public UITable Table;

        private ISFSArray _data;
        private ISFSObject _zndata;//庄家的牛数据
        private bool _isRejoin;
        private int _round;


        public void ShowBg(bool isShow = false)
        {
            BgParent.SetActive(isShow);
        }

        private void SetData()
        {
            if (_data == null) return;

            if (Table.transform.childCount > 1)
            {
                for (var i = 1; i < Table.transform.childCount; i++)
                {
                    Table.transform.GetChild(i).GetComponent<EntiretyItem>().List.gameObject.SetActive(false);
                }
            }

            var gameRound = YxWindowUtils.CreateItem(EntiretyItem, Table.transform);

            gameRound.PlayTween.Play(true);
            if (_isRejoin)
            {
                _round++;
                gameRound.GameRound.text = _round + "";
                ShowBg();
            }
            else
            {
                gameRound.GameRound.text = App.GetGameData<TtzGameData>().CurrentRound.ToString();
            }

            var zRate = "";
            foreach (ISFSObject userData in _data)
            {
                var isZ = userData.ContainsKey(InteractParameter.IsBanker) && userData.GetBool(InteractParameter.IsBanker);
                if (!isZ) continue;
                zRate = "x" + userData.GetInt(InteractParameter.RobRate);
                _zndata = userData.GetSFSObject(InteractParameter.NiuData);
                break;
            }
            foreach (ISFSObject userData in _data)
            {

                var temp = YxWindowUtils.CreateItem(AResM, gameRound.List.transform);
                var isZ = userData.ContainsKey(InteractParameter.IsBanker) && userData.GetBool(InteractParameter.IsBanker);
                userData.PutBool(InteractParameter.IsZ, isZ);
                if (isZ)
                {
                    userData.PutSFSObject("znData", _zndata);
                    userData.PutUtfString(InteractParameter.ZhuangData, zRate);
                }
                temp.SetData(userData);
            }

            Table.repositionNow = true;

            gameRound.List.repositionNow = true;

            if (!_isRejoin) Invoke("Close", 5);
        }

        public void AddResult(ISFSArray responseData)
        {
            _isRejoin = true;
            _round = 0;
            foreach (ISFSArray data in responseData)
            {
                SetResult(data);
            }
            if (_isRejoin) Invoke("Close", 5);
            _isRejoin = false;

        }
        public void SetResult(ISFSArray responseData)
        {
            var gdata = App.GetGameData<TtzGameData>();

            _data = responseData;
            if (gdata.CurrentRound == gdata.MaxRound) return;
            SetData();
        }

        public void Close()
        {
            ShowBg();
        }
    }
}
