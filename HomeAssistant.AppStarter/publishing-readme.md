== Contributor-publishing to Nuget.org ==

* Go to property pages for the project, packaging tab
* Increment the version (PACKAGE VERSION!!) and write release notes. Important that version number is incremented or publishing will fail.
* Make a pull request
* When pull request has been approved (done manually by the github repo owners) an autoamtic build in VSTS will start - build, package and publish to nuget.org
* Verify that the new version is "validating" here: https://www.nuget.org/packages/HomeAssistant.AppStarter/
* After an hour or so, the package should be validated and ready to be consumed from nuget.org

== Publishing to Nuget.org manually ==

* Go to property pages for the project, packaging tab
* Increment the version (PACKAGE VERSION!!) and write release notes
* Right click project, choose package
* See the bin/release folder for the .nupkg
* Login to nuget.org and go to https://www.nuget.org/packages/HomeAssistant.AppStarter/
* Then go to https://www.nuget.org/packages/manage/upload and upload
* Wait an hour or so before the package appears on nuget.org