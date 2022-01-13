dotnet tool install –global dotnet-sonarscanner
dotnet sonarscanner begin /k:"4oito6.Demonstration" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="aa5d1061981e7de631b9f48e8d822aed68021940"
dotnet build 4oito6.Demonstration.sln -c Release
dotnet test Core/4oito6.Demonstration.Test/4oito6.Demonstration.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage.opencover.xml
dotnet test AuditTrail/4oito6.Demonstration.AuditTrail.Test/4oito6.Demonstration.AuditTrail.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage.opencover.xml
dotnet test Contact/4oito6.Demonstration.Contact.Test/4oito6.Demonstration.Contact.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage.opencover.xml
dotnet test Person/4oito6.Demonstration.Person.Test/4oito6.Demonstration.Person.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage.opencover.xml
dotnet sonarscanner end /d:sonar.login="aa5d1061981e7de631b9f48e8d822aed68021940"
pause