namespace InfoSafe.ViewModel
{
    public class BlobVM
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public string? Content { get; set; }
        public BlobMetaDataVM? MetaData { get; set; }
    }
}