using Sfs2X.Entities.Data;
namespace Assets.Scripts.Game.jh.network{


     public class GameOverResponse
     {
         public int Gold;
         public string Nick;
         public int Id;
         public int Win;
         public int Lose;
         public int Abandon;
         public void Parse(ISFSObject data)
         {
             Gold = data.ContainsKey("gold") ? data.GetInt("gold") : 0;
             Nick = data.ContainsKey("nick") ? data.GetUtfString("nick") : null;
             Id = data.ContainsKey("id") ? data.GetInt("id") : 0;
             Win = data.ContainsKey("win") ? data.GetInt("win") : 0;
             Lose = data.ContainsKey("lose") ? data.GetInt("lose") : 0;
             Abandon = data.ContainsKey("abandon") ? data.GetInt("abandon") : 0;
         }
     }
}
