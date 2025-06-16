namespace Vertr.OrderExecution.Contracts.Enums;
public enum OperationType
{
    // Тип операции не определён.
    Unspecified = 0,

    // Пополнение брокерского счёта.
    //Input = 1,

    // Вывод денежных средств.
    //Output = 9,

    // Покупка ЦБ.
    Buy = 15,

    // Продажа ЦБ.
    Sell = 22,

    // Удержание комиссии за операцию.
    BrokerFee = 19,
}
