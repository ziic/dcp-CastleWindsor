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

using System;
using System.Linq.Expressions;

namespace dcp.CastleWindsor
{
    /// <summary>
    /// Allows you to retrieve parameter and member names without the use of magic strings.
    /// </summary>
    /// <example><![CDATA[
    /// public string SayHi(string name) {
    ///     if (null == name) {
    ///         throw new ArgumentNullException(Name.Of(() => name));
    ///     }
    ///     return string.Format("Hello, {0}!", name);
    /// }
    /// ]]>
    /// </example>
    public static class Name
    {
        public static string Of(Expression<Func<object>> expression)
        {
            if (null == expression)
            {
                throw new ArgumentNullException(Of(() => expression));
            }

            var memberExpression = expression.Body as MemberExpression;
            if (null != memberExpression)
            {
                return memberExpression.Member.Name;
            }

            var unaryExpression = expression.Body as UnaryExpression;
            if (null != unaryExpression && unaryExpression.Operand is MemberExpression)
            {
                memberExpression = (MemberExpression)unaryExpression.Operand;
                return memberExpression.Member.Name;
            }

            throw new NotSupportedException();
        }
    }
}