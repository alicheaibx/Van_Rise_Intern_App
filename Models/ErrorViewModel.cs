namespace Van_Rise_Intern_App.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
      public string  ErrorMessage { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
