using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FilmsCatalog.Models
{

    public class CustomYearRangeAttribute : RangeAttribute
    {
        public CustomYearRangeAttribute() : base(1895, DateTime.Now.Year) 
        { 
            this.ErrorMessage = $"Значение должно быть в диапазоне 1895 - {DateTime.Now.Year}";
        } 
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
            if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Размер файла не должен превышать {_maxFileSize / 1024 / 1024} MB.";
        }
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
        
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (!((IList<string>)_extensions).Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Только png и jpg-файлы!";
        }
    }

    [Index(nameof(ReleaseYear))]
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
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile PosterFile { get; set; }
    }
}