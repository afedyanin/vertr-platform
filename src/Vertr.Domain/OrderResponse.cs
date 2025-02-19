namespace Vertr.Domain;
public record class OrderResponse
{
    // Биржевой идентификатор заявки.
    public string OrderId { get; set; } = string.Empty;

    // UID идентификатор инструмента.
    public string OrderRequestId { get; set; } = string.Empty;

    // Идентификатор трекинга
    public string TrackingId { get; set; } = string.Empty;

    // Серверное время
    public DateTime ServerTime { get; set; }

    // Текущий статус заявки.
    public OrderExecutionStatus Status { get; set; }

    // Запрошено лотов.
    public long LotsRequested { get; set; }

    // Исполнено лотов.
    public long LotsExecuted { get; set; }

    // Начальная цена заявки. Произведение количества запрошенных лотов на цену.
    public decimal InitialOrderPrice { get; set; }

    // Исполненная средняя цена одного инструмента в заявке.
    public decimal ExecutedOrderPrice { get; set; }

    // Итоговая стоимость заявки, включающая все комиссии.
    public decimal TotalOrderAmount { get; set; }

    // Начальная комиссия. Комиссия рассчитанная при выставлении заявки.
    public decimal InitialCommission { get; set; }

    // Фактическая комиссия по итогам исполнения заявки.
    public decimal ExecutedCommission { get; set; }

    // Направление сделки.
    public OrderDirection Direction { get; set; }

    // Начальная цена за 1 инструмент.
    // Для получения стоимости лота требуется умножить на лотность инструмента.
    public decimal InitialSecurityPrice { get; set; }

    // Тип заявки.
    public OrderType OrderType { get; set; }

    // Дополнительные данные об исполнении заявки.
    public string Message { get; set; } = string.Empty;

    // UID идентификатор инструмента.
    public string InstrumentId { get; set; } = string.Empty;
}
