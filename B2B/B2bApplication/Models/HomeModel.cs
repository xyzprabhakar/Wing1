﻿using B2BClasses;
using B2BClasses.Database;
using B2BClasses.Services.Air;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2BClasses.Services.Enums;
using System.ComponentModel.DataAnnotations;
using B2BClasses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace B2bApplication.Models
{

    public class mdlPageSearch
    {
        public string Pagename{get;set;}
        public List<Document> alldocument { get; set; }
    }

    public class mdlFlightReview
    {
        [DataType(DataType.Password)]
        public string Mpin { get; set; }
        public mdlFareQuotRequest FareQuoteRequest { get; set; }
        public List<mdlFareQuotResponse> FareQuotResponse { get; set; }
        public List<mdlFareRuleResponse> FareRule{ get; set; }
        //public List<string> TraceId { get; set; }
        //public List<string> BookingId { get; set; }
        public List<mdlTravellerinfo> travellerInfo { get; set; }
        
        public mdlGstInfo gstInfo { get; set; }        
        public mdlFareQuoteCondition FareQuoteCondition{ get; set; }

        public string bookingid { get; set; }
        [DataType( DataType.EmailAddress) ]
        public string emails { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string contacts { get; set; }

        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }

        public double AdultBaseFare { get; set; }
        public double ChildBaseFare { get; set; }
        public double InfantBaseFare { get; set; }

        public double AdultTotalBaseFare { get; set; }
        public double ChildTotalBaseFare { get; set; }
        public double InfantTotalBaseFare { get; set; }

        public double TotalBaseFare { get; set; }
        public double FeeSurcharge { get; set; }
        public double TotalFare { get; set; }
        public double OtherCharge { get; set; }        
        public double NetFare { get; set; }
        public double Convenience { get; set; }
        public double TotalOtherCharge { get; set; }
        public double InsuranceCharge { get; set; }
        public double CouponAmount { get; set; }


        public void SetFareAmount()
        {
            AdultTotalBaseFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT?.FareComponent?.BaseFare ?? 0).Sum();
            ChildTotalBaseFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD?.FareComponent?.BaseFare ?? 0).Sum();
            InfantTotalBaseFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT?.FareComponent?.BaseFare ?? 0).Sum();
            if (AdultCount > 0)
            {
                AdultBaseFare = AdultTotalBaseFare ;
                AdultTotalBaseFare = AdultBaseFare * AdultCount;
            }
            if (ChildCount > 0)
            {
                ChildBaseFare = ChildTotalBaseFare ;
                ChildTotalBaseFare = ChildBaseFare * ChildCount;
            }
            if (InfantCount > 0)
            {
                InfantBaseFare = InfantTotalBaseFare ;
                InfantTotalBaseFare = InfantBaseFare * InfantCount;
            }
            TotalBaseFare = AdultTotalBaseFare+ ChildTotalBaseFare+ InfantTotalBaseFare;
            TotalFare = FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.TotalPrice ?? 0).Sum();
            FeeSurcharge = TotalFare - TotalBaseFare;
            OtherCharge = 0;
            Convenience= FareQuotResponse.Select(p => p.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.Convenience ?? 0).Sum();
            TotalOtherCharge = OtherCharge + Convenience;
            NetFare = TotalOtherCharge + TotalFare;
        }

        public void SetFareQuoteCondtion()
        {
            if (FareQuoteCondition == null)
            {
                FareQuoteCondition = new mdlFareQuoteCondition()
                {
                    dob = new mdlDobCondition()
                    {
                        adobr = FareQuotResponse.Any(p => p.FareQuoteCondition?.dob?.adobr ?? false),
                        cdobr = FareQuotResponse.Any(p => p.FareQuoteCondition?.dob?.cdobr ?? false),
                        idobr = FareQuotResponse.Any(p => p.FareQuoteCondition?.dob?.idobr ?? false),
                    },
                    GstCondition = new mdlGstCondition()
                    {
                        IsGstMandatory = FareQuotResponse.Any(p => p.FareQuoteCondition?.GstCondition?.IsGstMandatory ?? false),
                        IsGstApplicable = FareQuotResponse.Any(p => p.FareQuoteCondition?.GstCondition?.IsGstApplicable ?? false),
                    },
                    IsHoldApplicable = FareQuotResponse.All(p => p.FareQuoteCondition?.IsHoldApplicable ?? false),
                    PassportCondition = new mdlPassportCondition()
                    {
                        IsPassportExpiryDate = FareQuotResponse.Any(p => p.FareQuoteCondition?.PassportCondition?.IsPassportExpiryDate ?? false),
                        isPassportIssueDate = FareQuotResponse.Any(p => p.FareQuoteCondition?.PassportCondition?.isPassportIssueDate ?? false),
                        isPassportRequired = FareQuotResponse.Any(p => p.FareQuoteCondition?.PassportCondition?.isPassportRequired ?? false),
                    },
                    IsLCC= FareQuotResponse.All(p => p.FareQuoteCondition?.IsLCC ?? false),
                };
            }

                
        }

        public void BookingRequestDefaultData()
        {
            if (FareQuotResponse == null || FareQuotResponse.Count == 0)
            {
                return;
            }
            
            if (travellerInfo == null)
            {
                travellerInfo = new List<mdlTravellerinfo>();

                int AdultCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.AdultCount ?? 0;
                int ChildCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.ChildCount ?? 0;
                int InfantCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.InfantCount ?? 0;
                this.AdultCount = AdultCount;
                this.ChildCount = ChildCount;
                this.InfantCount = InfantCount;

                while (AdultCount > 0)
                {
                    travellerInfo.Add(new mdlTravellerinfo() { passengerType = enmPassengerType.Adult });
                    AdultCount--;
                }
                while (ChildCount > 0)
                {
                    travellerInfo.Add(new mdlTravellerinfo() { passengerType = enmPassengerType.Child });
                    ChildCount--;
                }
                while (InfantCount > 0)
                {
                    travellerInfo.Add(new mdlTravellerinfo() { passengerType = enmPassengerType.Infant });
                    InfantCount--;
                }
            }
            else
            {
                this.AdultCount= FareQuotResponse?.FirstOrDefault()?.SearchQuery?.AdultCount ?? 0;
                this.ChildCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.ChildCount ?? 0;
                this.InfantCount = FareQuotResponse?.FirstOrDefault()?.SearchQuery?.InfantCount ?? 0;
            }

            
            SetFareQuoteCondtion();
            SetFareAmount();
        }

        public async Task LoadFareQuotationAsync(int CustomerId, IBooking _booking, IMarkup _markup, DBContext _context)
        {
            _booking.CustomerId = CustomerId;
            this.FareQuotResponse = new List<mdlFareQuotResponse>();
            this.FareRule = new List<mdlFareRuleResponse>();
            //Check Is Flight is Already Booked Coreponding to this TraceID
            if (_context.tblFlightBookingSegmentMaster.Where(p => p.TraceId == FareQuoteRequest.TraceId && p.BookingStatus != enmBookingStatus.Pending).Count() > 0)
            {
                throw new Exception("Flight Ticket is already Booked");
            }

            this.FareQuotResponse.AddRange(await _booking.FareQuoteAsync(FareQuoteRequest));
            foreach (var md in FareQuotResponse)
            {
                if (md.Results != null)
                {
                    _markup.CustomerId = CustomerId;
                    _markup.CustomerMarkup(md.Results, CustomerId);
                    _markup.WingMarkupAmount(md.Results, md.SearchQuery.AdultCount, md.SearchQuery.ChildCount, md.SearchQuery.InfantCount);
                    if (this.travellerInfo == null)
                    {
                        this.travellerInfo = new List<mdlTravellerinfo>();
                        for (int i = 0; i < md.SearchQuery.AdultCount; i++)
                        {
                            this.travellerInfo.Add(new mdlTravellerinfo()
                            {
                                Title = "MR",
                                passengerType = enmPassengerType.Adult,
                                FirstName = string.Empty,
                                LastName = string.Empty,
                                cabinclass=md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT.CabinClass.ToString(),
                                bookingclass = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT.ClassOfBooking.ToString(),
                                farebasis = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.ADULT.FareBasis.ToString()
                            });
                        }
                        for (int i = 0; i < md.SearchQuery.ChildCount; i++)
                        {
                            this.travellerInfo.Add(new mdlTravellerinfo()
                            {
                                Title = "MASTER",
                                passengerType = enmPassengerType.Child,
                                FirstName = string.Empty,
                                LastName = string.Empty,
                                cabinclass = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD.CabinClass.ToString(),
                                bookingclass = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD.ClassOfBooking.ToString(),
                                farebasis = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.CHILD.FareBasis.ToString()
                            });
                        }
                        for (int i = 0; i < md.SearchQuery.InfantCount; i++)
                        {
                            this.travellerInfo.Add(new mdlTravellerinfo()
                            {
                                Title = "MASTER",
                                passengerType = enmPassengerType.Infant,
                                FirstName = string.Empty,
                                LastName = string.Empty,
                                cabinclass = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT.CabinClass.ToString(),
                                bookingclass = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT.ClassOfBooking.ToString(),
                                farebasis = md.Results?.FirstOrDefault()?.FirstOrDefault()?.TotalPriceList?.FirstOrDefault()?.INFANT.FareBasis.ToString()
                            });
                        }

                    }
                    _markup.WingConvenienceAmount(md, this.travellerInfo);
                    _markup.WingDiscountAmount(md, this.travellerInfo);
                    _markup.CalculateTotalPriceAfterMarkup(md.Results, md.SearchQuery.AdultCount, md.SearchQuery.ChildCount, md.SearchQuery.InfantCount, "review");
                }
            }
            await _booking.CustomerFlightDetailSave(FareQuoteRequest.TraceId, FareQuotResponse);
        }

    }

    public class mdlFlighBook
    {
        public List<mdlFareQuotResponse> FareQuotResponse { get; set; }
        public List<bool> IsSucess { get; set; }
        public List<string> BookingId { get; set; }
        public List<string> PNR { get; set; }
        public List<mdlTravellerinfo> travellerInfo { get; set; }
        public mdlDeliveryinfo deliveryInfo { get; set; }
    }

    public class mdlWingMarkupWraper
    { 
        public mdlWingMarkup WingMarkup { get; set; }
        public MultiSelectList _CustomerMaster { get; set; }
        public MultiSelectList _Airline { get; set; }
        public MultiSelectList _CustomerType { get; set; }
        public MultiSelectList _ServiceProvider { get; set; }
        public MultiSelectList _PassengerType { get; set; }
        public MultiSelectList _CabinClass { get; set; }

        public class BasicData
        {
            public int Id { get; set; }
            public string  Name { get; set; }
        }

        public void SetDefaultDropDown(DBContext _context)
        {
            GetAirline(_context);
            GetCustomer(_context);
            GetCustomerType();
            GetServiceProvider();
            GetPassengerType();
            GetCabinClass();
        }

        protected void GetAirline(DBContext _context)
        {
          _Airline= new MultiSelectList( _context.tblAirline.Where(p => !p.IsDeleted).Select(p => new { AirlineId = p.Id, AirlineName = p.Name + " - " + p.Code }), "AirlineId", "AirlineName", WingMarkup?.MarkupAirline);
        }
        protected void GetCustomer(DBContext _context)
        {
            _CustomerMaster = new MultiSelectList(_context.tblCustomerMaster.Where(p => p.IsActive).Select(p => new { CustomerId = p.Id, CustomerName = p.CustomerName + " - " + p.Code }), "CustomerId", "CustomerName", WingMarkup?.MarkupCustomerDetail);
        }
        protected void GetCustomerType()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps=Enum.GetValues(typeof(enmCustomerType));
            foreach (var temp in temps)
            {
                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription()?? temp.ToString()
                });
            }
            _CustomerType = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupCustomerType);
        }
        protected void GetServiceProvider()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps = Enum.GetValues(typeof(enmServiceProvider));
            foreach (var temp in temps)
            {
                if ((enmServiceProvider)temp == enmServiceProvider.None)
                {
                    continue;
                }

                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription() ?? temp.ToString()
                });
            }
            _ServiceProvider = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupServiceProvider);
        }
        protected void GetPassengerType()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps = Enum.GetValues(typeof(enmPassengerType));
            foreach (var temp in temps)
            {
                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription() ?? temp.ToString()
                });
            }
            _PassengerType = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupPassengerType);
        }
        protected void GetCabinClass()
        {
            List<BasicData> bd = new List<BasicData>();
            var temps = Enum.GetValues(typeof(enmCabinClass));
            foreach (var temp in temps)
            {
                bd.Add(new BasicData()
                {
                    Id = (int)temp,
                    Name = ((Enum)temp).GetDescription() ?? temp.ToString()
                });
            }
            _CabinClass = new MultiSelectList(bd, "Id", "Name", WingMarkup?.MarkupCabinClass);
        }


    }


    public class mdlWingMarkupReport
    {
        public int Id{ get; set; }
        public string Applicability { get; set; }
        public string FlightType { get; set; }
        public string ServiceProvider{ get; set; }
        public string CustomerType{ get; set; }
        public string Customer { get; set; }
        public string PassengerType{ get; set; }
        public string FlightClass{ get; set; }
        public string Airline { get; set; }
        public double Amount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }

    public class mdlConvenienceReport
    {
        public int Id { get; set; }
        public string Applicability { get; set; }
        public string FlightType { get; set; }
        public string ServiceProvider { get; set; }
        public string CustomerType { get; set; }
        public string Customer { get; set; }
        public string PassengerType { get; set; }
        public string FlightClass { get; set; }
        public string Airline { get; set; }
        public enmGender gender { get; set; }
        public double Amount { get; set; }
        public int DayCount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }

    public class mdlDiscountReport
    {
        public int Id { get; set; }
        public string Applicability { get; set; }
        public string FlightType { get; set; }
        public string ServiceProvider { get; set; }
        public string CustomerType { get; set; }
        public string Customer { get; set; }
        public string PassengerType { get; set; }
        public string FlightClass { get; set; }
        public string Airline { get; set; }
        public enmGender gender { get; set; }
        public double Amount { get; set; }
        public int DayCount { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }

    public class mdlCustomerFlightAPIReport
    {
        public int Id { get; set; }
       
        public string FlightType { get; set; }
        public string ServiceProvider { get; set; }
        public string CustomerType { get; set; }
        public string Customer { get; set; }
 
        public string FlightClass { get; set; }
        public string Airline { get; set; }
     
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveToDt { get; set; }
        public DateTime ModifiedDt { get; set; }
        public string ModifiedBy { get; set; }
        public string remarks { get; set; }

    }


    public class mdlFlightCancel
    {
        public string traceId { get; set; }
        public int segementDisplayOrder { get; set; }
        public string remarks { get; set; }
        public List< mdlPassengers> passengers { get; set; }
    }
    public class mdlPassengers
    {
        public int pid { get; set; }
        public bool check { get; set; }
    }

    public class mdlProviderSettings
    {
        [Display(Name="Service Provider")]
        public enmServiceProvider ServiceProvider { get; set; }
        [Display(Name = "Is Active")]
        public bool IsEnabled { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Modified By Name")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDt { get; set; } 
    }



    #region ***** Packages ***************
    public class mdlPackageReports
    {
        [Required]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; } = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
        [Required]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; } = Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy"));
        public List<tblPackageMaster> Packagedata { get; set; }
    }

    public class mdlPackageSearch
    {
        public List<string> AllLocatioin { get; set; }
        public List<string> SelectedLocation { get; set; }
        public double MinPriceRange { get; set; }
        public double MaxPriceRange { get; set; }
        public int MinDays { get; set; }
        public int MaxDays { get; set; }
        public List<tblPackageMaster> PackageData{ get; set; }
        public int OrderBy { get; set; }//1 Order by price Asc,2 order by Price Dsc
    }


    public class mdlPackageMaster
    {
        public int PackageId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 4)]
        [RegularExpression("[a-zA-Z/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Package name")]
        public string PackageName { get; set; }
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 0)]
        [RegularExpression("[a-zA-Z/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Location")]
        public string LocationName { get; set; }
        [Display(Name = "Is Domestic")]
        public bool IsDomestic { get; set; } = true;
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 0)]
        [RegularExpression("[a-zA-Z/.\\s-]*$", ErrorMessage = "Invalid {0}, no special charcter")]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }
        [Required]
        [StringLength(4000, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 0)]
        [Display(Name = "Long Description")]
        public string LongDescription { get; set; }
        
        [Display(Name = "Thumbnail")]
        public string ThumbnailImage { get; set; }
        [Display(Name = "Image")]
        public string AllImage { get; set; }

        [DataType(DataType.Date)]
        public DateTime EffectiveFromDt { get; set; } =  DateTime.Now;
        [DataType(DataType.Date)]
        public DateTime EffectiveToDt { get; set; } = DateTime.Now.AddMonths(1);
        [Required]
        [Display(Name = "Adult Price")]
        [Range(0, 1000000)]
        public double AdultPrice { get; set; }
        
        [Required]
        [Display(Name = "No of Day")]
        [Range(0, 100)]
        public int NumberOfDay { get; set; }
        [Required]
        [Display(Name = "No of Night")]
        [Range(0, 100)]
        public int NumberOfNight { get; set; }


        
        [Required]
        [Display(Name = "Child Price")]
        [Range(0, 1000000)]
        public double ChildPrice { get; set; }
        [Required]
        [Display(Name = "Infant Price")]
        [Range(0, 1000000)]
        public double InfantPrice { get; set; }
        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;        
        [Display(Name = "Upload Package Image")]
        public List<IFormFile> UploadPackageImage { get; set; }        
        [Display(Name = "Upload Thumbnail")]
        public IFormFile UploadPackageThumbnail { get; set; }
        public List<byte[]> fileDataPackageImage { set; get; } = null;
        public byte[] fileDataThumbnail { set; get; } = null;
    }


   


    #endregion

}


