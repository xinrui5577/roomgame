using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Data
{
    public class UserObj :BaseObj
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string User;
        /// <summary>
        /// Id,数据库中的Id
        /// </summary>
        public int Id;
        /// <summary>
        /// 是否是本人
        /// </summary>
        public bool IsMe;
        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="data"></param>
        public void Init(SFSObject data)
        {
            if (data == null)
            {
                return;
            }


            if (data.ContainsKey("User"))
            {
                User = data.GetUtfString("User");
            }
            if (data.ContainsKey("Id"))
            {
                Id = data.GetInt("Id");
            }
            if (data.ContainsKey("isMe"))
            {
                IsMe = data.GetBool("isMe");
            }
        }
    }
}
