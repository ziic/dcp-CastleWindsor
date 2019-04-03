using System;
using System.Xml;
using Castle.Core.Resource;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors;

namespace dcp.CastleWindsor
{
    public sealed class XmlProcessor
    {
        private readonly IXmlProcessorEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlProcessor"/> class.
        /// </summary>
        public XmlProcessor()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlProcessor"/> class.
        /// </summary>
        /// <param name="environmentName">Name of the environment.</param>
        /// <param name="resourceSubSystem">The resource sub system.</param>
        public XmlProcessor(string environmentName, IResourceSubSystem resourceSubSystem)
        {
            engine = new XmlProcessorEngine(environmentName, resourceSubSystem);
            RegisterProcessors();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlProcessor"/> class.
        /// </summary>
        public XmlProcessor(string environmentName)
        {
            engine = new XmlProcessorEngine(environmentName);
            RegisterProcessors();
        }

        private void RegisterProcessors()
        {
            AddElementProcessor(typeof(IfElementProcessor));
            AddElementProcessor(typeof(DefineElementProcessor));
            AddElementProcessor(typeof(UndefElementProcessor));
            AddElementProcessor(typeof(ChooseElementProcessor));
            AddElementProcessor(typeof(PropertiesElementProcessor));
            AddElementProcessor(typeof(AttributesElementProcessor));
            AddElementProcessor(typeof(IncludeElementProcessor));
            AddElementProcessor(typeof(IfProcessingInstructionProcessor));
            AddElementProcessor(typeof(DefinedProcessingInstructionProcessor));
            AddElementProcessor(typeof(UndefProcessingInstructionProcessor));
            AddElementProcessor(typeof(DefaultTextNodeProcessor));
            AddElementProcessor(typeof(EvalProcessingInstructionProcessor));
        }

        private void AddElementProcessor(Type t)
        {
            engine.AddNodeProcessor(t);
        }

        public XmlNode Process(XmlNode node)
        {
            try
            {
                if (node.NodeType == XmlNodeType.Document)
                {
                    node = (node as XmlDocument).DocumentElement;
                }

                engine.DispatchProcessAll(new DefaultXmlProcessorNodeList(node));

                return node;
            }
            catch (ConfigurationProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var message = $"Error processing node {node.Name}, inner content {node.InnerXml}";

                throw new ConfigurationProcessingException(message, ex);
            }
        }

        public XmlNode Process(IResource resource)
        {
            try
            {
                using (resource)
                {
                    var doc = new XmlDocument();
                    using (var stream = resource.GetStreamReader())
                    {
                        doc.Load(stream);
                    }

                    engine.PushResource(resource);

                    var element = Process(doc.DocumentElement);

                    engine.PopResource();

                    return element;
                }
            }
            catch (ConfigurationProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var message = $"Error processing node resource {resource}";

                throw new ConfigurationProcessingException(message, ex);
            }
        }
    }
}
