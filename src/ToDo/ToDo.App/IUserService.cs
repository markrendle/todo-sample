namespace ToDo.App
{
    using System;

    using Nancy.Authentication.Forms;

    public interface IUserService : IUsernameMapper
    {
        Guid? Authenticate(string email, string password);
    }
}