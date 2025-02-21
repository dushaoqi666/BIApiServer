namespace BIApiServer;
    public class ApiResponse<T>
    {
        public string stateCode { get; set; }
        public bool isSucess { get; set; }
        public string message { get; set; }
        public List<T> data { get; set; }
    }

