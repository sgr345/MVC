using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace MVC6.Utilities
{
	public class Common
	{
        public static void SetDataProtection(IServiceCollection services, string keyPath, string applicationName, Enum cryptoType)
        {
            var builder = services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                    .SetApplicationName(applicationName);

            switch (cryptoType)
            {
                case Enums.CryptoType.Unmanaged:
                    builder.UseCryptographicAlgorithms(
                            new AuthenticatedEncryptorConfiguration()
                            {
                                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                                ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                            });
                    break;
                case Enums.CryptoType.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration()
                        {
                            // A type that subclasses SymmetricAlgorithm
                            EncryptionAlgorithmType = typeof(Aes),

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // A type that subclasses KeyedHashAlgorithm
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        });
                    break;
                default:
                    break;
            }
        }
    }
}

