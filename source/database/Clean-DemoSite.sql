USE Graffiti_DemoSite
UPDATE graffiti_ObjectStore
SET Data ='<?xml version="1.0" encoding="utf-16"?>
<SiteSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Title>Graffiti CMS</Title>
  <TagLine>A Graffiti CMS powered site</TagLine>
  <Theme>default</Theme>
  <TimeZoneOffSet>0</TimeZoneOffSet>
  <FeaturedId>0</FeaturedId>
  <EmailServer>mail</EmailServer>
  <EmailFrom>graffiti@yoursite.com</EmailFrom>
  <EmailRequiresSSL>false</EmailRequiresSSL>
  <EmailServerRequiresAuthentication>false</EmailServerRequiresAuthentication>
  <EmailPort>25</EmailPort>
  <UseCustomHomeList>false</UseCustomHomeList>
</SiteSettings>'
WHERE [Name] = 'sitesettings'
DELETE FROM dbo.graffiti_Comments
DELETE FROM dbo.graffiti_Logs
DELETE FROM dbo.graffiti_Post_Statistics
DELETE FROM dbo.graffiti_Posts
DELETE FROM dbo.graffiti_Categories WHERE [Id] > 1
DELETE FROM graffiti_ObjectStore WHERE [Name] LIKE 'DnugLeipzig%'
DELETE FROM graffiti_ObjectStore WHERE [Name] LIKE 'CustomFormSettings%'
DELETE FROM graffiti_ObjectStore WHERE [Name] LIKE 'NavigationSettings%'