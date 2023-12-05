using System;
using System.Collections.Generic;

#nullable disable

namespace RPFO.Data.ViewModel
{

    public partial class PersonnelProfileViewModel
    {
        public string Code { get; set; }
        public PersonnelProfile Data { get; set; }
    }

    public class PersonnelProfile
    {
        public string Error { get; set; }
        public PersonnelProfileDetail Data { get; set; }
    }

    public class PersonnelProfileDetail
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Position_id { get; set; }
        public string Place_current { get; set; }
        public string Private_code { get; set; }
        public string Birthday { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
