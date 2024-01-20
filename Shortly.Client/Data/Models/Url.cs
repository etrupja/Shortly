namespace Shortly.Client.Data.Models
{
    public class Url
    {
        public int Id { get; set; }
        public string OriginalLink { get; set; }
        public string ShortLink { get; set; }
        public int NrOfClicks { get; set; }
        public int? UserId { get; set; }


        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
