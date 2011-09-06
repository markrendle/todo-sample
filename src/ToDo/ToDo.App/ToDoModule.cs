namespace ToDo.App
{
    using System;
    using System.Dynamic;
    using Nancy;
    using Nancy.Extensions;
    using Nancy.Security;
    using Simple.Data;

    public class ToDoModule : NancyModule
    {
        private readonly dynamic _db = Database.OpenNamedConnection("todo");
        public ToDoModule()
            : base("/todo")
        {
            this.RequiresAuthentication();

            Get["/"] = _ =>
                           {
                               dynamic model = new ExpandoObject();
                               model.ToDos = _db.Users.FindAllByEmail(Context.Items["username"]).ToDos.OrderByAdded();
                               return View["todos", model];
                           };

            Post["/"] = req =>
                            {
                                var user = _db.Users.FindByEmail(Context.Items["username"]);
                                _db.Todos.Insert(UserId: user.Id, Text: Request.Form.Text);
                                return Context.GetRedirect("~/todo/");
                            };
        }
    }
}