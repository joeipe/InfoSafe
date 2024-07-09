namespace InfoSafe.ViewModel
{
    public class BlobResponseVM
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public BlobVM Blob { get; set; }

        public BlobResponseVM()
        {
            Blob = new BlobVM();
        }
    }
}