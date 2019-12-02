namespace HPSocket
{
    public interface IHttpEasyData
    {
        #region Easy属性

        /// <summary>
        /// 自动解压缩, 默认true, gzip/deflate 自动解压缩 
        /// </summary>
        bool AutoDecompression { get; set; }

        #endregion
    }
}
