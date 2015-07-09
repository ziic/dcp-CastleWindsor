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

// This code is based upon Castle Windsor 1.0.3

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Windsor;

namespace dcp.CastleWindsor
{
    public class DcpWindsorContainer : WindsorContainer
    {
        public DcpWindsorContainer()
            : base(new XmlInterpreter())
        { }

        public DcpWindsorContainer(string fileName)
        {
            var extension2Handler = new Dictionary<string, Action<string>>
            {
                { ".xml", configureWithXml }, // XML: <castle>...</castle>
                { ".boo", configureWithBoo }, // Binsor (BOO) script
                { ".fsx", configureWithFsx }, // F# script
                { ".ps1", configureWithPsh }, // PowerShell script
            };

            var extension = Path.GetExtension(fileName).ToLower();
            var handlerQuery = from h in extension2Handler
                               where h.Key == extension
                               select h.Value;

            if (false == handlerQuery.Any())
            {
                throw new ArgumentException(
                    "Please supply a file with one of the following extensions in order to configure windsor: "
                        + extension2Handler.Keys.InOrder().Join(", "),
                    CastleWindsor.Name.Of(() => fileName)
                );
            }

            var handler = handlerQuery.First();
            handler(fileName);
        }

        private void configureWithXml(string xmlFileName)
        {
            var interpreter = new XmlInterpreter(xmlFileName);
            interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore, Kernel);
            RunInstaller();
        }

        private void configureWithBoo(string booFileName)
        {
            throw new NotImplementedException();
        }

        private void configureWithFsx(string fsxFileName)
        {
            throw new NotImplementedException();
        }

        private void configureWithPsh(string ps1FileName)
        {
            throw new NotImplementedException();
        }
    }
}