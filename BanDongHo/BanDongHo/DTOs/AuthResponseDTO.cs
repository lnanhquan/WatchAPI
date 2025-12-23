namespace WatchAPI.DTOs;

public class AuthResponseDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public IList<String> Roles { get; set; }
}
