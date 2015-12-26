namespace Virgil.SDK.Keys.Domain
{
    using System.Threading.Tasks;
    using Infrastructurte;
    using TransferObject;

    public class IdentityTokenRequest
    {
        private readonly VirgilVerifyResponse response;

        internal IdentityTokenRequest()
        {
        }

        internal IdentityTokenRequest(VirgilVerifyResponse virgilVerifyResponse)
        {
            this.response = virgilVerifyResponse;
        }

        public string Identity { get; private set; }

        public IdentityType IdentityType { get; private set; }

        internal static async Task<IdentityTokenRequest> Verify(string value, IdentityType type)
        {
            var identityService = ServiceLocator.Services.Identity;
            var request = await identityService.Verify(value, type);
            return new IdentityTokenRequest(request)
            {
                Identity = value,
                IdentityType = type
            };
        }

        public async Task<IdentityToken> Confirm(string confirmationCode, ConfirmOptions options = null)
        {
            options = options ?? ConfirmOptions.Default;

            var identityService = ServiceLocator.Services.Identity;
            var token = await identityService.Confirm(
                        confirmationCode, this.response.ActionId, 
                        options.TimeToLive, options.CountToLive);

            return new IdentityToken(this, token);
        }
    }
}