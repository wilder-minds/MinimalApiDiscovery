# MinimalApiDiscovery

## Version History

- 0.1.0: Initial Version
- 0.1.1: Added warning if you are doing constructor injection in classes that implement IApi 
- 0.2.1: Added warning to prevent you from adding IApi twice to the service container
- 1.0.0: Removed need to AddApis in service collection
- 1.0.1: Changed from reflection to Source Generation
- 1.0.2: Fixed issue with including the source generator
- 1.0.3: Properly including the source generator in the package
- 1.0.4: Optimize to have lesser impact on Roslyn