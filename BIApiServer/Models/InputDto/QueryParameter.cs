namespace BIApiServer.Models.InputDto;

public class QueryBaseParameter
{
    /// <summary>
    /// 页码，从1开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页记录数
    /// </summary>
    public int PageSize { get; set; } = 10;
    
}