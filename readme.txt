PROJECT THUNDERDOME


INTRODUCTION:
---------------------------------
This program allows data to be pushed to Vault clients.  It also allows a user to back up their various client settings.


REQUIREMENTS:
---------------------------------
- Vault Workgroup/Professional 2018


TO CREATE DEPLOYMENTS (ADMIN):
---------------------------------
1. Run the install and log-in Vault Explorer as an administrator.
2. Create a new folder for the purposes of storing your deployment.
3. Set the security on the folder so that you have full access, but everyone else has read-only access.
4. Under the Tools menu, select "Tools->Thunderdome->Configure Deployment"
5. In the Create Deployment dialog, selet your deployment folder. 
6. Select the contents of the deployment package.  These contents are taken from your local Vault environment and copied into the deployment package.
7. Click OK.
8. Now, all other Vault users who have Thunderdome will be prompted to receive updates the next time they log in to Vault Explorer.



TO USE (NON-ADMIN):
---------------------------------
Run the install and start Vault Explorer. 
If the administrator has set up a deployment, you may be prompted to receive updates.  If you agree, you will need to exit Vault Explorer for the update to complete.

If you want to backup your Vault settings "Thunderdome->Backup Vault Settings" command from the tools menu.  The command zips up all your client Vault settings into a single file.
If you want to restore your settings, extract the zip to the Autodesk folder of your user application settings.  
Ex. C:\Users\MyUsername\AppData\Roaming\Autodesk


NOTES:
---------------------------------
- For security reasons, files related to login are not part of the backup nor can they be added to deployments.
- There cannot be any more than 1 deployment file.
- Deploying "Configuration Files" will overwrite user's existing settings.  For example, deploying "Shortcuts" will overwrite user's shortcuts with yours.
- Plug-ins will only be deployed if the user does not already have the plug-in.  Thunderdome will not attempt to update or patch existing plug-ins.
- If somebody selects the "No, and never ask me again" option when prompted then later changes their mind, the dialog can be re-activated by deleting the "settings.xml" file in the Extensions\Thunderdome folder.
- Not all plug-ins can be part of a deployment.  Only plug-ins from justonesandzeros.typepad.com and plug-ins signed with Thunderdome.snk will be included.  This behavior is to avoid conflicts with the Vault App Store.  If you want your plug-in to be deployable, you can find Thunderdome.snk by downloading the source code.
- In the Create Deployment dialog.  The first level of checkboxes is for multi-select only.  The second-tier checkboxes is what determines the contents of the package.
- When deploying DECO files, you will probably want to include the "DECO settings" entry.  That's the file that determines which XAML files are hooked to which custom objects.


RELEASE NOTES:
---------------------------------
2020.25.0.1 - Updated for Vault 2020. Added support for VDS Customization Folders (*.Custom).
2018.0.1.0 - Added support for VDS (Vault Data Standard) Quickstart HelpFiles.
2018.0.0.0 - Updated for Vault 2018. - Note Thunderdome is no longer distributed as an Autodesk Exchange Application. Technical Sales D&M Team EMEA continues to maintain the project as SDK sample.
5.0.1.0 - Updated for Vault 2015.  Added support for Data Standard customizations (but not the Data Standard extension itself).  Removed support for vLogic.  Thunderdome will only deploy extensions from
the same Vault version (ex. Thunderdome 2015 will not deploy a 2014 extension).
4.0.3.0 - Fixed issue where saved searches didn't show up for sites where ADMS and AVFS are not on the same server.
4.0.1.0 - Upgraded for Vault 2014.
3.0.1.0 - Upgraded for Vault 2013.  Ease of use improvements.  Fixed problems when Unque File Names is turned on.
2.0.2.0 - Bugfix - settings files are no longer read-only after download from Vault.
2.0.1.0 - Upgraded for Vault 2012.  
1.0.1.0 - Initial Release