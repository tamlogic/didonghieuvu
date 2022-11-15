namespace ManageEmployee.Models
{
    public class PagePrintViewModel
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public PrintViewModel QrCode { get; set; }
        public PrintViewModel Barcode { get; set; }
    }
    public class PrintViewModel
    {
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int? Size { get; set; }
        public double? MarginLeft { get; set; }
        public double? MarginRight { get; set; }
        public double? MarginTop { get; set; }
        public double? MarginBottom { get; set; }
    }
}