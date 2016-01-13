namespace Virgil.SDK.Domain
{
    using System;
    using System.Threading.Tasks;
    using Virgil.SDK.TransferObject;

    public class Identity
    {
        protected Identity()
        {
        }

        internal Identity(VirgilIdentityDto virgilIdentityDto)
        {
            this.Id = virgilIdentityDto.Id;
            this.Value = virgilIdentityDto.Value;
            this.Type = virgilIdentityDto.Type;
            this.CreatedAt = virgilIdentityDto.CreatedAt;
        }

        public Guid Id { get; protected set; }

        public string Value { get; protected set; }

        public IdentityType Type { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public async Task<IdentityTokenRequest> Verify()
        {
            return await IdentityTokenRequest.Verify(this.Value, this.Type).ConfigureAwait(false);
        }

        public static async Task<IdentityTokenRequest> Verify(string value, IdentityType type = IdentityType.Email)
        {
            return await IdentityTokenRequest.Verify(value, type).ConfigureAwait(false);
        }
    }
}