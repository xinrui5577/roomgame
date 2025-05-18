using System;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class PlayerInfoDetailCtrl : MonoBehaviour
    {
        public UITexture HeadTexture;
        public UILabel NameLabel;
        public UILabel IdLabel;
        public UILabel IpLabel;

        public GameObject DetailInfoGob;

        private int _pointSeat = -1;
        private int _pointSeadId = 0;

        public GameObject LeftPlayerGob;
        public GameObject RightPlayerGob;
        public GameObject SelfPlayerGob;

        public GameObject LeftSp;
        public GameObject RightSp;
        public GameObject SelfSp;

        public GameObject[] FaceAnimLeft;
        public GameObject[] FaceAnimRight;
        public GameObject[] FaceAnimSelf;

        [SerializeField] public string[] SoundKeyStr;

        public void Awake()
        {
            Ddz2RemoteServer.AddOnMsgChatEvt(OnUserTalk);
        }

        private void OnUserTalk(object sender, DdzEventArgs.DdzbaseEventArgs e)
        {
            var data = e.IsfObjData;

            if (!data.ContainsKey(FaceMoveType) || !data.ContainsKey(PointSeat) || !data.ContainsKey(RequestKey.KeySeat)) return;


            DetailInfoGob.SetActive(false);

            var faceMoveType = data.GetInt(FaceMoveType);
            var seat = data.GetInt(RequestKey.KeySeat);
            var pointseat = data.GetInt(PointSeat);

            var globalData = App.GetGameData<GlobalData>();

            if (seat == globalData.GetSelfSeat)
            {
                SetOrgPosTo(SelfSp, SelfPlayerGob.transform.localPosition, pointseat, faceMoveType);
            }
            else if (seat == globalData.GetLeftPlayerSeat)
            {
                SetOrgPosTo(LeftSp, LeftPlayerGob.transform.localPosition, pointseat, faceMoveType);
            }
            else if (seat == globalData.GetRightPlayerSeat)
            {
                SetOrgPosTo(RightSp, RightPlayerGob.transform.localPosition, pointseat, faceMoveType);
            }

        }

        private void SetOrgPosTo(GameObject orgPosGob, Vector3 fromPos, int pointseat, int faceMoveType)
        {

            if (orgPosGob == null || orgPosGob.GetComponent<TweenPosition>() == null || orgPosGob.GetComponent<UISprite>() == null) return;

            var sp = orgPosGob.GetComponent<UISprite>();
            sp.spriteName = faceMoveType.ToString(CultureInfo.InvariantCulture);
            sp.MakePixelPerfect();

            var orgPos = orgPosGob.GetComponent<TweenPosition>();
            orgPosGob.SetActive(true);
            orgPos.from = fromPos;
            var globalData = App.GetGameData<GlobalData>();

            GameObject[] pointToSps = null;

            if (pointseat == globalData.GetSelfSeat)
            {
                orgPos.to = SelfPlayerGob.transform.localPosition;
                pointToSps = FaceAnimSelf;
            }
            else if (pointseat == globalData.GetLeftPlayerSeat)
            {
                orgPos.to = LeftPlayerGob.transform.localPosition;
                pointToSps = FaceAnimLeft;
            }
            else if (pointseat == globalData.GetRightPlayerSeat)
            {
                orgPos.to = RightPlayerGob.transform.localPosition;
                pointToSps = FaceAnimRight;
            }

            orgPos.ResetToBeginning();
            if (pointToSps != null)
            {
                foreach (var pointToSp in pointToSps)
                {
                    pointToSp.SetActive(false);
                }
            }
            else
                return;

            EventDelegate.Add(orgPos.onFinished, () =>
            {
                orgPosGob.SetActive(false);

                StartCoroutine(StartAnimMove(pointToSps, faceMoveType));

            }, true);
            orgPos.PlayForward();

        }

        private IEnumerator StartAnimMove(GameObject[] pointToSps, int faceMoveType)
        {
            if (SoundKeyStr!=null && faceMoveType >= 0 && faceMoveType < SoundKeyStr.Length)
            {
                Facade.Instance<MusicManager>().Play(SoundKeyStr[faceMoveType]);
            }

            var uispAnim = pointToSps[faceMoveType].GetComponent<UISpriteAnimation>();

            if (uispAnim == null) yield break;
            uispAnim.gameObject.SetActive(true);
            uispAnim.ResetToBeginning();
            uispAnim.Play();

            /*            switch (faceMoveType)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                            case 5:
                                break;
                            default:

                                yield break;
                        }*/



            yield return new WaitForSeconds(3f);

            foreach (var pointToSp in pointToSps)
            {
                pointToSp.SetActive(false);
            }

        }


        public void OnClickLeftPlayerInfo()
        {
            var globalData = App.GetGameData<GlobalData>();
            _pointSeat = globalData.GetLeftPlayerSeat;
            SetPlayerInfo(globalData.GetUserInfo(globalData.GetLeftPlayerSeat));
        }


        public void OnClickRightPlayerInfo()
        {
            var globalData = App.GetGameData<GlobalData>();
            _pointSeat = globalData.GetRightPlayerSeat;
            SetPlayerInfo(globalData.GetUserInfo(globalData.GetRightPlayerSeat));
        }

        public void OnClickSelfPlayerInfo()
        {
            var globalData = App.GetGameData<GlobalData>();
            _pointSeat = globalData.GetSelfSeat;
            SetPlayerInfo(globalData.GetUserInfo(globalData.GetSelfSeat));
        }

        private void SetPlayerInfo(ISFSObject userData)
        {
            if (userData.ContainsKey("ip"))
                IpLabel.text = userData.GetUtfString("ip");

            if (userData.ContainsKey(RequestKey.KeyId))
            {
                _pointSeadId = userData.GetInt(RequestKey.KeyId);
                IdLabel.text = _pointSeadId.ToString(CultureInfo.InvariantCulture);
            }


            if (userData.ContainsKey(RequestKey.KeyName))
                NameLabel.text = userData.GetUtfString(RequestKey.KeyName);

            if (userData.ContainsKey(NewRequestKey.KeyAvatar))
            {
                short sex = 0;
                if (userData.ContainsKey(NewRequestKey.KeySex))
                    sex = userData.GetShort(NewRequestKey.KeySex);

                DDzUtil.LoadRealHeadIcon(userData.GetUtfString(NewRequestKey.KeyAvatar), sex, HeadTexture);

            }

            DetailInfoGob.SetActive(true);
        }

        public void OnCloseUi()
        {
            DetailInfoGob.SetActive(false);
        }

        public const string FaceMoveType = "FaceMove";
        public const string PointSeat = "PointSeat";
        public void OnClickAnimBtn(GameObject obj)
        {
            if (_pointSeat == App.GetGameData<GlobalData>().GetSelfSeat) return;
            int faceType;
            try
            {
                faceType = int.Parse(obj.name);
            }
            catch (Exception)
            {
                return;
            }

            GlobalData.ServInstance.SendFaceMove(faceType, _pointSeat);


        }




    }
}
