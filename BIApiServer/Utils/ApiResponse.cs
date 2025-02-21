namespace BIApiServer.Utils
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Success(T data)
        {
            return new ApiResponse<T>(data);
        }

        public ApiResponse()
        {
            Code = 200;
            Message = "Success";
        }

        public ApiResponse(T data)
        {
            Code = 200;
            Message = "Success";
            Data = data;
        }

        public ApiResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public ApiResponse(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}