# Pass-Through Validations API

This is a pass-through version of the Validations API specification.  It is intended for Ed-Fi implementations where an external validation engine and data store exists.  This implementation primarily uses ADO.Net queries against a SQL Server database.  It also includes a custom Validate endpoint to trigger validations from a separate API (not part of the original Validations API specification) and is configured to call a separate API for certain resources for Finance clients.  Use of this project requires an understanding of C# and SQL.  It is expected that the code provided will be an initial entry point, but that modifications will be required based on implementation specific needs.

## Set Up
- Set up tables or views in your source database to represent the descriptors and resources in the specification. Details on default referenced views listed below.
- Add the project to the Ed-Fi-ODS-Implementation\Application folder and include in solution.
- In EdFi.Ods.WebApi, add a project reference to Wi.Dpi.Validations.
- In Wi.Dpi.Validations\SqlAccessTokenProvider.cs, assumes a connection string containing "database.windows.net" will require an Azure access token.
- For Finance API (if using), implement a service which inherits from IWiseMetaService.
--For Validate resource, 
- Add "Collections_Ods" connection string, "Validations" feature, "ValidationSettings:CollectionsUrl" and "ValidationSettings:FinanceUrl" to appsettings.json. 
- Build and run it.  You may need to explicitly clear your cache if "Validations" does not appear in the "Other" section in Swagger.

## Database Tables/Views Referenced
### Rule Status Descriptor
dbo.RuleStatusDescriptorsForValidationsApi
- Id, --varchar, not null
- CodeValue, --varchar, not null
- Description--varchar, not null
- Namespace, --varchar, not null
- RuleStatusDescriptorId, --int, not null
- ShortDescription --varchar, not null

### Run Status Descriptor
dbo.RunStatusDescriptorsForValidationsApi
- Id, --varchar, not null
- CodeValue, --varchar, not null
- Description--varchar, not null
- Namespace, --varchar, not null
- RunStatusDescriptorId, --int, not null
- ShortDescription --varchar, not null

### Severity Descriptor
dbo.SeverityDescriptorsForValidationsApi
- Id, --varchar, not null
- CodeValue, --varchar, not null
- Description--varchar, not null
- Namespace, --varchar, not null
- SeverityDescriptorId, --int, not null
- ShortDescription --varchar, not null

### Validation Logic Type Descriptor
dbo.ValidationLogicTypeDescriptorsForValidationsApi
- Id, --varchar, not null
- CodeValue, --varchar, not null
- Description--varchar, not null
- Namespace, --varchar, not null
- ValidationLogicTypeDescriptorId, --int, not null
- ShortDescription --varchar, not null

### Validation Rule
dbo.RulesForValidationsApi
- RuleId, --varchar, not null
- RuleIdentifier, --varchar, not null
- RuleSource, --varchar, not null
- HelpUrl, --varchar, null
- ShortDescription, --varchar, null
- Description, --varchar, null
- RuleStatusDescriptor, --varchar, not null
- Category, --varchar, null
- SeverityDescriptor, --varchar, not null
- ExternalRuleId, --varchar, null
- ValidationLogicTypeDescriptor, --varchar, null
- ValidationLogic, --varchar, null

### Validation Rule Run
dbo.RuleRunsForValidationsApi
- Id, --varchar, not null
- RunIdentifier, --varchar, not null
- RunStartDateTime, --datetime, not null
- RunFinishDateTime, --datetime, null
- RunStatusDescriptor, --varchar, not null
- Host, --varchar, null
- ValidationEngine, --varchar, null
- EducationOrganizationId, --int, null
- Discriminator, --varchar, null
- Namespace, --varchar, null
- EdOrgResourceId --uniqueidentifier, not null

### Validations Result
dbo.MessagesForValidationsApi
- Id, --varchar, not null
- ResultIdentifier, --varchar, not null
- RunIdentifier, --varchar, not null
- RunId, --varchar, not null
- RuleIdentifier, --varchar, not null
- RuleSource, --varchar, not null
- RuleId, --varchar, not null
- ResourceId, --uniqueidentifier, null
- ResourceType, --varchar, null
- EducationOrganizationId, --int, null
- Discriminator, --varchar, null
- EdOrgResourceId, --uniqueidentifier, null
- StudentUniqueId, --varchar, null
- StudentResourceId, --uniqueidentifier, null
- StaffUniqueId, --varchar, null
- StaffResourceId, --uniqueidentifier, null
- Namespace, --varchar, null
- Acknowledged, --bit, null
- SchoolId, --int, null
- LocalEducationAgencyId, --int, not null
- SchoolYear, --smallint, not null
- ResourceClaimId --int, not null



## Legal Information

Copyright (c) 2022 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.
