# Build
| master | develop |
|--------|---------|
|[![Build status](https://ci.appveyor.com/api/projects/status/8kcdeffb0s415rmh?svg=true)](https://ci.appveyor.com/project/holtsoftware/particlesdk)|[![Build status](https://ci.appveyor.com/api/projects/status/3oa42ovdknh4i627?svg=true)](https://ci.appveyor.com/project/holtsoftware/particlesdk-qe393)|

# Nuget
| Release | Beta |
|---------|------|
|[![NuGet](https://img.shields.io/nuget/v/ParticleNET.ParticleSDK.svg)](https://www.nuget.org/packages/ParticleNET.ParticleSDK)|[![NuGet](https://img.shields.io/nuget/vpre/ParticleNET.ParticleSDK.svg)](https://www.nuget.org/packages/ParticleNET.ParticleSDK)|

# ParticleSDK
.NET SDK for Particle.io (formally spark.io)

Implemented as a Portable Library targeting .NET 4.5, WP 8, Windows 8.

[Automatic Documentation](http://particlenet.github.io/Docs/index.html)

# Tinker App
The example app is an implementation of the Tinker App for WP 8.1, Windows 8.1 and Windows 10 found [here](https://github.com/ParticleNET/Particle-Windows-app).

**Download Tinker App**<br />
[Windows Store 8.1](http://apps.microsoft.com/windows/app/f9a2a89a-1adb-49ce-abff-0da6be35aa0c)<br />
[Windows Phone Store 8.1](http://windowsphone.com/s?appid=71eaa2c4-b093-4d1a-b5d3-046e6c2f9826)

## Adding to your project
To install ParticleSDK, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)<br />
<code>
Install-Package ParticleNET.ParticleSDK
</code>

## Not currently implemented
* Compiling and flashing files with the cloud
* Flashing a compiled file
* Getting a device directly by its id
* Getting a stream of events
* Getting a stream of events from a device
* Publishing an event
