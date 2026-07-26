namespace Game.RTS
{
    [System.Serializable]
    public abstract class GameMode
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
    }
    [System.Serializable]
    public class StandardGameMode : GameMode
    {
        public override string Name => "Standard";
        public override string Description => "This is the standard game mode.";
    }
}
