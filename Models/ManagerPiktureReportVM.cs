using System.Collections.Generic;

namespace PROJEKTDB.Models
{
    public class ManagerPiktureRowVM
    {
        public string PikId { get; set; } = "";
        public string PikTit { get; set; } = "";
        public decimal PikCmim { get; set; }

        public string GalId { get; set; } = "";
        public string GalEm { get; set; } = "";

        public string ArtistId { get; set; } = "";
        public string ArtistEm { get; set; } = "";

        public int ShiturSasi { get; set; }        // sa cope jane shitur gjithsej
        public decimal Xhiro { get; set; }         // SUM(sasi * cmim)
    }

    public class ManagerPiktureReportVM
    {
        public int TotalPiktura { get; set; }
        public int TotalShiturSasi { get; set; }
        public decimal TotalXhiro { get; set; }

        public List<ManagerPiktureRowVM> Rows { get; set; } = new();
    }
}
