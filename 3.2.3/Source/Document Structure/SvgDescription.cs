using System.ComponentModel;

namespace Svg
{
    [DefaultProperty("Text")]
    [SvgElement("desc")]
    public partial class SvgDescription : SvgElement, ISvgDescriptiveElement
    {
        public override string ToString()
        {
            return this.Content;
        }

        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgDescription>();
        }
    }
}
