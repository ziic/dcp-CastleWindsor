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
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Resource;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors;

namespace dcp.CastleWindsor
{
    public class XmlProcessorEngine : IXmlProcessorEngine
    {
        private readonly Regex appSettingPropertyRegex = new Regex(@"^AppSetting[._](.+)$", RegexOptions.IgnoreCase);
        private readonly Regex cnnStrPropertyRegex = new Regex(@"^CnnStr[:._](.+?)(\.providerName)?$", RegexOptions.IgnoreCase);
        private readonly Regex flagPattern = new Regex(@"^(\w|_)+$");
        private readonly IDictionary properties = new HybridDictionary();
        private readonly IDictionary flags = new HybridDictionary();
        private readonly Stack resourceStack = new Stack();
        private readonly Hashtable nodeProcessors = new Hashtable();
        private readonly IXmlNodeProcessor defaultElementProcessor;
        private IResourceSubSystem resourceSubSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultXmlProcessorEngine"/> class.
        /// </summary>
        /// <param name="environmentName">Name of the environment.</param>
        public XmlProcessorEngine(string environmentName)
            : this(environmentName, new DefaultResourceSubSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultXmlProcessorEngine"/> class.
        /// </summary>
        /// <param name="environmentName">Name of the environment.</param>
        /// <param name="resourceSubSystem">The resource sub system.</param>
        public XmlProcessorEngine(string environmentName, IResourceSubSystem resourceSubSystem)
        {
            AddEnvNameAsFlag(environmentName);
            this.resourceSubSystem = resourceSubSystem;
            defaultElementProcessor = new DefaultElementProcessor();
        }

        public void AddNodeProcessor(Type type)
        {
            if (typeof(IXmlNodeProcessor).IsAssignableFrom(type))
            {
                IXmlNodeProcessor processor = Activator.CreateInstance(type) as IXmlNodeProcessor;

                foreach (XmlNodeType nodeType in processor.AcceptNodeTypes)
                {
                    RegisterProcessor(nodeType, processor);
                }
            }
            else
            {
                throw new XmlProcessorException("{0} does not implement IElementProcessor interface", type.FullName);
            }
        }

        /// <summary>
        /// Processes the element.
        /// </summary>
        /// <param name="nodeList">The element.</param>
        /// <returns></returns>
        public void DispatchProcessAll(IXmlProcessorNodeList nodeList)
        {
            while (nodeList.MoveNext())
            {
                DispatchProcessCurrent(nodeList);
            }
        }

        /// <summary>
        /// Processes the element.
        /// </summary>
        /// <param name="nodeList">The element.</param>
        /// <returns></returns>
        public void DispatchProcessCurrent(IXmlProcessorNodeList nodeList)
        {
            IXmlNodeProcessor processor = GetProcessor(nodeList.Current);

            if (processor != null)
            {
                processor.Process(nodeList, this);
            }
        }

        private IXmlNodeProcessor GetProcessor(XmlNode node)
        {
            IXmlNodeProcessor processor = null;
            IDictionary processors = nodeProcessors[node.NodeType] as IDictionary;

            if (processors != null)
            {
                processor = processors[node.Name] as IXmlNodeProcessor;

                // sometimes nodes with the same name will not accept a processor
                if (processor == null || !processor.Accept(node))
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        processor = defaultElementProcessor;
                    }
                }
            }

            return processor;
        }

        private void RegisterProcessor(XmlNodeType type, IXmlNodeProcessor processor)
        {
            if (!nodeProcessors.Contains(type))
            {
                nodeProcessors[type] = new Hashtable();
            }

            IDictionary typeProcessors = (IDictionary)nodeProcessors[type];

            if (typeProcessors.Contains(processor.Name))
            {
                throw new XmlProcessorException("There is already a processor register for {0} with name {1} ", type, processor.Name);
            }
            typeProcessors.Add(processor.Name, processor);
        }

        public bool HasFlag(string flag)
        {
            return flags.Contains(GetCanonicalFlagName(flag));
        }

        public void AddFlag(string flag)
        {
            flags[GetCanonicalFlagName(flag)] = true;
        }

        public void RemoveFlag(string flag)
        {
            flags.Remove(GetCanonicalFlagName(flag));
        }

        public void PushResource(IResource resource)
        {
            resourceStack.Push(resource);
        }

        public void PopResource()
        {
            resourceStack.Pop();
        }

        public bool HasSpecialProcessor(XmlNode node)
        {
            return GetProcessor(node) != defaultElementProcessor;
        }

        public IResource GetResource(String uri)
        {
            IResource resource = resourceStack.Count > 0 ? resourceStack.Peek() as IResource : null;

            if (uri.IndexOf(Uri.SchemeDelimiter) != -1)
            {
                return resource == null ? resourceSubSystem.CreateResource(uri) :
                                                                                    resourceSubSystem.CreateResource(uri, resource.FileBasePath);
            }
            if (resourceStack.Count > 0)
            {
                return resource.CreateRelative(uri);
            }

            throw new XmlProcessorException("Cannot get relative resource '" + uri + "', resource stack is empty");
        }

        public void AddProperty(XmlElement content)
        {
            properties[content.Name] = content;
        }

        public bool HasProperty(String name)
        {
            var match = appSettingPropertyRegex.Match(name);
            if (match.Success)
            {
                var settingName = match.Groups[1].Value;
                var settingValue = ConfigurationManager.AppSettings[settingName];
                if (null != settingValue)
                {
                    return true;
                }
            }

            match = cnnStrPropertyRegex.Match(name);
            if (match.Success)
            {
                var cnnStrName = match.Groups[1].Value;
                var cnnStr = ConfigurationManager.ConnectionStrings[cnnStrName];
                if (null != cnnStr)
                {
                    return true;
                }
            }

            return properties.Contains(name);
        }

        public XmlElement GetProperty(string key)
        {
            var match = appSettingPropertyRegex.Match(key);
            if (match.Success)
            {
                var settingName = match.Groups[1].Value;
                var settingValue = ConfigurationManager.AppSettings[settingName];
                if (null != settingValue)
                {
                    return new XElement(settingName, settingValue).ToXmlElement();
                }
            }

            match = cnnStrPropertyRegex.Match(key);
            if (match.Success)
            {
                var cnnStrName = match.Groups[1].Value;
                var cnnStr = ConfigurationManager.ConnectionStrings[cnnStrName];
                if (null != cnnStr)
                {
                    var providerNameReqd = match.Groups[2].Value.Length > 0;
                    var cnnStrValue = (providerNameReqd ? cnnStr.ProviderName : cnnStr.ConnectionString);

                    if (null != cnnStrValue)
                    {
                        return new XElement(cnnStrName, cnnStrValue).ToXmlElement();
                    }
                }
            }

            XmlElement prop = properties[key] as XmlElement;
            return prop == null ? null : prop.CloneNode(true) as XmlElement;
        }

        private void AddEnvNameAsFlag(string environmentName)
        {
            if (environmentName != null)
            {
                AddFlag(environmentName);
            }
        }

        private string GetCanonicalFlagName(string flag)
        {
            flag = flag.Trim().ToLower();

            if (!flagPattern.IsMatch(flag))
            {
                throw new XmlProcessorException("Invalid flag name '{0}'", flag);
            }

            return flag;
        }
    }
}