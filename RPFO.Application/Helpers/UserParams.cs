namespace RPFO.Application.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 2000;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value;}
        }
        public string status { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string Store { get; set; }
        public string SlocId { get; set; }
        public string Item { get; set; }
        public string Merchandise { get; set; }
        public string keyword { get; set; }
        public string Keyword { get; set; }
        public string Customer { get; set; }
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string UomCode { get; set; }
        public string CompanyCode { get; set; }
        public string Version { get; set; }
        public string Id { get; set; }
    
        public string OrderBy { get; set; }
    }
}