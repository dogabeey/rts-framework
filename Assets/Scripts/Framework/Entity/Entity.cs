namespace Game.Entity
{
    public class Entity
    {
        public string Id { get; }
        public string Name { get; set; }

        public Entity(string name = "")
        {
            Id = System.Guid.NewGuid().ToString();
            Name = name;
        }
    }
}
