using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public interface IRuntimeData
    {   
        void ResetData();
        void SetData(ISFSObject data);      
    }

    public enum RuntimeDataType
    {
        None,
        Players,
        Config,
        Game,
        Room,
    }
}
