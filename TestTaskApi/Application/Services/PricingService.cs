using TestTaskApi.Application.Services.Interfaces;

namespace TestTaskApi.Application.Services
{
    public class PricingService : IPricingService
    {
        // Границы тарифных зон в течение суток (часы)
        private static readonly int[] Boundaries = { 0, 6, 9, 12, 14, 18, 23, 24 };

        public decimal CalculateRoomCost(DateTime start, DateTime end, decimal basePricePerHour)
        {
            decimal total = 0;
            var cursor = start;

            while (cursor < end)
            {
                var nextBoundary = GetNextBoundary(cursor);
                var segmentEnd = nextBoundary < end ? nextBoundary : end;

                var durationHours = (decimal)(segmentEnd - cursor).TotalHours;
                var multiplier = GetMultiplier(cursor.TimeOfDay);

                total += durationHours * basePricePerHour * multiplier;
                cursor = segmentEnd;
            }

            return total;
        }

        private static DateTime GetNextBoundary(DateTime cursor)
        {
            var hourOfDay = cursor.TimeOfDay.TotalHours;

            foreach (var boundary in Boundaries)
            {
                if (boundary > hourOfDay)
                    return cursor.Date.AddHours(boundary);
            }

            // Ничего не нашли на сегодня - граница в полночь следующего дня
            return cursor.Date.AddDays(1);
        }

        private static decimal GetMultiplier(TimeSpan timeOfDay)
        {
            var hour = timeOfDay.TotalHours;

            if (hour >= 12 && hour < 14)
                return 1.15m; // Пиковые часы: +15%

            if (hour >= 6 && hour < 9)
                return 0.9m; // Утренние часы: -10%

            if (hour >= 18 && hour < 23)
                return 0.8m; // Вечерние часы: -20%

            if (hour >= 9 && hour < 18)
                return 1m; // Стандартные часы: базовая цена

            return 1m; // Вне описанных диапазонов (23:00-06:00) — базовая цена по умолчанию
        }
    }
}
