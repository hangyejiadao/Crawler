using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace Crawler.Analyzer
{
    public class BlankTokenizer : CharTokenizer
    {
         
        public BlankTokenizer(TextReader in_Renamed)
            : base(in_Renamed)
        {

        }
        public BlankTokenizer(AttributeSource source, TextReader in_Renamed)
            : base(source, in_Renamed)
        {
        }
        public BlankTokenizer(AttributeSource.AttributeFactory factory, TextReader in_Renamed)
            : base(factory, in_Renamed)
        {
        }
        protected override bool IsTokenChar(char c)
        {
            return c != ' ';
        }
    }
}