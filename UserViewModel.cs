namespace Projekt_pre_skolu.Models
{
    public class UserViewModel
    {
        public string Username { get; set; } // Varchar(50) mapujeme na string
        public string Password { get; set; } // Varchar(50) tiež mapujeme na string
        public string Message { get; set; }
        public bool IsError { get; set; }
    }
}
