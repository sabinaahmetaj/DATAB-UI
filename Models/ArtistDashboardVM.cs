namespace PROJEKTDB.Models
{
    public class ArtistDashboardVM
    {
        public string PerId { get; set; } = "";
        public string ArtistEmri { get; set; } = "";
        public decimal TotalXhiro { get; set; }
        public List<ArtistPiktureRowVM> Pikturat { get; set; } = new();
    }

    public class ArtistPiktureRowVM
    {
        public string PikId { get; set; } = "";
        public string PikTit { get; set; } = "";
        public decimal PikCmim { get; set; }
        public int ShiturSasi { get; set; }
        public decimal Xhiro { get; set; }
    }
}
