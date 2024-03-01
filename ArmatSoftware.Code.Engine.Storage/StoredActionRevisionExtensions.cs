using System;
using System.Security.Cryptography;
using System.Text;

namespace ArmatSoftware.Code.Engine.Storage;

public static class StoredActionRevisionExtensions
{
    private const string UtcDateSerializationFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

    public static void TamperProof(this StoredActionRevision revision)
    {
        if (!string.IsNullOrWhiteSpace(revision.Hash)) throw new ArgumentException("Repeated hashing attempt made");
        if (string.IsNullOrWhiteSpace(revision.Code)) throw new ArgumentException("Code is required for tamper proofing");
        if (string.IsNullOrWhiteSpace(revision.Author)) throw new ArgumentException("Author is required for tamper proofing");
        if (string.IsNullOrWhiteSpace(revision.Comment)) throw new ArgumentException("Comment is required for tamper proofing");
        if (revision.Created == default) throw new ArgumentException("Created date is required for tamper proofing");
        revision.Hash = Base64Hash(revision);
    }
    
    public static void CheckTamperProof(this StoredActionRevision revision)
    {
        if (string.IsNullOrWhiteSpace(revision.Hash)) throw new ArgumentException("Hash is required for tamper proof checking");
        if (revision.Hash != Base64Hash(revision)) throw new InvalidOperationException("Revision has been tampered with");
    }
    
    private static string Base64Hash(this StoredActionRevision revision)
    {
        string salt = revision.Created.ToString(UtcDateSerializationFormat);
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(salt)))
        {
            byte[] hashBytes =
                hmac.ComputeHash(Encoding.UTF8.GetBytes($"{revision.Code}.{revision.Author}.{revision.Comment}"));
            var base64Hash = Convert.ToBase64String(hashBytes);
            return base64Hash;
        }
    }
}