

using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using static System.Net.Mime.MediaTypeNames;

namespace MsalOidc.Infrastructure;

public class HashNonceMiddleware
{
    private readonly RequestDelegate _next;

    public HashNonceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            string signature = jsonToken.RawSignature;//JpB-vhAvZUS-0-z1mp6Sk2DodooazOl0TAR_8YvxYLWx28svsLasCQB-VvmEGueOmFmZIth-fDMHV_9SykcXMEa5rqkq-VnBsN5sJafh0y4tTCLFdlXygns5RDKYsToMhpAPla7cpbXS9uwcjOMUdZ98GPPtG75Z19knANcnMAqWnjBkYRBjvnl_ii8GzWRB7mefccH4LYc_JkL255Laa_fffV1Ab71SaBEA3-cWeRjb01X0pqCiAgXLM3mWoYgZCYaxEW90b6LzM4FfT3Or6LKnT0weTsdborF7tqeaPAami1BP9MSuvFjZyDMb5_PiLXRakawdcEql-Rt8evt9XQ

            //recherche de la bonne signature, tentative avec et sans hash nonce
            //https://login.microsoftonline.com/6034845f-1372-457d-8d8e-3ea438724be1/v2.0/.well-known/openid-configuration
            //"jwks_uri":"https://login.microsoftonline.com/6034845f-1372-457d-8d8e-3ea438724be1/discovery/v2.0/keys"
            //string n = "t6Q2XSeWnMA_-crH2UbftfS01QDAqHoPQFqsRtVkxG4eyamnNlTl3Da07QQkjpPEbLoLtgtMI2Pr0plO7xU9f94mhbfK_UJ6Y0KcWxhwKMkCgnzcFOQF4eH_AICHLOKa8vPthtcprNcCmjbksW5TYBZi6uLhFLw_HsjGOxhK0VaDWnWizNVeqvzVB0jt9Vdmfhs6Zohy_1b2Wusdad1NmSKzhC74IDjlIaFoik_ZJJdtLOgoIwOZTLW0M1UKhRrWtj7AjVCnE_zBiloACm1IrIM_PymE10cJJ6WFz29ep4g7X65xCEU6zJ5oIFibvk6cKKcFNB7FFjbehYVpw5BxVQ";
            //string e = "AQAB";
            //string x5c = "MIIC/TCCAeWgAwIBAgIICHb5qy8hKKgwDQYJKoZIhvcNAQELBQAwLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDAeFw0yNDAxMTUxODA0MTRaFw0yOTAxMTUxODA0MTRaMC0xKzApBgNVBAMTImFjY291bnRzLmFjY2Vzc2NvbnRyb2wud2luZG93cy5uZXQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC3pDZdJ5acwD/5ysfZRt+19LTVAMCoeg9AWqxG1WTEbh7Jqac2VOXcNrTtBCSOk8Rsugu2C0wjY+vSmU7vFT1/3iaFt8r9QnpjQpxbGHAoyQKCfNwU5AXh4f8AgIcs4pry8+2G1yms1wKaNuSxblNgFmLq4uEUvD8eyMY7GErRVoNadaLM1V6q/NUHSO31V2Z+GzpmiHL/VvZa6x1p3U2ZIrOELvggOOUhoWiKT9kkl20s6CgjA5lMtbQzVQqFGta2PsCNUKcT/MGKWgAKbUisgz8/KYTXRwknpYXPb16niDtfrnEIRTrMnmggWJu+TpwopwU0HsUWNt6FhWnDkHFVAgMBAAGjITAfMB0GA1UdDgQWBBQLGQYqt7pRrKWQ25XWSi6lGN818DANBgkqhkiG9w0BAQsFAAOCAQEAtky1EYTKZvbTAveLmL3VCi+bJMjY5wyDO4Yulpv0VP1RS3dksmEALOsa1Bfz2BXVpIKPUJLdvFFoFhDqReAqRRqxylhI+oMwTeAsZ1lYCV4hTWDrN/MML9SYyeQ441Xp7xHIzu1ih4rSkNwrsx231GTfzo6dHMsi12oEdyn6mXavWehBDbzVDxbeqR+0ymhCgeYjIfCX6z2SrSMGYiG2hzs/xzypnIPnv6cBMQQDS4sdquoCsvIqJRWmF9ow79oHhzSTwGJj4+jEQi7QMTDR30rYiPTIdE63bnuARdgNF/dqB7n4ZJv566jvbzHpfCTqrJyj7Guvjr9i56NpLmz2DA==";

            //string signatureNonHashedNonce = GetSignedJwtToken(jsonToken, n, e, x5c);

            string nonce = jsonToken.Header["nonce"].ToString();

            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(nonce));
            string hashedNonce = BitConverter.ToString(bytes, 0, bytes.Length).Replace("-", "").ToLower();

            jsonToken.Header["nonce"] = hashedNonce;

            //string signatureAvecHashedNonce = GetSignedJwtToken(jsonToken, n, e, x5c);

            token = handler.WriteToken(jsonToken);
            //context.Request.Headers["Authorization"] = "Bearer " + token;
        }
        catch (Exception ex)
        {
            //ne rien faire
        }
        await _next(context);
    }

    public string GetSignedJwtToken(JwtSecurityToken jwtToken, string n, string e, string x5c)
    {
        string modulus = n;
        string exponent = e;

        // Créer un objet RSA
        using RSA rsa = RSA.Create();
        {
            // Créer un objet RSAParameters et définir ses propriétés
            RSAParameters rsaParameters = new RSAParameters
            {
                Modulus = WebEncoders.Base64UrlDecode(modulus), //Convert.FromBase64String(modulus),
                Exponent = Convert.FromBase64String(exponent)
            };

            // Importer les paramètres dans l'objet RSA
            rsa.ImportParameters(rsaParameters);

            var tokenHandler = new JwtSecurityTokenHandler();
            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
            //jwtToken.SigningKey = signingCredentials.Key;
            //return tokenHandler.WriteToken(jwtToken);

            string header = jwtToken.Header.Base64UrlEncode();
            string payload = jwtToken.Payload.Base64UrlEncode();
            string text = header + "." + payload;
            CryptoProviderFactory cryptoProviderFactory = signingCredentials.CryptoProviderFactory ?? signingCredentials.Key.CryptoProviderFactory;
            SignatureProvider signatureProvider = cryptoProviderFactory.CreateForSigning(signingCredentials.Key, signingCredentials.Algorithm);
            string signature = Base64UrlEncoder.Encode(signatureProvider.Sign(Encoding.UTF8.GetBytes(text)));
            signature = JwtTokenUtilities.CreateEncodedSignature(text, signingCredentials);
            return signature;
        }
    }


}
