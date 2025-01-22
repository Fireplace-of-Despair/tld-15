using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ACherryPie.Security;

/// <summary> Hashing service </summary>
public interface IHashingService
{
    /// <summary> Hashing result </summary>
    public class HashResult
    {
        /// <summary> Hash result as a hex <seealso cref="string"/> </summary>
        public string HexHash { get; set; } = string.Empty;

        /// <summary> Hash salt as a hex <seealso cref="string"/> </summary>
        public string HexSalt { get; set; } = string.Empty;
    }

    /// <summary> Hash plaint text</summary>
    /// <param name="plainText"> Plain text </param>
    /// <param name="hexSalt"> Salt as hex <seealso cref="string"/>. If hexSalt is null, the new salt will be created</param>
    /// <returns> <seealso cref="HashResult"/> </returns>
    HashResult Hash(string plainText, string? hexSalt = null);

    string Hash(IFormFile formFile);
}

public sealed class HashingService(IConfiguration configuration) : IHashingService
{
    /// <summary> Pepper </summary>
    private readonly string _pepper = configuration.GetSection("Security:Pepper").Value!;

    /// <summary> Salt size </summary>
    private readonly uint _saltSize = uint.Parse(configuration.GetSection("Security:SaltSize").Value!);

    /// <summary> Hash plaint text</summary>
    /// <param name="plainText"> Plain text </param>
    /// <param name="hexSalt"> Salt as hex string. If hexSalt is null, the new salt will be created</param>
    /// <returns> <seealso cref="HashResult"/> </returns>
    public IHashingService.HashResult Hash(string plainText, string? hexSalt = null)
    {
        ArgumentNullException.ThrowIfNull(plainText);

        var text = AddPepper
        (
            Encoding.UTF8.GetBytes(plainText),
            Encoding.UTF8.GetBytes(_pepper)
        );

        byte[] salt;
        if (string.IsNullOrEmpty(hexSalt))
        {
            salt = GenerateRandomArray(_saltSize);
        }
        else
        {
            salt = Convert.FromHexString(hexSalt);
        }

        var result = CalcSaltedHash(text, salt);
        return new IHashingService.HashResult
        {
            HexHash = Convert.ToHexString(result),
            HexSalt = hexSalt ?? Convert.ToHexString(salt)
        };
    }

    public string Hash(IFormFile formFile)
    {
        var result = string.Empty;

        using (var md5 = MD5.Create())
        {
            using (var fileStream = formFile.OpenReadStream())
            {
                result = Convert.ToHexStringLower(md5.ComputeHash(fileStream));
            }
        }

        return result;
    }

    /// <summary> Calculate hash of the plain text with salt</summary>
    /// <param name="plainText"> Plain text </param>
    /// <param name="salt"> Salt </param>
    /// <returns> Hash of the plain text with salt </returns>
    private static byte[] CalcSaltedHash(byte[] plainText, byte[] salt)
    {
        var plainTextWithSalt = new byte[plainText.Length + salt.Length];
        plainText.CopyTo(plainTextWithSalt, 0);
        salt.CopyTo(plainTextWithSalt, plainText.Length);

        return Rfc2898DeriveBytes.Pbkdf2(plainText, salt, 100000, HashAlgorithmName.SHA384, 128);
    }

    /// <summary>Create <see cref="byte"/> array filled with random numbers </summary>
    /// <param name="size"><see cref="byte"/> array size </param>
    /// <returns><see cref="byte"/> array filled with random numbers </returns>
    private static byte[] GenerateRandomArray(uint size)
    {
        using (var rngCsp = RandomNumberGenerator.Create())
        {
            var result = new byte[size];
            rngCsp.GetNonZeroBytes(result);

            return result;
        }
    }

    /// <summary> Add extra salt to the plain text </summary>
    /// <param name="plainText"> Plain text </param>
    /// <param name="pepper"> Pepper </param>
    /// <returns> Plain text with pepper </returns>
    private static byte[] AddPepper(byte[] plainText, byte[] pepper)
    {
        var plainTextWithPepper = new byte[plainText.Length + pepper.Length];
        plainText.CopyTo(plainTextWithPepper, 0);
        pepper.CopyTo(plainTextWithPepper, plainText.Length);

        return plainTextWithPepper;
    }

}
