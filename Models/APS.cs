namespace MyAutodeskAPS.Models
{
    public partial class Aps
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string bucket;

        public Aps(string clientId, string clientSecret, string bucket = null)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.bucket = string.IsNullOrEmpty(bucket) ? string.Format("{0}-basic-app", this.clientId.ToLower()) : bucket;
        }
    }

}