namespace PaymentService.Services
{
    public interface IInvoiceProcessor
    {
        Task ProcessInvoicesAsync(CancellationToken cancellationToken);
    }
}
