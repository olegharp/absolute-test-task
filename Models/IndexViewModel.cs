using System.Collections.Generic;

namespace FilmsCatalog.Models
{

    public class IndexViewModel
    {
        public IEnumerable<Movie> Movies { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}