namespace Shortly.Client.Data.Models
{
    public class User
    {
        public User()
        {
            Urls = new List<Url>();
        }

        public int Id { get; set; }
        public string Email { get; set; }

        public List<Url> Urls { get; set; }
    }
}
