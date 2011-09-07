namespace ToDo.App
{
    using Nancy;
    using Nancy.Authentication.Forms;

public class ToDoBootstrapper : DefaultNancyBootstrapper
{
    protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
    {
#if DEBUG
        StaticConfiguration.DisableCaches = true;
#endif 

        base.InitialiseInternal(container);

        FormsAuthentication.Enable(
            this, 
            new FormsAuthenticationConfiguration
                {
                    RedirectUrl = "~/login",
                    UsernameMapper = container.Resolve<IUsernameMapper>()
                });
    }
}
}