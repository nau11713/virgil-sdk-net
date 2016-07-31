﻿#region "Copyright (C) 2015 Virgil Security Inc."
/**
 * Copyright (C) 2015 Virgil Security Inc.
 *
 * Lead Maintainer: Virgil Security Inc. <support@virgilsecurity.com>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     (1) Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 *
 *     (2) Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in
 *     the documentation and/or other materials provided with the
 *     distribution.
 *
 *     (3) Neither the name of the copyright holder nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ''AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
 * IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

namespace Virgil.SDK
{
    using System;
    using System.Collections.Generic;

    using Virgil.SDK.Cryptography;

    /// <summary>
    /// The <see cref="VirgilKey"/> object represents an opaque reference to keying material 
    /// that is managed by the user agent.
    /// </summary>
    public sealed class VirgilKey
    {
        private readonly CryptoContainer cryptoContainer;

        /// <summary>
        /// Prevents a default instance of the <see cref="VirgilKey"/> class from being created.
        /// </summary>
        private VirgilKey(CryptoContainer cryptoContainer)
        {
            this.cryptoContainer = cryptoContainer;
        }

        /// <summary>
        /// Creates an instance of <see cref="VirgilKey"/> object that represents a new named key, 
        /// using default key storage cryptoService.
        /// </summary>
        /// <param name="keyName">The name of the key.</param>
        /// <returns>An instance of <see cref="VirgilKey"/> that represent a newly created key.</returns>
        public static VirgilKey Create(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) 
                throw new ArgumentException(Localization.ExceptionArgumentIsNullOrWhitespace, nameof(keyName));
            
            var data = new Dictionary<string, string>
            {
                { "Id", Guid.NewGuid().ToString() }
            };

            var @params = new CryptoContainerParams(keyName, "Default", data);
            var cryptoContainer = new DefaultCryptoContainer(@params);

            return new VirgilKey(cryptoContainer);
        }

        /// <summary>
        /// Creates an instance of <see cref="VirgilKey" /> object that represents a new named key.
        /// </summary>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="keyContainer">The key container.</param>
        /// <returns>An instance of <see cref="VirgilKey" /> that represent a newly created key.</returns>
        public static VirgilKey Create(string keyName, CryptoContainer keyContainer)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentException(Localization.ExceptionArgumentIsNullOrWhitespace, nameof(keyName));

            return new VirgilKey(keyContainer);
        }

        /// <summary>
        /// Imports the specified key data.
        /// </summary>
        /// <param name="keyData">The key data.</param>
        /// <returns>VirgilKey.</returns>
        public static VirgilKey Import(VirgilBuffer keyData)
        {
            throw new NotImplementedException();
        }

        public static bool Exists(string keyName)
        {
            throw new NotImplementedException();
        }

        public static VirgilKey Load(string keyName)
        {
            throw new NotImplementedException();
        }

        public VirgilBuffer Export()
        {
            throw new NotImplementedException();
        }

        public VirgilBuffer Sign(VirgilBuffer data)
        {
            var signature = this.cryptoContainer.PerformDecryption(data.ToBytes());
            return VirgilBuffer.FromBytes(signature);
        }

        public VirgilBuffer Decrypt(VirgilBuffer cipherdata)
        {
            var data = this.cryptoContainer.PerformDecryption(cipherdata.ToBytes());
            return VirgilBuffer.FromBytes(data);
        }
    }
}