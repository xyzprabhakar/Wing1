﻿using B2BClasses.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using B2BClasses.Services.Enums;
using System.Globalization;
using B2BClasses;

namespace B2bApplication
{
    public interface ICurrentUsers
    {
        public string Name { get; }        
        public int CustomerId { get; }
        public int UserId { get; }
        public  List<int>RoleId { get; }
        public enmCustomerType CustomerType { get; }
        public bool HaveClaim(enmDocumentMaster claimId);
        public double WalletBalance { get; set; }
        public double CreditBalance { get; set; }
        CultureInfo cultureInfo { get;}
        public string MPin { get; }

    }
    public class CurrentUsers : ICurrentUsers
    {
        
        private CultureInfo _cultureInfo= new CultureInfo("en-IN",false);
        IHttpContextAccessor _httpContext;
        private string _Name , _MPin;
        private int _UserId, _CustomerId;
        private  List<int> _RoleId;        
        private readonly DBContext _context;
        private  enmCustomerType _CustomerType;

        private double? _WalletBalance;
        private double? _CreditBalance;

        public CurrentUsers(IHttpContextAccessor httpContext, DBContext context)
        {
           
            _httpContext = httpContext;
            _context = context;
            string tempUserId = httpContext.HttpContext.User.FindFirst("_UserId")?.Value;
            int.TryParse(tempUserId, out _UserId);
            
            _Name= httpContext.HttpContext.User.FindFirst("_Name")?.Value;            
            string tempCustomerId = httpContext.HttpContext.User.FindFirst("_CustomerId")?.Value;
            int.TryParse(tempCustomerId, out _CustomerId);
            string tempCustomerType = httpContext.HttpContext.User.FindFirst("_CustomerType")?.Value;
            _CustomerType = enmCustomerType.B2C;
            if (!string.IsNullOrEmpty(tempCustomerType))
            {
                Enum.TryParse<enmCustomerType>(tempCustomerType,  out _CustomerType);
            }
            
        }




        private void SetWalletBalance()
        {
            string MPin = Settings.Encrypt("0000");
            var defaultBalance=_context.tblCustomerBalance.Where(p => p.CustomerId == _CustomerId).FirstOrDefault();
            if (defaultBalance == null)
            {
                _context.tblCustomerBalance.Add(new tblCustomerBalance() { CustomerId = _CustomerId, CreditBalance = 0, ModifiedDt = DateTime.Now, MPin = MPin, WalletBalance = 0 });
                _context.SaveChanges();
                this._WalletBalance = 0;
                this._CreditBalance = 0;
                this._MPin = MPin;
            }
            else
            {
                this._WalletBalance = defaultBalance.WalletBalance;
                this._CreditBalance = defaultBalance.CreditBalance;
                this._MPin = Settings.Decrypt( defaultBalance.MPin);

            }

        }


        public CultureInfo cultureInfo { get { return _cultureInfo; } }
        public string Name { get { return _Name; } }
        public int CustomerId { get { return _CustomerId; } }
        public int UserId { get { return _UserId; } }
        public double WalletBalance { get { if (_WalletBalance == null) { SetWalletBalance(); } return _WalletBalance.Value; } set { _WalletBalance = value; } }
        public double CreditBalance { get { if (_CreditBalance == null) { SetWalletBalance(); } return _CreditBalance.Value; } set { _CreditBalance = value; } }
        public enmCustomerType CustomerType { get { return _CustomerType; } }
        public string MPin { get { if (_MPin == null) { SetWalletBalance(); } return _MPin; } }
        public List<int> RoleId
        {
            get
            {
                if (_RoleId == null)
                {
                    _RoleId = _context.tblUserRole.Where(p => p.UserId == _UserId ).Select(p => p.Role.Value).ToList();
                }
                return _RoleId;
            }
        }

        public bool HaveClaim(enmDocumentMaster claimId)
        {
            return _context.tblRoleClaim.Where(p => RoleId.Contains(p.Role.Value) && !p.IsDeleted && p.ClaimId == claimId).Count() > 0 ? true : false;
        }
    }
}