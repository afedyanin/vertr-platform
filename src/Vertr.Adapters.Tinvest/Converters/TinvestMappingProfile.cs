using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

namespace Vertr.Adapters.Tinvest.Converters;

// https://github.com/AutoMapper/AutoMapper.Extensions.EnumMapping

internal class TinvestMappingProfile : Profile
{
    public TinvestMappingProfile()
    {
        CreateMap<Tinkoff.InvestApi.V1.PriceType, Domain.PriceType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.TimeInForceType, Domain.TimeInForceType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OrderType, Domain.OrderType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OrderExecutionReportStatus, Domain.OrderExecutionStatus>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.OrderDirection, Domain.OrderDirection>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.InstrumentType, Domain.InstrumentType>()
            .ConvertUsingEnumMapping()
            .ReverseMap();

        CreateMap<Tinkoff.InvestApi.V1.CandleInterval, Domain.CandleInterval>()
            .ConvertUsingEnumMapping()
            .ReverseMap();


        CreateMap<Tinkoff.InvestApi.V1.InstrumentShort, Domain.Instrument>();
        CreateMap<Tinkoff.InvestApi.V1.Instrument, Domain.InstrumentDetails>();
    }
}
