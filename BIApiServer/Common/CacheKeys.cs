public static class CacheKeys
{
    public static string FileList(int pageIndex, int pageSize) 
        => $"file:list:{pageIndex}:{pageSize}";
    
    public static string FileDetail(long fileId) 
        => $"file:detail:{fileId}";
} 