
using RPFO.Data.Entities;
using System;
using System.Collections.Generic;

namespace RPFO.Data.ViewModels
{
    
    public partial class QueryModel
    { 
        public string CompanyCode { get; set; }
        public string FunctionId { get; set; }
        public string Query { get; set; }
        public string QueryExcute { get; set; }
       
        public List<ParramObject> ParramObjects { get; set; } = new List<ParramObject>()  ;

    }
    public class ParramObject
    {
        public string Id { get; set; }
        public string Value { get; set; }

    }

}
