namespace MapsProxyApi.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Token { get; set; }

        public IEnumerable<ServiceUsageLimitEntity> UsageLimits { get; set; }
    }
}