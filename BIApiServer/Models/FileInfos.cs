using SqlSugar;

namespace BIApiServer.Models;

 [SugarTable("Files", TableDescription = "文件表")]
    public class FileInfos : Entity
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDescription = "文件名称", IsNullable = false,
            ColumnDataType = "varchar(255)")]
        public string Name { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        [SugarColumn(ColumnName = "size", ColumnDescription = "文件大小（字节）", IsNullable = false)]
        public long Size { get; set; }

        /// <summary>
        /// 文件存储路径
        /// </summary>
        [SugarColumn(ColumnName = "path", ColumnDescription = "文件存储路径", IsNullable = false,
            ColumnDataType = "varchar(500)")]
        public string Path { get; set; }

        /// <summary>
        /// 所属文件夹ID
        /// </summary>
        [SugarColumn(ColumnName = "folder_id", ColumnDescription = "所属文件夹ID", IsNullable = false)]
        public long FolderId { get; set; }

        /// <summary>
        /// 文件类型（MIME 类型）
        /// </summary>
        [SugarColumn(ColumnName = "file_type", ColumnDescription = "文件类型", IsNullable = false,
            ColumnDataType = "varchar(100)")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件的 MD5 加密值
        /// </summary>
        [SugarColumn(ColumnName = "md5_hash", ColumnDescription = "MD5 加密值", IsNullable = false,
            ColumnDataType = "varchar(32)")]
        public string Md5Hash { get; set; }

        [SugarColumn(ColumnName = "file_name", ColumnDescription = "文件全名", IsNullable = false)]
        public string FileName { get; set; }
        

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID", IsNullable = false)]
        public long UserId { get; set; }
        
    }