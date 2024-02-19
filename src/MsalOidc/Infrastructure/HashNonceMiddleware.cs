

using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Logging;

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
        IdentityModelEventSource.ShowPII = true;
        try
        {
            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            //bool isValid = VerifySignature(jsonToken);

            //Unable to match keys
            //var configManager = new ConfigurationManager<OpenIdConnectConfiguration>($"https://login.microsoftonline.com/38ae3bcd-9579-4fd4-adda-b42e1495d55a/v2.0/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            //var openidconfig = configManager.GetConfigurationAsync().Result;

            //var tmp = handler.ValidateToken(token, new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidateAudience = true,
            //    ValidateLifetime = true,
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKeys = openidconfig.SigningKeys,
            //    ValidIssuer = "https://sts.windows.net/38ae3bcd-9579-4fd4-adda-b42e1495d55a/",
            //    ValidAudience = "00000003-0000-0000-c000-000000000000",
            //}, out _);

            if (jsonToken.Header.TryGetValue("nonce", out object nonceAsObject))
            {
                string nonce = nonceAsObject.ToString();
                using SHA256 sha256Hash = SHA256.Create();
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(nonce));
                string hashedNonce = Base64UrlEncoder.Encode(bytes);

                //jsonToken.Header.Remove("x5t");
                jsonToken.Header.Remove("nonce");
                jsonToken.Header.Add("nonce", hashedNonce);

                //isValid = VerifySignature(jsonToken);
                string newHeader = Base64UrlEncoder.Decode(jsonToken.RawHeader);
                newHeader = newHeader.Replace("\"nonce\":\"" + nonce + "\"", "\"nonce\":\"" + hashedNonce + "\"");
                newHeader = Base64UrlEncoder.Encode(newHeader);

                token = $"{newHeader}.{jsonToken.RawPayload}.{jsonToken.RawSignature}";

                //jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                //var tmp = handler.ValidateToken(token, new TokenValidationParameters
                //{
                //    ValidateIssuer = true,
                //    ValidateAudience = true,
                //    ValidateLifetime = true,
                //    ValidateIssuerSigningKey = true,
                //    IssuerSigningKeys = openidconfig.SigningKeys,
                //    ValidIssuer = "https://sts.windows.net/38ae3bcd-9579-4fd4-adda-b42e1495d55a/",
                //    ValidAudience = "00000003-0000-0000-c000-000000000000",
                //}, out _);
                context.Request.Headers["Authorization"] = "Bearer " + token;
            }
        }
        catch (Exception ex)
        {
            //ne rien faire
        }
        await _next(context);
    }

    private bool VerifySignature(JwtSecurityToken jsonToken)
    {
        byte[] bytesTmp = Encoding.UTF8.GetBytes(jsonToken.RawHeader + "." + jsonToken.RawPayload);
        byte[] signatureTmp = Base64UrlEncoder.DecodeBytes(jsonToken.RawSignature);

        using (SHA256 sha256HashTmp = SHA256.Create())
        {
            //https://login.microsoftonline.com/38ae3bcd-9579-4fd4-adda-b42e1495d55a/v2.0/.well-known/openid-configuration
            //https://login.microsoftonline.com/38ae3bcd-9579-4fd4-adda-b42e1495d55a/discovery/v2.0/keys
            //kid kWbkaa6qs8wsTnBwiiNYOhHbnAw
            string x5c = "MIIC/TCCAeWgAwIBAgIICHb5qy8hKKgwDQYJKoZIhvcNAQELBQAwLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDAeFw0yNDAxMTUxODA0MTRaFw0yOTAxMTUxODA0MTRaMC0xKzApBgNVBAMTImFjY291bnRzLmFjY2Vzc2NvbnRyb2wud2luZG93cy5uZXQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC3pDZdJ5acwD/5ysfZRt+19LTVAMCoeg9AWqxG1WTEbh7Jqac2VOXcNrTtBCSOk8Rsugu2C0wjY+vSmU7vFT1/3iaFt8r9QnpjQpxbGHAoyQKCfNwU5AXh4f8AgIcs4pry8+2G1yms1wKaNuSxblNgFmLq4uEUvD8eyMY7GErRVoNadaLM1V6q/NUHSO31V2Z+GzpmiHL/VvZa6x1p3U2ZIrOELvggOOUhoWiKT9kkl20s6CgjA5lMtbQzVQqFGta2PsCNUKcT/MGKWgAKbUisgz8/KYTXRwknpYXPb16niDtfrnEIRTrMnmggWJu+TpwopwU0HsUWNt6FhWnDkHFVAgMBAAGjITAfMB0GA1UdDgQWBBQLGQYqt7pRrKWQ25XWSi6lGN818DANBgkqhkiG9w0BAQsFAAOCAQEAtky1EYTKZvbTAveLmL3VCi+bJMjY5wyDO4Yulpv0VP1RS3dksmEALOsa1Bfz2BXVpIKPUJLdvFFoFhDqReAqRRqxylhI+oMwTeAsZ1lYCV4hTWDrN/MML9SYyeQ441Xp7xHIzu1ih4rSkNwrsx231GTfzo6dHMsi12oEdyn6mXavWehBDbzVDxbeqR+0ymhCgeYjIfCX6z2SrSMGYiG2hzs/xzypnIPnv6cBMQQDS4sdquoCsvIqJRWmF9ow79oHhzSTwGJj4+jEQi7QMTDR30rYiPTIdE63bnuARdgNF/dqB7n4ZJv566jvbzHpfCTqrJyj7Guvjr9i56NpLmz2DA==";
            byte[] x5cBytes = Encoding.UTF8.GetBytes(x5c);
            var certificate = new X509Certificate2(x5cBytes);
            bool isValid = certificate.GetRSAPublicKey().VerifyHash(sha256HashTmp.ComputeHash(bytesTmp), signatureTmp, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return isValid;
        }

    }

}
