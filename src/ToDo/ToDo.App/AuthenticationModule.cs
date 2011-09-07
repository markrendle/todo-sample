namespace ToDo.App
{
    using System;
    using System.Dynamic;

    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Extensions;

    using Simple.Data;

    public class AuthenticationModule : NancyModule
    {
        private readonly IUserService userService;

        private readonly dynamic _db = Database.OpenNamedConnection("todo");

        public AuthenticationModule(IUserService userService)
        {
            this.userService = userService;

            Get["/login"] = x =>
            {
                dynamic model = new ExpandoObject();
                model.Errored = this.Request.Query.error.HasValue;

                return View["login", model];
            };

            Get["/logout"] = x => this.LogoutAndRedirect("~/");

            Post["/login"] = x =>
            {
                Guid? userGuid = this.userService.Authenticate(this.Request.Form.Email, this.Request.Form.Password);

                if (!userGuid.HasValue)
                {
                    return Context.GetRedirect("~/login?error=true&email=" + (string)this.Request.Form.Email);
                }

                DateTime? expiry = null;
                if (this.Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            };

            Get["/logout"] = x => this.LogoutAndRedirect("~/");

            Get["/register"] = x =>
                                   {
                                        dynamic model = new ExpandoObject();
                                        model.Errored = this.Request.Query.error.HasValue;
                                       return View["register", model];
                                   };

            Post["/register"] = x =>
                                    {
                                        try
                                        {
                                            Guid userGuid = userService.Register(Request.Form.Email,
                                                                                Request.Form.Password,
                                                                                Request.Form.ConfirmPassword);
                                            DateTime? expiry = null;
                                            if (this.Request.Form.RememberMe.HasValue)
                                            {
                                                expiry = DateTime.Now.AddDays(7);
                                            }

                                            return this.LoginAndRedirect(userGuid, expiry);
                                        }
                                        catch (ArgumentException)
                                        {
                                            return Context.GetRedirect("~/register?error=true");
                                        }
                                    };
        }
    }
}