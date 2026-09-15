namespace TestTaskApi.Application.Services.Interfaces
{
    public interface IPricingService
    {
        decimal CalculateRoomCost(DateTime start, DateTime end, decimal basePricePerHour);
    }
}
