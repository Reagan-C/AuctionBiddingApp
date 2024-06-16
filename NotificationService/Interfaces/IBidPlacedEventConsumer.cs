namespace NotificationService.Interfaces
{
    public interface IBidPlacedEventConsumer
    {
        void ConsumeEvent(string message);
    }
}
