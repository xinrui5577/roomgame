using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    /// <summary>
    /// 每张房卡的所有局数信息的Item
    /// </summary>
    public class RoundDataItem : MonoBehaviour
    {
        public UILabel CreatRoomTime;
        public UILabel RoomId;
       
        public UIGrid PlayerTotalGrid;
        public PlayerTotalItem PlayerTotalItem;

        public UIGrid PlayerSingleGrid;
        public PlayerSingleItem PlayerSingleItem;
        
        public void InitEachPlayData(object dataInfo)
        {
            while (PlayerTotalGrid.transform.childCount > 0)
            {
                DestroyImmediate(PlayerTotalGrid.transform.GetChild(0).gameObject);
            }
            if (dataInfo == null) return;
            if (!(dataInfo is List<object>)) return;
            var datas = dataInfo as List<object>;
            
            foreach (var data in datas)
            {
                if(data==null)return;
                var eachData = (Dictionary<string, object>)data;
                var playId =eachData.ContainsKey("id")? eachData["id"].ToString():"";
                var playerName =eachData.ContainsKey("name")? eachData["name"].ToString():"";
                var playGold =eachData.ContainsKey("gold") ?eachData["gold"].ToString():"";
                string avatar = eachData.ContainsKey("avatar")? eachData["avatar"].ToString():"";
                var obj = CreatObj(PlayerTotalItem.gameObject, PlayerTotalGrid.transform);
                obj.GetComponent<PlayerTotalItem>().InitPlayerData(playerName, playGold, avatar);
            }
            PlayerTotalGrid.repositionNow = true;
            PlayerTotalGrid.Reposition();
        }
        private GameObject CreatObj(GameObject child, Transform parent)
        {
            var obj = Instantiate(child);
            obj.SetActive(true);
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
            return obj;
        }
    }
}
