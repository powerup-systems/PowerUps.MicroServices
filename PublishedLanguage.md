Published language
==================

powerups.notification.pushover.tenantconfiguration
--------------------------------------------------

```csharp
new {
    id = Guid.NewGuid(),
    eventType = "powerups.notification.facade.pushover.tenantconfiguration",
    version = 1,
    tenant = "<tenant>",
    apiKey = "<pushover api key>",
}
```


powerups.notification.pushover.send
-----------------------------------

```csharp
new {
    id = Guid.NewGuid(),
    eventType = "powerups.notification.facade.pushover.send",
    version = 1,
    userKey = "<pushover user key>",
    message = "<message>",
    tenant = "<tenant>",
}
```

