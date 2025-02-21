namespace Van_Rise_Intern_App.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}