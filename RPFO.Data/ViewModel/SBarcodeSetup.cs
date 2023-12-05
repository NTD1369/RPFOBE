using Newtonsoft.Json;
using RPFO.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class SBarcodeSetup
    {
        public string CompanyCode { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public int? PrefixPosition { get; set; }
        public int? BarCodePosition { get; set; }
        public int? WeightPosition { get; set; }
        public int? PLUPosition { get; set; }
        public int? QtyPosition { get; set; }
        public int? AmountPosition { get; set; }
        public int? CheckPosition { get; set; }
        public int? PLULength { get; set; }
        public int? BarCodeLength { get; set; }
        public int? QtyLength { get; set; }
        public int? WeightLength { get; set; }
        public int? CheckCodeLength { get; set; }
        public int? AmountLength { get; set; }
        public string CheckCode { get; set; }
        public string CharSeparator { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string AmountCalculation { get; set; }
        public int? AmountValue { get; set; }
        public string WeightCalculation { get; set; }
        public int? WeightValue { get; set; }
      
        public int? PrefixCheckLength { get; set; }
        public bool? IsOrgPrice { get; set; }
        public string CustomF1 { get; set; }
        public string CustomF2 { get; set; }
        public string CustomF3 { get; set; }
        public string CustomF4 { get; set; }
        public string CustomF5 { get; set; }
    }
   
}
