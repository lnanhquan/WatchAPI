namespace WatchAPI.Options;

public class AuthCookieOptions
{
    public string AccessTokenName { get; set; } = "access_token";

    public string RefreshTokenName { get; set; } = "refresh_token";

    public bool HttpOnly { get; set; } = true;

    public bool Secure { get; set; } = true;

    public SameSiteMode SameSite { get; set; } = SameSiteMode.None;

    public int AccessTokenExpireMinutes { get; set; }

    public int RefreshTokenExpireDays { get; set; }

    public string Path { get; set; } = "/";
}
