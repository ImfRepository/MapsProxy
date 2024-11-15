namespace WebUI.Domain.Entities
{
    public class ServiceEntity : BaseEntity
    {
        public string Name { get; set; }

        public int UsedTimes { get; set; }

        public int MaxUses { get; set; }
    }
}