using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace Crawler.Analyzer
{
    public class CommaTokenizer : CharTokenizer
    {
        public CommaTokenizer(TextReader input) : base(input)
        {

        }

        public CommaTokenizer(AttributeSource source, TextReader input) : base(source, input)
        {

        }

        public CommaTokenizer(AttributeFactory factory, TextReader input) : base(factory, input)
        {

        }

        protected override bool IsTokenChar(char c)
        {
            return c != ',';
        }
    }
}
