namespace Vertr.Domain;
public enum OrderExecutionStatus
{
    // None
    Unspecified = 0,

    // Исполнена
    Fill = 1,

    // Отклонена
    Rejected = 2,

    // Отменена пользователем
    Cancelled = 3,

    // Новая
    New = 4,

    // Частично исполнена
    Partiallyfill = 5
}
