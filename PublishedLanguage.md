Published language
==================

powerups.notification.pushover.send
-----------------------------------

```csharp
new
    {
        id = Guid.NewGuid(),
        eventType = "powerups.notification.pushover.send",
        version = 1,
        apiKey = "<pushover api key>",
        userKey = "<pushover user key>",
        message = "X-mas is here",
        tenant = "tenant",
    }
```
