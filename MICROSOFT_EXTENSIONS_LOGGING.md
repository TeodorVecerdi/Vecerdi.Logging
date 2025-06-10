# Microsoft.Extensions.Logging Integration

This file provides examples of how to use the new Microsoft.Extensions.Logging integration with Vecerdi.Logging.

## Basic Usage

### 1. Using Vecerdi.Logging as a Provider for Microsoft.Extensions.Logging

```csharp
using Microsoft.Extensions.Logging;
using Vecerdi.Logging.Extensions;

// Create a logger factory and add Vecerdi.Logging provider
using var factory = new LoggerFactory();
factory.AddVecerdi();

var logger = factory.CreateLogger("MyCategory");
logger.LogInformation("This message will be routed through Vecerdi.Logging");
logger.LogWarning("This is a warning");
logger.LogError("This is an error");
```

### 2. Using Unity Logging Provider (Unity Projects)

```csharp
using Microsoft.Extensions.Logging;
using Vecerdi.Logging.Extensions;

// Create a logger factory and add Unity provider
using var factory = new LoggerFactory();
factory.AddUnity();

var logger = factory.CreateLogger("UnityCategory");
logger.LogInformation("This will appear in Unity's Debug.Log");
logger.LogWarning("This will appear in Unity's Debug.LogWarning");
logger.LogError("This will appear in Unity's Debug.LogError");
```

### 3. Backwards Compatibility

The existing Vecerdi.Logging API is still available (though marked as obsolete):

```csharp
// Legacy API (still works but is obsolete)
#pragma warning disable CS0618
Vecerdi.Logging.Log.Information("Legacy message");
Vecerdi.Logging.Log.Warning("Legacy warning");
Vecerdi.Logging.Log.Error("Legacy error");
#pragma warning restore CS0618
```

## Unity Integration

In Unity projects, the UnityLoggerProvider automatically registers itself at startup, so logging should work out of the box. However, if you want to use Microsoft.Extensions.Logging, you can create a LoggerFactory and add the Unity provider:

```csharp
// In a Unity script
using Microsoft.Extensions.Logging;
using Vecerdi.Logging.Extensions;

public class MyUnityScript : MonoBehaviour {
    private ILogger _logger;
    
    void Start() {
        var factory = new LoggerFactory();
        factory.AddUnity();
        _logger = factory.CreateLogger<MyUnityScript>();
        
        _logger.LogInformation("Unity script started");
    }
}
```

## Migration Guide

### From Legacy Vecerdi.Logging to Microsoft.Extensions.Logging

**Before:**
```csharp
using Vecerdi.Logging;

Log.Information("Hello World");
Log.Warning("This is a warning");
Log.Error("This is an error");
```

**After:**
```csharp
using Microsoft.Extensions.Logging;
using Vecerdi.Logging.Extensions;

using var factory = new LoggerFactory();
factory.AddVecerdi(); // or factory.AddUnity() for Unity projects

var logger = factory.CreateLogger("MyApp");
logger.LogInformation("Hello World");
logger.LogWarning("This is a warning");
logger.LogError("This is an error");
```

## Benefits

1. **Standard API**: Use the industry-standard Microsoft.Extensions.Logging API
2. **Plug-and-Play**: Simply call `factory.AddUnity()` to enable Unity logging
3. **Backwards Compatible**: Existing code continues to work
4. **Flexible**: Can route to either Vecerdi.Logging system or directly to Unity
5. **Categories**: Full support for logger categories and levels