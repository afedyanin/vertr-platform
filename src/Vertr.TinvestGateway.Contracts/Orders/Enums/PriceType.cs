namespace Vertr.TinvestGateway.Contracts.Orders.Enums;

public enum PriceType
{
    // Значение не определено
    Unspecified = 0,

    // Цена в пунктах (только для фьючерсов и облигаций)
    Point = 1,

    // Цена в валюте расчётов по инструменту
    Currency = 2
}