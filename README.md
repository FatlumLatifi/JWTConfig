# Required Env Vars:
CLIENT_URL = your front end origin of url, Audience
SERVER_URL = your back end origin of url, Issuer
JWT_KEY = jwt key

## MINIMAL USE

```C#
var builder = WebApplication.CreateBuilder();

// ! ADD YOUR IDENTITY/IDENTITYCORE SERVICE BEFORE
JWTConfig.AddAuthServices(builder);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
```