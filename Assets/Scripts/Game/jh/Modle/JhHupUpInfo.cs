using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using Assets.Scripts.Game.jh.Public;
using System.Collections;

namespace Assets.Scripts.Game.jh.Modle
{
    public enum HupType
    {
        TongYi = 3,
        Start = 2,
        None = 0,
        JuJue = -1,
    }
    public class JhHupUpInfo : MonoBehaviour
    {
        
        public class HupUp
        {
            public YxBaseUserInfo UserInfo;
            public HupType Status;

            public ISFSObject GetSfsObject()
            {
                ISFSObject obj = SFSObject.NewInstance();
                obj.PutUtfString("Name", UserInfo.NickM);
                obj.PutInt("Sex", UserInfo.SexI);
                obj.PutUtfString("Head", UserInfo.AvatarX);
                obj.PutInt("Status", (int)Status);
                return obj;
            }
        }

        //投票时间
        public int HupTime;
        //是不是 第一次投票
        [HideInInspector]
        public bool IsNotFirstTime;
        [HideInInspector]
        protected long StartTime;
        [HideInInspector]
        protected List<HupUp> UserHupUp;
        [HideInInspector]
        public EventObject EventObj;
        [HideInInspector]
        protected string SelfID;
        [HideInInspector]
        protected bool SelfChoose;
        public void OnRecevie(object data)
        {
            EventData eData = (EventData) data;
            switch (eData.Name)
            {
                case "HupUpResponse":
                    OnHupUpResponse(eData.Data);
                    break;
            }
        }

        public void FrashHupUpUser()
        {
            UserHupUp = new List<HupUp>();
            JhGameTable table = App.GetGameData<JhGameTable>();
            foreach (var userInfo in table.UserInfoDict)
            {
                HupUp hup = new HupUp ();
                hup.UserInfo = userInfo.Value;
                hup.Status = 0;
                UserHupUp.Add(hup);
            }
            SelfID = table.GetPlayerInfo<JhUserInfo>().UserId;
        }

        public void OnGameInfo(ISFSObject data)
        {
            if (data.ContainsKey("tptime"))
            {
                HupTime = data.GetInt("tptime");
            }

            string hupInfo;
            if (data.ContainsKey("hup"))
            {
                hupInfo = data.GetUtfString("hup");
            }
            else
            {
                return;
            }
            //接收重连解散信息
            if (!string.IsNullOrEmpty(hupInfo))
            {
                IsNotFirstTime = true;
                StartTime = data.ContainsKey("hupstart") ? data.GetLong("hupstart") : 0;
                string[] ids = hupInfo.Split(',');
                
                for (int i = 0; i < ids.Length; i++)
                {
                    for (int j = 0, max = UserHupUp.Count; j < max; j++)
                    {
                        var id = int.Parse(ids[i]);
                        if (id.Equals(int.Parse(UserHupUp[j].UserInfo.UserId)))
                        {
                            //2发起投票 3同意 -1拒绝
                            UserHupUp[j].Status = i == 0 ? HupType.Start : HupType.TongYi;
                            if (id.Equals(int.Parse(SelfID)))
                            {
                                SelfChoose = true;
                            }
                        }
                    }
                }

                ISFSObject sendObj = SFSObject.NewInstance();
                ISFSArray arr = SFSArray.NewInstance();
                foreach (HupUp hupUp in UserHupUp)
                {
                    arr.AddSFSObject(hupUp.GetSfsObject());
                }
                sendObj.PutSFSArray("Users", arr);
                sendObj.PutInt("HupTime", HupTime);
                sendObj.PutLong("StartTime", StartTime);
                sendObj.PutBool("SelfChoose", SelfChoose);
                //发送投票view
                EventObj.SendEvent("HupUpViewEvent", "HupUp", sendObj);
            }
        }

        protected void OnHupUpResponse(object data)
        {

            ISFSObject obj = (ISFSObject) data;
            int id = obj.GetInt(RequestKey.KeyId);
            int starus = obj.GetInt(RequestKey.KeyType);

            if (starus == 2)
            {
                SelfChoose = false;
                foreach (HupUp hupUp in UserHupUp)
                {
                    hupUp.Status = 0;
                }
            }
            foreach (HupUp hupUp in UserHupUp)
            {
                if (hupUp.UserInfo.UserId.Equals(id.ToString()))
                {
                    hupUp.Status = (HupType)starus;
                }
            }
            if (id.Equals(int.Parse(SelfID)))
            {
                SelfChoose = true;
            }

            ISFSObject sendObj = SFSObject.NewInstance();
            if (!IsNotFirstTime)
            {
                StartTime = JhFunc.GetTimeStamp();
                IsNotFirstTime = true;
                sendObj.PutInt("HupTime", HupTime);
                sendObj.PutLong("StartTime", StartTime);
            }

            ISFSArray arr = SFSArray.NewInstance();
            foreach (HupUp hupUp in UserHupUp)
            {
                arr.AddSFSObject(hupUp.GetSfsObject());
            }
            sendObj.PutSFSArray("Users", arr);
            sendObj.PutBool("SelfChoose", SelfChoose);
            //发送投票view
            EventObj.SendEvent("HupUpViewEvent", "HupUp", sendObj);

            if (starus == -1)
            {
                CloseHup();
                Reset();
            }
        }

        protected void CloseHup()
        {
            //延迟一秒 发送关闭
            StartCoroutine(CloseHupIe());
        }

        public IEnumerator CloseHupIe()
        {
            yield return new WaitForSeconds(1.0f);
            EventObj.SendEvent("HupUpViewEvent", "Close", null);
        }

        public void Reset()
        {
            foreach (HupUp hupUp in UserHupUp)
            {
                hupUp.Status = 0;
            }
            StartTime = 0;
            IsNotFirstTime = false;
            SelfChoose = false;
        }
    }
}
