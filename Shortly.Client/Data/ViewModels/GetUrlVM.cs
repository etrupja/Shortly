namespace Shortly.Client.Data.ViewModels
{
    public class GetUrlVM
    {
        public int Id { get; set; }
        public string OriginalLink { get; set; }
        public string ShortLink { get; set; }
        public int NrOfClicks { get; set; }
        public string? UserId { get; set; }

        public GetUserVM? User { get; set; }
    }
}
