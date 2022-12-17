﻿namespace InflationArchive.Helpers
{
    public class Filter
    {
        private string _order;
        private string _orderBy;
        private int _pageSize;
        private int _pageNr;
        private decimal _minPrice;
        private decimal _maxPrice;

        public string Order
        {
            get => _order;
            set => _order = value == FilterConstants.Descending
                ? FilterConstants.Descending
                : FilterConstants.Ascending;
        }

        public string OrderBy
        {
            get => _orderBy;
            set => _orderBy = value == FilterConstants.OrderByPrice
                ? FilterConstants.OrderByPrice
                : FilterConstants.OrderByName;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Max(1, Math.Min(value, 100));
        }

        public int PageNr
        {
            get => _pageNr;
            set => _pageNr = Math.Max(0, value);
        }

        public string Category { get; set; }

        public string Name { get; set; }

        public decimal MinPrice
        {
            get => _minPrice;
            set => _minPrice = Convert.ToDecimal(Math.Max(0, Math.Round(value, 3)));
        }

        public decimal MaxPrice
        {
            get => _maxPrice;
            set => _maxPrice = value <= 0
                ? decimal.MaxValue
                : Convert.ToDecimal(Math.Round(value, 3));
        }

        public Filter()
        {
            Order = "";
            OrderBy = "";
            PageSize = 0;
            PageNr = 0;
            Category = "";
            Name = "";
            MinPrice = 0;
            MaxPrice = 0;
        }
    }

    public static class FilterConstants
    {
        public const string Ascending = "asc";
        public const string Descending = "desc";
        public const string OrderByName = "name";
        public const string OrderByPrice = "price";
    }
}