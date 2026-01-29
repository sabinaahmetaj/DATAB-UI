using System;
using System.Collections.Generic;

namespace PROJEKTDB.Models
{
    // 1 rresht = 1 fature (brenda nje galerie)
    // Nese 1 fature ka piktura nga disa galeri, ajo fature shfaqet ne secilen galeri
    // me totalin perkates per ate galeri.
    public class GalleryInvoiceRowVM
    {
        public string GalId { get; set; } = "";
        public string GalName { get; set; } = "";

        public int FatId { get; set; }
        public DateTime FatDat { get; set; }

        public decimal TotalInThisGallery { get; set; }
    }

    // Totali i pergjithshem i shitjeve per nje galeri
    public class GalleryTotalVM
    {
        public string GalId { get; set; } = "";
        public string GalName { get; set; } = "";

        public decimal TotalSales { get; set; }
    }

    // ViewModel kryesor qe shkon ne View:
    // - TotalsPerGallery: totali per cdo galeri
    // - InvoicesPerGallery: lista e faturave (te grupuara sipas galerisë)
    public class GalleryInvoicesReportVM
    {
        public List<GalleryTotalVM> TotalsPerGallery { get; set; } = new List<GalleryTotalVM>();
        public List<GalleryInvoiceRowVM> InvoicesPerGallery { get; set; } = new List<GalleryInvoiceRowVM>();
    }
}
