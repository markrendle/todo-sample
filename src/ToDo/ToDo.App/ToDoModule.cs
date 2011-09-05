namespace ToDo.App
{
    using Nancy;
    using Nancy.Security;

    public class ToDoModule : NancyModule
    {
        public ToDoModule() : base("/todo")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => "Secure";
        }
    }
}