# ParticleSDK
.NET SDK for Particle.io (formally spark.io)

Implemented as a Portable Library targeting .NET 4.5, WP 8, Windows 8.

The example app will be an implementation of the Tinker App for WP 8.1, Windows 8.1 and Windows 10 found here https://github.com/ParticleNET/Particle-Windows-app

## Adding to your project
To install ParticleSDK, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)<br />
<code>
Install-Package ParticleNET.ParticleSDK -Pre
</code>

## Known Issues with published version
* Currently login throws an error when running on mono
