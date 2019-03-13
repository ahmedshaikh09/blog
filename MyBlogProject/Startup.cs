using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyBlogProject.Startup))]
namespace MyBlogProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
