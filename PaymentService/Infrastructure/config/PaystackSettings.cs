namespace PaymentService.Infrastructure.config
{
    public class PaystackSettings
    {
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public string CallBackUrl { get; set; }
        public string CancelActionUrl { get; set; }
    }
}
