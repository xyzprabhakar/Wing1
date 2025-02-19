﻿using B2BClasses.Database;
using B2BClasses.Models;
using B2BClasses.Services.Air;
using B2BClasses.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B2BClasses
{
   

    public interface IMarkup
    {
        DateTime BookingDate { get; set; }
        int CustomerId { get; set; }
        DateTime EffectiveFromDt { get; set; }
        DateTime EffectiveToDt { get; set; }

        bool AddConvenience(mdlWingMarkup mdl, int UserId);
        bool AddCustomerFlightAPI(mdlWingMarkup mdl, int UserId);
        bool AddDiscount(mdlWingMarkup mdl, int UserId);
        bool AddMarkup(mdlWingMarkup mdl, int UserId);
        bool CalculateTotalPriceAfterMarkup(List<List<mdlSearchResult>> mdls, int AdultCount, int ChildCount, int InfantCount,string mode);
        void CustomerMarkup(List<List<mdlSearchResult>> mdl,int custid);
        List<mdlWingMarkup> LoadConvenience(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false);
        List<mdlWingMarkup> LoadCustomerFlightAPI(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false);
        List<mdlWingMarkup> LoadDiscount(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false);
        List<mdlWingMarkup> LoadMarkup(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false);
        void LoadMarkupAirlineCode(List<mdlWingMarkup> _mdl);
        void LoadMarkupCustomerCode(List<mdlWingMarkup> _mdl);
        bool PassengerTypeConvenience(mdlWingMarkup mdlA, int AdultCount, int ChildCount, int InfantCount);
        bool PassengerTypeDiscount(mdlWingMarkup mdlA, int AdultCount, int ChildCount, int InfantCount);
        bool RemoveConvenience(int Id, int UserId);
        bool RemoveCustomerFlightAPI(int Id, int UserId);
        bool RemoveDiscount(int Id, int UserId);
        bool RemoveMarkup(int Id, int UserId);
        void WingConvenienceAmount(mdlFareQuotResponse mdl, List<mdlTravellerinfo> travellerInfo);
        void WingDiscountAmount(List<List<mdlSearchResult>> mdl, int AdultCount = 1, int ChildCount = 0, int InfantCount = 0);
        void WingDiscountAmount(mdlFareQuotResponse mdl, List<mdlTravellerinfo> travellerInfo);
        void WingMarkupAmount(List<List<mdlSearchResult>> mdl, int AdultCount = 1, int ChildCount = 0, int InfantCount = 0);
    }

    public class Markup :  IMarkup
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public int CustomerId { get { return _CustomerId; } set { _CustomerId = value; } }
        private int _CustomerId;
        public DateTime EffectiveFromDt { get { return _EffectiveFromDt.Value; } set { _EffectiveFromDt = value; } }
        private DateTime? _EffectiveFromDt;

        public DateTime EffectiveToDt { get { return _EffectiveToDt.Value; } set { _EffectiveToDt = value; } }
        private DateTime? _EffectiveToDt;

        public DateTime BookingDate { get { return _BookingDate.Value; } set { _BookingDate = value; } }
        private DateTime? _BookingDate;

        private readonly string _DefaultAirServiceProvider;
        public Markup(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            if (_EffectiveFromDt == null)
            {
                _EffectiveFromDt = DateTime.Now;
            }

            if (_EffectiveToDt == null)
            {
                _EffectiveToDt = DateTime.Now;
            }
            if (_BookingDate == null)
            {
                _BookingDate = DateTime.Now;
            }
            _DefaultAirServiceProvider = _config["DefaultAirServiceProvider"];
        }

        public List<mdlWingMarkup> LoadMarkup(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false)
        {
            List<mdlWingMarkup> _mdl = null;
            if (Id > 0)
            {
                _mdl = _context.tblWingMarkupMaster.Where(p => p.Id == Id && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingMarkupServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingMarkupCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingMarkupCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingMarkupPassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingMarkupFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingMarkupAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();

            }
            else if (FromEffectiveDt)
            {
                _mdl = _context.tblWingMarkupMaster.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt && p.EffectiveToDt >= this._EffectiveFromDt && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingMarkupServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingMarkupCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingMarkupCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingMarkupPassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingMarkupFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingMarkupAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }
            else if (IsForCustomer)
            {
                enmCustomerType CustomerType = _context.tblCustomerMaster.Where(p => p.Id == CustomerId).FirstOrDefault()?.CustomerType ?? enmCustomerType.B2C;
                _mdl = _context.tblWingMarkupMaster.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt && p.EffectiveToDt >= this._EffectiveFromDt &&
             ((p.IsAllCustomer && p.IsAllCustomerType) ||
             (p.IsAllCustomer && p.tblWingMarkupCustomerType.Where(p => p.customerType == CustomerType).Count() > 0) ||
             (p.tblWingMarkupCustomerDetails.Where(p => p.CustomerId == _CustomerId).Count() > 0)
             )
             && !p.IsDeleted).Select(p => new mdlWingMarkup
             {
                 Id = p.Id,
                 Applicability = p.Applicability,
                 DirectFlight = p.DirectFlight,
                 IsAllProvider = p.IsAllProvider,
                 IsAllCustomerType = p.IsAllCustomerType,
                 IsAllCustomer = p.IsAllCustomer,
                 IsAllPessengerType = p.IsAllPessengerType,
                 IsAllFlightClass = p.IsAllFlightClass,
                 IsAllAirline = p.IsAllAirline,
                 Gender = p.Gender,
                 Amount = p.Amount,
                 EffectiveFromDt = p.EffectiveFromDt,
                 EffectiveToDt = p.EffectiveToDt,
                 ModifiedBy = p.ModifiedBy,
                 ModifiedDt = p.ModifiedDt,
                 remarks = p.Remarks,
                 MarkupServiceProvider = p.tblWingMarkupServiceProvider.Select(p => p.ServiceProvider).ToList(),
                 MarkupCustomerType = p.tblWingMarkupCustomerType.Select(p => p.customerType).ToList(),
                 MarkupCustomerDetail = p.tblWingMarkupCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                 MarkupPassengerType = p.tblWingMarkupPassengerType.Select(p => p.PassengerType).ToList(),
                 MarkupCabinClass = p.tblWingMarkupFlightClass.Select(p => p.CabinClass).ToList(),
                 MarkupAirline = p.tblWingMarkupAirline.Select(p => p.AirlineId.Value).ToList()
             }).ToList();
            }
            else
            {
                _mdl = _context.tblWingMarkupMaster.Where(p => !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingMarkupServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingMarkupCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingMarkupCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingMarkupPassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingMarkupFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingMarkupAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }

            if (_mdl == null)
            {
                _mdl = new List<mdlWingMarkup>();
            }
            return _mdl;
        }

        public List<mdlWingMarkup> LoadConvenience(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false)
        {
            List<mdlWingMarkup> _mdl = null;
            if (Id > 0)
            {
                _mdl = _context.tblWingConvenience.Where(p => p.Id == Id && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingConvenienceServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingConvenienceCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingConvenienceCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingConveniencePassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingConvenienceFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingConvenienceAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();

            }
            else if (FromEffectiveDt)
            {
                _mdl = _context.tblWingConvenience.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt && p.EffectiveToDt >= this._EffectiveFromDt && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingConvenienceServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingConvenienceCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingConvenienceCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingConveniencePassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingConvenienceFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingConvenienceAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }
            else if (IsForCustomer)
            {
                enmCustomerType CustomerType = _context.tblCustomerMaster.Where(p => p.Id == CustomerId).FirstOrDefault()?.CustomerType ?? enmCustomerType.B2C;
                _mdl = _context.tblWingConvenience.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt && p.EffectiveToDt >= this._EffectiveFromDt &&
             ((p.IsAllCustomer && p.IsAllCustomerType) ||
             (p.IsAllCustomer && p.tblWingConvenienceCustomerType.Where(p => p.customerType == CustomerType).Count() > 0) ||
             (p.tblWingConvenienceCustomerDetails.Where(p => p.CustomerId == _CustomerId).Count() > 0)
             )
             && !p.IsDeleted).Select(p => new mdlWingMarkup
             {
                 Id = p.Id,
                 Applicability = p.Applicability,
                 DirectFlight = p.DirectFlight,
                 IsAllProvider = p.IsAllProvider,
                 IsAllCustomerType = p.IsAllCustomerType,
                 IsAllCustomer = p.IsAllCustomer,
                 IsAllPessengerType = p.IsAllPessengerType,
                 IsAllFlightClass = p.IsAllFlightClass,
                 IsAllAirline = p.IsAllAirline,
                 Gender = p.Gender,
                 Amount = p.Amount,
                 EffectiveFromDt = p.EffectiveFromDt,
                 EffectiveToDt = p.EffectiveToDt,
                 ModifiedBy = p.ModifiedBy,
                 ModifiedDt = p.ModifiedDt,
                 remarks = p.Remarks,
                 MarkupServiceProvider = p.tblWingConvenienceServiceProvider.Select(p => p.ServiceProvider).ToList(),
                 MarkupCustomerType = p.tblWingConvenienceCustomerType.Select(p => p.customerType).ToList(),
                 MarkupCustomerDetail = p.tblWingConvenienceCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                 MarkupPassengerType = p.tblWingConveniencePassengerType.Select(p => p.PassengerType).ToList(),
                 MarkupCabinClass = p.tblWingConvenienceFlightClass.Select(p => p.CabinClass).ToList(),
                 MarkupAirline = p.tblWingConvenienceAirline.Select(p => p.AirlineId.Value).ToList()
             }).ToList();
            }
            else
            {
                _mdl = _context.tblWingConvenience.Where(p => !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingConvenienceServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingConvenienceCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingConvenienceCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingConveniencePassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingConvenienceFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingConvenienceAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }

            if (_mdl == null)
            {
                _mdl = new List<mdlWingMarkup>();
            }
            //var MarkupAirlineIds = _mdl.SelectMany(p => p.MarkupAirline).ToList();
            //var AirlineDetails = _context.tblAirline.Where(p => MarkupAirlineIds.Contains(p.Id)).ToList();
            //foreach (var md in _mdl)
            //{
            //    if (md.MarkupAirline == null)
            //    {
            //        md.MarkupAirline = new List<int>();
            //        md.MarkupAirlineCode = new List<string>();
            //        continue;
            //    }
            //    md.MarkupAirlineCode = (from t1 in md.MarkupAirline
            //                            join t2 in AirlineDetails on t1 equals t2.Id
            //                            select t2.Code).ToList();
            //}
            return _mdl;
        }

        public List<mdlWingMarkup> LoadDiscount(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false)
        {
            List<mdlWingMarkup> _mdl = null;
            if (Id > 0)
            {
                _mdl = _context.tblWingDiscount.Where(p => p.Id == Id && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    DayCount = p.DayCount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingDiscountServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingDiscountCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingDiscountCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingDiscountPassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingDiscountFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingDiscountAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();

            }
            else if (FromEffectiveDt)
            {
                _mdl = _context.tblWingDiscount.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt && p.EffectiveToDt >= this._EffectiveFromDt && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    DayCount = p.DayCount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingDiscountServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingDiscountCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingDiscountCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingDiscountPassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingDiscountFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingDiscountAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }
            else if (IsForCustomer)
            {

                enmCustomerType CustomerType = _context.tblCustomerMaster.Where(p => p.Id == CustomerId).FirstOrDefault()?.CustomerType ?? enmCustomerType.B2C;
                _mdl = _context.tblWingDiscount.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt && p.EffectiveToDt >= this._EffectiveFromDt &&

             ((p.IsAllCustomer && p.IsAllCustomerType) ||
             (p.IsAllCustomer && p.tblWingDiscountCustomerType.Where(p => p.customerType == CustomerType).Count() > 0) ||
             (p.tblWingDiscountCustomerDetails.Where(p => p.CustomerId == _CustomerId).Count() > 0)
             )
             && !p.IsDeleted).Select(p => new mdlWingMarkup
             {
                 Id = p.Id,
                 Applicability = p.Applicability,
                 DirectFlight = p.DirectFlight,
                 IsAllProvider = p.IsAllProvider,
                 IsAllCustomerType = p.IsAllCustomerType,
                 IsAllCustomer = p.IsAllCustomer,
                 IsAllPessengerType = p.IsAllPessengerType,
                 IsAllFlightClass = p.IsAllFlightClass,
                 IsAllAirline = p.IsAllAirline,
                 Gender = p.Gender,
                 Amount = p.Amount,
                 DayCount = p.DayCount,
                 EffectiveFromDt = p.EffectiveFromDt,
                 EffectiveToDt = p.EffectiveToDt,
                 ModifiedBy = p.ModifiedBy,
                 ModifiedDt = p.ModifiedDt,
                 remarks = p.Remarks,
                 MarkupServiceProvider = p.tblWingDiscountServiceProvider.Select(p => p.ServiceProvider).ToList(),
                 MarkupCustomerType = p.tblWingDiscountCustomerType.Select(p => p.customerType).ToList(),
                 MarkupCustomerDetail = p.tblWingDiscountCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                 MarkupPassengerType = p.tblWingDiscountPassengerType.Select(p => p.PassengerType).ToList(),
                 MarkupCabinClass = p.tblWingDiscountFlightClass.Select(p => p.CabinClass).ToList(),
                 MarkupAirline = p.tblWingDiscountAirline.Select(p => p.AirlineId.Value).ToList()
             }).ToList().Where(p => p.DayCount <= this._BookingDate.Value.Subtract(this._EffectiveFromDt.Value).Days).ToList();
            }
            else
            {
                _mdl = _context.tblWingDiscount.Where(p => !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    Applicability = p.Applicability,
                    DirectFlight = p.DirectFlight,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    Gender = p.Gender,
                    Amount = p.Amount,
                    DayCount = p.DayCount,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingDiscountServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingDiscountCustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingDiscountCustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupPassengerType = p.tblWingDiscountPassengerType.Select(p => p.PassengerType).ToList(),
                    MarkupCabinClass = p.tblWingDiscountFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingDiscountAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }

            if (_mdl == null)
            {
                _mdl = new List<mdlWingMarkup>();
            }
            //var MarkupAirlineIds = _mdl.SelectMany(p => p.MarkupAirline).ToList();
            //var AirlineDetails = _context.tblAirline.Where(p => MarkupAirlineIds.Contains(p.Id)).ToList();
            //foreach (var md in _mdl)
            //{
            //    if (md.MarkupAirline == null)
            //    {
            //        md.MarkupAirline = new List<int>();
            //        md.MarkupAirlineCode = new List<string>();
            //        continue;
            //    }
            //    md.MarkupAirlineCode = (from t1 in md.MarkupAirline
            //                            join t2 in AirlineDetails on t1 equals t2.Id
            //                            select t2.Code).ToList();
            //}
            return _mdl;
        }


        public List<mdlWingMarkup> LoadCustomerFlightAPI(int Id = 0, bool FromEffectiveDt = false, bool IsForCustomer = false)
        {
            List<mdlWingMarkup> _mdl = null;
            if (Id > 0)
            {
                _mdl = _context.tblWingCustomerFlightAPI.Where(p => p.Id == Id && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingCustomerFlightAPIServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingCustomerFlightAPICustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingCustomerFlightAPICustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupCabinClass = p.tblWingCustomerFlightAPIFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingCustomerFlightAPIAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();

            }
            else if (FromEffectiveDt)
            {
                _mdl = _context.tblWingCustomerFlightAPI.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt && !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingCustomerFlightAPIServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingCustomerFlightAPICustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingCustomerFlightAPICustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupCabinClass = p.tblWingCustomerFlightAPIFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingCustomerFlightAPIAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }
            else if (IsForCustomer)
            {
                enmCustomerType CustomerType = _context.tblCustomerMaster.Where(p => p.Id == CustomerId).FirstOrDefault()?.CustomerType ?? enmCustomerType.B2C;
                _mdl = _context.tblWingCustomerFlightAPI.Where(p => p.EffectiveFromDt <= this._EffectiveFromDt &&
             ((p.IsAllCustomer && p.IsAllCustomerType) ||
             (p.IsAllCustomer && p.tblWingCustomerFlightAPICustomerType.Where(p => p.customerType == CustomerType).Count() > 0) ||
             (p.tblWingCustomerFlightAPICustomerDetails.Where(p => p.CustomerId == _CustomerId).Count() > 0)
             )
             && !p.IsDeleted).Select(p => new mdlWingMarkup
             {
                 Id = p.Id,
                 IsAllProvider = p.IsAllProvider,
                 IsAllCustomerType = p.IsAllCustomerType,
                 IsAllCustomer = p.IsAllCustomer,
                 IsAllPessengerType = p.IsAllPessengerType,
                 IsAllFlightClass = p.IsAllFlightClass,
                 IsAllAirline = p.IsAllAirline,
                 EffectiveFromDt = p.EffectiveFromDt,
                 EffectiveToDt = p.EffectiveToDt,
                 ModifiedBy = p.ModifiedBy,
                 ModifiedDt = p.ModifiedDt,
                 remarks = p.Remarks,
                 MarkupServiceProvider = p.tblWingCustomerFlightAPIServiceProvider.Select(p => p.ServiceProvider).ToList(),
                 MarkupCustomerType = p.tblWingCustomerFlightAPICustomerType.Select(p => p.customerType).ToList(),
                 MarkupCustomerDetail = p.tblWingCustomerFlightAPICustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                 MarkupCabinClass = p.tblWingCustomerFlightAPIFlightClass.Select(p => p.CabinClass).ToList(),
                 MarkupAirline = p.tblWingCustomerFlightAPIAirline.Select(p => p.AirlineId.Value).ToList()
             }).ToList();
            }
            else
            {
                _mdl = _context.tblWingCustomerFlightAPI.Where(p => !p.IsDeleted).Select(p => new mdlWingMarkup
                {
                    Id = p.Id,
                    IsAllProvider = p.IsAllProvider,
                    IsAllCustomerType = p.IsAllCustomerType,
                    IsAllCustomer = p.IsAllCustomer,
                    IsAllPessengerType = p.IsAllPessengerType,
                    IsAllFlightClass = p.IsAllFlightClass,
                    IsAllAirline = p.IsAllAirline,
                    EffectiveFromDt = p.EffectiveFromDt,
                    EffectiveToDt = p.EffectiveToDt,
                    ModifiedBy = p.ModifiedBy,
                    ModifiedDt = p.ModifiedDt,
                    remarks = p.Remarks,
                    MarkupServiceProvider = p.tblWingCustomerFlightAPIServiceProvider.Select(p => p.ServiceProvider).ToList(),
                    MarkupCustomerType = p.tblWingCustomerFlightAPICustomerType.Select(p => p.customerType).ToList(),
                    MarkupCustomerDetail = p.tblWingCustomerFlightAPICustomerDetails.Select(p => p.CustomerId.Value).ToList(),
                    MarkupCabinClass = p.tblWingCustomerFlightAPIFlightClass.Select(p => p.CabinClass).ToList(),
                    MarkupAirline = p.tblWingCustomerFlightAPIAirline.Select(p => p.AirlineId.Value).ToList()
                }).ToList();
            }

            if (_mdl == null)
            {
                _mdl = new List<mdlWingMarkup>();
            }
            //var MarkupAirlineIds = _mdl.SelectMany(p => p.MarkupAirline).ToList();
            //var AirlineDetails = _context.tblAirline.Where(p => MarkupAirlineIds.Contains(p.Id)).ToList();
            //foreach (var md in _mdl)
            //{
            //    if (md.MarkupAirline == null)
            //    {
            //        md.MarkupAirline = new List<int>();
            //        md.MarkupAirlineCode = new List<string>();
            //        continue;
            //    }
            //    md.MarkupAirlineCode = (from t1 in md.MarkupAirline
            //                            join t2 in AirlineDetails on t1 equals t2.Id
            //                            select t2.Code).ToList();
            //}
            return _mdl;
        }

        public void LoadMarkupAirlineCode(List<mdlWingMarkup> _mdl)
        {
            var MarkupAirlineIds = _mdl.SelectMany(p => p.MarkupAirline).ToList();
            var AirlineDetails = _context.tblAirline.Where(p => MarkupAirlineIds.Contains(p.Id)).ToList();
            foreach (var md in _mdl)
            {
                if (md.MarkupAirline == null || md.MarkupAirline.Count == 0)
                {
                    md.MarkupAirline = new List<int>();
                    md.MarkupAirlineCode = new List<string>();
                    continue;
                }
                md.MarkupAirlineCode = (from t1 in md.MarkupAirline
                                        join t2 in AirlineDetails on t1 equals t2.Id
                                        select t2.Code).ToList();
            }
        }

        public void LoadMarkupCustomerCode(List<mdlWingMarkup> _mdl)
        {
            var MarkupCustomerIds = _mdl.SelectMany(p => p.MarkupCustomerDetail).ToList();
            var CustomerDetails = _context.tblCustomerMaster.Where(p => MarkupCustomerIds.Contains(p.Id)).ToList();
            foreach (var md in _mdl)
            {
                if (md.MarkupCustomerDetail == null || md.MarkupCustomerDetail.Count == 0)
                {
                    md.MarkupCustomerDetail = new List<int>();
                    md.MarkupCustomerCode = new List<string>();
                    continue;
                }
                md.MarkupCustomerCode = (from t1 in md.MarkupCustomerDetail
                                         join t2 in CustomerDetails on t1 equals t2.Id
                                         select t2.Code).ToList();
            }
        }

        private bool AirlineApplicability(mdlWingMarkup mdlA, mdlSearchResult mdlS)
        {
            if (mdlA.IsAllAirline)
            {
                return true;
            }
            if ((
                             from t1 in mdlS.Segment
                             join t2 in mdlA.MarkupAirlineCode on t1.Airline.Code equals t2
                             select t2
                              ).Any())
            {
                return true;
            }
            return false;
        }

        private bool ServiceProviderApplicability(mdlWingMarkup mdlA, mdlSearchResult mdlS)
        {
            if (mdlA.IsAllProvider)
            {
                return true;
            }

            var sp = (enmServiceProvider)Convert.ToInt32(mdlS.TotalPriceList.FirstOrDefault().ResultIndex?.Split("_").FirstOrDefault() ?? _DefaultAirServiceProvider);
            if (sp == enmServiceProvider.None)
            {
                if (mdlA.MarkupServiceProvider.Any(q => q == sp))
                {
                    return true;
                }
            }

            return false;
        }

        private bool FlightCabinClass(mdlWingMarkup mdlA, mdlTotalpricelist mdlP)
        {
            if (mdlA.IsAllFlightClass)
            {
                return true;
            }
            if (mdlA.MarkupCabinClass?.Contains(mdlP.ADULT.CabinClass) ?? false)
            {
                return true;
            }
            return false;

        }

        private bool FlightDirectMarkup(mdlWingMarkup mdlA, mdlSearchResult mdlS)
        {
            if (mdlA.DirectFlight.HasFlag(enmDirectFlight.DirectFlight) && (mdlS.Segment?.Count() ?? 0) == 1)
            {
                return true;
            }
            if (mdlA.DirectFlight.HasFlag(enmDirectFlight.ConnectFlight) && (mdlS.Segment?.Count() ?? 0) > 1)
            {
                return true;
            }
            return false;
        }

        public void WingMarkupAmount(List<List<mdlSearchResult>> mdl, int AdultCount = 1, int ChildCount = 0, int InfantCount = 0)
        {
            if (mdl == null)
            {
                return;
            }
            List<mdlWingMarkup> allMarkup = LoadMarkup(0, false, true);
            LoadMarkupAirlineCode(allMarkup);
            for (int i = 0; i < mdl.Count; i++)
            {
                for (int j = 0; j < mdl[i].Count; j++)
                {
                    for (int k = 0; k < allMarkup.Count; k++)
                    {

                        if (AirlineApplicability(allMarkup[k], mdl[i][j]) &&
                            ServiceProviderApplicability(allMarkup[k], mdl[i][j]) &&
                            FlightDirectMarkup(allMarkup[k], mdl[i][j])
                            )
                        {

                            if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnTicket))
                            {
                                for (int j1 = 0; j1 < mdl[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl[i][j].TotalPriceList[j1]))
                                    {
                                        mdl[i][j].TotalPriceList[j1].WingMarkup =
                                            mdl[i][j].TotalPriceList[j1].WingMarkup + allMarkup[k].Amount;
                                    }
                                }
                            }
                            else if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnPassenger))
                            {
                                for (int j1 = 0; j1 < mdl[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl[i][j].TotalPriceList[j1]))
                                    {
                                        //Adult Markup
                                        if (AdultCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Adult) || allMarkup[k].IsAllPessengerType))
                                        {
                                            mdl[i][j].TotalPriceList[j1].ADULT.FareComponent.WingMarkup =
                                            mdl[i][j].TotalPriceList[j1].ADULT?.FareComponent?.WingMarkup ?? 0 + (allMarkup[k].Amount);
                                        }
                                        if (ChildCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Child) || allMarkup[k].IsAllPessengerType))
                                        {
                                            mdl[i][j].TotalPriceList[j1].CHILD.FareComponent.WingMarkup =
                                            mdl[i][j].TotalPriceList[j1].CHILD?.FareComponent?.WingMarkup ?? 0 + (allMarkup[k].Amount);
                                        }
                                        if (InfantCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Infant) || allMarkup[k].IsAllPessengerType))
                                        {
                                            mdl[i][j].TotalPriceList[j1].INFANT.FareComponent.WingMarkup =
                                            mdl[i][j].TotalPriceList[j1].INFANT?.FareComponent?.WingMarkup ?? 0 + (allMarkup[k].Amount);
                                        }
                                    }
                                }
                            }

                        }




                    }

                }
            }
        }

        public void WingDiscountAmount(List<List<mdlSearchResult>> mdl, int AdultCount = 1, int ChildCount = 0, int InfantCount = 0)
        {
            if (mdl == null)
            {
                return;
            }
            for (int i = 0; i < mdl.Count; i++)
            {
                this._BookingDate = mdl[i].FirstOrDefault()?.Segment.FirstOrDefault()?.DepartureTime ?? DateTime.Now;
                List<mdlWingMarkup> allMarkup = LoadDiscount(0, false, true);
                LoadMarkupAirlineCode(allMarkup);
                for (int j = 0; j < mdl[i].Count; j++)
                {
                    for (int k = 0; k < allMarkup.Count; k++)
                    {

                        if (AirlineApplicability(allMarkup[k], mdl[i][j]) &&
                            ServiceProviderApplicability(allMarkup[k], mdl[i][j]) &&
                            FlightDirectMarkup(allMarkup[k], mdl[i][j])
                            )
                        {

                            if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnTicket))
                            {
                                for (int j1 = 0; j1 < mdl[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl[i][j].TotalPriceList[j1]))
                                    {
                                        mdl[i][j].TotalPriceList[j1].Discount =
                                            mdl[i][j].TotalPriceList[j1].Discount + allMarkup[k].Amount;
                                    }
                                }
                            }
                            else if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnPassenger))
                            {
                                for (int j1 = 0; j1 < mdl[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl[i][j].TotalPriceList[j1]))
                                    {
                                        //Adult Markup
                                        if (AdultCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Adult) || allMarkup[k].IsAllPessengerType))
                                        {
                                            mdl[i][j].TotalPriceList[j1].ADULT.FareComponent.WingDiscount =
                                            mdl[i][j].TotalPriceList[j1].ADULT?.FareComponent?.WingDiscount ?? 0 + (allMarkup[k].Amount);
                                        }
                                        if (ChildCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Child) || allMarkup[k].IsAllPessengerType))
                                        {
                                            mdl[i][j].TotalPriceList[j1].CHILD.FareComponent.WingDiscount =
                                            mdl[i][j].TotalPriceList[j1].CHILD?.FareComponent?.WingDiscount ?? 0 + (allMarkup[k].Amount);
                                        }
                                        if (InfantCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Infant) || allMarkup[k].IsAllPessengerType))
                                        {
                                            mdl[i][j].TotalPriceList[j1].INFANT.FareComponent.WingDiscount =
                                            mdl[i][j].TotalPriceList[j1].INFANT?.FareComponent?.WingDiscount ?? 0 + (allMarkup[k].Amount);
                                        }
                                    }
                                }
                            }

                        }




                    }

                }
            }
        }


        public void CustomerMarkup(List<List<mdlSearchResult>> mdl,int custid)
        {
            if (mdl == null)
            {
                return;
            }
            double MarkupAmount = 0;
            MarkupAmount = _context.tblCustomerMarkup.Where(p => !p.IsDeleted && p.CustomerId==custid).Sum(p => p.MarkupAmt);
            for (int i = 0; i < ((mdl?.Count) ?? 0); i++)
            {
                for (int j = 0; j < (mdl[i]?.Count??0); j++)
                {
                    for (int j1 = 0; j1 < mdl[i][j].TotalPriceList.Count; j1++)
                    {
                        mdl[i][j].TotalPriceList[j1].CustomerMarkup = MarkupAmount;
                    }
                }
            }
        }

        public bool PassengerTypeConvenience(mdlWingMarkup mdlA, int AdultCount, int ChildCount, int InfantCount)
        {
            bool returndata = false;
            if (mdlA.IsAllCustomerType)
            {
                return true;
            }
            if (AdultCount > 0)
            {
                returndata = mdlA.MarkupPassengerType.Any(p => p == enmPassengerType.Adult);
                if (returndata)
                {
                    return true;
                }
            }
            if (ChildCount > 0)
            {
                returndata = mdlA.MarkupPassengerType.Any(p => p == enmPassengerType.Child);
                if (returndata)
                {
                    return true;
                }
            }
            if (InfantCount > 0)
            {
                returndata = mdlA.MarkupPassengerType.Any(p => p == enmPassengerType.Infant);
                if (returndata)
                {
                    return true;
                }
            }

            return returndata;
        }

        public bool PassengerTypeDiscount(mdlWingMarkup mdlA, int AdultCount, int ChildCount, int InfantCount)
        {
            bool returndata = false;
            if (mdlA.IsAllCustomerType)
            {
                return true;
            }
            if (AdultCount > 0)
            {
                returndata = mdlA.MarkupPassengerType.Any(p => p == enmPassengerType.Adult);
                if (returndata)
                {
                    return true;
                }
            }
            if (ChildCount > 0)
            {
                returndata = mdlA.MarkupPassengerType.Any(p => p == enmPassengerType.Child);
                if (returndata)
                {
                    return true;
                }
            }
            if (InfantCount > 0)
            {
                returndata = mdlA.MarkupPassengerType.Any(p => p == enmPassengerType.Infant);
                if (returndata)
                {
                    return true;
                }
            }

            return returndata;
        }

        public void WingConvenienceAmount(mdlFareQuotResponse mdl, List<mdlTravellerinfo> travellerInfo)
        {
            if (mdl == null)
            {
                return;
            }
            int AdultMaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Adult && (p.Title.Trim().ToLower() == "mr" || p.Title.Trim().ToLower() == "master")).Count() ?? 0;
            int AdultFemaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Adult && (p.Title.Trim().ToLower() == "ms" || p.Title.Trim().ToLower() == "mrs")).Count() ?? 0;
            int ChildMaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "mr" || p.Title.Trim().ToLower() == "master")).Count() ?? 0;
            int ChildFemaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "ms" || p.Title.Trim().ToLower() == "mrs")).Count() ?? 0;
            int InfantMaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "mr" || p.Title.Trim().ToLower() == "master")).Count() ?? 0;
            int InfantFemaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "ms" || p.Title.Trim().ToLower() == "mrs")).Count() ?? 0;

            List<mdlWingMarkup> allMarkup = LoadConvenience(0, false, true);
            LoadMarkupAirlineCode(allMarkup);
            for (int i = 0; i < mdl.Results.Count; i++)
            {
                for (int j = 0; j < mdl.Results[i].Count; j++)
                {
                    for (int k = 0; k < allMarkup.Count; k++)
                    {

                        if (AirlineApplicability(allMarkup[k], mdl.Results[i][j]) &&
                            ServiceProviderApplicability(allMarkup[k], mdl.Results[i][j]) &&
                            FlightDirectMarkup(allMarkup[k], mdl.Results[i][j]) &&
                            PassengerTypeConvenience(allMarkup[k], AdultMaleCount + AdultFemaleCount, ChildMaleCount + ChildFemaleCount, InfantMaleCount + InfantFemaleCount)
                            )
                        {

                            if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnTicket))
                            {
                                for (int j1 = 0; j1 < mdl.Results[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl.Results[i][j].TotalPriceList[j1]))
                                    {

                                        if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                        {
                                            mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                            mdl.Results[i][j].TotalPriceList[j1].Convenience + allMarkup[k].Amount;
                                        }
                                        else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && AdultMaleCount > 0)
                                        {
                                            mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                            mdl.Results[i][j].TotalPriceList[j1].Convenience + allMarkup[k].Amount;
                                        }
                                        else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && AdultFemaleCount > 0)
                                        {
                                            mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                            mdl.Results[i][j].TotalPriceList[j1].Convenience + allMarkup[k].Amount;
                                        }

                                    }

                                }
                            }
                            else if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnPassenger))
                            {
                                for (int j1 = 0; j1 < mdl.Results[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl.Results[i][j].TotalPriceList[j1]))
                                    {
                                        //Adult Convenience
                                        if (mdl.SearchQuery.AdultCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Adult) || (allMarkup[k].IsAllPessengerType)))
                                        {
                                            if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                            mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (AdultMaleCount + AdultFemaleCount));
                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && AdultMaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (AdultMaleCount));
                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && AdultFemaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (AdultFemaleCount));
                                            }


                                        }
                                        if (mdl.SearchQuery.ChildCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Child) || allMarkup[k].IsAllPessengerType))
                                        {
                                            if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                               mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (ChildMaleCount + ChildFemaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && ChildMaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                               mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (ChildMaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && ChildFemaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                               mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (ChildFemaleCount));
                                            }
                                        }
                                        if (mdl.SearchQuery.InfantCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Infant) || allMarkup[k].IsAllPessengerType))
                                        {
                                            if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                              mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (InfantMaleCount + InfantFemaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && InfantMaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (InfantMaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && InfantFemaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience =
                                                mdl.Results[i][j].TotalPriceList[j1].Convenience + (allMarkup[k].Amount * (InfantFemaleCount));
                                            }
                                        }
                                    }
                                }
                            }

                        }




                    }

                }
            }
        }

        public void WingDiscountAmount(mdlFareQuotResponse mdl, List<mdlTravellerinfo> travellerInfo)
        {
            if (mdl == null)
            {
                return;
            }
            int AdultMaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Adult && (p.Title.Trim().ToLower() == "mr" || p.Title.Trim().ToLower() == "master")).Count() ?? 0;
            int AdultFemaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Adult && (p.Title.Trim().ToLower() == "ms" || p.Title.Trim().ToLower() == "mrs")).Count() ?? 0;
            int ChildMaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "mr" || p.Title.Trim().ToLower() == "master")).Count() ?? 0;
            int ChildFemaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "ms" || p.Title.Trim().ToLower() == "mrs")).Count() ?? 0;
            int InfantMaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "mr" || p.Title.Trim().ToLower() == "master")).Count() ?? 0;
            int InfantFemaleCount = travellerInfo?.Where(p => p.passengerType == enmPassengerType.Child && (p.Title.Trim().ToLower() == "ms" || p.Title.Trim().ToLower() == "mrs")).Count() ?? 0;

            for (int i = 0; i < mdl.Results.Count; i++)
            {
                this._BookingDate = mdl.Results[i].FirstOrDefault()?.Segment.FirstOrDefault()?.DepartureTime ?? DateTime.Now;
                List<mdlWingMarkup> allMarkup = LoadDiscount(0, false, true);
                LoadMarkupAirlineCode(allMarkup);

                for (int j = 0; j < mdl.Results[i].Count; j++)
                {
                    for (int k = 0; k < allMarkup.Count; k++)
                    {

                        if (AirlineApplicability(allMarkup[k], mdl.Results[i][j]) &&
                            ServiceProviderApplicability(allMarkup[k], mdl.Results[i][j]) &&
                            FlightDirectMarkup(allMarkup[k], mdl.Results[i][j]) &&
                            PassengerTypeDiscount(allMarkup[k], AdultMaleCount + AdultFemaleCount, ChildMaleCount + ChildFemaleCount, InfantMaleCount + InfantFemaleCount)
                            )
                        {

                            if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnTicket))
                            {
                                for (int j1 = 0; j1 < mdl.Results[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl.Results[i][j].TotalPriceList[j1]))
                                    {

                                        if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                        {
                                            mdl.Results[i][j].TotalPriceList[j1].Discount =
                                            mdl.Results[i][j].TotalPriceList[j1].Discount + allMarkup[k].Amount;
                                        }
                                        else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && AdultMaleCount > 0)
                                        {
                                            mdl.Results[i][j].TotalPriceList[j1].Discount =
                                            mdl.Results[i][j].TotalPriceList[j1].Discount + allMarkup[k].Amount;
                                        }
                                        else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && AdultFemaleCount > 0)
                                        {
                                            mdl.Results[i][j].TotalPriceList[j1].Discount =
                                            mdl.Results[i][j].TotalPriceList[j1].Discount + allMarkup[k].Amount;
                                        }

                                    }

                                }
                            }
                            else if (allMarkup[k].Applicability.HasFlag(enmMarkupApplicability.OnPassenger))
                            {
                                for (int j1 = 0; j1 < mdl.Results[i][j].TotalPriceList.Count; j1++)
                                {
                                    if (FlightCabinClass(allMarkup[k], mdl.Results[i][j].TotalPriceList[j1]))
                                    {
                                        //Adult Discount
                                        if (mdl.SearchQuery.AdultCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Adult) || (allMarkup[k].IsAllPessengerType)))
                                        {
                                            if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                            mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (AdultMaleCount + AdultFemaleCount));
                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && AdultMaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                                mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (AdultMaleCount));
                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && AdultFemaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                                mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (AdultFemaleCount));
                                            }


                                        }
                                        if (mdl.SearchQuery.ChildCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Child) || allMarkup[k].IsAllPessengerType))
                                        {
                                            if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                               mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (ChildMaleCount + ChildFemaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && ChildMaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                               mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (ChildMaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && ChildFemaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                               mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (ChildFemaleCount));
                                            }
                                        }
                                        if (mdl.SearchQuery.InfantCount > 0 && (allMarkup[k].MarkupPassengerType.Any(p => p == enmPassengerType.Infant) || allMarkup[k].IsAllPessengerType))
                                        {
                                            if (allMarkup[k].Gender.HasFlag(enmGender.Male) && allMarkup[k].Gender.HasFlag(enmGender.Female) && allMarkup[k].Gender.HasFlag(enmGender.Other))
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                              mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (InfantMaleCount + InfantFemaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Male) && InfantMaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                                mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (InfantMaleCount));

                                            }
                                            else if (allMarkup[k].Gender.HasFlag(enmGender.Female) && InfantFemaleCount > 0)
                                            {
                                                mdl.Results[i][j].TotalPriceList[j1].Discount =
                                                mdl.Results[i][j].TotalPriceList[j1].Discount + (allMarkup[k].Amount * (InfantFemaleCount));
                                            }
                                        }
                                    }
                                }
                            }

                        }




                    }

                }
            }
        }

        public bool AddMarkup(mdlWingMarkup mdl, int UserId)
        {
            tblWingMarkupMaster data = new tblWingMarkupMaster();

            data.Applicability = mdl.Applicability;
            data.DirectFlight = mdl.DirectFlight;
            data.IsAllProvider = mdl.IsAllProvider;
            data.IsAllCustomerType = mdl.IsAllCustomerType;
            data.IsAllCustomer = mdl.IsAllCustomer;
            data.IsAllPessengerType = mdl.IsAllPessengerType;
            data.IsAllFlightClass = mdl.IsAllFlightClass;
            data.IsAllAirline = mdl.IsAllAirline;
            data.Gender = mdl.Gender;
            data.Amount = mdl.Amount;
            data.EffectiveFromDt = mdl.EffectiveFromDt;
            data.EffectiveToDt = mdl.EffectiveToDt;
            data.CreatedBy = UserId;
            data.CreatedDt = DateTime.Now;
            data.IsDeleted = false;
            data.ModifiedBy = UserId;
            data.ModifiedDt = DateTime.Now;
            data.Remarks = mdl.remarks;
            data.tblWingMarkupServiceProvider = mdl.MarkupServiceProvider?.Select(p => new tblWingMarkupServiceProvider
            {
                ServiceProvider = p
            }).ToList();
            data.tblWingMarkupCustomerType = mdl.MarkupCustomerType?.Select(p => new tblWingMarkupCustomerType
            {
                customerType = p
            }).ToList();
            data.tblWingMarkupCustomerDetails = mdl.MarkupCustomerDetail?.Select(p => new tblWingMarkupCustomerDetails
            {
                CustomerId = p
            }).ToList();
            data.tblWingMarkupPassengerType = mdl.MarkupPassengerType?.Select(p => new tblWingMarkupPassengerType
            {
                PassengerType = p
            }).ToList();
            data.tblWingMarkupFlightClass = mdl.MarkupCabinClass?.Select(p => new tblWingMarkupFlightClass
            {
                CabinClass = p
            }).ToList();
            data.tblWingMarkupAirline = mdl.MarkupAirline?.Select(p => new tblWingMarkupAirline
            {
                AirlineId = p
            }).ToList();

            _context.tblWingMarkupMaster.Add(data);
            _context.SaveChanges();

            return true;

        }

        public bool RemoveMarkup(int Id, int UserId)
        {
            var _markupuMaster = _context.tblWingMarkupMaster.Where(p => p.Id == Id).FirstOrDefault();
            _markupuMaster.IsDeleted = true;
            _markupuMaster.ModifiedBy = UserId;
            _markupuMaster.ModifiedDt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }

        public bool AddConvenience(mdlWingMarkup mdl, int UserId)
        {
            tblWingConvenience data = new tblWingConvenience();

            data.Applicability = mdl.Applicability;
            data.DirectFlight = mdl.DirectFlight;
            data.IsAllProvider = mdl.IsAllProvider;
            data.IsAllCustomerType = mdl.IsAllCustomerType;
            data.IsAllCustomer = mdl.IsAllCustomer;
            data.IsAllPessengerType = mdl.IsAllPessengerType;
            data.IsAllFlightClass = mdl.IsAllFlightClass;
            data.IsAllAirline = mdl.IsAllAirline;
            data.Gender = mdl.Gender;
            data.Amount = mdl.Amount;
            data.EffectiveFromDt = mdl.EffectiveFromDt;
            data.EffectiveToDt = mdl.EffectiveToDt;
            data.CreatedBy = UserId;
            data.CreatedDt = DateTime.Now;
            data.IsDeleted = false;
            data.ModifiedBy = UserId;
            data.ModifiedDt = DateTime.Now;
            data.Remarks = mdl.remarks;
            data.tblWingConvenienceServiceProvider = mdl.MarkupServiceProvider?.Select(p => new tblWingConvenienceServiceProvider
            {
                ServiceProvider = p
            }).ToList();
            data.tblWingConvenienceCustomerType = mdl.MarkupCustomerType?.Select(p => new tblWingConvenienceCustomerType
            {
                customerType = p
            }).ToList();
            data.tblWingConvenienceCustomerDetails = mdl.MarkupCustomerDetail?.Select(p => new tblWingConvenienceCustomerDetails
            {
                CustomerId = p
            }).ToList();
            data.tblWingConveniencePassengerType = mdl.MarkupPassengerType?.Select(p => new tblWingConveniencePassengerType
            {
                PassengerType = p
            }).ToList();
            data.tblWingConvenienceFlightClass = mdl.MarkupCabinClass?.Select(p => new tblWingConvenienceFlightClass
            {
                CabinClass = p
            }).ToList();
            data.tblWingConvenienceAirline = mdl.MarkupAirline?.Select(p => new tblWingConvenienceAirline
            {
                AirlineId = p
            }).ToList();

            _context.tblWingConvenience.Add(data);
            _context.SaveChanges();

            return true;

        }

        public bool AddCustomerFlightAPI(mdlWingMarkup mdl, int UserId)
        {
            tblWingCustomerFlightAPI data = new tblWingCustomerFlightAPI();

            data.IsAllProvider = mdl.IsAllProvider;
            data.IsAllCustomerType = mdl.IsAllCustomerType;
            data.IsAllCustomer = mdl.IsAllCustomer;
            data.IsAllFlightClass = mdl.IsAllFlightClass;
            data.IsAllAirline = mdl.IsAllAirline;
            data.EffectiveFromDt = mdl.EffectiveFromDt;
            data.EffectiveToDt = mdl.EffectiveToDt;
            data.CreatedBy = UserId;
            data.CreatedDt = DateTime.Now;
            data.IsDeleted = false;
            data.ModifiedBy = UserId;
            data.ModifiedDt = DateTime.Now;
            data.Remarks = mdl.remarks;
            data.tblWingCustomerFlightAPIServiceProvider = mdl.MarkupServiceProvider?.Select(p => new tblWingCustomerFlightAPIServiceProvider
            {
                ServiceProvider = p
            }).ToList();
            data.tblWingCustomerFlightAPICustomerType = mdl.MarkupCustomerType?.Select(p => new tblWingCustomerFlightAPICustomerType
            {
                customerType = p
            }).ToList();
            data.tblWingCustomerFlightAPICustomerDetails = mdl.MarkupCustomerDetail?.Select(p => new tblWingCustomerFlightAPICustomerDetails
            {
                CustomerId = p
            }).ToList();

            data.tblWingCustomerFlightAPIFlightClass = mdl.MarkupCabinClass?.Select(p => new tblWingCustomerFlightAPIFlightClass
            {
                CabinClass = p
            }).ToList();
            data.tblWingCustomerFlightAPIAirline = mdl.MarkupAirline?.Select(p => new tblWingCustomerFlightAPIAirline
            {
                AirlineId = p
            }).ToList();

            _context.tblWingCustomerFlightAPI.Add(data);
            _context.SaveChanges();

            return true;

        }

        public bool RemoveConvenience(int Id, int UserId)
        {
            var _markupuMaster = _context.tblWingConvenience.Where(p => p.Id == Id).FirstOrDefault();
            _markupuMaster.IsDeleted = true;
            _markupuMaster.ModifiedBy = UserId;
            _markupuMaster.ModifiedDt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }


        public bool RemoveCustomerFlightAPI(int Id, int UserId)
        {
            var _markupuMaster = _context.tblWingCustomerFlightAPI.Where(p => p.Id == Id).FirstOrDefault();
            _markupuMaster.IsDeleted = true;
            _markupuMaster.ModifiedBy = UserId;
            _markupuMaster.ModifiedDt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }
        public bool AddDiscount(mdlWingMarkup mdl, int UserId)
        {
            tblWingDiscount data = new tblWingDiscount();

            data.Applicability = mdl.Applicability;
            data.DirectFlight = mdl.DirectFlight;
            data.IsAllProvider = mdl.IsAllProvider;
            data.IsAllCustomerType = mdl.IsAllCustomerType;
            data.IsAllCustomer = mdl.IsAllCustomer;
            data.IsAllPessengerType = mdl.IsAllPessengerType;
            data.IsAllFlightClass = mdl.IsAllFlightClass;
            data.IsAllAirline = mdl.IsAllAirline;
            data.Gender = mdl.Gender;
            data.Amount = mdl.Amount;
            data.DayCount = mdl.DayCount;
            data.EffectiveFromDt = mdl.EffectiveFromDt;
            data.EffectiveToDt = mdl.EffectiveToDt;
            data.CreatedBy = UserId;
            data.CreatedDt = DateTime.Now;
            data.IsDeleted = false;
            data.ModifiedBy = UserId;
            data.ModifiedDt = DateTime.Now;
            data.Remarks = mdl.remarks;
            data.tblWingDiscountServiceProvider = mdl.MarkupServiceProvider?.Select(p => new tblWingDiscountServiceProvider
            {
                ServiceProvider = p
            }).ToList();
            data.tblWingDiscountCustomerType = mdl.MarkupCustomerType?.Select(p => new tblWingDiscountCustomerType
            {
                customerType = p
            }).ToList();
            data.tblWingDiscountCustomerDetails = mdl.MarkupCustomerDetail?.Select(p => new tblWingDiscountCustomerDetails
            {
                CustomerId = p
            }).ToList();
            data.tblWingDiscountPassengerType = mdl.MarkupPassengerType?.Select(p => new tblWingDiscountPassengerType
            {
                PassengerType = p
            }).ToList();
            data.tblWingDiscountFlightClass = mdl.MarkupCabinClass?.Select(p => new tblWingDiscountFlightClass
            {
                CabinClass = p
            }).ToList();
            data.tblWingDiscountAirline = mdl.MarkupAirline?.Select(p => new tblWingDiscountAirline
            {
                AirlineId = p
            }).ToList();

            _context.tblWingDiscount.Add(data);
            _context.SaveChanges();

            return true;

        }

        public bool RemoveDiscount(int Id, int UserId)
        {
            var _markupuMaster = _context.tblWingDiscount.Where(p => p.Id == Id).FirstOrDefault();
            _markupuMaster.IsDeleted = true;
            _markupuMaster.ModifiedBy = UserId;
            _markupuMaster.ModifiedDt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }


        public bool CalculateTotalPriceAfterMarkup(List<List<mdlSearchResult>> mdls, int AdultCount, int ChildCount, int InfantCount,string mode)
        {
            if (mdls == null)
            {
                return false;
            }

            double WingbaseFare = 0;
            //update with markup
            foreach (var mdl in mdls)
            {
                foreach (var md in mdl)
                {
                    foreach (var tp in md.TotalPriceList)
                    {
                        //We need to Show Only Adult Fare
                        WingbaseFare = 0;
                        if (mode == "search")
                        {
                            if (tp?.ADULT?.FareComponent?.TotalFare != null && AdultCount>0)
                            {
                                tp.ADULT.FareComponent.NewTotalFare = (tp?.ADULT?.FareComponent?.TotalFare ?? 0) + (tp?.ADULT?.FareComponent?.WingMarkup ?? 0);
                                WingbaseFare = WingbaseFare + (tp.ADULT.FareComponent.NewTotalFare);
                                tp.Discount = tp.Discount + (tp.ADULT?.FareComponent?.WingDiscount ?? 0);

                            }
                            else if (tp?.CHILD?.FareComponent?.TotalFare != null && ChildCount>0)
                            {
                                tp.CHILD.FareComponent.NewTotalFare = (tp?.CHILD?.FareComponent?.TotalFare ?? 0) + (tp?.CHILD?.FareComponent?.WingMarkup ?? 0);
                                WingbaseFare = WingbaseFare + (tp.CHILD.FareComponent.NewTotalFare);
                                tp.Discount = tp.Discount + (tp.CHILD?.FareComponent?.WingDiscount ?? 0);
                            }
                            else if (tp?.INFANT?.FareComponent?.TotalFare != null && InfantCount>0)
                            {
                                tp.INFANT.FareComponent.NewTotalFare = (tp?.INFANT?.FareComponent?.TotalFare ?? 0) + (tp?.INFANT?.FareComponent?.WingMarkup ?? 0);
                                WingbaseFare = WingbaseFare + (tp.INFANT.FareComponent.NewTotalFare);
                                tp.Discount = tp.Discount + (tp.INFANT?.FareComponent?.WingDiscount ?? 0);
                            }
                        }
                        else
                        {
                            if (tp?.ADULT?.FareComponent?.TotalFare != null && AdultCount>0)
                            {
                                tp.ADULT.FareComponent.NewTotalFare = ((tp?.ADULT?.FareComponent?.TotalFare ?? 0) + (tp?.ADULT?.FareComponent?.WingMarkup ?? 0))* AdultCount;
                                WingbaseFare = WingbaseFare + ((tp.ADULT.FareComponent.NewTotalFare));
                                tp.Discount = tp.Discount + ((tp.ADULT?.FareComponent?.WingDiscount ?? 0) * AdultCount);

                            }
                            if (tp?.CHILD?.FareComponent?.TotalFare != null && ChildCount > 0)
                            {
                                tp.CHILD.FareComponent.NewTotalFare = ((tp?.CHILD?.FareComponent?.TotalFare ?? 0)+ (tp?.CHILD?.FareComponent?.WingMarkup ?? 0)) * ChildCount ;
                                WingbaseFare = WingbaseFare + ((tp.CHILD.FareComponent.NewTotalFare));
                                tp.Discount = tp.Discount + ((tp.CHILD?.FareComponent?.WingDiscount ?? 0) *ChildCount);
                            }
                            if (tp?.INFANT?.FareComponent?.TotalFare != null && InfantCount > 0)
                            {
                                tp.INFANT.FareComponent.NewTotalFare = ((tp?.INFANT?.FareComponent?.TotalFare ?? 0) + (tp?.INFANT?.FareComponent?.WingMarkup ?? 0)) * InfantCount;
                                WingbaseFare = WingbaseFare + ((tp.INFANT.FareComponent.NewTotalFare));
                                tp.Discount = tp.Discount + ((tp.INFANT?.FareComponent?.WingDiscount ?? 0) * InfantCount);
                            }
                        }
                        tp.BaseFare = WingbaseFare;
                        tp.TotalPrice = tp.BaseFare + tp.WingMarkup + tp.CustomerMarkup;
                        tp.NetPrice = tp.TotalPrice + tp.Convenience -tp.Discount;
                    }
                }
            }
            return true;
        }
    }
}
