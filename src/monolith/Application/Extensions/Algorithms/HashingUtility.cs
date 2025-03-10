namespace Application.Extensions.Algorithms;

public static class HashingUtility
{
    public static string ComputeSha256Hash(string input)
    {
        if (input is null)
            throw new ArgumentNullException(nameof(input));

        using SHA256 sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}