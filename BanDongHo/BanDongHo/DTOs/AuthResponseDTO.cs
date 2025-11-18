namespace BanDongHo.DTOs
{
    public class AuthResponseDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IList<String> Roles { get; set; }
        public string Token { get; set; }
    }
}
