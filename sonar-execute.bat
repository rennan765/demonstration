dotnet tool install –global dotnet-sonarscanner
dotnet sonarscanner begin /k:"4oito6.Demonstration" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="aa5d1061981e7de631b9f48e8d822aed68021940"
dotnet build 4oito6.Demonstration.sln
dotnet sonarscanner end /d:sonar.login="aa5d1061981e7de631b9f48e8d822aed68021940"