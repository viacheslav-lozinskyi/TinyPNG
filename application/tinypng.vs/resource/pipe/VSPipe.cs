
using Microsoft.VisualStudio.Shell;
using resource.tool;

namespace resource.pipe
{
    internal class VSPipe : extension.AnyPipe
    {
        protected override void _Execute(atom.Trace context, string pipe, string value)
        {
            //var a_Name1 = GetAttribute(value, NAME.ATTRIBUTE.EVENT);
            //var a_Name2 = GetAttribute(value, NAME.ATTRIBUTE.ID);
            ThreadHelper.ThrowIfNotOnUIThread();
            {
                package.TinyPNG.Instance.ShowOptionPage(typeof(VSOptions));
            }
        }
    }
}
