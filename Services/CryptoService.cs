using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace PaymentTest.Services
{
    class CryptoService
    {
        public string GetSign(string dataString)
        {
            X509Certificate2 cert = GetCertificateFromStorage("Olexii");

            if (!cert.HasPrivateKey)
                throw new Exception("Certificate must contain a private key.");
            
            var rsa = (RSACng)cert.PrivateKey;

            byte[] binData = Encoding.ASCII.GetBytes(dataString);
            var binSignature = rsa.SignData(binData, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            var signature = Convert.ToBase64String(binSignature);

            return signature;
        }

        public bool Verify(string dataString, string signature)
        {
            X509Certificate2 cert = GetCertificateFromStorage("Signature");

            var rsa = (RSACng)cert.PublicKey.Key;

            var binData = Encoding.ASCII.GetBytes(dataString);
            var binSignature = Convert.FromBase64String(signature);

            return rsa.VerifyData(binData, binSignature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        private X509Certificate2 GetCertificateFromStorage(string subjectName)
        {
            X509Certificate2 cert = null;

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var certs = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);

                if (certs.Count == 0)
                    throw new Exception($"There are no certificates for {subjectName} in private storage for local mashine.");

                if (certs.Count > 1)
                    throw new Exception($"There are more when one certificates for {subjectName} in private storage for local mashine.");

                cert = certs[0];
            }
            finally
            {
                store.Close();
            }

            return cert;
        }
    }
}