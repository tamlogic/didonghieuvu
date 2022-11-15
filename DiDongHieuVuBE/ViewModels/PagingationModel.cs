﻿namespace ManageEmployee.ViewModels
{
    public class PagingationRequestModel
    {
        public bool isSort { get; set; } = false;
        public string? SortField { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? SearchText { get; set; }
    }
}