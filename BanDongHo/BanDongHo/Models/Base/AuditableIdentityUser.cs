using Microsoft.AspNetCore.Identity;

namespace WatchAPI.Models.Base
{
    public abstract class AuditableIdentityUser : IdentityUser
    {
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public string? CreatedBy { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }
        public string? UpdatedBy { get; protected set; }

        public bool IsDeleted { get; protected set; } = false;
        public DateTime? DeletedAt { get; protected set; }
        public string? DeletedBy { get; protected set; }

        public int Version { get; protected set; } = 0;

        protected void IncreaseVersion() => Version++;

        public void SetCreated(string? user) => CreatedBy = user;

        public void SetUpdated(string? user)
        {
            UpdatedBy = user;
            UpdatedAt = DateTime.UtcNow;
            IncreaseVersion();
        }

        public void SoftDelete(string? user)
        {
            IsDeleted = true;
            DeletedBy = user;
            DeletedAt = DateTime.UtcNow;
            IncreaseVersion();
        }
    }
}
