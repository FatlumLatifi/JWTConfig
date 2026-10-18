using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

public static class JWTConfig
{

    public static void AddAuthServices(WebApplicationBuilder builder, bool validateIssuer = true, bool validateAudience = true,
        bool validateLifetime = true, bool ValidateIssuerKey = true,
        string defaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
        string defaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = defaultAuthenticateScheme;
            options.DefaultChallengeScheme = defaultChallengeScheme;
        })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = validateIssuer,
        ValidateAudience = validateAudience,
        ValidateLifetime = validateLifetime,
        ValidateIssuerSigningKey = ValidateIssuerKey,

        ValidIssuer = Data.WEBAPP_SERVER_URL,
        ValidAudience = Data.WEBAPP_CLIENT_URL,
        IssuerSigningKey = Data.SecurityKey

    };
});
        builder.Services.AddAuthorization();
    }

    public static async Task<TokenValidationResult> ValidateAsync(string token)
    {
        return await new JsonWebTokenHandler().ValidateTokenAsync(token,
            new TokenValidationParameters
            {
                ValidIssuer = Data.WEBAPP_SERVER_URL,
                ValidAudience = Data.WEBAPP_CLIENT_URL,
                IssuerSigningKey = Data.SecurityKey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true
            });
    }



    public static async Task<string> GenerateToken(string userID)
    {
        var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, userID),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

        var jwtToken = new JwtSecurityToken(Data.WEBAPP_SERVER_URL, Data.WEBAPP_CLIENT_URL,
            claims, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Data.SignCreds);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }


    public static class Data
    {

        public static readonly string WEBAPP_CLIENT_URL = Environment.GetEnvironmentVariable("CLIENT_URL") ?? "http://localhost:3000";
        public static readonly string WEBAPP_SERVER_URL = Environment.GetEnvironmentVariable("SERVER_URL") ?? "http://localhost:8080";


        public static readonly string jwtkey = Environment.GetEnvironmentVariable("JWT_KEY")!;
        public static readonly byte[] jwtKeyBytes = Encoding.UTF8.GetBytes(jwtkey);

        public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(jwtKeyBytes);
        public static readonly SigningCredentials SignCreds = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);


    }

}


