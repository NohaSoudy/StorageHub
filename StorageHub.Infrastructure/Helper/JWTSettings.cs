namespace StorageHub.Infrastructure
{
    public class JWTSettings
    {
        public string SecretKey { get; set; }     // Secret key used to sign the token
        public string Issuer { get; set; }       // Who created the token
        public string Audience { get; set; }     // Who the token is intended for
        public int ExpiryMinutes { get; set; }    // How long the token is valid (optional)
    }
}
