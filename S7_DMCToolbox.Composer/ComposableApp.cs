using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using DmcLib.Wpf;
using Template.Engine;

namespace S7_DMCToolbox.Composer
{
    public abstract class ComposableApp : DmcApp
    {
        protected ComposableApp(Guid guid) 
            : base(guid)
        {
            
        }

        protected override AggregateCatalog GetCatalogs()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(GlobalEngine.GlobalEngine).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(TemplateEngine).Assembly));
            return catalog;
        }
    }
}