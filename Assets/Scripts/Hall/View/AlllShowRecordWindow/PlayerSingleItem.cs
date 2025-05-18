using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    /// <summary>
    /// 玩家单局信息的Item
    /// </summary>
    public class PlayerSingleItem : MonoBehaviour
    {
        public UISprite ReplayBtn;

        public UIGrid ReplayGrid;
        public PlayerSingleData PlayerSingleData;

        public void InitEachPlayData(object dataInfo)
        {
            while (ReplayGrid.transform.childCount > 0)
            {
                DestroyImmediate(ReplayGrid.transform.GetChild(0).gameObject);
            }
             if (dataInfo == null) return;
            if (!(dataInfo is List<object>)) return;
            var datas = dataInfo as List<object>;

            foreach (var data in datas)
            {
                if(data==null)return;
                var eachData = (Dictionary<string, object>)data;
                var playId =eachData.ContainsKey("id")?eachData["id"].ToString():"";
                var playerName =eachData.ContainsKey("name")? eachData["name"].ToString():"";
                var playGold =eachData.ContainsKey("gold")? eachData["gold"].ToString():"";
                var obj = CreatObj(PlayerSingleData.gameObject, ReplayGrid.transform);
                string hu = eachData.ContainsKey("hu")? eachData["hu"].ToString():"0";
                obj.GetComponent<PlayerSingleData>().InitPlayerData(playerName, playGold,hu);
            }
            ReplayGrid.repositionNow = true;
            ReplayGrid.Reposition();
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
