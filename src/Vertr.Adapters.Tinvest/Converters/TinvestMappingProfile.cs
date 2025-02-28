using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Vertr.Domain.Enums;

namespace Vertr.Adapters.Tinvest.Converters;

internal class TinvestMappingProfile : Profile
{
    public TinvestMappingProfile()
    {
        CreateMap<Tinkoff.InvestApi.V1.PriceType, PriceType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.TimeInForceType, TimeInForceType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OrderType, Domain.OrderType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OrderExecutionReportStatus, OrderExecutionReportStatus>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OrderDirection, OrderDirection>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.InstrumentType, InstrumentType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.CandleInterval, CandleInterval>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OperationType, OperationType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OperationState, OperationState>()
            .ConvertUsingEnumMapping()
            .ReverseMap();


        CreateMap<Tinkoff.InvestApi.V1.InstrumentShort, Domain.Instrument>();
        CreateMap<Tinkoff.InvestApi.V1.Instrument, Domain.InstrumentDetails>();
        CreateMap<Tinkoff.InvestApi.V1.PostOrderResponse, Domain.PostOrderResponse>();

        CreateMap<Tinkoff.InvestApi.V1.OrderState, Domain.OrderState>()
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToDateTime()))
            .ForMember(dest => dest.OrderStages, opt => opt.MapFrom(src => src.Stages.ToArray()));

        CreateMap<Tinkoff.InvestApi.V1.Account, Domain.Account>()
            .ForMember(dest => dest.OpenedDate, opt => opt.MapFrom(src => src.OpenedDate.ToDateTime()))
            .ForMember(dest => dest.ClosedDate, opt => opt.MapFrom(src => src.ClosedDate.ToDateTime()))
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<Tinkoff.InvestApi.V1.HistoricCandle, Domain.HistoricCandle>()
            .ForMember(dest => dest.TimeUtc, opt => opt.MapFrom(src => src.Time.ToDateTime()))
            .ForMember(dest => dest.CandleSource, opt => opt.MapFrom(src => (int)src.CandleSource))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsComplete));

        CreateMap<Tinkoff.InvestApi.V1.MoneyValue, Domain.Money>()
            .ConvertUsing(new MoneyConverter());

        CreateMap<Domain.Money, Tinkoff.InvestApi.V1.MoneyValue>()
            .ConvertUsing(new MoneyConverter());

        CreateMap<Tinkoff.InvestApi.V1.Operation, Domain.Operation>()
            .ConvertUsing(new OperationConverter());

        CreateMap<Tinkoff.InvestApi.V1.OperationTrade, Domain.OperationTrade>()
            .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime.ToDateTime()));

        CreateMap<Tinkoff.InvestApi.V1.OrderStage, Domain.OrderStage>()
            .ForMember(dest => dest.ExecutionTime, opt => opt.MapFrom(src => src.ExecutionTime.ToDateTime()));
    }
}
