﻿#region Copyright (C) Virgil Security Inc.
// Copyright (C) 2015-2016 Virgil Security Inc.
// 
// Lead Maintainer: Virgil Security Inc. <support@virgilsecurity.com>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions 
// are met:
// 
//   (1) Redistributions of source code must retain the above copyright
//   notice, this list of conditions and the following disclaimer.
//   
//   (2) Redistributions in binary form must reproduce the above copyright
//   notice, this list of conditions and the following disclaimer in
//   the documentation and/or other materials provided with the
//   distribution.
//   
//   (3) Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived 
//   from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR ''AS IS'' AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
// IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Virgil.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using Virgil.SDK.Storage;
    using Virgil.SDK.Exceptions;
    using Virgil.SDK.Cryptography;

    /// <summary>
    /// The <see cref="VirgilKey"/> class represents a user's high-level Private key which provides 
    /// a list of methods that allows to store the key and perform cryptographic operations like 
    /// Decrypt, Sign etc.
    /// </summary>
    public sealed class VirgilKey
    {
        private readonly VirgilApiContext context;
        private readonly IPrivateKey privateKey;

        /// <summary>
        /// Prevents a default instance of the <see cref="VirgilKey"/> class from being created.
        /// </summary>
        internal VirgilKey(VirgilApiContext context, IPrivateKey privateKey)
        {
            this.context = context; 
            this.privateKey = privateKey;
        }
        
        /// <summary>
        /// Exports the <see cref="VirgilKey"/> to default format, specified in Crypto API.
        /// </summary>
        public VirgilBuffer Export(string password = null)
        {
            var exportedPrivateKey = this.context.Crypto.ExportPrivateKey(this.privateKey, password);
            return new VirgilBuffer(exportedPrivateKey);
        }

        /// <summary>
        /// Generates a digital signature for specified data using current <see cref="VirgilKey"/>.
        /// </summary>
        /// <param name="data">The data for which the digital signature will be generated.</param>
        /// <returns>A new buffer that containing the result from performing the operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public VirgilBuffer Sign(VirgilBuffer data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            
            var signature = this.context.Crypto.Sign(data.GetBytes(), this.privateKey);
            return new VirgilBuffer(signature);
        }

        /// <summary>
        /// Decrypts the specified cipher data using <see cref="VirgilKey"/>.
        /// </summary>
        /// <param name="cipherBuffer">The encrypted data.</param>
        /// <returns>A byte array containing the result from performing the operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public VirgilBuffer Decrypt(VirgilBuffer cipherBuffer)
        {
            if (cipherBuffer == null)
                throw new ArgumentNullException(nameof(cipherBuffer));
            
            var data = this.context.Crypto.Decrypt(cipherBuffer.GetBytes(), this.privateKey);
            return new VirgilBuffer(data);
        }

        /// <summary>
        /// Encrypts and signs the data.
        /// </summary>
        /// <param name="buffer">The data to be encrypted.</param>
        /// <param name="recipients">The list of <see cref="VirgilCard"/> recipients.</param>
        /// <returns>The encrypted data</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public VirgilBuffer SignThenEncrypt(VirgilBuffer buffer, IEnumerable<VirgilCard> recipients)
        {
            if (recipients == null)
                throw new ArgumentNullException(nameof(recipients));
            
            var publicKeys = recipients.Select(pk => pk.PublicKey).ToArray();
            var cipherdata = this.context.Crypto.SignThenEncrypt(buffer.GetBytes(), this.privateKey, publicKeys);

            return new VirgilBuffer(cipherdata);
        }

        /// <summary>
        /// Decrypts and verifies the data.
        /// </summary>
        /// <param name="cipherbuffer">The data to be decrypted.</param>
        /// <param name="card">The signer's <see cref="VirgilCard"/>.</param>
        /// <returns>The decrypted data, which is the original plain text before encryption.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public VirgilBuffer DecryptThenVerify(VirgilBuffer cipherbuffer, VirgilCard card)
        {
            var plaitext = this.context.Crypto
                .DecryptThenVerify(cipherbuffer.GetBytes(), this.privateKey, card.PublicKey);

            return new VirgilBuffer(plaitext);
        }

        /// <summary>
        /// Saves a current <see cref="VirgilKey"/> in secure storage.
        /// </summary>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="password">The password (optional).</param>
        public VirgilKey Save(string keyName, string password = null)
        {
            var exportedPrivateKey = this.context.Crypto.ExportPrivateKey(this.privateKey, password);
            var keyEntry = new KeyEntry
            {
                Name = keyName,
                Value = exportedPrivateKey
            };

            if (this.context.KeyStorage.Exists(keyEntry.Name))
                throw new VirgilKeyIsAlreadyExistsException();

            this.context.KeyStorage.Store(keyEntry);
            return this;
        }

        /// <summary>
        /// Exports the Public key value from current <see cref="VirgilKey"/>.
        /// </summary>
        /// <returns>A new <see cref="VirgilBuffer"/> that contains Public Key value.</returns>
        public VirgilBuffer ExportPublicKey()
        {
            var publicKey = this.context.Crypto.ExtractPublicKey(this.privateKey);
            return VirgilBuffer.From(this.context.Crypto.ExportPublicKey(publicKey));
        }
    }
}