using System;

namespace FilmsCatalog.Models
{
    public class PageInfo
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItems { get; private set; }
        public int PageSize { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public PageInfo(int totalItems, int? page, int pageSize, int pagingLinkNumber)
        {
            var currentPage = (page ?? 0) != 0 ? (int)page : 1;
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            if(totalPages < pagingLinkNumber)
                pagingLinkNumber = totalPages;
            var startPage = currentPage - pagingLinkNumber / 2;
            if (startPage <= 0)
                startPage = 1;
            var endPage = startPage + pagingLinkNumber - 1;
            if(endPage > totalPages){
                endPage = totalPages;
                startPage = endPage - (pagingLinkNumber - 1);
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
        public bool HasPreviousPage
        {
            get
            {
                return (CurrentPage > 1);
            }
        }
 
        public bool HasNextPage
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }
    }
}