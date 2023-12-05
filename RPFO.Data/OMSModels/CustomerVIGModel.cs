using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.OMSModels
{
    //public class CustomerVIGModel
    //{
    //    [JsonProperty("id")]
    //    public string ID { get; set; }
    //    //[JsonProperty("mobile")]
    //    public string Mobile { get; set; }
    //    //[JsonProperty("name")]
    //    public string Name { get; set; }
    //    //[JsonProperty("email")]
    //    public string Email { get; set; }
    //    //[JsonProperty("sourceID")]
    //    public string SourceID { get; set; }
    //    [JsonProperty("custom_fields")]
    //    public List<CustomField> Custom_Fields { get; set; }
    //}

    //public class CustomField
    //{
    //    public string Name { get; set; }
    //    public string Value { get; set; }
    //}

    public class CustomerVIGModel
    {
        //[JsonProperty("id")]
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        //[JsonProperty("created_on")]
        public string created_on { get; set; }
        public string updated_on { get; set; }
        //[JsonProperty("created_by")]
        public string created_by { get; set; }
        //[JsonProperty("registered_store")]
        public string registered_store { get; set; } //đi từ POS value: JAPOS ; Ecom value: JAECOM 
        public string Group { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        //[JsonProperty("member_type")]
        public string member_type { get; set; }
        public string Description { get; set; }
        //[JsonProperty("residental_type")]
        public string residental_type { get; set; }
        //[JsonProperty("acquisition_channel")]
        public string acquisition_channel { get; set; }
        //[JsonProperty("source_of_customers")]
        public string source_of_customers { get; set; }
        //[JsonProperty("promotion_tracker")]
        public string promotion_tracker { get; set; }
        //[JsonProperty("reference_name")]
        public string reference_name { get; set; }
        //[JsonProperty("reference_email")]
        public string reference_email { get; set; }
        //[JsonProperty("reference_mobile")]
        public string reference_mobile { get; set; }
        //[JsonProperty("waiver_skill")]
        public string waiver_skill { get; set; }
        //[JsonProperty("social_account")]
        public string social_account { get; set; }
        //[JsonProperty("family_member")]
        public List<FamilyMember> family_member { get; set; }
        //[JsonProperty("updated_store")]
        public string updated_store { get; set; }
    }

    public class FamilyMember
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        //[JsonProperty("waiver_relationship")]
        public string waiver_relationship { get; set; }
    }
}
