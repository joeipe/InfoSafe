namespace InfoSafe.ViewModel
{
    public class BlobRequestVM
    {
        public string FileName { get; set; } = null!;
        public BlobMetaDataVM? MetaData { get; set; }
    }
}