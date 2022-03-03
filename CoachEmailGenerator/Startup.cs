using CoachEmailGenerator.Common;
using CoachEmailGenerator.Interfaces;
using CoachEmailGenerator.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoachEmailGenerator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public GoogleCredential GoogleCredential { get; set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewEngine());
            });

            services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                    })
                //.AddCookie(options => options.LoginPath = "/")
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["GoogleApiAuth:ClientId"];
                    options.ClientSecret = Configuration["GoogleApiAuth:ClientSecret"];

                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.Scope.Add("https://mail.google.com");
                    // TODO: I think we can use https://www.googleapis.com/auth/gmail.compose which is a little more restrictive
                    // than https://mail.google.com
                    // TODO:  Test to see if that works

                    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                    options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
                    options.SaveTokens = true;

                    options.Events.OnCreatingTicket = ctx =>
                    {
                        List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                        tokens.Add(new AuthenticationToken()
                        {
                            Name = "TicketCreated",
                            Value = DateTime.UtcNow.ToString()
                        });

                        // An alternative to this method would be to call 
                        // GET https://www.googleapi.com/oauth2/v1/tokeninfo?access_token= {ACCESS_TOKEN goes here}
                        // I think the method below is better, but this is a reliable alternative
                        // See https://gsuite-developers.googleblog.com/2012/01/tips-on-using-apis-discovery-service.html for more info
                        tokens.Add(new AuthenticationToken()
                        {
                            Name = "scope",
                            Value = ctx.TokenResponse.Response?.RootElement.GetProperty("scope").ToString()
                        });

                        ctx.Properties.StoreTokens(tokens);

                        // Get Google Creds for email execution
                        //GoogleCredential = GoogleCredential.FromAccessToken(ctx.AccessToken);

                        return Task.CompletedTask;
                    };
                });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<GmailApiService>();

            if (Configuration["dataService"] == "Azure")
            {
                services.AddTransient<IDataService, AzureDataService>();
                services.AddTransient<ISchoolService, SchoolService>();
            }
            else
            {
                services.AddTransient<IDataService, LocalDataService>();
            }



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
