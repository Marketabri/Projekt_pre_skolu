using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt_pre_skolu.Models
{
    public class StudentViewModel
    {
        public StudentViewModel() { }
        public long Id { get; set; } // Pre bigint používame long
        public string Name { get; set; } // Varchar(50) mapujeme na string
        public string Surname { get; set; } // Varchar(50) tiež mapujeme na string
        public string Mobil { get; set; } // Varchar(50) pre mobil, tiež mapujeme na string
        public string Email { get; set; } = string.Empty; // Nvarchar(50) mapujeme na string
        public int? FavouriteSubjectId { get; set; }
        [NotMapped]
        public List<SelectListItem> Subjects { get; set; }
        public string? Image { get; set; }
    }
}
