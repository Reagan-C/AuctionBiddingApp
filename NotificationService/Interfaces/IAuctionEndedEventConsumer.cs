namespace NotificationService.Interfaces
{
    public interface IAuctionEndedEventConsumer
    {
        void ConsumeEvent(string message);
    }
}
