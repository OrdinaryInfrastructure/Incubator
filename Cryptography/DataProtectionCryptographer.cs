using Microsoft.AspNetCore.DataProtection;
using Odin.Logging;
using Odin.System;

namespace Odin.Cryptography
{
    /// <summary>
    /// Uses the .NET DataProtection APIs to encrypt and decrypt using IDataProtector.
    /// </summary>
    public sealed class DataProtectionCryptographer : ICryptographer
    {
        private readonly IDataProtector _protector;
        private ILoggerWrapper<DataProtectionCryptographer> _logger;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="logger"></param>
        /// <param name="purpose"></param>
        public DataProtectionCryptographer(IDataProtectionProvider provider, ILoggerWrapper<DataProtectionCryptographer> logger, string purpose = "Default")
        {
            _logger = logger;
            _protector = provider.CreateProtector(purpose);
        }

        /// <summary>
        /// Attempts decryption
        /// </summary>
        /// <param name="protectedString"></param>
        /// <returns></returns>
        public ResultValue<string?> TryDecrypt(string protectedString)
        {
            if (string.IsNullOrWhiteSpace(protectedString)) return ResultValue<string?>.Fail($"{nameof(protectedString)} is nullor empty");
            try
            {
                string decrypted = _protector.Unprotect(protectedString);
                return ResultValue<string?>.Succeed(decrypted);
            }
            catch (Exception err)
            {
                _logger.LogError($"{nameof(TryDecrypt)} error",err);
                return ResultValue<string?>.Fail(err.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="protectedString"></param>
        /// <returns></returns>
        public string Decrypt(string protectedString)
        {
            return _protector.Unprotect(protectedString);
        }

        /// <summary>
        /// Attempts encryption, returning false if an Exception occurs.
        /// </summary>
        /// <param name="unProtectedString"></param>
        /// <returns></returns>
        public ResultValue<string?> TryEncrypt(string unProtectedString)
        {
            if (string.IsNullOrWhiteSpace(unProtectedString)) return ResultValue<string?>.Fail($"{nameof(unProtectedString)} is null");
            try
            {
                string encrypted = _protector.Protect(unProtectedString);
                return ResultValue<string?>.Succeed(encrypted);
            }
            catch (Exception err)
            {
                _logger.LogError($"{nameof(TryEncrypt)} error",err);
                return ResultValue<string?>.Fail(err.Message);
            }
        }

        /// <inheritdoc />
        public string Encrypt(string unProtectedString)
        {
            return _protector.Protect(unProtectedString);
        }
    }
}
