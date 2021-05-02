﻿using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount
{
    public class CreditorAccountModel : StandardEntity, IValidatableObject
    {
        #region Supplier
        [MaxLength(512)]
        public string SupplierName { get; set; }
        [MaxLength(128)]
        public string SupplierCode { get; set; }

        public bool SupplierIsImport { get; set; }
        #endregion

        #region Division
        public int DivisionId { get; set; }

        public string DivisionCode { get; set; }

        public string DivisionName { get; set; }
        #endregion

        #region Unit
        public int UnitId { get; set; }

        public string UnitCode { get; set; }

        public string UnitName { get; set; }
        #endregion


        #region UnitReceiptNote
        [MaxLength(128)]
        public string UnitReceiptNoteNo { get; set; }

        public string Products { get; set; }

        public DateTimeOffset? UnitReceiptNoteDate { get; set; }

        public decimal UnitReceiptNoteDPP { get; set; }

        public decimal UnitReceiptNotePPN { get; set; }

        public decimal UnitReceiptMutation { get; set; }
        #endregion

        #region BankExpenditureNote
        public int BankExpenditureNoteId { get; set; }
        [MaxLength(128)]
        public string BankExpenditureNoteNo { get; set; }

        public DateTimeOffset? BankExpenditureNoteDate { get; set; }

        public decimal BankExpenditureNoteDPP { get; set; }

        public decimal BankExpenditureNotePPN { get; set; }

        public decimal BankExpenditureNoteMutation { get; set; }
        #endregion

        #region Memo has been changed to SPBNo or NI No
        [MaxLength(128)]
        public string MemoNo { get; set; }

        public DateTimeOffset? MemoDate { get; set; }

        public decimal MemoDPP { get; set; }

        public decimal MemoPPN { get; set; }

        public decimal MemoMutation { get; set; }
        [MaxLength(512)]
        public string PaymentDuration { get; set; }
        #endregion
        [MaxLength(128)]
        public string InvoiceNo { get; set; }

        public decimal FinalBalance { get; set; }
        [MaxLength(64)]
        public string CurrencyCode { get; set; }

        public decimal DPPCurrency { get; set; }
        public decimal CurrencyRate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
