using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.Entities
{
    public partial class SACStoreProcedure
    {
       
        public Guid Id { get; set; } 
        public string FunctionService { get; set; }
        public string NameOfStore { get; set; }
        public string TableName { get; set; }
        public string Description { get; set; }
        public bool? IsRequired { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }
        public string Content4 { get; set; }
        public string Content5 { get; set; }
        public string Content6 { get; set; }
        public string Content7 { get; set; }
        public string Content8 { get; set; }
        public string Content9 { get; set; }
        public string Content10 { get; set; }
        public string Status { get; set; }  
        public DateTime? CreatedOn { get; set; }
    
    }
}
