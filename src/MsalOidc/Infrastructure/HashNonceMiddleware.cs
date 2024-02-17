

using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

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
            string nonce = jsonToken.Header["nonce"].ToString();

            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(nonce));
            string hashedNonce = BitConverter.ToString(bytes, 0, bytes.Length).Replace("-", "").ToLower();

            jsonToken.Header["nonce"] = hashedNonce;
            token = handler.WriteToken(jsonToken);
            context.Request.Headers["Authorization"] = "Bearer " + token;
        }
        catch (Exception ex)
        {
            //ne rien faire
        }
        await _next(context);
    }
}
