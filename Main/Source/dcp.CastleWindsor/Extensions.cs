using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace dcp.CastleWindsor
{
    public static class Extensions
    {
        public static XmlElement ToXmlElement(this XElement xe)
        {
            var elementStr = xe.ToString();
            var doc = new XmlDocument();
            var node = doc.ReadNode(XmlReader.Create(new StringReader(elementStr)));
            return (XmlElement)node;
        }

        public static IEnumerable<T> InOrder<T>(this IEnumerable<T> enumerableOfT)
        {
            return enumerableOfT.OrderBy(t => t);
        }

        public static string Join<T, TSep>(this IEnumerable<T> enumerableOfT, TSep separator)
        {
            var sb = new StringBuilder();

            if (enumerableOfT.Any())
            {
                sb.Append(enumerableOfT.First());
            }

            foreach (var t in enumerableOfT.Skip(1))
            {
                sb.Append(separator).Append(t);
            }

            return sb.ToString();
        }
    }
}