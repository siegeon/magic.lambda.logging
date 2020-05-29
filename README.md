
# Magic Lambda Logging

[![Build status](https://travis-ci.org/polterguy/magic.lambda.logging.svg?master)](https://travis-ci.org/polterguy/magic.lambda.logging)

Log4net wrapper slots for [Magic](https://github.com/polterguy/magic). More specifically, this project provides the following slots.

* __[log.info]__ - Information log entries, typically smaller pieces of information
* __[log.debug]__ - Debug log entries, typically additional debugging information not enabled in production
* __[log.error]__ - Error log entries, typically exceptions
* __[log.fatal]__ - Fatal log entries, from which the application cannot recover from

## Configuration

Notice, you're responsible for configuring Log4Net yourself, which can normally be done with something such as the following
in for instance your _"Program.cs"_ as your application is starting up. Notice, _"magic.library"_ automatically wires up log4net
with a similar type of configuration like this, saving log entries into your _"/logs/magic.log"_ file. You might want to fine
grain your logging more, if you expect a huge amount of log entries.

```csharp
/*
 * This piece of code depends upon that you have an XML file configuring
 * log4net called "log4net.config" at the base of your assembly's folder.
 */
var log4netConfig = new XmlDocument();

log4netConfig.Load(
    File.OpenRead(
        string.Concat(
            AppDomain.CurrentDomain.BaseDirectory, 
            "log4net.config")));

var repo = 
    LogManager.CreateRepository(
        Assembly.GetEntryAssembly(), 
        typeof(log4net.Repository.Hierarchy.Hierarchy));

log4net.Config.XmlConfigurator.Configure(
    repo, 
    log4netConfig["log4net"]);
```

Then you can create a _"log4net.config"_ file with for instance the following content.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="RollingFile" type="log4net.Appender.FileAppender">
    <file value="logs/magic.log" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%-5p %d{hh:mm:ss} %message%newline" />
    </layout>
  </appender>
  <root>
    <level value="ALL" />
    <appender-ref ref="RollingFile" />
  </root>
</log4net>
```

To see more details about how to configure Log4Net, feel free to 
read the [log4net documentation](https://logging.apache.org/log4net/release/features.html).

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 5 hours after you put Magic into production, it will stop functioning, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
