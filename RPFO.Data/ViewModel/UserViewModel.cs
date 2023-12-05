using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class UserViewModel: MUser
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; } 
        public string Role { get; set; } 
    }
}
