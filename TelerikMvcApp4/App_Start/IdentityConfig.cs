﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;

using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;

using System.Net.Mail;
using System.Net.Http;
using SendGrid.Helpers.Mail;
//using AspNet.Identity.Oracle;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using TelerikMvcApp4.Models;

namespace TelerikMvcApp4
{
   public class EmailService : IIdentityMessageService
   {
      public async Task SendAsync(IdentityMessage message)
      {
         await configSendGridasync(message);
      }

        public async Task SendEmailAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {

            string apiKey = "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";//Environment.GetEnvironmentVariable("SENDGRID_APIKEY", EnvironmentVariableTarget.User);
            dynamic sg = new SendGridAPIClient(apiKey);

            Email from = new Email("no_reply@century.com.py");
            string subject = message.Subject;
            Email to = new Email(message.Destination);
            Content content = new Content("text/html", message.Body);
            Mail mail = new Mail(from, subject, to, content);

            
            dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());

      

        }

   }

   // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
   public class ApplicationUserManager : UserManager<ApplicationUser>
   {
      public ApplicationUserManager(IUserStore<ApplicationUser> store)
         : base(store)
      {
      }

      public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
      {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>() /*as OracleDatabase*/));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
         {
            AllowOnlyAlphanumericUserNames = false,
            RequireUniqueEmail = true
         };

         // Configure validation logic for passwords
         //manager.PasswordValidator = new PasswordValidator
         //{
         //    RequiredLength = 6,
         //    RequireNonLetterOrDigit = true,
         //    RequireDigit = true,
         //    RequireLowercase = true,
         //    RequireUppercase = true,
         //};

         // Configure user lockout defaults
         manager.UserLockoutEnabledByDefault = true;
         manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
         manager.MaxFailedAccessAttemptsBeforeLockout = 5;

         // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
         // You can write your own provider and plug it in here.
         manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
         {
            MessageFormat = "Your security code is {0}"
         });
         manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
         {
            Subject = "Security Code",
            BodyFormat = "Your security code is {0}"
         });
         manager.EmailService = new EmailService();
         
         var dataProtectionProvider = options.DataProtectionProvider;
         if (dataProtectionProvider != null)
         {
            manager.UserTokenProvider =
                new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
         }
         return manager;
      }
   }

   // Configure the application sign-in manager which is used in this application.
   public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
   {
      public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
         : base(userManager, authenticationManager)
      {
      }

      public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
      {
         return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
      }

      public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
      {
         return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
      }
   }
}
