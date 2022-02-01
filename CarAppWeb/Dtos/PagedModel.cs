using System.Collections.Generic;

namespace CarAppWeb.Dtos
{
    public class CarAppPagedModel<T>
    {
        public int Page { get; set; }
        public List<T> Elements { get; set; }
        public int ItemsPerPage { get; set; }

        public CarAppPagedModel()
        {
            Page = 1;
            ItemsPerPage = 2;
        }
        public CarAppPagedModel(int page)
        {
            Page = page;
            ItemsPerPage = 2;
        }
        public CarAppPagedModel(int page, int itemsPerPage)
        {
            Page = page;
            ItemsPerPage = itemsPerPage;
        }
    }
}