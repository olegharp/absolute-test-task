using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace FilmsCatalog.Models
{

    public class CustomYearRangeAttribute : RangeAttribute
    {
        public CustomYearRangeAttribute() : base(1895, DateTime.Now.Year) 
        { 
            this.ErrorMessage = $"Значение должно быть в диапазоне 1895 - {DateTime.Now.Year}";
        } 
    }

    public class Movie
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        [StringLength(100, ErrorMessage = "Не должно превышать 100 символов")] 
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; }

        [Display(Name = "Описание")] 
        [StringLength(1024, ErrorMessage = "Превышена допустимая длина")]
        public string Description { get; set; }

        public string CreatorIdentityName { get; set; }

        [NotMapped]
        public bool IsCreator { get; set; }

        [Display(Name = "Добавил")] 
        [NotMapped]
        public string CreatorName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [CustomYearRange]
        [Display(Name = "Год выпуска")] 
        public int ReleaseYear { get; set; }

        [Display(Name = "Режиссер")] 
        [StringLength(100, ErrorMessage = "Не должно превышать 100 символов")] 
        public string Director { get; set; }

        public byte[] Poster { get; set; }

        [NotMapped]
        [Display(Name = "Постер")] 
        public IFormFile PosterFile { get; set; }
    }
}