namespace WatchAPI.Constants;

public static class AuthConstants
{
    public const int MinUserNameLength = 3;

    public const int MaxUserNameLength = 50;

    public const int MinPasswordLength = 6;

    public const int MaxPasswordLength = 100;

    public const int RefreshTokenBytes = 64;

    public const string PasswordRegex = @"(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+";
}
