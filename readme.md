Query Mapper
====

Enables writing queries against a DTO object, then executing against an entity store..

The goal was to isolate data entity contracts to the data layer, then expose the DTO's for those objects as IQueryable<DTO> on the services layer.

The DTO's can be exposed as an ODATA endpoint on a Rest service.  Instead of requiring the query to be hand written between the Service and Repository, we can use a query mapper just like we use AutoMapper to replace the handwritten mapping code.