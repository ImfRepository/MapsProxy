namespace MapsProxyApi.Domain.Entities
{
    public class ServiceUsageLimitEntity : BaseEntity
    {
        public int UserId { get; set; }

        public int ServiceId { get; set; }

        public int UsedTimes { get; set; }

        public int MaxUses { get; set; }

        public UserEntity User { get; set; }

        public ServiceEntity Service { get; set; }
    }
}
