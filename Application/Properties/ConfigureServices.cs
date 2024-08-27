//using System;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//namespace Application.Properties
//{
//    public class ConfigureServices
//    {
//        private readonly IConfiguration _configuration;

//        public ConfigureServices(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public void Configure(IServiceCollection services)
//        {
//            services.AddAuthentication(cfg =>
//            {
//                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//            }).AddJwtBearer(x =>
//            {
//                x.RequireHttpsMetadata = false;
//                x.SaveToken = false;
//                x.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(
//                        Encoding.UTF8.GetBytes(_configuration["ConnectionStrings:JWT_Secret"])
//                    ),
//                    ValidateIssuer = false,
//                    ValidateAudience = false,
//                    ClockSkew = TimeSpan.Zero
//                };
//            });

//        }
//    }
//}
