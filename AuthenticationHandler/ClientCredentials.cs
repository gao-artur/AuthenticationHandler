namespace AuthenticationHandler
{
    public class ClientCredentials
    {
        /// <summary>
        /// The client_id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// The client_secret
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Required scopes separated by spaces.
        /// </summary>
        public string Scopes { get; set; }
    }
}
