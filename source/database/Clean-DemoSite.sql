DELETE FROM dbo.graffiti_Post_Statistics
DELETE FROM dbo.graffiti_Posts
DELETE FROM dbo.graffiti_Categories WHERE [Id] > 1
DELETE FROM graffiti_ObjectStore WHERE [Name] LIKE 'DnugLeipzig%'
DELETE FROM graffiti_ObjectStore WHERE [Name] LIKE 'CustomFormSettings%'