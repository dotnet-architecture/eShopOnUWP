using System;
using System.Threading.Tasks;

using Windows.Storage.Streams;

namespace eShop.UWP.Authentication
{
    static public class AuthenticationService
    {
        static public Task<Result> AuthenticateAsync(string userName, string password)
        {
            // Perform authentication here.
            // This sample accepts any user name and password.
            return Task.FromResult(Result.Ok());
        }

        static public Task<bool> RegisterPassportCredentialWithServerAsync(IBuffer publicKey)
        {
            // TODO:
            // Register the public key and attestation of the key credential with the server
            // In a real-world scenario, this would likely also include:
            //      - Certificate chain for attestation endorsement if available
            //      - Status code of the Key Attestation result : Included / retrieved later / retry type
            return Task.FromResult(true);
        }
    }
}
