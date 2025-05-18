namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class C2SDismissRoomArgs : EvtHandlerArgs
    {
        public int DismissType;
    }

    public class C2SThrowoutCardArgs : EvtHandlerArgs
    {
        public int Card;
    }

    public class C2STingArgs : EvtHandlerArgs
    {
        public int Prol;
        public int Card;
        public int[] LiangCards;
    }
}
