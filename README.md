There is now an offical sdk from Particle [Particle Windows Cloud SDK](https://github.com/spark/particle-windows-sdk)

# Build
| master | develop |
|--------|---------|
|[![Build status](https://ci.appveyor.com/api/projects/status/8kcdeffb0s415rmh?svg=true)](https://ci.appveyor.com/project/holtsoftware/particlesdk)|[![Build status](https://ci.appveyor.com/api/projects/status/3oa42ovdknh4i627?svg=true)](https://ci.appveyor.com/project/holtsoftware/particlesdk-qe393)|

# ParticleSDK
.NET SDK for Particle.io (formally spark.io)

### Framework Specific Versions
Native Library for UWP/UAP, Windows 8.1, Windows Phone 8.1, .NET 4.5. Using Framework Specific version of HttpClient.
```
Install-Package ParticleNET.ParticleSDK -Pre
```
| Release | Beta |
|---------|------|
|[![NuGet](https://img.shields.io/nuget/v/ParticleNET.ParticleSDK.svg)](https://www.nuget.org/packages/ParticleNET.ParticleSDK)|[![NuGet](https://img.shields.io/nuget/vpre/ParticleNET.ParticleSDK.svg)](https://www.nuget.org/packages/ParticleNET.ParticleSDK)|

### Portable Versions
Portable Library supporting UWP, Win8.1, Windows Phone 8.1, .NET 4.5. Should also work with mono and Xamarin but not tested. Using HttpClient package from Microsoft for consistency. 
```
Install-Package ParticleNET.ParticleSDK.Portable -Pre
```
| Release | Beta |
|---------|------|
|[![NuGet](https://img.shields.io/nuget/v/ParticleNET.ParticleSDK.Portable.svg)](https://www.nuget.org/packages/ParticleNET.ParticleSDK.Portable)|[![NuGet](https://img.shields.io/nuget/vpre/ParticleNET.ParticleSDK.Portable.svg)](https://www.nuget.org/packages/ParticleNET.ParticleSDK.Portable)|

[Documentation](http://particlenet.github.io/Docs/index.html)

# Tinker App
The example app is an implementation of the Tinker App for WP 8.1, Windows 8.1 and Windows 10 found [here](https://github.com/ParticleNET/Particle-Windows-app).

**Download Tinker App**<br />
[Windows Store 8.1](http://apps.microsoft.com/windows/app/f9a2a89a-1adb-49ce-abff-0da6be35aa0c)<br />
[Windows Phone Store 8.1](http://windowsphone.com/s?appid=71eaa2c4-b093-4d1a-b5d3-046e6c2f9826)

## Examples
### Connecting to the Cloud
```C#
using(var cloud = new ParticleCloud())
{
    var result = await cloud.LoginWithUserAsync("user@email.com", "test");
    if(result.Success) // User is logged in
    {
        // Continue to next step
    }
    else // User is not logged in
    {
        // tell the user there not logged in
    }
}
```
### Get a list of devices
```C#
var result = await cloud.GetDevicesAsync();
if(result.Success)
{
    List<ParticleDevice> list = result.Data;
    foreach(var device in list)
    {
        Console.WriteLine(device.Name);
    }
}
```

### Call a function on a device
```C#
    var result = await device.CallFunction("Name", "Parameter");
    if(result.Success)
    {
        Console.WriteLine("Name Fuction Returned: {0}", result.Data);
    }
    else
    {
        // Error while calling function
    }
```

### Get a Variable Value
```C#
    var result = await device.GetVariableValueAsync("temp");
    if(result.Success)
    {
        var variable = result.Data;
        Console.WriteLine("Variable {0} with type {1} has the value {2}", variable.Name, variable.Type, variable.Value);
        // Refresh the value with the variable object
        var r = await variable.RefreshValueAsync();
        if(r.Success)
        {
            // The value has been updated
            Console.WriteLine("Updated Value {0}", variable.Value);
        }
        else
        {
            // There was a problem refreshing the value
            Console.WriteLine("Error Updating variable value: Error {0} ErrorDescription {1}", r.Error, r.ErrorDescription);
        }
    }
    else
    {
        // Error while getting variable value
    }
```

## Not currently implemented
* Compiling and flashing files with the cloud
* Flashing a compiled file
* Getting a device directly by its id
* Getting a stream of events
* Getting a stream of events from a device
* Publishing an event
