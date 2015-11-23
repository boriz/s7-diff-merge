using System.ComponentModel.Composition;

namespace S7_DMCToolbox.Base.Modules
{
    [InheritedExport]
    public interface ITemplateEngine : IBaseEngine
    {
        void TestMethod();
        void Stop();
    }
}