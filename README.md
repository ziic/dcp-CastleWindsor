# dcp-CastleWindsor

Useful utils for castle windsor. Contains exented container to work with appSettings and connectionStrings from web.config or app.config

The library contains a custom windsor container extended from default that can load a configuration from an xml-file. A configuration may include parameters which reference to AppSettings, ConnectionStrings section from web.config(app.config)

# Example

**web.config**
```xml
 <appSettings>
    <add key="theAnswer" value="42"/>
  </appSettings>
  <connectionStrings>
    <add name="conn" connectionString="connectionString"/>
  </connectionStrings>
```

**windsor.xml** - windsor configuration file

```xml
  <components>
    <component
      id="answerProvider"
      service="dcp.CastleWindsor.Tests.IAnswerProvider, dcp.CastleWindsor.Tests"
      type="dcp.CastleWindsor.Tests.AnswerProvider, dcp.CastleWindsor.Tests"
      >
      <parameters>
        <theAnswer>#{AppSetting.theAnswer}</theAnswer>
        <conn>#{CnnStr.conn}</conn>
      </parameters>
    </component>
  </components>
</castle>
```

# Use
* create a custom container 
```c#
var container = new DcpWindsorContainer("Windsor.xml");
```
or
* create default castle container with custome XmlInterpreter
```c#
var xmlInterpreter = new dcp.CastleWindsor.XmlInterpreter("windsor.xml");
var container = new WindsorContainer(xmlInterpreter);
```

# Download
Nuget package https://www.nuget.org/packages/dcp.CastleWindsor/1.0.0
