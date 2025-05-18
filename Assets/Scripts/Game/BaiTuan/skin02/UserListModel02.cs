using UnityEngine;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class UserListModel02 : UserListModel
    {
        [SerializeField] private UISprite _rankMark;

        [SerializeField] private UILabel _rankLabel;

        [SerializeField] private string _rankMarkName;


        public override void SetInfo(YxBaseUserInfo brnnUser, bool isBanker = false, int rank = -1)
        {
            base.SetInfo(brnnUser, isBanker, rank);
            if (rank <= 3)
            {
                _rankMark.spriteName = string.Format("{0}{1}", _rankMarkName, rank);
                SetObjActive(_rankMark.gameObject, true);
                _rankMark.MakePixelPerfect();
                SetObjActive(_rankLabel.gameObject, false);
            }
            else
            {
                _rankLabel.text = rank.ToString();
                SetObjActive(_rankLabel.gameObject, true);
                SetObjActive(_rankMark.gameObject, false);
            }
        }

        void SetObjActive(GameObject obj, bool active)
        {
            if (obj == null)
                return;
            obj.SetActive(active);
        }

        //public override void SetInfo(string players, int rank)
        //{
        //    Debug.LogError(" ==== setInfo 02 ==== ");
        //    base.SetInfo(players, rank);
        //    if(rank > 3)
        //    {
        //        _rankLabel.text = rank.ToString();
        //        _rankMark.gameObject.SetActive(false);
        //        _rankLabel.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        _rankMark.spriteName = _rankMarkName + rank.ToString();
        //        _rankMark.gameObject.SetActive(true);
        //        _rankLabel.gameObject.SetActive(false);
        //    }
        //}

        public override void SetInfo(string players, int rank)
        {
            base.SetInfo(players, rank);
            if (rank > 3)
            {
                _rankLabel.text = rank.ToString();
                _rankMark.gameObject.SetActive(false);
                _rankLabel.gameObject.SetActive(true);
            }
            else
            {
                _rankMark.spriteName = _rankMarkName + rank.ToString();
                _rankMark.gameObject.SetActive(true);
                _rankLabel.gameObject.SetActive(false);
            }
        }
    }
}