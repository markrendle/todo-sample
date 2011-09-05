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

            Post["/login"] = x =>
            {
                Guid? userGuid = this.userService.Authenticate(this.Request.Form.Username, this.Request.Form.Password);

                if (!userGuid.HasValue)
                {
                    return Context.GetRedirect("~/login?error=true&username=" + (string)this.Request.Form.Username);
                }

                DateTime? expiry = null;
                if (this.Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            };

            Get["/logout"] = x => this.LogoutAndRedirect("~/");
        }
    }
}