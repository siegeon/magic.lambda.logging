
# Audit logging from Hyperlambda

Logging wrapper slots for Magic. More specifically, this project provides the following slots.

* __[log.info]__ - Information log entries, typically smaller pieces of information
* __[log.debug]__ - Debug log entries, typically additional debugging information not enabled in production
* __[log.error]__ - Error log entries, typically exceptions
* __[log.fatal]__ - Fatal log entries, from which the application cannot recover from

By default, this project will log into your `magic.log_entries` database/table, using either MySQL, PostgreSQL, or
Microsoft SQL Server. This allows you to use SQL to generate statistics on top of your logs. An example of
logging an info piece of information can be found below.

```
log.info:Howdy world from magic.lambda.logging!
```

**Notice** - You can supply content to your log item two different ways. Either as a piece of string, or if you choose
to set its value to nothing, it will concatenate all children node's values, after having evaluated it as a lambda
object. This allows you to create rich log entries, based upon evaluating the children of the log invocation.
This gives you a convenience shortcut to create log entries that have strings concatenated as their content,
without having to manually concatenate them yourself. An example of the latter can be found below.

```
.a:foo bar

log.info
   .:'A value is '
   get-value:x:@.a
```

If you provide a value for your log invocation, and children nodes will be assumed to be _"meta information"_ associated
with the invocation, allowing you to log data such as follows.

```
log.info:Some log entry
   username:foo
   ip:123.123.123.123
```

The above will log _"some log entry"_ for you, and associate **[username]** and **[ip]** with your log entry as meta data.
This allows you to use the same log entry for each similar invocation type, and parametrise your log entry with meta information,
giving you aggregate capabilities on log entries having the same content, while still preserving meta information unique
to each log entry. Notice, if you don't provide a value to your log invocation, no meta data will be associated with your
log entry, but all arguments will rather be concatenated as a single string and end up in the _"content"_ parts of
your log entry.

## Exception logging

The **[log.error]** and **[log.fatal]** slots give you the option of logging an exception stack trace, by parametrising
your invocation with an **[exception]** argument, which will _not_ be assumed to be meta information, but rather end up
in the exception column as you log the item. All other parameters will still be evaluated or used as meta information.

## Project website

The source code for this repository can be found at [github.com/polterguy/magic.lambda.logging](https://github.com/polterguy/magic.lambda.logging), and you can provide feedback, provide bug reports, etc at the same place.

## Quality gates

- ![Build status](https://github.com/polterguy/magic.lambda.logging/actions/workflows/build.yaml/badge.svg)
- [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=alert_status)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=bugs)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=code_smells)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=coverage)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=ncloc)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=security_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=sqale_index)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
- [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.lambda.logging&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=polterguy_magic.lambda.logging)
