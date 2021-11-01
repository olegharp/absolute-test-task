using System;

namespace FilmsCatalog.Models
{
    public class PageInfo
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItemsCount { get; private set; }
        public int PaginationPageCount { get; private set; }

        public PageInfo(int totalCount, int pageNumber, int pageSize, int paginationPageCount)
        {
            TotalItemsCount = totalCount;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            PaginationPageCount = paginationPageCount % 2 == 0 ? paginationPageCount - 1 : paginationPageCount;
        }
        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }
 
        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}