# Build API using Data API builder
Data API Builder creates API to access database. You can publish this API with Azure API management.

Links
- https://learn.microsoft.com/en-us/azure/data-api-builder/get-started/get-started-with-data-api-builder


## Install DAB

Using DotNet, install first.

```
dotnet tool install --global Microsoft.DataApiBuilder
```

Next we will use DAB CLI.

## Create API definition and run DAB locally
 
We will use DB created by WebAPI project in this repository - table Contacts.

```powershell
$connstr=(az sql db show-connection-string --server jjazmssql --name jjazmssqldb --client ado.net)
$connstr = $connstr.replace("<username>", "jj").replace("<password>", "XXXXXXXXXXXXXXX").replace("""","")

dab init --database-type "mssql" --connection-string $connstr --host-mode "Development"
dab add Contact --source dbo.Contact --permissions "anonymous:*"

dab start
```

Check dab-config.json, you can simply modify tables etc.

!!! is case sensitive 

REST query url
- https://localhost:5001/api/Contact
- https://localhost:5001/api/Contact/ContactId/1

GraphQL 
- https://localhost:5001/graphql/

```
{
  contacts(first: 5, orderBy: { FullName: DESC }) {
    items {
      ContactId
      FullName
    }
  }
}
```

## Run DAB in Azure

https://learn.microsoft.com/en-us/azure/data-api-builder/running-in-azure