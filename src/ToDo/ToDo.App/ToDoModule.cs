namespace ToDo.App
{
    using System;
    using System.Dynamic;
    using Nancy;
    using Nancy.Extensions;
    using Nancy.Responses;
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
                var todo = _db.Todos.Insert(UserId: user.Id, Text: Request.Form.Text);
                return Request.IsAjaxRequest() ? (Response)new JsonResponse(todo) : Context.GetRedirect("~/todo/");
            };

            Post["/{id}/complete"] = req =>
            {
                var user = _db.Users.FindByEmail(Context.Items["username"]);
                var todo = _db.Todos.FindById(req.id);

                if (todo.UserId != user.Id)
                {
                    return HttpStatusCode.Forbidden;
                }

                _db.Users.UpdateById(Id: todo.Id, Done: (todo.Done == null) ? (DateTime?)DateTime.Now : null);

                return Request.IsAjaxRequest() ? (Response)new JsonResponse(todo) : Context.GetRedirect("~/todo/");
            };

            Get["/{id}/complete"] = req =>
            {
                var user = _db.Users.FindByEmail(Context.Items["username"]);
                var todo = _db.Todos.FindById(req.id);

                if (todo.UserId != user.Id)
                {
                    return HttpStatusCode.Forbidden;
                }

                _db.Users.UpdateById(Id: todo.Id, Done: (todo.Done == null) ? (DateTime?)DateTime.Now : null);

                return Request.IsAjaxRequest() ? (Response)new JsonResponse(todo) : Context.GetRedirect("~/todo/");
            };
        }
    }
}