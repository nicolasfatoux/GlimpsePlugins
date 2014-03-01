using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NfxLab.GlimpsePlugins.Test.Startup))]
namespace NfxLab.GlimpsePlugins.Test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
