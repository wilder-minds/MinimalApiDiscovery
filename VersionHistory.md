# MinimalApiDiscovery

## Version History

0.1.0: Initial Version
0.1.1: Added warning if you are doing constructor injection in classes that implement IApi 
0.2.1: Added warning to prevent you from adding IApi twice to the service container
0.2.2: Moved all code to MapApis - if you were depending on Service Injection, 
       please be aware that this version doesn't add your APIs to the
       service collection.