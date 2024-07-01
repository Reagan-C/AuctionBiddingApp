namespace PaymentService.Services
{
    public interface IInvoiceProcessor
    {
        Task ProcessInvoiceAsync(string message);
        void StartConsuming();
    }
}
