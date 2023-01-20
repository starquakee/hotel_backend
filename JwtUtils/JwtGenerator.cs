using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecurityUtils;

/// <summary>
/// 用于做用户身份校验的工具类
/// </summary>
public static class JwtGenerator
{
    private static readonly string issuer = "hotel.kevinc.ltd";
    private static readonly int validDays = 7;
    private static SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes("s1wQeOPHZoJCqn2bhFuqOQVuKDQBtmD40SctP7BUJgQw4olU"));

    /// <summary>
    /// 用UserId的值来生成一个Jwt，此后即可用这个Jwt来表明用户身份
    /// </summary>
    /// <param name="userId">用户的Id，在数据库里面那个user类的主键</param>
    /// <param name="payload">一些附加字段，你们可以自己定义。这个字符串不能过大，会引发一些http服务器的异常</param>
    /// <returns>一个jwt字符串</returns>
    public static string GetJwt(uint userId, in string payload)
    {
        var aud = Convert.ToString(userId);
        var now = DateTime.UtcNow;
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = aud,
            NotBefore = now,
            Expires = now.AddDays(validDays),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Jwt-Payload", payload)
            }),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return jwtSecurityTokenHandler.CreateEncodedJwt(descriptor);
    }

    /// <summary>
    /// 从Header中获取UserId和Payload的办法，呼应上面的jwt生成方法
    /// 用了扩展函数的形式包装好，无需直接访问方法体
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns>一个元组，分别是UserId和Payload</returns>
    /// <exception cref="ArgumentNullException">缺乏必须的请求头</exception>
    /// <exception cref="SecurityTokenException">Token不合法</exception>
    public static (uint, string) GetUserIdFromHeader(this HttpContext httpContext)
    {
        // 有两个FallBack，一个是直接从Authorization头读取，一个是解析我自定义的Header，因为网关会帮忙解析Jwt
        // 为了方便开发使用，这里是有两个读取手段的，会自动先尝试我自己定义的Header，然后再尝试Authorization头，实在不行，就报错
        var userIdFromHeader = httpContext.Request.Headers.FirstOrDefault(i => i.Key.ToLower() == "x-userid").Value.FirstOrDefault();
        var payloadFromHeader = httpContext.Request.Headers.FirstOrDefault(i => i.Key.ToLower() == "x-jwt-payload").Value.FirstOrDefault() ?? string.Empty;

        if (userIdFromHeader == null)
        {
            var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new ArgumentNullException(nameof(httpContext), "Authorization or X-UserId request header must be provided!");
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                jwtSecurityTokenHandler.ValidateToken(authHeader, new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                }, out _);
            }
            catch
            {
                throw new SecurityTokenException();
            }

            var jwt = jwtSecurityTokenHandler.ReadJwtToken(authHeader);
            userIdFromHeader = jwt.Audiences.First();
            payloadFromHeader = jwt.Claims.FirstOrDefault(c => c.Type == "Jwt-Payload")?.Value ?? string.Empty;
        }

        return (uint.Parse(userIdFromHeader), payloadFromHeader);
    }
}