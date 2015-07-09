// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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