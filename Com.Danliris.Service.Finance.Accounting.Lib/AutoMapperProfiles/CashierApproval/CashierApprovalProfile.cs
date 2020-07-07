﻿using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CashierApproval;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CashierApproval;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.AutoMapperProfiles.CashierApproval
{
    public class CashierApprovalProfile : Profile
    {
        public CashierApprovalProfile()
        {
            CreateMap<CashierApprovalModel, CashierApprovalViewModel>()

                .ForPath(d => d.Currency.Id, opt => opt.MapFrom(s => s.CurrencyId))
                .ForPath(d => d.Currency.Code, opt => opt.MapFrom(s => s.CurrencyCode))
                .ForPath(d => d.Currency.Rate, opt => opt.MapFrom(s => s.CurrencyRate))
                .ForPath(d => d.Currency.Symbol, opt => opt.MapFrom(s => s.CurrencySymbol))

                .ForPath(d => d.Unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.Unit.Code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.Unit.Name, opt => opt.MapFrom(s => s.UnitName))

                .ReverseMap();
        }
    }
}
