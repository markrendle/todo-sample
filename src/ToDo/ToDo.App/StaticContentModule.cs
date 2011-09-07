namespace ToDo.App
{
    using Nancy;

    public class StaticContentModule : NancyModule
    {
        public StaticContentModule() : base("/content")
        {
            Get["/styles/{file}"] = x => Response.AsCss("Content/" + (string)x.file);

            Get["/scripts/{file}"] = x => Response.AsJs("Content/" + (string)x.file);

            Get["/images/{file}"] = x => Response.AsImage("Content/" + (string)x.file);
        }
    }
}