using System.Configuration;
using System.Xml;
using Castle.Core.Resource;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor;

namespace dcp.CastleWindsor
{
    public class XmlInterpreter : global::Castle.Windsor.Configuration.Interpreters.XmlInterpreter
    {
        public XmlInterpreter() { }
        public XmlInterpreter(string fileName) : base(fileName) { }


        public override void ProcessResource(IResource source, IConfigurationStore store, IKernel kernel)
        {
            var processor
                 = kernel == null
                 ? new XmlProcessor(EnvironmentName)
                 : new XmlProcessor(
                     EnvironmentName,
                     kernel.GetSubSystem(SubSystemConstants.ResourceKey) as IResourceSubSystem
                 );

            try
            {
                var element = processor.Process(source);
                

                Deserialize(element, store, null);
            }
            catch (XmlProcessorException)
            {
                const string message = "Unable to process xml resource ";

                throw new ConfigurationErrorsException(message);
            }
        }

        
    }
}